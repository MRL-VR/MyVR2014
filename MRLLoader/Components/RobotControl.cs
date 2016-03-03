using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MRL.Communication.Internal_Commands;
using MRL.Communication.Internal_Objects;
using MRL.ImageProcessor;
using MRL.Utils;
using MRL.Communication.Tools;
using MRL.ComStation;

namespace MRL.Components
{
    public partial class RobotControl : UserControl
    {
        public delegate void SelectedChangedEventHandler(RobotControl sender);

        public static event SelectedChangedEventHandler SelectedChanged;

        private bool isSelected = false;
        private double prevMilisec;
        private double totalRemainTime = 0;
        private Bitmap _Image;

        public int Signal { set; get; }

        public SignalType SignalStatus { set; get; }

        public static RobotControl Selected { get; set; }

        public bool Disconnected { get; set; }

        private static object imageLock = new object();

        public RobotControl(RobotInfo robotInfo, bool disconnected)
        {
            InitializeComponent();
            MaximumSize = Size;
            Info = robotInfo;
            gbMain.Text = Info.Name + "  # " + robotInfo.MountIndex + "  (" + Info.Type + ")";
            Disconnected = disconnected;

            //if (RobotType != "Kenaf")
            //{
            //    Height -= gbArm.Height;
            //    gbArm.Visible = false;
            //}
        }

        public bool RobotLamp
        {
            get { return chkRobotLight.Checked; }
            set { chkRobotLight.Checked = value; }
        }

        public Bitmap Image
        {
            get
            {
                //if (Disconnected)
                //    return null;
                Bitmap i = null;
                lock (imageLock)
                {
                    if (_Image != null)
                        i = _Image.Clone() as Bitmap;
                }
                return i;
            }
            set
            {
                lock (imageLock)
                {
                    //if (Disconnected)
                    //    return;
                    _Image = value;
                    //if (IsSelected)
                    //    _Image = ImageFiltering.Instance.Apply(_Image);
                }
            }
        }

        public string RobotName { get { return Info.Name; } }
        public string RobotType { get { return Info.Type; } }
        public int RobotMountIndex { get { return Info.MountIndex; } }
        public int Battery { get; set; }

        public RobotInfo Info { get; private set; }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                selectRobot();
            }
        }

        public void ShowData()
        {
            if (Battery > prgBattery.Maximum)
                Battery = prgBattery.Maximum;
            try
            {
                prgBattery.Value = Battery;
            }
            catch {
                prgBattery.Value = 0;
            }
            double totalMSec = Environment.TickCount - prevMilisec;
            totalRemainTime = Battery * totalMSec;
            prevMilisec = Environment.TickCount;
            lblPercent.Text = prgBattery.Value + "% : ";
            lblDisconnect.Visible = Disconnected;
            routeLbl.Visible = (SignalStatus == SignalType.ROUTED ? true : false);
            picCameraImage.Image = Image;
            signalPercentageLbl.Text = Signal + "%";
            showSignal();
        }

        private void Control_DoubleClick(object sender, EventArgs e)
        {
            IsSelected = true;
        }

        private void showSignal()
        {
            int p = (Signal * 10) / 100;
            if (Disconnected)
            {
                signalImg.Image = MRL.Loader.Properties.Resources.Antena_0;
                return;
            }

            switch (p)
            {
                case 10:
                case 9:
                case 8:
                    signalImg.Image = MRL.Loader.Properties.Resources.Antena_100;
                    break;
                case 7:
                case 6:
                    signalImg.Image = MRL.Loader.Properties.Resources.Antena_80;
                    break;
                case 5:
                case 4:
                    signalImg.Image = MRL.Loader.Properties.Resources.Antena_60;
                    break;
                case 2:
                case 3:
                    signalImg.Image = MRL.Loader.Properties.Resources.Antena_40;
                    break;
                case 1:
                    signalImg.Image = MRL.Loader.Properties.Resources.Antena_20;
                    break;
                case 0:
                    signalImg.Image = MRL.Loader.Properties.Resources.Antena_0;
                    break;
            }
        }

        private void selectRobot()
        {
            if (IsSelected)
            {
                if (Selected == this)
                    return;
                if (Selected != null)
                    Selected.IsSelected = false;
                Selected = this;
            }
            BackColor = IsSelected ? Color.LightBlue : Color.White;
            if (SelectedChanged != null)
                SelectedChanged(this);
        }

        private void tmRemainTime_Tick(object sender, EventArgs e)
        {
            if (totalRemainTime == 0)
                return;
            totalRemainTime -= 1000;
            TimeSpan remainTime = new TimeSpan(0, 0, 0, 0, (int)totalRemainTime);
            if (remainTime.TotalMilliseconds > 0)
            {
                string rt = "";
                rt += (remainTime.Minutes < 10 ? "0" + remainTime.Minutes.ToString() : remainTime.Minutes.ToString());
                rt += ":" + (remainTime.Seconds < 10 ? "0" + remainTime.Seconds.ToString() : remainTime.Seconds.ToString());

                lblPercent.Text = prgBattery.Value + "% : " + rt;
            }
        }

        public CheckBox ArmChk_fl
        {
            get { return chkArm_fl; }
            set { chkArm_fl = value; }
        }

        public CheckBox ArmChk_fr
        {
            get { return chkArm_fr; }
            set { chkArm_fr = value; }
        }
        public CheckBox ArmChk_rl
        {
            get { return chkArm_rl; }
            set { chkArm_rl = value; }
        }
        public CheckBox ArmChk_rr
        {
            get { return chkArm_rr; }
            set { chkArm_rr = value; }
        }
        public void ArmDeg_Reduce(double val)
        {
            if (chkArm_fl.Checked)
            {
                double tmp = double.Parse(chkArm_fl.Text);
                chkArm_fl.Text = (tmp - val).ToString();
            }
            if (chkArm_fr.Checked)
            {
                double tmp = double.Parse(chkArm_fr.Text);
                chkArm_fr.Text = (tmp - val).ToString();
            }
            if (chkArm_rl.Checked)
            {
                double tmp = double.Parse(chkArm_rl.Text);
                chkArm_rl.Text = (tmp - val).ToString();
            }
            if (chkArm_rr.Checked)
            {
                double tmp = double.Parse(chkArm_rr.Text);
                chkArm_rr.Text = (tmp - val).ToString();
            }
        }
        public void ArmDeg_Add(double val)
        {
            if (chkArm_fl.Checked)
            {
                double tmp = double.Parse(chkArm_fl.Text);
                chkArm_fl.Text = (tmp + val).ToString();
            }
            if (chkArm_fr.Checked)
            {
                double tmp = double.Parse(chkArm_fr.Text);
                chkArm_fr.Text = (tmp + val).ToString();
            }
            if (chkArm_rl.Checked)
            {
                double tmp = double.Parse(chkArm_rl.Text);
                chkArm_rl.Text = (tmp + val).ToString();
            }
            if (chkArm_rr.Checked)
            {
                double tmp = double.Parse(chkArm_rr.Text);
                chkArm_rr.Text = (tmp + val).ToString();
            }
        }

        public List<double> getArmValues()
        {
            List<double> vals = new List<double>();

            vals.Add(double.Parse(chkArm_fl.Text));
            vals.Add(double.Parse(chkArm_fr.Text));
            vals.Add(-double.Parse(chkArm_rl.Text));
            vals.Add(-double.Parse(chkArm_rr.Text));

            return vals;
        }

        public void SendString(BaseInternalObject obj, MessagePriority messagePriority = MessagePriority.INP_MSG_MEDIUM)
        {
            BaseStation.Instance.SendStringToBot(RobotName, obj, messagePriority);
        }

        public void SendDriveCommand(double value, DriveType type)
        {
            SendString(new Drive(USARDrive.toUSARCommand(value, type)));
        }

        public void SendArmCommand(List<double> values)
        {
            SendString(new Drive(USARDrive.toArmCommand(values)));
        }
    }
}
