using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using MRL.Commons;
using MRL.Communication.Internal_Objects;
using MRL.Communication.Tools;
using MRL.CustomMath;
using MRL.Exploration;
using MRL.Exploration.PathPlannig;
using MRL.IDE.Base;
using MRL.ImageProcessor;
using MRL.Mapping;
using MRL.Utils;
using MRL.Exploration.MultiAgent;
using MRL.Communication.External_Commands;

namespace MRL.ComStation
{
    public class RobotModel
    {
        public string Name;
        public int MountIndex;
        public Pose2D PreviousCapturedPose;
        public Pose3D UpdatedPose3D;
        public float BatteryLife;
        public Bitmap CameraImage;
        public Pose2D UpdatedSensorPos;
        public Laser CurrLaser;

        public List<Hop> InterfaceHops;
    }

    public class RobotModelManager
    {
        private ConcurrentDictionary<int, string> RobotModelsByID = null;
        private ConcurrentDictionary<string, RobotModel> RobotModelsByName = null;

        public RobotModelManager()
        {
            RobotModelsByID = new ConcurrentDictionary<int, string>(Environment.ProcessorCount, 10);
            RobotModelsByName = new ConcurrentDictionary<string, RobotModel>(Environment.ProcessorCount, 10);
        }

        public void AddRobotModel(RobotModel rm)
        {
            RobotModelsByID.TryAdd(rm.MountIndex, rm.Name);
            RobotModelsByName.TryAdd(rm.Name, rm);
        }

        public RobotModel GetRobotModelByName(string name)
        {
            return RobotModelsByName[name];
        }

        public RobotModel GetRobotModelByMID(int mountIndex)
        {
            return RobotModelsByName[RobotModelsByID[mountIndex]];
        }

        public List<RobotModel> RobotModels
        {
            get
            {
                return RobotModelsByName.Select(a => a.Value).ToList();
            }
        }

        public int Count
        {
            get { return RobotModelsByID.Count; }
        }
    }


    public class BaseStation : BaseModel
    {
        #region Internal Classes


        #endregion

        #region Public Variables

        public delegate void OnCurrentFPSDelegate(int fps);

        public event Action<List<Pose2D>, int> ReceviedPath;
        public event Action<int, float> UpdateBatteryData;
        public event Action<Bitmap[]> UpdateImage;
        public event OnCurrentFPSDelegate OnCurrentFPS;
        public event Action<string, Signal> RobotConnectionState;
        public event Action<int, Pose3D> updateRobot;
        public event Action<FrontierList> FrontierListReceived;
        public event Action<List<SignalLine>> updateSignal;

        public List<IMNode> explNodes = null;
        public IMNode goalNode = null;
        public int goalID = -1;

        public EGMap GlobalMap { get; private set; }
        public RobotModelManager RobotManager = new RobotModelManager();

        public static BaseStation Instance { get; private set; }

        #endregion

        #region Private Variables

        private ConcurrentDictionary<string, RangeScan> receivedRangeScansRobots = new ConcurrentDictionary<string, RangeScan>();

        //private RRTConnectManager rrtConnectManager;
        private FrameRateCalculator FRC = new FrameRateCalculator();
        private MultiExp multiExpPlanner;
        private static object ViewLock = new object();

        private RRTConnect pathPlannig = new RRTConnect();
        private ImageServerLink mImageServer;

        private volatile string mSelectedRobot = "";
        private int mCapturedImageCount = 0;


        private int mReceivedRangeScans = 0;
        private int RANGESCAN_PROCESSING_PERIOD = 1;

        //private List<RobotCamera> CameraControls;
        #endregion

        #region Public Methods

        static BaseStation()
        {
            Instance = new BaseStation();
        }

        private BaseStation()
            : base(ProjectCommons.config.mapperInfo)
        {
            foreach (RobotInfo ri in ProjectCommons.config.botInfo)
                if (ri.Spawned)
                {
                    RobotManager.AddRobotModel(
                            new RobotModel()
                            {
                                Name = ri.Name,
                                MountIndex = ri.MountIndex,
                                BatteryLife = 100f,
                                CameraImage = null,
                                UpdatedPose3D = ri.GetPosition(),
                                UpdatedSensorPos = null
                            }
                    );
                }

            // initialize the mapper
            double mapResolution = double.Parse(ProjectCommons.config.getValue("Map_Resolution"));
            double mapDownsample = double.Parse(ProjectCommons.config.getValue("Map_DownSample"));
            double mapWallThickness = double.Parse(ProjectCommons.config.getValue("Map_WallThickness"));

            EGMap.egmParam[EGMap.RESOLUTION] = mapResolution;
            EGMap.egmParam[EGMap.DOWNSAMPLE] = mapDownsample;
            EGMap.egmParam[EGMap.WALL_THICKNESS] = mapWallThickness;

            GlobalMap = new EGMap();
            GlobalMap.start();
            //pathPlannig.Init(GlobalMap);
            multiExpPlanner = new MultiExp();
            multiExpPlanner.Init(GlobalMap);
        }

        public override void Mount()
        {
            try
            {
                mSimulationLink = new SimulationLink(ProjectCommons.config.simHost, ProjectCommons.config.simPort);
                mSimulationLink.Connect();

                if (mSimulationLink.IsConnected)
                {
                    mSimulationLink.Send(ThisRobot.GetINITCommand());
                    RobotSpawned = true;

                    Thread.Sleep(2000);

                    mNetworkManager = new NetworkManager(ThisRobot.Name, ThisRobot.RobotPort);
                    mNetworkManager.OnNewPacketReceived += new NetworkManager.OnNewPacketReceivedDlg(Run_RobotWSSStringPacket_Received);
                    mNetworkManager.Start(RobotLinkType.COMMAND_LINK, RobotLinkType.VIDEO_LINK);

                    if ((ImageTransferType)ProjectCommons.config.IMAGE_TRANSFER_TYPE == ImageTransferType.DIRECT)
                    {
                        mImageServer = new ImageServerLink(ProjectCommons.config.videoHost, ProjectCommons.config.videoPort, ThisRobot);
                        //mImageServer.CameraTileIndex = new ImageServerLink.TileIndex(ThisRobot.MountIndex);
                        mImageServer.IsMultiViewImage = false;
                        mImageServer.OnImageServerPacket_Received += new ImageServerLink.ImageServerPacket(RobotImageServerPacket_Received);
                    }

                    if (mNetworkManager.GetRegisteredPorts != null)
                    {
                        ThisRobot.RobotPort = mNetworkManager.GetRegisteredPorts[0].portNumber;
                        RegisteredPorts.AddRange(mNetworkManager.GetRegisteredPorts);
                    }

                    //rrtConnectManager = new RRTConnectManager();
                    //rrtConnectManager.Start(GlobalMap);
                    //rrtConnectManager.ReceivedPath_event += new RRTConnectManager.ReceivedPathDlg(rrtConnectManager_ReceivedPath_event);
                    new Thread(() =>
                    {
                        while (mNetworkManager.IsManagerStarted)
                        {
                            foreach (var item in ProjectCommons.config.botInfo)
                                if (item.Spawned)
                                {
                                    Signal r = mNetworkManager.GetSignalRobot(item.Name);

                                    switch (r.Type)
                                    {
                                        case SignalType.DIRECT:
                                            if (RobotConnectionState != null)
                                                RobotConnectionState(item.Name, r);

                                            break;
                                        case SignalType.ROUTED:
                                            if (r.Status)
                                            {
                                                var iHops = RobotManager.GetRobotModelByName(item.Name).InterfaceHops;
                                                if (iHops != null)
                                                    if (iHops.Count > 0)
                                                    {
                                                        Hop fHop = iHops[0].Clone() as Hop;

                                                        if (RobotConnectionState != null)
                                                            RobotConnectionState(item.Name,
                                                                new Signal()
                                                                {
                                                                    Status = true,
                                                                    Type = SignalType.ROUTED,
                                                                    Value = fHop.Signal
                                                                });
                                                    }
                                            }
                                            else
                                            {
                                                if (RobotConnectionState != null)
                                                    RobotConnectionState(item.Name,
                                                        new Signal()
                                                        {
                                                            Status = false,
                                                            Type = SignalType.ROUTED,
                                                            Value = 0
                                                        });
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                }
                            //============================================
                            if (updateSignal != null)
                                updateSignal(getSignalTree());
                            Thread.Sleep(500);
                        }
                    }) { IsBackground = true }.Start();
                }
            }
            catch (Exception e)
            {
                USARLog.println(" >> " + e.ToString(), this.ToString());
                ProjectCommons.writeConsoleMessage("Error In BaseStation.Mount() : " + e.InnerException, ConsoleMessageType.Error);
            }
        }

        public override void Unmount()
        {
            GlobalMap.stop();
            if (RobotSpawned)
            {
                mNetworkManager.Stop();

                mSimulationLink.OnUSARMessage_Received -= RobotUSARMessage_Received;
                if (mSimulationLink.IsConnected) mSimulationLink.Disconnect();
            }
        }

        public bool ChangeViewport(string currRobotName)
        {
            lock (ViewLock)
            {
                if (currRobotName != null)
                {
                    mSelectedRobot = currRobotName;
                    //mNetworkManager.SendStringToBot(mSelectedRobot, new ImageServer() { Status = false }, MessagePriority.INP_MSG_LOW);
                    var lp = RegisteredPorts.Where(x => x.linkContentType == RobotLinkType.VIDEO_LINK).FirstOrDefault();
                    if (lp.portNumber > 0)
                        SendStringToBot(currRobotName, new ImageServer() { Status = true, PortNumber = lp.portNumber });
                    return true;
                }
                return false;
            }
        }

        public void ActiveImageServer()
        {
            if ((ImageTransferType)ProjectCommons.config.IMAGE_TRANSFER_TYPE == ImageTransferType.DIRECT)
            {
                if (mImageServer != null)
                    mImageServer.Activate();
            }
            else
            {
                ProjectCommons.writeConsoleMessage("Image Transfer Type Is On Network", ConsoleMessageType.Exclamation);
            }
        }

        public void DeactiveImageServer()
        {
            if ((ImageTransferType)ProjectCommons.config.IMAGE_TRANSFER_TYPE == ImageTransferType.DIRECT)
            {
                if (mImageServer != null)
                    mImageServer.Deactivate();
            }
            else
            {
                ProjectCommons.writeConsoleMessage("Image Transfer Type Is On Network", ConsoleMessageType.Exclamation);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendStringToBot(string selectedRobot, BaseInternalObject msg, MessagePriority messagePriority = MessagePriority.INP_MSG_LOW)
        {
            if (mNetworkManager != null)
                mNetworkManager.SendStringToBot(selectedRobot, msg, messagePriority);
        }

        #endregion

        #region Path Plannig Controller

        //public void RequestPathPlannig(Pose2D src, Pose2D dst, int botIndex)
        //{
        //    if (rrtConnectManager != null)
        //        rrtConnectManager.AddPathPlannigRequest(new RRTParameters() { sourcePose = src, goalPose = dst, robotIndex = botIndex });
        //}

        private void rrtConnectManager_ReceivedPath_event(List<Pose2D> pathList, int robotIndex)
        {
            if (ReceviedPath != null)
                ReceviedPath(pathList, robotIndex);
        }

        #endregion

        #region Image Server Controller

        protected internal void RobotImageServerPacket_Received(List<byte[]> viewports)
        {
            ImagReceived_Direct(viewports);
        }

        #endregion

        #region WSS String Messages

        protected override void RobotWSSStringPacket_Received(BaseInternalObject newBIO, string senderName, List<Hop> interfaceHops)
        {
            RobotModel mRobotModel = RobotManager.GetRobotModelByName(senderName);

            mRobotModel.InterfaceHops = interfaceHops.ToList();

            lock (ViewLock)
            {
                lock (mRobotModel)
                {
                    switch (newBIO.MessageID)
                    {
                        case (byte)InternalMessagesID.Battery:
                            PacketReceived_Battery(newBIO, mRobotModel);
                            break;

                        case (byte)InternalMessagesID.RangeScan:
                            PacketReceived_RangeScan(newBIO, senderName, mRobotModel);
                            break;

                        case (byte)InternalMessagesID.Position3D:
                            PacketReceived_Position(newBIO, mRobotModel);
                            break;

                        case (byte)InternalMessagesID.VictimRFID:
                            break;

                        case (byte)InternalMessagesID.CameraImage:
                            PacketReceived_Image(newBIO, senderName, mRobotModel);
                            break;

                        case (byte)InternalMessagesID.IMNode:
                            PacketReceived_IMNode(newBIO, senderName);
                            break;

                        case (byte)InternalMessagesID.FrontierList:
                            FrontierList frontierList = newBIO as FrontierList;
                            multiExpPlanner.UpdateFrontiers(frontierList.FrontiersList);
                            FrontierListReceived(frontierList);
                            break;
                    }
                }
            }
        }


        private List<SignalLine> getSignalTree()
        {

            List<RobotModel> robotModels = BaseStation.Instance.RobotManager.RobotModels;

            List<SignalLine> signals = new List<SignalLine>();
            foreach (var sRobot in robotModels)
            {
                if (ProjectCommons.config.botInfo[sRobot.MountIndex].Type.Equals("AirRobot")) continue;
                        
                if (sRobot.InterfaceHops == null) continue;

                bool signalStatus = BaseStation.Instance.mNetworkManager.GetSignalRobot(sRobot.Name).Status;
                if (signalStatus)
                {
                    if (sRobot.InterfaceHops.Count > 1)
                    {

                            for (int i = 0; i < sRobot.InterfaceHops.Count - 1; i++)
                            {
                                RobotModel h = BaseStation.Instance.RobotManager.GetRobotModelByName(sRobot.InterfaceHops[i].RobotName);
                                RobotModel t = BaseStation.Instance.RobotManager.GetRobotModelByName(sRobot.InterfaceHops[i + 1].RobotName);
                                Pose2D hPos = null;
                                Pose2D tPos = null;
                                if (h.UpdatedSensorPos != null)
                                    hPos = new Pose2D(h.UpdatedSensorPos.X, h.UpdatedSensorPos.Y, h.UpdatedSensorPos.Rotation);
                                else
                                    hPos = new Pose2D(ProjectCommons.config.botInfo[h.MountIndex].StartPoint.X, ProjectCommons.config.botInfo[h.MountIndex].StartPoint.Y, 0);

                                if (t.UpdatedSensorPos != null)
                                    tPos = new Pose2D(t.UpdatedSensorPos.X, t.UpdatedSensorPos.Y, t.UpdatedSensorPos.Rotation);
                                else
                                    tPos = new Pose2D(ProjectCommons.config.botInfo[t.MountIndex].StartPoint.X, ProjectCommons.config.botInfo[t.MountIndex].StartPoint.Y, 0);

                                int sH = sRobot.InterfaceHops[i].Signal;
                                int sT = sRobot.InterfaceHops[i + 1].Signal;

                                if (hPos != null && tPos != null)
                                    signals.Add(new SignalLine() { RealHead = hPos, RealTail = tPos, SignalByPercent = sT });
                            
                        }
                        if (BaseStation.Instance.mNetworkManager != null)
                        {
                            Pose2D bsPos = new Pose2D(ProjectCommons.config.mapperInfo.StartPoint.X, ProjectCommons.config.mapperInfo.StartPoint.Y, 0);
                            RobotModel lModel = BaseStation.Instance.RobotManager.GetRobotModelByName(sRobot.InterfaceHops[sRobot.InterfaceHops.Count - 1].RobotName);
                            Pose2D lPos = null;
                            if (lModel.UpdatedSensorPos != null)
                                lPos = new Pose2D(lModel.UpdatedSensorPos.X, lModel.UpdatedSensorPos.Y, lModel.UpdatedSensorPos.Rotation);
                            else
                            {
                                lPos = new Pose2D(ProjectCommons.config.botInfo[lModel.MountIndex].StartPoint.X, ProjectCommons.config.botInfo[lModel.MountIndex].StartPoint.Y, 0);
                            }

                            Signal signal = BaseStation.Instance.mNetworkManager.GetSignalRobot(sRobot.InterfaceHops[sRobot.InterfaceHops.Count - 1].RobotName);
                            if (bsPos != null && lPos != null)
                                signals.Add(new SignalLine() { RealHead = bsPos, RealTail = lPos, SignalByPercent = (signal != null ? signal.Value : 0) });
                        }
                    }
                    else
                    {
                        Pose2D bsPos = new Pose2D(ProjectCommons.config.mapperInfo.StartPoint.X, ProjectCommons.config.mapperInfo.StartPoint.Y, 0);
                        Pose2D rPos = BaseStation.Instance.RobotManager.GetRobotModelByName(sRobot.Name).UpdatedSensorPos;
                        Signal signal = BaseStation.Instance.mNetworkManager.GetSignalRobot(sRobot.Name);

                        if (rPos != null && bsPos != null)
                            signals.Add(new SignalLine() { RealHead = bsPos, RealTail = rPos, SignalByPercent = signal.Value });
                    }
                }
            }

            return signals;
        }

        private void PacketReceived_IMNode(BaseInternalObject newBIO, string senderName)
        {
            IMNode imNode = newBIO as IMNode;
            //Recovery(imNode, senderName);
        }

        private void ImagReceived_Direct(List<byte[]> newImages)
        {
            try
            {
                UpdateFPS();
                var images = newImages.Select(x => x.Length == 0 ? null : new Bitmap(new MemoryStream(x))).ToArray();

                if (UpdateImage != null)
                    UpdateImage(images);
            }
            catch
            {
                ProjectCommons.writeConsoleMessage("Error In BaseStation->ImagReceived_Direct Function", ConsoleMessageType.Error);
            }
        }

        private void PacketReceived_Image(BaseInternalObject newBIO, string senderName, RobotModel mRobotModel)
        {
            if (!senderName.Equals(mSelectedRobot))
            {
                mNetworkManager.SendStringToBot(senderName, new ImageServer() { Status = false }, MessagePriority.INP_MSG_LOW);
                return;
            }

            UpdateFPS();

            Camera cPacket = newBIO as Camera;
            var images = cPacket.GetImages();
            if (UpdateImage != null)
                UpdateImage(images);

            //Bitmap img = images[0];
            //if (cPacket.sequenceNumber > mRobotModel.LastFrameSeqNumber)
            //{
            //    mRobotModel.LastFrameSeqNumber = cPacket.sequenceNumber;

            //    mRobotModel.CameraImage = img;

            //    if (!CheckIFCameraImageAdded(mRobotModel.MountIndex, mRobotModel.UpdatedPose3D) && !mRobotModel.Name.Equals(mSelectedRobot))
            //        if (AddRobotCameraImage != null)
            //            AddRobotCameraImage(mCapturedImageCount++, img, mRobotModel.UpdatedPose3D, mRobotModel.Name);
            //}
        }

        private void UpdateFPS()
        {
            int i = FRC.CalculateFrameRate();
            if (OnCurrentFPS != null)
                OnCurrentFPS(i);
        }

        private void PacketReceived_Position(BaseInternalObject newBIO, RobotModel mRobotModel)
        {
            Position3D bData = newBIO as Position3D;
            if (bData != null)
            {
                //ProjectCommons.writeConsoleMessage(mRobotModel.Name + " / Pos : " + bData.Position, ConsoleMessageType.Information);
                mRobotModel.UpdatedPose3D = bData.Position;

                if (updateRobot != null) updateRobot(mRobotModel.MountIndex, mRobotModel.UpdatedPose3D);
            }
        }

        private void PacketReceived_RangeScan(BaseInternalObject newBIO, string senderName, RobotModel mRobotModel)
        {
            RangeScan bData = newBIO as RangeScan;
            ProjectCommons.ConsoleMessage("Range Scan Data is received", ConsoleMessageType.Information);
            if (bData != null)
            {
                if (mRobotModel.UpdatedSensorPos != null)
                {
                    bData.SensorGlobalPose =
                        GlobalMap.RefinePose(mRobotModel.UpdatedSensorPos, bData.SensorGlobalPose, bData);
                    mRobotModel.UpdatedSensorPos = bData.SensorGlobalPose;
                }
                else
                {
                    mRobotModel.UpdatedSensorPos = bData.SensorGlobalPose;
                }

                mRobotModel.UpdatedPose3D = bData.SensorOffset3D;

                if (mReceivedRangeScans % RANGESCAN_PROCESSING_PERIOD == 0)
                {
                    mReceivedRangeScans = 0;
                    GlobalMap.addScan(bData);
                }

                GlobalMap.addView(bData.RobotGlobalPose);

                if (updateRobot != null) updateRobot(mRobotModel.MountIndex, mRobotModel.UpdatedPose3D);
                mRobotModel.CurrLaser = new Laser(bData);
            }
            mReceivedRangeScans++;
            updateReceivedRangeScansRobots(senderName, bData);
        }

        private void PacketReceived_Battery(BaseInternalObject newBIO, RobotModel mRobotModel)
        {
            Battery bData = newBIO as Battery;
            if (bData != null)
            {
                mRobotModel.BatteryLife = bData.Life;
                if (UpdateBatteryData != null)
                    UpdateBatteryData(mRobotModel.MountIndex, bData.Life);
            }
        }

        private void updateReceivedRangeScansRobots(string senderName, RangeScan rs)
        {
            if (receivedRangeScansRobots.ContainsKey(senderName))
                receivedRangeScansRobots[senderName] = rs;
            else
                receivedRangeScansRobots.TryAdd(senderName, rs);
        }

        #endregion

        #region WSS Video Messages Controller

        //protected internal void RobotWSSVideoPacket_Received(BaseInternalObject newBIO, string senderName)
        //{
        //    //if (!string.IsNullOrEmpty(mSelectedRobot))
        //    //    if (packet.SenderName != mSelectedRobot) return;

        //    RobotModel mRobotModel = RobotManager.GetRobotModelByName(senderName);

        //    try
        //    {
        //        //string pID = Enum.GetName(typeof(InternalMessagesID), packet.Data.MessageID);
        //        //ProjectCommons.writeConsoleMessage("Packet ID : " + pID + "/ Time in Queue:" +
        //        //                                   (packet.SendTime) + " Send Time : " + (Environment.TickCount - packet.SendOnSocketTime),
        //        //                                   ConsolMessageType.Information);

        //        Camera cPacket = ((Camera)newBIO);
        //        //Bitmap img = new Bitmap(new MemoryStream(cPacket.Image));

        //        //ProjectCommons.writeConsoleMessage("IsMultiView:" + cPacket.isMultiView, ConsolMessageType.Information);
        //        //if (cPacket.isMultiView)
        //        //{
        //        //    mRobotModel.SyncObject.WaitOne();
        //        //    mRobotModel.CameraImage = img;

        //        //    CameraControls[mRobotModel.MountIndex].Image = img;
        //        //    mRobotModel.SyncObject.ReleaseMutex();
        //        //}
        //        //else
        //        {
        //            int mountedRobots = RobotManager.Count;
        //            for (int i = 0; i < mountedRobots; i++)
        //            {
        //                //ImageServerLink.TileIndex tile = new ImageServerLink.TileIndex(i);

        //                //int offsetX = tile.TileX * USARConstant.SUB_CAM_WIDTH;
        //                //int offsetY = tile.TileY * USARConstant.SUB_CAM_HEIGHT;

        //                //if (offsetX + USARConstant.SUB_CAM_WIDTH > img.Width)
        //                //    offsetX = img.Width - USARConstant.SUB_CAM_WIDTH;
        //                //if (offsetY + USARConstant.SUB_CAM_HEIGHT > img.Height)
        //                //    offsetY = img.Height - USARConstant.SUB_CAM_HEIGHT;

        //                //Rectangle area = new Rectangle(offsetX, offsetY, USARConstant.SUB_CAM_WIDTH, USARConstant.SUB_CAM_HEIGHT);
        //                byte[] current = cPacket.Images[i];
        //                Bitmap RobotImage = new Bitmap(new MemoryStream(current));
        //                RobotModel r = RobotManager.GetRobotModelByMID(i);
        //                r.SyncObject.WaitOne();
        //                r.CameraImage = RobotImage;

        //                if (!CheckIFCameraImageAdded(r.MountIndex, r.UpdatedPose3D) && !r.Name.Equals(mSelectedRobot))
        //                    mGeoViewer.addRobotCameraImage(mCapturedImageCount++, RobotImage, r.UpdatedPose3D, r.Name);

        //                CameraControls[r.MountIndex].Image = r.CameraImage;
        //                r.SyncObject.ReleaseMutex();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        USARLog.println(e.ToString(), "BaseStation >>>>>>>> ");
        //    }
        //}

        protected internal bool CheckIFCameraImageAdded(int id, Pose2D pose)
        {
            RobotModel rm = RobotManager.GetRobotModelByMID(id);
            if (rm.PreviousCapturedPose != null)
            {
                //check it
                float movedDist = (float)MathHelper.getDistance(pose, rm.PreviousCapturedPose);
                float movedRot = (float)MathHelper.angleDif(pose.Rotation, rm.PreviousCapturedPose.Rotation, UnitType.UNIT_RAD);

                if (movedRot > 0.4375f)
                {
                    rm.PreviousCapturedPose = pose;
                    return false;
                }
                if (movedDist > 2f)
                {
                    rm.PreviousCapturedPose = pose;
                    return false;
                }

                return true;
            }
            else
            {
                rm.PreviousCapturedPose = pose;
                return false;
            }
        }

        public void CaptureCameraImage()
        {
            if (string.IsNullOrEmpty(mSelectedRobot)) return;

            RobotModel rm = RobotManager.GetRobotModelByName(mSelectedRobot);
            lock (rm)
            {
                if (AddRobotCameraImage != null)
                    AddRobotCameraImage(mCapturedImageCount++, rm.CameraImage, rm.UpdatedPose3D, rm.Name);
            }
        }

        public event Action<int, Bitmap, Pose3D, string> AddRobotCameraImage;

        #endregion

        #region USAR Messages Controller

        protected override void RobotUSARMessage_Received(string msg)
        {
        }

        #endregion
    }

}
