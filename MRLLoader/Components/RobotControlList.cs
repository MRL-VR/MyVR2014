using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MRL.Commons;
using MRL.Utils;
using MRL.Communication.Tools;
using MRL.ComStation;

namespace MRL.Components
{
    public partial class RobotControlList : UserControl
    {
        public static List<RobotControl> Cameras { get; private set; }

        public static RobotControlList Instance { get; private set; }

        static RobotControlList()
        {
            Instance = new RobotControlList();
        }

        private RobotControlList()
        {
            Instance = this;
            InitializeComponent();
            Dock = System.Windows.Forms.DockStyle.Fill;
            Cameras = new List<RobotControl>();
            AddRobotsCamera();
        }

        void CameraList_Load(object sender, EventArgs e)
        {
            BaseStation.Instance.UpdateBatteryData += (x, y) => Cameras[x].Battery = (int)y;
            BaseStation.Instance.UpdateImage += new Action<Bitmap[]>(mComStation_UpdateImage);
            BaseStation.Instance.RobotConnectionState += new Action<string, Signal>(mComStation_RobotConnectionState);
        }

        private void mComStation_UpdateImage(Bitmap[] obj)
        {
            for (int i = 0; i < Cameras.Count; i++)
                Cameras[i].Image = obj[i];
        }

        private void mComStation_RobotConnectionState(string arg1, Signal arg2)
        {
            var c = Cameras.Where(x => x.RobotName == arg1).FirstOrDefault();
            if (c != null)
            {
                c.Disconnected = !arg2.Status;
                c.Signal = arg2.Value;
                c.SignalStatus = arg2.Type;
            }
        }

        public static void Select(int p)
        {
            if (Cameras.Count >= p)
                Cameras[p - 1].IsSelected = true;
        }

        public static void UpdateData()
        {
            foreach (var item in Cameras)
                item.ShowData();
        }

        private void AddRobotsCamera()
        {
            List<Color> cameraColors = new List<Color>(new Color[] { Color.Yellow, Color.Red, Color.Green, 
                                                Color.Blue, Color.Cyan, Color.Magenta, 
                                                Color.Brown, Color.Purple });
            RobotControl.SelectedChanged += RobotCamera_SelectedChanged;

            try
            {
                for (int i = 0; i < ProjectCommons.config.botInfo.Count; i++)
                    if (ProjectCommons.config.botInfo[i].Spawned)
                        Cameras.Add(new RobotControl(ProjectCommons.config.botInfo[i], true));
                Controls.AddRange(Cameras.Select(x => x as Control).ToArray());
                ArrangeCameras();
            }
            catch (Exception)
            {
            }
        }

        private void RobotCamera_SelectedChanged(RobotControl sender)
        {
            if (sender.IsSelected)
            {
                var c = RobotControl.Selected;
                if (c != null)
                {
                    ProjectCommons.writeConsoleMessage("Selected Index = " + c.RobotMountIndex, ConsoleMessageType.Exclamation);

                    if (BaseStation.Instance != null)
                        new Thread(() => BaseStation.Instance.ChangeViewport(RobotControl.Selected.RobotName)) { IsBackground = true }.Start();
                }
            }
        }

        private void RobotControlList_SizeChanged(object sender, EventArgs e)
        {
            ArrangeCameras();
        }

        private void ArrangeCameras()
        {
            if (Cameras.Count > 0)
            {
                Point p = new Point();
                int w = Cameras[0].Width;
                int h = Cameras[0].Height;
                const int r = 4; // r = max items in each row
                int c = 0;
                for (int i = 0; i < Cameras.Count; i++)
                {
                    Cameras[i].Location = p;
                    p.X += w;
                    c++;

                    if (p.X + w > Width || c > r)
                    {
                        c = 0;
                        p.X = 0;
                        p.Y += h;
                    }
                }
            }
        }
    }
}
