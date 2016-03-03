using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MRL.Commons;
using MRL.Utils;

namespace MRL.Components
{
    public partial class frmMain : Form
    {
        #region "Private Variables"
        private string staticAddress = "";
        private const int DelayForRun = 1000;
        private bool inEditMode = false;
        private RegisteryController regEdit = new RegisteryController();
        private bool bTestMode = false;

        #endregion

        #region "Private Methods"

        private void init()
        {
            string address = Application.StartupPath;
            int len = address.IndexOf("MRLLoader");
            if (len == -1)
                return;
            staticAddress = address.Substring(0, len);
        }

        private void loadConfig(bool loadOD)
        {
            if (loadOD)
            {
                //get address from registery
                string defaultPath = regEdit.getRegValue(RegisteryController.Root.CurrentUser, ProjectCommons.RegisteryManager.CurrentRegisteryAddress, ProjectCommons.RegisteryManager.DefaultConfigAddress);
                string[] temp = defaultPath.Split('\\');
                string fileName = temp[temp.Length - 1];
                string initialDirectory = defaultPath.Substring(0, defaultPath.Length - fileName.Length);

                if (defaultPath != "" && Directory.Exists(initialDirectory))
                {
                    dlgOF.FileName = fileName;
                    dlgOF.InitialDirectory = initialDirectory;
                }
                else
                    dlgOF.InitialDirectory = staticAddress + "Config\\";


                if (dlgOF.ShowDialog() == DialogResult.OK)
                {
                    if (!String.IsNullOrEmpty(dlgOF.FileName))
                    {
                        ProjectCommons.config = new USARConfig(dlgOF.FileName);
                        regEdit.SetRegValue(RegisteryController.Root.CurrentUser, ProjectCommons.RegisteryManager.CurrentRegisteryAddress,
                                            ProjectCommons.RegisteryManager.DefaultConfigAddress, dlgOF.FileName, RegisteryController.DataType.String);
                        return;
                    }
                }

                //reload again
                loadConfig(true);
            }
            else { ProjectCommons.config.reloadConfig(); return; }
        }

        private void viewAgentsInList()
        {
            string name = "", type = "";

            inEditMode = true;

            lstAgents.Items.Clear();
            lstSpawn.Items.Clear();

            if (ProjectCommons.config != null)
            {
                name = ProjectCommons.config.mapperInfo.Name;
                type = ProjectCommons.config.mapperInfo.Type;
                lstAgents.Items.Add(name + " (" + type + ")", false);
                lstSpawn.Items.Add(name + " (" + type + ")", false);
                int len = ProjectCommons.config.botInfo.Count;
                for (int i = 0; i < len; i++)
                {
                    name = ProjectCommons.config.botInfo[i].Name;
                    type = ProjectCommons.config.botInfo[i].Type;
                    lstAgents.Items.Add(name + " (" + type + ")", false);
                    lstSpawn.Items.Add(name + " (" + type + ")", false);
                }

                string lstAgents_SelectedItems = regEdit.getRegValue(RegisteryController.Root.CurrentUser, ProjectCommons.RegisteryManager.CurrentRegisteryAddress, ProjectCommons.RegisteryManager.lstAgentsItemsSelected);
                string lstSpawn_SelectedItems = regEdit.getRegValue(RegisteryController.Root.CurrentUser, ProjectCommons.RegisteryManager.CurrentRegisteryAddress, ProjectCommons.RegisteryManager.lstSpawnItemsSelected);

                string[] t_index = lstAgents_SelectedItems.Split(',');
                foreach (string s in t_index)
                {
                    try { lstAgents.SetItemChecked(int.Parse(s), true); }
                    catch { }
                }

                t_index = lstSpawn_SelectedItems.Split(',');
                foreach (string s in t_index)
                {
                    try { lstSpawn.SetItemChecked(int.Parse(s), true); }
                    catch { }
                }
            }
            inEditMode = false;
            Enabled = true;
            btnStart.Focus();
        }

        private void startAgents()
        {
            this.Invoke(new Action(this.Hide));
            startRobots();
            startComStation();
        }

        private void startComStation()
        {
            List<int> SpawningRobots = new List<int>();
            for (int i = 1; i <= ProjectCommons.config.botInfo.Count; i++)
                if (lstAgents.GetItemChecked(i))
                {
                    ProjectCommons.config.botInfo[i - 1].Spawned = true;
                    ProjectCommons.config.botInfo[i - 1].MountIndex = i - 1;
                }

            if (lstSpawn.GetItemChecked(0))
                this.Invoke(new Action(() => new frmBaseStation().Show()));
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message + " \n\n Source:\n" + e.Exception.Source);
        }

        private void startRobots()
        {
            string SpawningRobots = "";
            int count = ProjectCommons.config.botInfo.Count;
            for (int i = 1; i <= count; i++)
            {
                //if in the lst its not checked then it shouldn't be run
                if (!lstAgents.GetItemChecked(i))
                    continue;

                SpawningRobots += (i - 1) + ",";
            }
            SpawningRobots = SpawningRobots.Substring(0, SpawningRobots.Length - 1);

            string address = "MRL.Robot.exe";

            //int spawnIndex = 0;
            for (int i = 1; i <= count; i++)
            {
                //if in the lst its not checked then it shouldn't be run
                if (!lstAgents.GetItemChecked(i))
                    continue;

                if (!lstSpawn.GetItemChecked(i))
                    continue;

                Thread.Sleep(DelayForRun);

                string argument = string.Format("CCFG {{Config#{0}}} {{robotIndex#{1}}} {{spawnIndex#{2}}} {{SR#{3}}} {{TM#{4}}}\r\n",
                                                ProjectCommons.config.configFileName, i - 1, i - 1, SpawningRobots, bTestMode);

                if (checkBoxRobotProcess.Checked)
                {
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var app = AppDomain.CreateDomain("MRLComStation");
                            app.ExecuteAssembly(address, argument.Split(' '));
                        }
                        catch (Exception x)
                        {
                            MessageBox.Show(x.ToString());
                        }
                    });
                }
                else
                    Process.Start(address, argument);
#if !DEBUG
                MessageBox.Show("Is Robot #" + i + " Run?");
#else
                Thread.Sleep(1000);
#endif
            }
        }

        #endregion

        #region "Private Form Methods"

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(102, 173, 219);
            Application.EnableVisualStyles();

            Enabled = false;
            Thread t = new Thread(FormLoad) { IsBackground = true };
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void FormLoad()
        {

            init();
            loadConfig(true);
            Invoke(new Action(viewAgentsInList));
            //Invoke(new Action(ShowMonitor));
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            frmConfig frm = new frmConfig();
            frm.ConfigFileName = ProjectCommons.config.configFileName;
            frm.ShowDialog();
            loadConfig(false);
            viewAgentsInList();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnSetting.Enabled = false;
            btnGetPose.Enabled = false;

            string lstA = "";
            string lstS = "";
            for (int i = 0; i < lstAgents.Items.Count; i++)
            {
                if (lstAgents.GetItemChecked(i))
                    lstA += i.ToString() + ",";
                if (lstSpawn.GetItemChecked(i))
                    lstS += i.ToString() + ",";
            }
            if (lstA.Length > 0)
            {
                lstA = lstA.Substring(0, lstA.Length - 1);
                regEdit.SetRegValue(RegisteryController.Root.CurrentUser, ProjectCommons.RegisteryManager.CurrentRegisteryAddress,
                                    ProjectCommons.RegisteryManager.lstAgentsItemsSelected, lstA, RegisteryController.DataType.String);
            }
            if (lstS.Length > 0)
            {
                lstS = lstS.Substring(0, lstS.Length - 1);
                regEdit.SetRegValue(RegisteryController.Root.CurrentUser, ProjectCommons.RegisteryManager.CurrentRegisteryAddress,
                                    ProjectCommons.RegisteryManager.lstSpawnItemsSelected, lstS, RegisteryController.DataType.String);
            }

            new Thread(startAgents) { IsBackground = true }.Start();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lstAgents_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (inEditMode) return;

            if (e.Index == 0)
                e.NewValue = CheckState.Checked;

            lstSpawn.SetItemChecked(e.Index, (e.NewValue == CheckState.Checked ? true : false));
        }

        private void lstSpawn_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (inEditMode) return;

            if (e.NewValue == CheckState.Checked)
            {
                if (!lstAgents.GetItemChecked(e.Index))
                    e.NewValue = CheckState.Unchecked;
            }
        }

        #endregion


        //private static string ROBOT_PREFIX = "RobotStart";
        //private static string ROBOT_PREFIX = "robot";
        //private static string WCS_PREFIX = "basestation";
        //private static string WCS_PREFIX = "commstation";
        //private static string GOAL_PREFIX = "target";
        private void btnGetPose_Click(object sender, EventArgs e)
        {
            frmServerInformation frm = new frmServerInformation();
            frm.Show();
            return;
            //Communication con = new Communication(ProjectCommons.config.simHost, ProjectCommons.config.simPort);


            //string StartPoses = con.sendAndReceive("GETSTARTPOSES");
            //if (String.IsNullOrEmpty(StartPoses))
            //    return;
            //MessageBox.Show("Original Message:\r\n" + StartPoses);
            //string[]  parsed = StartPoses.Split(' ');
            //string msg_mapper = "\n   Mapper Position: \n";
            //string msg_pose = "   Start Positions: \n";
            //string msg_gp = "   Goal Points: \n";

            //ProjectCommons.config.GoalPoints.Clear();

            //int robotsCount = 0;

            //int c = parsed.Length;
            //for (int j = 0; j < c; j++)
            //{
            //    parsed[j] = parsed[j].Replace("}", "");
            //    parsed[j] = parsed[j].Replace("{", "");
            //    parsed[j] = parsed[j].Replace("\r\n", "");

            //    if (parsed[j].ToLower().Contains(ROBOT_PREFIX.ToLower()))
            //        robotsCount++;
            //}
            ////
            //for (int j = 0; j < c; j++)
            //{
            //    if (parsed[j].ToLower().Contains(WCS_PREFIX.ToLower()))
            //    {
            //        j++;
            //        ProjectCommons.config.mapperInfo.PositionString = parsed[j];
            //        msg_mapper += "      " + " 1" + ") " + parsed[j];
            //        j++;
            //        ProjectCommons.config.mapperInfo.RotationString = parsed[j];
            //        msg_mapper += " - " + parsed[j];
            //        msg_mapper += Environment.NewLine;
            //        continue;
            //    }

            //    int count = robotsCount;
            //    for (int i = 0; i < count; i++)
            //    {
            //        if (i >= ProjectCommons.config.botInfo.Count)
            //            break;
            //        string temp = ROBOT_PREFIX + (i + 1);
            //        if (parsed[j].ToLower().Contains(temp.ToLower()))
            //        {
            //            j++;
            //            ProjectCommons.config.botInfo[i].PositionString = parsed[j];
            //            msg_pose += "      " + (i + 1).ToString() + ") " + parsed[j];
            //            j++;
            //            ProjectCommons.config.botInfo[i].RotationString = parsed[j];
            //            msg_pose += " - " + parsed[j];
            //            msg_pose += Environment.NewLine;
            //            continue;
            //        }
            //    }
            //    if (parsed[j].ToLower().Contains(GOAL_PREFIX.ToLower()))
            //    {
            //        j++;
            //        Vector3 p = new Vector3(parsed[j]);
            //        if (!ProjectCommons.config.GoalPoints.Contains(p))
            //        {
            //            ProjectCommons.config.GoalPoints.Add(p);
            //            msg_gp += "      " + (ProjectCommons.config.GoalPoints.Count).ToString() + ") " + p.ToString();
            //            msg_gp += Environment.NewLine;
            //        }
            //        continue;
            //    }
            //}

            //if (!String.IsNullOrEmpty(StartPoses))
            //{
            //    ProjectCommons.config.Save();
            //}

            //MessageBox.Show("All Point added successfully." + msg_mapper + msg_pose + msg_gp);
        }

        private void itmRun_Click(object sender, EventArgs e)
        {
            bTestMode = !((ToolStripMenuItem)sender == itmRun);

            foreach (ToolStripMenuItem itm in cmsRun.Items)
            {
                if (itm == (ToolStripMenuItem)sender)
                    itm.Font = new Font(itm.Font.FontFamily, itm.Font.Size, FontStyle.Bold);
                else
                    itm.Font = new Font(itm.Font.FontFamily, itm.Font.Size, FontStyle.Regular);
            }
        }

        private void btnShowMenu_Click(object sender, EventArgs e)
        {
            cmsRun.Show(gbbtn, new Point(btnStart.Location.X, btnStart.Location.Y + btnStart.Size.Height + 2));
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers.HasFlag(Keys.Control))
                if (e.KeyCode <= Keys.D8 && e.KeyCode >= Keys.D0)
                {
                    int n = e.KeyCode - Keys.D0;
                    for (int i = 0; i < lstAgents.Items.Count; i++)
                    {
                        lstAgents.SetItemChecked(i, i <= n);
                        lstSpawn.SetItemChecked(i, i <= n);
                    }
                }
        }
    }
}