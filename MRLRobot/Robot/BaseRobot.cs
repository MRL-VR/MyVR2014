using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MRL.Commons;
using MRL.Communication.External_Commands;
using MRL.Communication.Internal_Commands;
using MRL.Communication.Internal_Objects;
using MRL.Communication.Tools;
using MRL.CustomMath;
using MRL.Exploration.ScanMatchers;
using MRL.Exploration.ScanMatchers.Base;
using MRL.Utils;
using MRL.Exploration.Frontiers;
using MRL.IDE.Base;
using MRL.Command;
using MRLRobot.Exploration;
using MRL.Robot.Skills;
using MRL.Command.Joystick;
using MRL.Command.Drive;
using MRLRobot.Robot.Skills.RobotAction;
using SlimDX.DirectInput;
using MRL.Mapping;
using MRL.Localization;

namespace MRL.IDE.Robot
{
    public class BaseRobot : BaseModel
    {
        #region Internal Classes

        internal class BaseStationProperties
        {
            public string BaseName;
            public int? BaseVideoPortNumber;

            public bool SendingViewportImage;

            public BaseStationProperties()
            {
                BaseName = ProjectCommons.config.mapperInfo.Name;
                BaseVideoPortNumber = null;
                SendingViewportImage = false;
            }
        }

        #endregion

        #region Static Valiables

        public static BaseRobot Instance;

        public static bool dualScanner = false;
        public static double maxSpeed = 4.0f;
        public static double minSpeed = 0.0f;
        public static double highSpeed = 2.0f;
        public static double lowSpeed = 0.8f;
        public static double speedScale = 0.15f;
        public static double safeRoll = 0.25f;
        public static double safePitch = 0.25f;
        public static float safeRange = 0.34f; // in meter
        public static float defaultFOV = 0.8727f; // in degree
        public static int taskTimeout = 60000; // 60 sec
        public static int MaxLastestPath = 5;

        #endregion

        #region Internal Valiables

        internal ImageServerLink mImageServer;
        internal BaseStationProperties mBaseStation = new BaseStationProperties();
        internal MessageParser mSimMsgParser = new MessageParser();
        internal AbstractDrive mMovingDriver;
        internal CmdGenerator joystickController;
        internal IScanMatchers mScanMatcher;

        internal BaseExploration robotExploration;
        internal BaseExploration semiAutonomous;
        internal LocalizationMode RobotLocalizationMode = LocalizationMode.LM_GROUNDTRUTH;
        internal bool UseSeed = false;
        internal ScanObservation mPrevScan;
        internal float mFactor = 1000f;
        internal Pose2D mDeltaPose;
        internal Pose2D mNullSeed = new Pose2D();
        internal Pose2D mPrevPosScanSent = null;
        internal int mPrevTimeScanSent;
        internal int currFrame = 0;
        internal CancellationTokenSource ctsExit = new CancellationTokenSource();
        internal volatile MachineState robotMachineState = MachineState.STA_DISCONNECTED;
        internal bool RobotHasLaser;
        internal Pose2D RangeScannerLocalPos;
        internal EGMap localMap;
        public IMULocalization imuLocalization;


        #endregion

        #region Public Methods
        public BaseRobot(RobotInfo me, int mountIndex)
            : base(me)
        {
            ThisRobot.MountIndex = mountIndex;
            WorldModel.Instance.EstimatedPose = ThisRobot.GetPosition();
            mScanMatcher = new WeightedScanMatcher();
        }
        public static Pose2D ConvertGlobalToCarmen(Pose2D p)
        {
            return new Pose2D(p.X, -p.Y, -p.Rotation);
        }
        public override void Mount()
        {
            try
            {
                mSimulationLink = new SimulationLink(ProjectCommons.config.simHost, ProjectCommons.config.simPort);
                //we must setup this event after initializing all variables that used in RobotUSARMessage_Received() function
                mSimulationLink.OnUSARMessage_Received += new SimulationLink.USARMessage(RobotUSARMessage_Received);
                mSimulationLink.Connect();

                if (mSimulationLink.IsConnected)
                {
                    mSimulationLink.Send(ThisRobot.GetINITCommand());
                    RobotSpawned = true;

                    Thread.Sleep(2000);

                    mNetworkManager = new NetworkManager(ThisRobot.Name, ThisRobot.RobotPort);
                    mNetworkManager.OnNewPacketReceived += new NetworkManager.OnNewPacketReceivedDlg(Run_RobotWSSStringPacket_Received);
                    mNetworkManager.Start(RobotLinkType.COMMAND_LINK);

                    if (mNetworkManager.GetRegisteredPorts != null)
                    {
                        ThisRobot.RobotPort = mNetworkManager.GetRegisteredPorts[0].portNumber;
                        RegisteredPorts.AddRange(mNetworkManager.GetRegisteredPorts);
                    }

                    SkillManager.Instance.MissionSkill.Actuators += new Action<string>(MissionSkill_Acuators);
                    SkillManager.Instance.ActionSkill.Actuators += new Action<string>(ActionSkill_Actuators);

                    mImageServer = new ImageServerLink(ProjectCommons.config.videoHost, ProjectCommons.config.videoPort, ThisRobot);
                    //mImageServer.CameraTileIndex = new ImageServerLink.TileIndex(ThisRobot.MountIndex);
                    mImageServer.IsMultiViewImage = false;
                    mImageServer.OnImageServerPacket_Received += new ImageServerLink.ImageServerPacket(RobotImageServerPacket_Received);

                    if (robotMachineState != MachineState.STA_EXPLORATION)
                        if (semiAutonomous != null)
                            semiAutonomous.Start();


                    if (localMap != null) localMap.start();

                    if (imuLocalization != null) imuLocalization.Start();

                    Task.Factory.StartNew(() =>
                    {
                        //bool _isStopped = false;
                        while (!ctsExit.IsCancellationRequested)
                        {
                            if (mNetworkManager != null)
                            {
                                string baseName;

                                lock (mBaseStation)
                                {
                                    baseName = mBaseStation.BaseName;
                                }
                                bool costToBase = mNetworkManager.GetSignalRobot(baseName).Status;
                                if (!costToBase && robotMachineState != MachineState.STA_DISCONNECTED)
                                {

                                    string Command = mMovingDriver.GetStopCommand();

                                    if (robotMachineState != MachineState.STA_BEGINMISSION)
                                        if (mSimulationLink != null)
                                            mSimulationLink.Send(Command);

                                    if (mImageServer != null)
                                    {
                                        mImageServer.Deactivate();
                                        ProjectCommons.writeConsoleMessage("False/ImageServer", ConsoleMessageType.Exclamation);
                                    }
                                    ProjectCommons.writeConsoleMessage("Disconnected", ConsoleMessageType.Information);

                                    if (robotMachineState != MachineState.STA_LOCK)
                                        robotMachineState = MachineState.STA_DISCONNECTED;
                                }
                                if (costToBase && robotMachineState == MachineState.STA_DISCONNECTED)
                                {
                                    ProjectCommons.writeConsoleMessage("Connected", ConsoleMessageType.Information);

                                    if (robotExploration != null)
                                        if (robotExploration.IsStarted())
                                            robotMachineState = MachineState.STA_EXPLORATION;
                                        else
                                            robotMachineState = MachineState.STA_JOYPAD;
                                }

                            }

                            Thread.Sleep(1000);
                        }
                    }, ctsExit.Token);

                }
            }
            catch (Exception e)
            {
                USARLog.println(" >> " + e.ToString(), this.ToString());
                ProjectCommons.writeConsoleMessage("Error In BaseRobot.Mount() : " + e.ToString(), ConsoleMessageType.Error);
            }
        }

        public override void Unmount()
        {
            if (RobotSpawned)
            {
                ctsExit.Cancel();

                mSimulationLink.OnUSARMessage_Received -= RobotUSARMessage_Received;
                mImageServer.OnImageServerPacket_Received -= RobotImageServerPacket_Received;

                if (mNetworkManager != null) mNetworkManager.Stop();
                if (mImageServer.IsConnected) mImageServer.Disconnect();
                if (mSimulationLink.IsConnected) mSimulationLink.Disconnect();
                if (localMap != null) localMap.stop();
                if (imuLocalization != null) imuLocalization.Stop();
            }
        }

        public Signal GetRobotSignal(string botName)
        {
            return mNetworkManager.GetSignalRobot(botName);
        }

        #endregion

        #region UTServer Controller
        //private float lastBatteryLife = 0f;
        private int BatteryLifeSendingTime = 0;
        protected override void RobotUSARMessage_Received(string msg)
        {
            try
            {
                mSimMsgParser.SimulationMessage = msg;
                switch (mSimMsgParser.MessageType)
                {
                    #region "STATE"

                    case (int)eSimulationMessageType.State:
                        {
                            State mNOW = mSimMsgParser.MessageData as State;

                            if (WorldModel.Instance.BatteryMaximumeCapacity == 0)
                                WorldModel.Instance.BatteryMaximumeCapacity = mNOW.iBatteryLife;

                            WorldModel.Instance.BatteryLife = (mNOW.iBatteryLife / WorldModel.Instance.BatteryMaximumeCapacity) * 100f;
                            WorldModel.Instance.LightState = mNOW.bLight;
                            WorldModel.Instance.ServerTime = mNOW.fTime;

                            if (checkIFBatteryMustSent())
                            {
                                int currTime = Environment.TickCount;
                                if ((currTime - BatteryLifeSendingTime) > 5000)
                                {
                                    Battery bt = new Battery(WorldModel.Instance.BatteryLife);
                                    SendMessageToComStation(bt, MessagePriority.INP_MSG_LOW);
                                    BatteryLifeSendingTime = currTime;
                                }
                            }
                        }
                        break;

                    #endregion

                    #region "GroundTruth"

                    case (int)eSimulationMessageType.GroundTruth:
                        {
                            GroundTruth mGTH = mSimMsgParser.MessageData as GroundTruth;

                            if (RobotLocalizationMode == LocalizationMode.LM_GROUNDTRUTH)
                            {
                                if (WorldModel.Instance.GTHPose3D != null)
                                {
                                    mDeltaPose = mGTH.GetPose3D() - WorldModel.Instance.GTHPose3D;
                                }

                                WorldModel.Instance.EstimatedPose = mGTH.GetPose2D();
                            }
                            WorldModel.Instance.GTHPose3D = mGTH;

                        }
                        break;

                    #endregion

                    #region "Odometry"

                    case ((int)eSimulationMessageType.Odometdy):
                        {
                            //Odometry mODO = mSimMsgParser.MessageData as Odometry;

                            //if (RobotLocalizationMode == LocalizationMode.LM_ODO_SLAM)
                            //{
                            //    if (mRobotModel.OdoPose2D != null)
                            //    {
                            //        mDeltaPose = mODO.GetPose2D() - mRobotModel.OdoPose2D;
                            //        mDeltaPose = new Pose2D(-mDeltaPose.Y, mDeltaPose.X, mDeltaPose.Rotation);
                            //    }
                            //}
                            //else if (RobotLocalizationMode == LocalizationMode.LM_RAW_ODOMETRY)
                            //{
                            //    mRobotModel.EstimatedPose = mODO.GetPose2D();
                            //}

                            //mRobotModel.OdoPose2D = mODO;
                        }
                        break;

                    #endregion

                    #region "INS"

                    case ((int)eSimulationMessageType.INS):
                        {
                            INS mINS = mSimMsgParser.MessageData as INS;

                            if (RobotLocalizationMode == LocalizationMode.LM_INS_SLAM)
                            {
                                if (WorldModel.Instance.INSPose3D != null)
                                {
                                    mDeltaPose = mINS.GetPose3D() - WorldModel.Instance.INSPose3D;
                                }
                            }
                            else if (RobotLocalizationMode == LocalizationMode.LM_RAW_INS)
                            {
                                WorldModel.Instance.EstimatedPose = mINS.GetPose2D();
                            }

                            WorldModel.Instance.INSPose3D = mINS.GetPose3D();

                            if (!RobotHasLaser)
                            {
                                if (checkIFScanMustSent())
                                {
                                    Position3D p = new Position3D(WorldModel.Instance.INSPose3D);
                                    SendMessageToComStation(p, MessagePriority.INP_MSG_LOW);
                                }
                            }
                        }
                        break;

                    #endregion

                    #region "Sonar"

                    case (int)eSimulationMessageType.Sonar:
                        {
                            Sonar mSonars = mSimMsgParser.MessageData as Sonar;
                            WorldModel.Instance.SonarManager.Update(mSonars);
                        }
                        break;

                    #endregion

                    #region "IMU"
                    case (int)eSimulationMessageType.IMU:
                        IMU mIMU = mSimMsgParser.MessageData as IMU;
                        WorldModel.Instance.RawIMU = mIMU;
                        if (imuLocalization != null)
                            imuLocalization.InsertNewIMU(mIMU);

                        break;

                    #endregion

                    #region "Laser"

                    case (int)eSimulationMessageType.Laser:
                        {
                            if (!RobotHasLaser) break;

                            Laser mLsr = mSimMsgParser.MessageData as Laser;
                            WorldModel.Instance.CurrentScan = new Laser(mLsr);

                            if (RobotLocalizationMode == LocalizationMode.LM_INS_SLAM ||
                                RobotLocalizationMode == LocalizationMode.LM_ODO_SLAM)
                            {
                                if (mScanMatcher != null)
                                {
                                    if (mPrevScan == null)
                                        mPrevScan = new ScanObservation(mFactor, WorldModel.Instance.CurrentScan);
                                    else
                                    {
                                        ScanObservation mScan = new ScanObservation(mFactor, WorldModel.Instance.CurrentScan);

                                        {
                                            MatchResult mResult = null;

                                            if (UseSeed && mDeltaPose != null)
                                                mResult = mScanMatcher.Match(mPrevScan, mScan, mDeltaPose);
                                            else
                                                mResult = mScanMatcher.Match(mPrevScan, mScan, mNullSeed);

                                            if (mResult.Converged)
                                            {
                                                Pose2D es = mResult.EstimatedOdometry;
                                                es.Position /= mFactor;
                                                es.Rotation = -es.Rotation;
                                                es = MathHelper.CorrectAngle(es);

                                                WorldModel.Instance.EstimatedPose = IcpScanMatcher.CompoundPoses(WorldModel.Instance.EstimatedPose, es);
                                            }
                                            else
                                            {
                                                if (UseSeed)
                                                    WorldModel.Instance.EstimatedPose = IcpScanMatcher.AddPoses(WorldModel.Instance.EstimatedPose, mDeltaPose);
                                            }

                                            mDeltaPose = new Pose2D();
                                            mPrevScan = mScan;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (mDeltaPose != null)
                                {
                                    WorldModel.Instance.EstimatedPose = IcpScanMatcher.AddPoses(WorldModel.Instance.EstimatedPose, mDeltaPose);
                                    mDeltaPose = new Pose2D();
                                }
                            }

                            if (WorldModel.Instance.EstimatedPose == null) break;

                            Pose2D sensorPose = new Pose2D();
                            sensorPose.X = WorldModel.Instance.EstimatedPose.X +
                                           RangeScannerLocalPos.X * Math.Cos(WorldModel.Instance.EstimatedPose.Rotation) +
                                           RangeScannerLocalPos.Y * Math.Sin(WorldModel.Instance.EstimatedPose.Rotation);

                            sensorPose.Y = WorldModel.Instance.EstimatedPose.Y +
                                           RangeScannerLocalPos.X * Math.Sin(WorldModel.Instance.EstimatedPose.Rotation) -
                                           RangeScannerLocalPos.Y * Math.Cos(WorldModel.Instance.EstimatedPose.Rotation);

                            sensorPose.Rotation = WorldModel.Instance.EstimatedPose.Rotation + RangeScannerLocalPos.Rotation;

                            WorldModel.Instance.CurrentScan.SensorGlobalPose = sensorPose;
                            WorldModel.Instance.CurrentScan.RobotGlobalPose = new Pose2D(WorldModel.Instance.EstimatedPose);

                            if (checkIFScanMustSent())
                            {
                                //we should send this message to comStation
                                ProjectCommons.ConsoleMessage("Doneeeeeeeee", ConsoleMessageType.Information);

                                if (WorldModel.Instance.CurrentScan != null && WorldModel.Instance.INSPose3D != null)
                                {
                                    RangeScan rs = new RangeScan(WorldModel.Instance.CurrentScan)
                                    {
                                        SensorGlobalPose3D = new Pose3D(sensorPose.X, sensorPose.Y, WorldModel.Instance.INSPose3D.Z,
                                                                        WorldModel.Instance.INSPose3D.NormalizedYaw(),
                                                                        WorldModel.Instance.INSPose3D.NormalizedPhi(),
                                                                        sensorPose.Rotation)
                                    };


                                    if (rs != null)
                                    {
                                        SendMessageToComStation(rs, MessagePriority.INP_MSG_LOW);
                                        localMap.addScan(WorldModel.Instance.CurrentScan);
                                        localMap.addView(WorldModel.Instance.EstimatedPose);
                                    }
                                }
                            }
                        }
                        break;

                    #endregion
                }
            }
            catch (Exception e)
            {
                USARLog.println(" >> " + e.ToString(), this.ToString());
                ProjectCommons.writeConsoleMessage(" >> " + e.ToString(), ConsoleMessageType.Error);
            }
        }

        private bool checkIFScanMustSent()
        {
            if (mPrevPosScanSent == null)
            {
                mPrevPosScanSent = new Pose2D(WorldModel.Instance.EstimatedPose);
                mPrevTimeScanSent = Environment.TickCount;

                return true;
            }
            else
            {
                double movedDist = MathHelper.getDistance(mPrevPosScanSent, WorldModel.Instance.EstimatedPose);
                double movedRot = MathHelper.angleDif(mPrevPosScanSent.Rotation, WorldModel.Instance.EstimatedPose.Rotation, UnitType.UNIT_RAD);

                if (movedRot > 0.1)
                {
                    mPrevPosScanSent = new Pose2D(WorldModel.Instance.EstimatedPose);
                    mPrevTimeScanSent = Environment.TickCount;
                    return true;
                }

                if (movedDist > 0.3)
                {
                    mPrevPosScanSent = new Pose2D(WorldModel.Instance.EstimatedPose);
                    mPrevTimeScanSent = Environment.TickCount;
                    return true;
                }

                if (Environment.TickCount - mPrevTimeScanSent > 3000)
                {
                    mPrevTimeScanSent = Environment.TickCount;
                    return true;
                }

                return false;
            }
        }

        private bool checkIFBatteryMustSent()
        {
            return true;
        }

        #endregion

        #region WSS Controller

        protected override void RobotWSSStringPacket_Received(BaseInternalObject newBIO, string senderName, List<Hop> interfaceHops)
        {
            try
            {
                switch (newBIO.MessageID)
                {
                    case (byte)InternalMessagesID.AutonomousChange:
                        {
                            if (robotMachineState == MachineState.STA_LOCK)
                            {
                                ProjectCommons.writeConsoleMessage("Robot is lock.", ConsoleMessageType.Error);
                                break;
                            }
                            AutonomousChange ac = newBIO as AutonomousChange;

                            if (robotExploration == null)
                            {
                                ProjectCommons.writeConsoleMessage("You dont have exploration for this kind of robot.", ConsoleMessageType.Information);
                                break;
                            }
                            if (!robotExploration.IsStarted())
                            {
                                robotMachineState = MachineState.STA_EXPLORATION;
                                ProjectCommons.writeConsoleMessage("Autonomous command is received, Current State = " + robotMachineState, ConsoleMessageType.Information);
                                if (semiAutonomous != null)
                                    semiAutonomous.Stop();
                                robotExploration.Start();
                            }
                            else
                            {
                                ProjectCommons.writeConsoleMessage("Your Robot Is On Autonomous System", ConsoleMessageType.Information);
                            }
                        }
                        break;

                    case (byte)InternalMessagesID.Drive:
                        {
                            if (robotMachineState == MachineState.STA_LOCK)
                            {
                                ProjectCommons.writeConsoleMessage("Robot is lock.", ConsoleMessageType.Error);
                                break;
                            }

                            if (robotMachineState == MachineState.STA_EXPLORATION)
                            {
                                ProjectCommons.writeConsoleMessage("Robot is on exploration.", ConsoleMessageType.Error);
                                break;
                            }

                            switch (robotMachineState)
                            {
                                case MachineState.STA_BEGINMISSION:
                                    robotMachineState = MachineState.STA_JOYPAD;
                                    if (semiAutonomous != null)
                                        semiAutonomous.Stop();
                                    ProjectCommons.writeConsoleMessage("Keyboard Mode Enabled And Mission Skill Is Canceled", ConsoleMessageType.Information);
                                    break;
                            }

                            string cmd = (newBIO as Drive).Command;
                            cmd = mMovingDriver.GetCommand(cmd);
                            mSimulationLink.Send(cmd);
                        }
                        break;

                    case (byte)InternalMessagesID.PressedKeys:
                        {
                            PressedKeys newPacket = newBIO as PressedKeys;
                            if (newPacket.keysList.Count > 0)
                            {
                                if (newPacket.keysList.Contains(Key.L))
                                {
                                    if (robotMachineState != MachineState.STA_LOCK)
                                    {
                                        robotMachineState = MachineState.STA_LOCK;
                                        if (robotExploration != null)
                                            robotExploration.Stop();
                                        if (semiAutonomous != null)
                                            semiAutonomous.Stop();
                                        ProjectCommons.writeConsoleMessage("Lock is locked.", ConsoleMessageType.Information);
                                    }
                                    else
                                    {
                                        robotMachineState = MachineState.STA_JOYPAD;

                                        if (semiAutonomous != null)
                                            semiAutonomous.Start();

                                        if (robotExploration != null)
                                            robotExploration.Stop();

                                        ProjectCommons.writeConsoleMessage("Lock is unlocked.", ConsoleMessageType.Information);

                                    }
                                }
                                else if (newPacket.keysList.Contains(Key.Escape))
                                {
                                    if (robotMachineState == MachineState.STA_EXPLORATION)
                                    {
                                        robotMachineState = MachineState.STA_JOYPAD;
                                        if (robotExploration != null)
                                            robotExploration.Stop();
                                        if (semiAutonomous != null)
                                            semiAutonomous.Start();
                                        ProjectCommons.writeConsoleMessage("Joyspad Mode Is Enabled And Autonomous System Is Disabled.", ConsoleMessageType.Information);
                                    }
                                }
                            }
                        }
                        break;

                    case (byte)InternalMessagesID.JoyStickData:
                        {
                            if (robotMachineState == MachineState.STA_LOCK)
                            {
                                ProjectCommons.writeConsoleMessage("Robot is lock.", ConsoleMessageType.Error);
                                break;
                            }
                            if (robotMachineState == MachineState.STA_EXPLORATION)
                            {
                                ProjectCommons.writeConsoleMessage("Robot is on exploration.", ConsoleMessageType.Error);
                                break;
                            }
                            switch (robotMachineState)
                            {
                                case MachineState.STA_BEGINMISSION:
                                    robotMachineState = MachineState.STA_JOYPAD;
                                    if (semiAutonomous != null)
                                        semiAutonomous.Stop();
                                    ProjectCommons.writeConsoleMessage("Joyspad Mode Enabled And Mission Skill Is Canceled", ConsoleMessageType.Information);
                                    break;
                            }

                            JoyStickData newPacket = newBIO as JoyStickData;
                            string cmd = joystickController.GetDriveCommnad(newPacket);

                            mSimulationLink.Send(cmd);
                        }
                        break;
                    case (byte)InternalMessagesID.ImageServer:
                        {
                            if ((ImageTransferType)ProjectCommons.config.IMAGE_TRANSFER_TYPE == ImageTransferType.NETWORK)
                            {
                                ImageServer mImgSrv = newBIO as ImageServer;

                                ProjectCommons.writeConsoleMessage("ImageServer Status = " + mImgSrv.Status, ConsoleMessageType.Information);

                                lock (mBaseStation)
                                {
                                    mBaseStation.BaseName = senderName;
                                    mBaseStation.BaseVideoPortNumber = mImgSrv.PortNumber;
                                    mBaseStation.SendingViewportImage = mImgSrv.Status;
                                }

                                if (mImgSrv.Status)
                                    mImageServer.Activate();
                                else
                                    mImageServer.Deactivate();
                            }
                            else
                            {
                                ProjectCommons.writeConsoleMessage("Image Server Is On Direct Mode", ConsoleMessageType.Exclamation);
                            }
                        }
                        break;

                    case (byte)InternalMessagesID.Mission:
                        {

                            if (robotMachineState == MachineState.STA_LOCK)
                            {
                                ProjectCommons.writeConsoleMessage("Robot is lock.", ConsoleMessageType.Error);
                                break;
                            }

                            if (robotMachineState == MachineState.STA_JOYPAD)
                            {
                                if (semiAutonomous != null)
                                {
                                    robotMachineState = MachineState.STA_BEGINMISSION;
                                    Mission mRobot = newBIO as Mission;
                                    SemiAutonomous missions = semiAutonomous as SemiAutonomous;
                                    missions.StartMission(mRobot);
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                ProjectCommons.writeConsoleMessage("Exeption in BaseRobot->RobotWSSStringPacket_Received-> " + e.ToString(), ConsoleMessageType.Error);
                USARLog.println("Exception in BaseRobot->RobotWSSStringPacket_Received = " + e, "BaseRobot");
            }
        }
        private void SendMessageToComStation(BaseInternalObject msg, MessagePriority priority = MessagePriority.INP_MSG_LOW)
        {
            string baseName = "";
            ProjectCommons.ConsoleMessage(msg.GetType()+"", ConsoleMessageType.Information);
            lock (mBaseStation)
            {
                baseName = mBaseStation.BaseName;
            }

            if (!string.IsNullOrEmpty(baseName))
            {
                if (mNetworkManager != null)
                {
                    mNetworkManager.SendStringToBot(baseName, msg, priority);
                }
            }
        }

        #endregion

        #region ImageServer Controller

        int skip = 0;

        protected internal void RobotImageServerPacket_Received(List<byte[]> viewports)
        {
            //if (++skip % 6 != 0)
            //    return;

            if ((ImageTransferType)ProjectCommons.config.IMAGE_TRANSFER_TYPE == ImageTransferType.DIRECT)
            {
                ProjectCommons.writeConsoleMessage("ImageServer Has Sent Data,But We Are In Direct Mode", ConsoleMessageType.Error);
                return;
            }

            if (!mImageServer.IsConnected) return;

            string baseName = "";
            bool viewPort = false;
            int? baseVideoPort;
            lock (mBaseStation)
            {
                baseName = mBaseStation.BaseName;
                viewPort = mBaseStation.SendingViewportImage;
                baseVideoPort = mBaseStation.BaseVideoPortNumber;
            }

            if (viewPort)
            {
                if (!string.IsNullOrEmpty(baseName) &&
                    baseVideoPort.HasValue)
                {
                    Camera CamMsg = new Camera() { Images = viewports, isMultiView = mImageServer.IsMultiViewImage, sequenceNumber = currFrame++ };
                    mNetworkManager.SendVideoToBot(baseName, baseVideoPort.Value, CamMsg, MessagePriority.INP_MSG_LOW);
                }
            }
        }

        #endregion

        #region Events

        private void MissionSkill_Acuators(string cmd)
        {
            mSimulationLink.Send(cmd);
        }

        private void ActionSkill_Actuators(string cmd)
        {
            mSimulationLink.Send(cmd);
        }

        #endregion
    }
}
