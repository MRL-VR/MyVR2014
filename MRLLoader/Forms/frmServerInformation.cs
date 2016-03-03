using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using MRL.Commons;
using MRL.Utils;

namespace MRL.Components
{
    public partial class frmServerInformation : Form
    {
        public frmServerInformation()
        {
            InitializeComponent();
        }

        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtServerCMD.Text))
            {
                MessageBox.Show("Enter server command.");
                txtServerCMD.Focus();
                return;
            }
            txtOriginalMessage.Text = "";
            lstAssigned.Items.Clear();
            lstGoalPoints.Items.Clear();
            cmbPositions.Items.Clear();
            cmbRobots.Items.Clear();

            var btn = sender as Control;
            var txt = btn.Text;
            btn.Text = "Sending ...";
            btn.Enabled = false;
            Task.Factory.StartNew(() =>
            {
                Communication.Tools.Communication con = new Communication.Tools.Communication(ProjectCommons.config.simHost, ProjectCommons.config.simPort);
                string StartPoses = con.sendAndReceive(txtServerCMD.Text);
                //StartPoses = "NFO {StartPoses 32} {PlayerStart101709 17.8720,0.6680,1.4720 0.0000,0.0000,-4.7124 PlayerStart101710 19.6640,0.6680,1.4720 0.0000,0.0000,-4.7124 PlayerStart101711 19.6640,2.4600,1.4720 0.0000,0.0000,-4.7124 PlayerStart101712 17.8720,2.4600,1.4720 0.0000,0.0000,-4.7124 PlayerStart101714 16.9760,2.4600,1.4720 0.0000,0.0000,-4.7124 PlayerStart101715 16.9760,0.6680,1.4720 0.0000,0.0000,-4.7124 PlayerStart101716 20.5600,0.6680,1.4720 0.0000,0.0000,-4.7124 PlayerStart101717 20.5600,2.4600,1.4720 0.0000,0.0000,-4.7124 PlayerStart101718 17.8720,1.1960,1.4720 0.0000,0.0000,-4.7124 PlayerStart101719 19.6640,1.1960,1.4720 0.0000,0.0000,-4.7124 PlayerStart101720 16.9760,1.1960,1.4720 0.0000,0.0000,-4.7124 PlayerStart101721 20.5600,1.1960,1.4720 0.0000,0.0000,-4.7124 PlayerStart101722 19.6640,1.8200,1.4720 0.0000,0.0000,-4.7124 PlayerStart101723 17.8720,1.8200,1.4720 0.0000,0.0000,-4.7124 PlayerStart101724 16.9760,1.8200,1.4720 0.0000,0.0000,-4.7124 PlayerStart101725 20.5600,1.8200,1.4720 0.0000,0.0000,-4.7124 PlayerStart101727 7.1520,36.8680,1.4720 0.0000,0.0000,-1.5708 PlayerStart101728 5.3600,36.8680,1.4720 0.0000,0.0000,-1.5708 PlayerStart101729 5.3600,35.0760,1.4720 0.0000,0.0000,-1.5708 PlayerStart101730 7.1520,35.0760,1.4720 0.0000,0.0000,-1.5708 PlayerStart101732 8.0480,35.0760,1.4720 0.0000,0.0000,-1.5708 PlayerStart101733 8.0480,36.8680,1.4720 0.0000,0.0000,-1.5708 PlayerStart101734 4.4640,36.8680,1.4720 0.0000,0.0000,-1.5708 PlayerStart101735 4.4640,35.0760,1.4720 0.0000,0.0000,-1.5708 PlayerStart101736 7.1520,36.3400,1.4720 0.0000,0.0000,-1.5708 PlayerStart101737 5.3600,36.3400,1.4720 0.0000,0.0000,-1.5708 PlayerStart101738 8.0480,36.3400,1.4720 0.0000,0.0000,-1.5708 PlayerStart101739 4.4640,36.3400,1.4720 0.0000,0.0000,-1.5708 PlayerStart101740 5.3600,35.7160,1.4720 0.0000,0.0000,-1.5708 PlayerStart101741 7.1520,35.7160,1.4720 0.0000,0.0000,-1.5708 PlayerStart101742 8.0480,35.7160,1.4720 0.0000,0.0000,-1.5708 PlayerStart101743 4.4640,35.7160,1.4720 0.0000,0.0000,-1.5708}";
                if (String.IsNullOrEmpty(StartPoses))
                    MessageBox.Show("Nothing received.");
                this.Invoke(new Action(() =>
                {
                    txtOriginalMessage.Text = StartPoses;
                    btn.Enabled = true;
                    btn.Text = txt;
                }));
            });
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            lstAssigned.DisplayMember = "NamePosition";
            int pIndex = cmbPositions.SelectedIndex;
            int rIndex = cmbRobots.SelectedIndex;
            if (pIndex < 0 || pIndex > cmbPositions.Items.Count)
                return;
            if (rIndex < 0 || rIndex > cmbRobots.Items.Count)
                return;
            RobotInfo rInfo = (RobotInfo)cmbRobots.Items[rIndex];
            string[] s = cmbPositions.Items[pIndex].ToString().Split('@');
            rInfo.PositionString = s[0].Trim();
            rInfo.RotationString = s[1].Trim();
            lstAssigned.Items.Add(rInfo);
            cmbPositions.Items.RemoveAt(pIndex);
            cmbRobots.Items.RemoveAt(rIndex);

            if (cmbPositions.Items.Count > pIndex)
                cmbPositions.SelectedIndex = pIndex;
            if (cmbRobots.Items.Count > rIndex)
                cmbRobots.SelectedIndex = rIndex;
            if (cmbPositions.SelectedIndex == -1 && cmbPositions.SelectedIndex > 0)
                cmbPositions.SelectedIndex = 0;
            if (cmbRobots.SelectedIndex == -1 && cmbRobots.SelectedIndex>0)
                cmbRobots.SelectedIndex = 0;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAccpt_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtComStation.Text))
                ProjectCommons.config.mapperInfo.PositionString = txtComStation.Text;
            for (int i = 0; i < lstAssigned.Items.Count; i++)
            {
                RobotInfo temp = (RobotInfo)lstAssigned.Items[i];
                foreach (RobotInfo rInfo in ProjectCommons.config.botInfo)
                {
                    if (temp.Name == rInfo.Name)
                    {
                        rInfo.PositionString = temp.PositionString;
                        rInfo.RotationString = temp.RotationString;
                    }
                }
            }
            ProjectCommons.config.GoalPoints.Clear();
            for (int i = 0; i < lstGoalPoints.Items.Count; i++)
            {
                ProjectCommons.config.GoalPoints.Add(new MRL.CustomMath.Vector3(lstGoalPoints.Items[i].ToString()));
            }
            ProjectCommons.config.Save();
            Close();
        }

        private void btnParseCommand_Click(object sender, EventArgs e)
        {
            txtComStationParseCMD.Text = txtComStationParseCMD.Text.Trim().ToLower();
            txtRobotParseCMD.Text = txtRobotParseCMD.Text.Trim().ToLower();
            txtGoalPointParseCMD.Text = txtGoalPointParseCMD.Text.Trim().ToLower();
            cmbRobots.DisplayMember = "Name";
            string StartPoses = txtOriginalMessage.Text.Trim().ToLower();

            string temp = StartPoses;
            int sIndex;
            if (!string.IsNullOrEmpty(txtComStationParseCMD.Text))
            {
                sIndex = temp.IndexOf(txtComStationParseCMD.Text);
                if (sIndex != -1)
                {
                    string str = temp.Substring(sIndex);
                    string[] parsed = str.Split(' ');
                    if (parsed.Length >= 2)
                    {
                        if (parsed[1].EndsWith("}"))
                            parsed[1] = parsed[1].Replace("}", "");
                        txtComStation.Text = parsed[1];
                    }
                }
            }
            if (!string.IsNullOrEmpty(txtRobotParseCMD.Text))
            {
                temp = StartPoses;
                sIndex = temp.IndexOf(txtRobotParseCMD.Text);
                int count = 0;
                while (sIndex != -1)
                {
                    temp = temp.Substring(sIndex);
                    string[] parsed = temp.Split(' ');
                    if (parsed.Length >= 3)
                    {
                        if (parsed[2].EndsWith("}"))
                            parsed[2] = parsed[2].Replace("}", "");
                        cmbPositions.Items.Add(parsed[1] + " @ " + parsed[2]);
                        count++;
                    }
                    sIndex = temp.IndexOf(txtRobotParseCMD.Text, txtRobotParseCMD.Text.Length);
                }
                foreach (RobotInfo rInfo in ProjectCommons.config.botInfo)
                    cmbRobots.Items.Add(new RobotInfo(rInfo));
                gbRobotPosition.Text = "Robots Position (Count: " + count + ")";
            }
            if (!string.IsNullOrEmpty(txtGoalPointParseCMD.Text))
            {
                temp = StartPoses;
                sIndex = temp.IndexOf(txtGoalPointParseCMD.Text);
                int count = 0;
                while (sIndex != -1)
                {
                    temp = temp.Substring(sIndex);
                    string[] parsed = temp.Split(' ');
                    if (parsed.Length >= 2)
                    {
                        if (parsed[1].EndsWith("}"))
                            parsed[1] = parsed[1].Replace("}", "");
                        lstGoalPoints.Items.Add(parsed[1]);
                        count++;
                    }
                    sIndex = temp.IndexOf(txtGoalPointParseCMD.Text, txtGoalPointParseCMD.Text.Length);
                }
                gbGoalPoint.Text = "Goal Points (Count: " + count + ")";
            }

            if (cmbPositions.Items.Count > 0)
                cmbPositions.SelectedIndex = 0;
            if (cmbRobots.Items.Count > 0)
                cmbRobots.SelectedIndex = 0;
        }

        private void btnSelectedRemoveGoalPoints_Click(object sender, EventArgs e)
        {
            //for (int i =0 ;i<lstGoalPoints.SelectedItem.;i++){
            
            //}

            //lstGoalPoints.Items.
            //lstGoalPoints.SelectedItem
        }


    }
}
