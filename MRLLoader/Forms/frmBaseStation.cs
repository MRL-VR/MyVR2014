using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MRL.Commons;
using MRL.Communication.Internal_Objects;
using MRL.CustomMath;
using MRL.Utils;
using SlimDX.DirectInput;
using MRL.ComStation;
using MRL.Communication.Tools;
using System.ComponentModel;

namespace MRL.Components
{
    public partial class frmBaseStation : Form
    {
        private KeyBoardController mKeyboard;
        private PilotJoyStickController pilotJoyStickController;

        private Font fntError = new Font("Tahoma", 8, FontStyle.Italic | FontStyle.Bold);
        private int STime = 1200;
        private int resultTime = 900;
        private bool isResultMode = false;
        private int FPS;
        private bool isRobotSpawned = false;

        public frmBaseStation()
        {
            InitializeComponent();
        }

        private void frmBaseStation_Load(object sender, EventArgs e)
        {
            cmbImageSender_Robots.Items.AddRange(ProjectCommons.config.botInfo.Where(x => x.Spawned).ToArray());
            if (cmbImageSender_Robots.Items.Count > 0)
                cmbImageSender_Robots.SelectedIndex = 0;
            ProjectCommons.ConsoleMessage += new ProjectCommons._addConsoleMessage(AddDebugMessages);

            geoViewer.GridMapper = BaseStation.Instance.GlobalMap;

            BaseStation.Instance.OnCurrentFPS += f => FPS = f;
            BaseStation.Instance.GlobalMap.geoReferenceMap_Updated += GlobalMap_geoReferenceMap_Updated;
            BaseStation.Instance.updateRobot += BaseStationInstance_updateRobot;
            BaseStation.Instance.AddRobotCameraImage += (p1, p2, p3, p4) => geoViewer.addRobotCameraImage(p1, p2, p3, p4);
            BaseStation.Instance.ReceviedPath += new Action<List<Pose2D>, int>(Instance_ReceviedPath);
            BaseStation.Instance.FrontierListReceived += new Action<FrontierList>(frontierListReceived);
            BaseStation.Instance.updateSignal += new Action<List<SignalLine>>(Instance_updateSignal);

            ProjectCommons.missionWidget_OnNewListReceived += new ProjectCommons._newMissionReceived(_missionArrived);

            mKeyboard = new KeyBoardController();
            mKeyboard.KeyPressed += new KeyBoardController.keyPressed(Keyboard_KeyPressed);
            mKeyboard.Connect(this.Handle);


            pilotJoyStickController = new PilotJoyStickController();
            pilotJoyStickController.PilotJoyStickData_Received += new Action<JoyStickData>(pilotJoyStickController_PilotJoyStickData_Received);
            pilotJoyStickController.Start();

            ProjectCommons.targetWidget_OnNewPointReceived += new ProjectCommons._newTargetReceived(_targetArrived);

            new Thread(BaseStation.Instance.Mount) { IsBackground = true }.Start();
            CamerasForm.Instance.Show();
            geoVeiwerProperties.BrowsableAttributes = new AttributeCollection(
            new CategoryAttribute("Widgets"));
        }

        void Instance_updateSignal(List<SignalLine> obj)
        {
            if (isRobotSpawned)
            {
                geoViewer.updateSignals(obj);
            }
        }

        int i = 0;
        void frontierListReceived(FrontierList fl)
        {
            ProjectCommons.writeConsoleMessage("FrontierPoint List -> " + fl.FrontiersList.Count, ConsoleMessageType.Information);
            geoViewer.removeAllVictims();
            foreach (var item in fl.FrontiersList)
            {
                geoViewer.addVictim(i++, new Pose3D(item.FrontierPosition.Position.X, item.FrontierPosition.Position.Y, 0, 0, 0, 0), 0);
            }
        }

        void pilotJoyStickController_PilotJoyStickData_Received(JoyStickData obj)
        {
            try
            {
                if (RobotControl.Selected != null)
                {
                    string receiveerName = RobotControl.Selected.RobotName;
                    BaseStation.Instance.SendStringToBot(receiveerName, obj, MessagePriority.INP_MSG_MEDIUM);
                }
            }
            catch (Exception e)
            {
                ProjectCommons.writeConsoleMessage("Exception in MainForm->pilotJoyStickController_PilotJoyStickData_Received", ConsoleMessageType.Error);
                USARLog.println("Exception in MainForm->pilotJoyStickController_PilotJoyStickData_Received >> " + e, "MainForm");
            }
            //ProjectCommons.writeConsoleMessage("JoyStick Data : " + obj.X + " " + obj.Y + " " + obj.Z, ConsoleMessageType.Information);
        }

        void Instance_ReceviedPath(List<Pose2D> arg1, int arg2)
        {
            Mission missionData = new Mission();
            string recieverName = ProjectCommons.config.botInfo[arg2].Name;

            if (arg1.Count > 0)
                geoViewer.addMission(arg2, arg1);

            missionData.pList = arg1;
            BaseStation.Instance.SendStringToBot(recieverName, missionData);
            ProjectCommons.writeConsoleMessage("Automated Mission Data Sent !!!", ConsoleMessageType.Information);
        }

        void BaseStationInstance_updateRobot(int i, Pose3D p)
        {
            geoViewer.updateRobot(i, p);
            isRobotSpawned = true;
        }

        void GlobalMap_geoReferenceMap_Updated(Bitmap map)
        {
            geoViewer.updateGeomap(map);
        }

        private void _targetArrived(int index, Pose2D start, Pose2D goal)
        {
            //BaseStation.Instance.RequestPathPlannig(start, goal, index);
            //Mission missionData = new Mission();
            //string recieverName = ProjectCommons.config.botInfo[index].Name;

            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //List<Pose2D> path = BaseStation.Instance.FindPath(start, goal);
            //stopwatch.Stop();
            //ProjectCommons.writeConsoleMessage("PathPlannigTime!!!" + stopwatch.ElapsedMilliseconds, ConsoleMessageType.Information);

            //if (path.Count > 0)
            //    geoViewer.addMission(index, path);

            //missionData.pList = path;
            //BaseStation.Instance.SendStringToBot(recieverName, missionData);
            //ProjectCommons.writeConsoleMessage("Automated Mission Data Sent !!!", ConsoleMessageType.Information);
        }

        private void _missionArrived(int index, List<Pose2D> pList)
        {
            if (pList.Count > 0)
            {
                Mission missionData = new Mission();
                string recieverName = ProjectCommons.config.botInfo[index].Name;
                missionData.pList = pList;
                BaseStation.Instance.SendStringToBot(recieverName, missionData);
                ProjectCommons.writeConsoleMessage("Mission Data Sent !!!", ConsoleMessageType.Information);
            }
        }

        private void frmBaseStation_KeyDown(object sender, KeyEventArgs e)
        {
            geoViewer.Viewer_KeyDown(e);
        }

        private void frmBaseStation_KeyUp(object sender, KeyEventArgs e)
        {
            geoViewer.Viewer_KeyUp(e);
        }

        private void AddDebugMessages(string msg, ConsoleMessageType type)
        {
            try
            {
                ListViewItem l = null;
                switch (type)
                {
                    case ConsoleMessageType.Error:
                        l = new ListViewItem() { ForeColor = Color.Red, Text = "E", Font = fntError };
                        l.SubItems.Add("    " + msg, l.ForeColor, l.BackColor, l.Font);
                        lstDebugger.Items.Add(l);
                        break;

                    case ConsoleMessageType.Information:
                        l = new ListViewItem() { ForeColor = Color.Blue, Text = "I" };
                        l.SubItems.Add(msg, l.ForeColor, l.BackColor, l.Font);
                        lstDebugger.Items.Add(l);
                        break;

                    case ConsoleMessageType.Normal:
                        l = new ListViewItem() { ForeColor = Color.Black, Text = ">" };
                        l.SubItems.Add(msg, l.ForeColor, l.BackColor, l.Font);
                        lstDebugger.Items.Add(l);
                        break;

                    case ConsoleMessageType.Exclamation:
                        l = new ListViewItem() { ForeColor = Color.Yellow, BackColor = Color.Green, Text = "!!" };
                        l.SubItems.Add(msg, l.ForeColor, l.BackColor, l.Font);
                        lstDebugger.Items.Add(l);
                        break;
                }

                if (lstDebugger.Items.Count > 30)
                    lstDebugger.Items.RemoveAt(0);

                lstDebugger.TopItem = l;
            }
            catch { }
        }

        private void Keyboard_KeyPressed(KeyboardState states)
        {
            if (ProjectCommons.keyboardCapturedByGeoViewer) return;

            foreach (Key key in states.PressedKeys)
            {
                if (key >= Key.D1 && key <= Key.D9)
                    RobotControlList.Select((int)key);
                else if (RobotControl.Selected != null)
                {
                    RobotControl.Selected.ProcessRobotKey(key);
                    if (key == Key.Escape)
                        geoViewer.removeMission(RobotControl.Selected.RobotMountIndex);
                }
            }
        }

        private void SimulationTime_Tick(object sender, EventArgs e)
        {
            int time;
            if (isResultMode)
            {
                resultTime--;
                time = resultTime;
            }
            else
            {
                STime--;
                time = STime;
            }
            int s, m;
            m = time / 60;
            s = time % 60;

            if (isResultMode)
                txtResultRemainingTime.Text = (m < 10 ? ("0" + m) : m.ToString()) + ":" + (s < 10 ? ("0" + s) : s.ToString());
            else
                txtTime.Text = (m < 10 ? ("0" + m) : m.ToString()) + ":" + (s < 10 ? ("0" + s) : s.ToString());
        }

        private void btnResultMode_Click(object sender, EventArgs e)
        {
            isResultMode = !isResultMode;
            if (isResultMode)
                btnResultMode.Text = "Finish result mode";
            else
                btnResultMode.Text = "Begin result mode";
        }

        private void DataTimer_Tick(object sender, EventArgs e)
        {
            lblFPS.Text = FPS.ToString();
            RobotControlList.UpdateData();
        }

        private void frmBaseStation_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (BaseStation.Instance != null)
                BaseStation.Instance.Unmount();

            ProjectCommons.ConsoleMessage -= AddDebugMessages;
            Application.Exit();
        }



    }
}