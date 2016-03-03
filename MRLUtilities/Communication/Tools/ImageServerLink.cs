using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AForge.Imaging.Filters;
using MRL.Commons;
using MRL.Utils;

namespace MRL.Communication.Tools
{

    /// <summary>
    /// this class used for handle ImageServer connection 
    /// and deliver images that received to major class by signals
    /// </summary>
    public class ImageServerLink
    {
        public delegate void ImageServerPacket(List<byte[]> cam);
        public ImageServerPacket OnImageServerPacket_Received;

        #region Internal Classes

        /// <summary>
        /// this internal class create for identifying
        /// robot (row,col) index in MultiView images
        /// </summary>
        //public class TileIndex
        //{
        //    public int TileX { get; set; }
        //    public int TileY { get; set; }

        //    public TileIndex(int tX, int tY)
        //    {
        //        TileX = tX;
        //        TileY = tY;
        //    }
        //    public TileIndex(int spawnedIndex)
        //    {
        //        TileX = spawnedIndex % (ProjectCommons.config.MAX_CAM_WIDTH / ProjectCommons.config.SUB_CAM_WIDTH);
        //        TileY = spawnedIndex / (ProjectCommons.config.MAX_CAM_HEIGHT / ProjectCommons.config.SUB_CAM_HEIGHT);
        //    }
        //}

        #endregion

        #region Variable Declaration

        // image socket variables
        private string host;
        private int port;

        private Socket imgSocket;
        private NetworkStream networkStream;
        private BinaryReader binReader;
        private BinaryWriter binWriter;

        // thread controlling variables
        private AutoResetEvent reactivateSignal = new AutoResetEvent(false);
        private volatile bool waitForReactivating = false;
        private bool IsRunning = false;

        // variables for syncronizing image & buffer
        private byte[] fullImageRequest = null;
        private volatile bool isMultiView;

        // variables for retriving images
        private Rectangle[] viewAreas;
        Crop[] crop;
        private RobotInfo thisRobot;
        // variable for detecting if request is responed
        private bool isCurrentRequestResponsed = true;
        //private FrameRateCalculator FRC = new FrameRateCalculator();

        #endregion

        #region Property Declaration

        //public TileIndex CameraTileIndex { get; set; }
        public bool IsMultiViewImage
        {
            get
            {
                return isMultiView;
            }
            set
            {
                isMultiView = value;
            }
        }
        public bool IsConnected
        {
            get
            {
                return (networkStream != null);
            }
        }

        #endregion

        #region Public Functions Declaration

        /// <summary>
        /// constructor of class
        /// </summary>
        /// <param name="imgCP">ImageServer socket properties (IP,Port) for connect to</param>
        /// <param name="parentImageListenerSignal">if image received successfully this signal will be set</param>
        public ImageServerLink(string imgSrv, int imgPort, RobotInfo me)
        {
            this.host = imgSrv;
            this.port = imgPort;
            this.thisRobot = me;
        }

        private void MakeViewAreas(Bitmap bmpFull)
        {
            if (viewAreas != null)
                return;
            viewAreas = new Rectangle[ProjectCommons.config.botInfo.Count];
            crop = new Crop[viewAreas.Length];

            int w = ProjectCommons.config.MAX_CAM_WIDTH, h = ProjectCommons.config.MAX_CAM_HEIGHT;
            int tw = ProjectCommons.config.CAM_TILE_X, th = ProjectCommons.config.CAM_TILE_Y;

            for (int i = 0; i < ProjectCommons.config.botInfo.Count; i++)
            {
                RobotInfo rInfo = ProjectCommons.config.botInfo[i];
                int offsetX = (rInfo.MountIndex % tw) * ProjectCommons.config.SUB_CAM_WIDTH;
                int offsetY = (rInfo.MountIndex / tw) * ProjectCommons.config.SUB_CAM_HEIGHT;
                if (offsetX + ProjectCommons.config.SUB_CAM_WIDTH > bmpFull.Width)
                    offsetX = bmpFull.Width - ProjectCommons.config.SUB_CAM_WIDTH;
                if (offsetY + ProjectCommons.config.SUB_CAM_HEIGHT > bmpFull.Height)
                    offsetY = bmpFull.Height - ProjectCommons.config.SUB_CAM_HEIGHT;
                viewAreas[i] = new Rectangle(offsetX, offsetY, ProjectCommons.config.SUB_CAM_WIDTH, ProjectCommons.config.SUB_CAM_HEIGHT);
                crop[i] = new Crop(viewAreas[i]);
            }
        }

        /// <summary>
        /// connect to image server
        /// </summary>
        /// <returns>if it successful, function returns true</returns>
        public bool Connect()
        {
            if (IsConnected) return true;

            try
            {
                imgSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                imgSocket.Connect(host, port);

                if (imgSocket.Connected)
                {
                    fullImageRequest = Encoding.ASCII.GetBytes("OK");

                    networkStream = new NetworkStream(imgSocket, true);
                    binReader = new BinaryReader(networkStream);
                    binWriter = new BinaryWriter(networkStream);

                    USARLog.println(" >> " + "ImageSocket connected successfully!", this.ToString());
                    ProjectCommons.writeConsoleMessage("ImageSocket connected successfully!", ConsoleMessageType.Information);

                    waitForReactivating = false;
                    new Thread(Run) { IsBackground = true, Priority = ThreadPriority.Highest }.Start();

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                USARLog.println(" >> " + e.ToString(), this.ToString());
                ProjectCommons.writeConsoleMessage("    ImageSocket connected UnSuccessfully!", ConsoleMessageType.Error);

                return false;
            }
        }

        /// <summary>
        /// disconnect from image server
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (waitForReactivating)
                    reactivateSignal.Set();

                IsRunning = false;

                if (networkStream != null) networkStream.Close();
                if (binReader != null) binReader.Close();
                if (binWriter != null) binWriter.Close();

                binReader = null;
                binWriter = null;
                networkStream = null;
                imgSocket = null;
            }
            catch (Exception e)
            {
                USARLog.println(" >> " + e.ToString(), this.ToString());
                ProjectCommons.writeConsoleMessage(e.ToString(), ConsoleMessageType.Information);
            }
        }

        /// <summary>
        /// activate the hibernated thread if it not
        /// created this function calls Connect() 
        /// </summary>
        public void Activate()
        {
            if (!IsConnected)
                Connect();
            else
                reactivateSignal.Set();
        }

        /// <summary>
        /// hibernate the image receiver thread for 
        /// reducing cpu usages
        /// </summary>
        public void Deactivate()
        {
            waitForReactivating = true;
            reactivateSignal.Reset();
        }

        #endregion

        #region Private Functions Declaration

        /// <summary>
        /// receive new image from socket and fill imageBuffer
        /// </summary>
        /// <returns>if buffer corrupted, this returns (False) else returns (True)</returns>
        /// 
        private bool receiveImage()
        {
            byte type = binReader.ReadByte();
            int size = binReader.ReadInt32();
            size = (int)IPAddress.NetworkToHostOrder(size);

            var bytes = binReader.ReadBytes(size);

            if (type <= 1)
                return false;

            if ((size > 250000) || (size <= 0))
                return false;

            //USARLog.println(FRC.CalculateFrameRate().ToString(), "FPS");

            List<byte[]> viewPorts = new List<byte[]>();

            using (MemoryStream msCam = new MemoryStream(bytes))
            using (Bitmap bmpFull = new Bitmap(msCam))
            {
                MakeViewAreas(bmpFull);
                for (int i = 0; i < ProjectCommons.config.botInfo.Count; i++)
                {
                    RobotInfo rInfo = ProjectCommons.config.botInfo[i];
                    if (!rInfo.Spawned)
                    {
                        viewPorts.Add(new byte[0]);
                        continue;
                    }
                    using (MemoryStream s = new MemoryStream())
                    using (var b = crop[i].Apply(bmpFull))
                    {
                        b.Save(s, System.Drawing.Imaging.ImageFormat.Jpeg);
                        viewPorts.Add(s.ToArray());
                    }
                }
            }
            RaiseEvent(viewPorts);
            return true;
        }

        private void RaiseEvent(List<byte[]> cam)
        {
            if (OnImageServerPacket_Received != null)
                OnImageServerPacket_Received(cam);
        }

        /// <summary>
        /// ask new image from image server
        /// </summary>
        private void askImage()
        {
            //if (!isCurrentRequestResponsed) return;

            binWriter.Write(fullImageRequest);
            binWriter.Flush();

            isCurrentRequestResponsed = false;
        }

        /// <summary>
        /// cleaning buffer of socket (Bad Image In Socket)
        /// </summary>
        private void drainImage()
        {
            if (imgSocket.Available > 0)
                binReader.ReadBytes(imgSocket.Available);
        }

        /// <summary>
        /// main thread for controlling image
        /// socket and receiving images
        /// </summary>
        /// 
        private void Run()
        {
            IsRunning = true;
            while (IsRunning)
            {
                if (waitForReactivating)
                    if (reactivateSignal.WaitOne())
                        waitForReactivating = false;

                try
                {
                    askImage();
                    receiveImage();
                    //if (imgSocket.Available > 0)
                    //{
                    //    bool valid = false; //receiveImage();
                    //    try
                    //    {
                    //        valid = receiveImage();
                    //    }
                    //    catch
                    //    { valid = false; }

                    //    if (!valid)
                    //        drainImage();

                    //    if (imgSocket.Available == 0)
                    //        isCurrentRequestResponsed = true;
                    //}
                }
                catch (IOException ie) { }
                catch (ArgumentException ae)
                {
                    USARLog.println("ImgSocket >> " + ae.ToString(), this.ToString());
                }
            }
        }

        #endregion

    }

}
