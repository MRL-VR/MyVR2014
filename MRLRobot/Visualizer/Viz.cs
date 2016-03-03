using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using MRL.Utils;
using MRL.Mapping;
using MRL.CustomMath;
using MRL.Communication.Internal_Objects;
using MRL.Communication.External_Commands;
using System.Windows;
using VisualizerLibrary;
using MRL.IDE.Robot;
using MRL.Commons;
using MRL.Exploration.Frontiers;
using MRL.Utils.GMath;
using MRL.Exploration.PathPlannig;

namespace MRLRobot
{
    public partial class Viz : Form
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        private Task myTask;
        private FrontierDetection frontierDetection;
        private Bitmap currBitmap;
        private RRTConnect rrtConnect = new RRTConnect();

        public Viz(bool IsPlayer = false)
        {
            InitializeComponent();

            recordLbl.Visible = false;

            frontierDetection = new FrontierDetection();

            myTask = new Task(() => run());
            if (!IsPlayer)
                myTask.Start();


            SlimControl1.MouseDownEvent += new NewSlimControl.MouseDownDel(SlimControl1_MouseDownEvent);

            if (BaseRobot.Instance != null && BaseRobot.Instance.ThisRobot.Type != "AirRobot")
                BaseRobot.Instance.localMap.geoReferenceMap_Updated += new MRL.Commons.ProjectCommons._geoRefrencedMap_Updated(LocalMap_geoReferenceMap_Updated);
        }

        void SlimControl1_MouseDownEvent(Pose2D P)
        {
            Visualizer.DrawPoint("path", P, Color.Orange);
            List<Pose2D> path = rrtConnect.FindPathRRTConnect(WorldModel.Instance.EstimatedPose, P, WorldModel.Instance.CurrentScan, 500);
            if (path != null)
                Visualizer.DrawListPoint("rrtPath", path, Color.Cyan);

            List<Line> l_list = rrtConnect.myMap.Select(a => new Line(new Position2D(a.Head), new Position2D(a.Tail) { DrawColor = Color.Black })).ToList();

            Visualizer.ClearStartWith("muli");
            for (int i = 0; i < l_list.Count; i++)
            {
                Visualizer.DrawLine("muli" + i, l_list[i].Head, l_list[i].Tail, Color.Black, 0.01f);
            }

            List<Line> tList = rrtConnect.myGraph.Select(a => new Line(new Position2D(a.Head), new Position2D(a.Tail) { DrawColor = Color.Black })).ToList();

            Visualizer.ClearStartWith("tree");
            for (int i = 0; i < tList.Count; i++)
            {
                Visualizer.DrawLine("tree" + i, tList[i].Head, tList[i].Tail, Color.OrangeRed, 0.01f);
            }



            //List<Pose2D> list = frontierDetection.GetDynamicFrontiers2(WorldModel.Instance.CurrentScan, WorldModel.Instance.EstimatedPose).Select(a => a.FrontierPosition).ToList();
            // Visualizer.DrawListPoint("f85", list, Color.Pink);

            // List<Frontier> fl = frontierDetection.GetDynamicFrontiers2(WorldModel.Instance.CurrentScan, WorldModel.Instance.EstimatedPose).ToList();
            //int i = 0;
            //Visualizer.ClearStartWith("aasc");
            //foreach (var item in fl)
            //{
            //    i++;
            //    var BeamPos1 = WorldModel.Instance.CurrentScan.GetBeamPos(WorldModel.Instance.EstimatedPose, item.StartRange);
            //    var BeamPos2 = WorldModel.Instance.CurrentScan.GetBeamPos(WorldModel.Instance.EstimatedPose, item.EndRange);

            //    Line Danger1 = new Line(new Position2D(Laser.GetLaserPosOnRobot(WorldModel.Instance.EstimatedPose)), new Position2D(BeamPos1), Color.OrangeRed, 0.02f);
            //    Line Danger2 = new Line(new Position2D(Laser.GetLaserPosOnRobot(WorldModel.Instance.EstimatedPose)), new Position2D(BeamPos2), Color.OrangeRed, 0.02f);
            //    Visualizer.DrawLine("aasc1cc" + i, Danger1);
            //    Visualizer.DrawLine("aasc2cc" + i, Danger2);
            //}



            CorrectPose(WorldModel.Instance.CurrentScan, WorldModel.Instance.EstimatedPose, P, WorldModel.Instance.SonarManager.GetSonarList());
            SlimControl1.invalid();
        }

        void LocalMap_geoReferenceMap_Updated(Bitmap map)
        {
            currBitmap = ReplaceColor(map, Color.FromArgb(255, 0, 0, 255), Color.FromArgb(0, 0, 0, 0));
            Visualizer.DrawMap(currBitmap);
            SlimControl1.invalid();
        }

        private void run()
        {
            while (!cts.Token.IsCancellationRequested)
            {
                if (BaseRobot.Instance.ThisRobot.Type != "AirRobot")
                {

                    label1.Text = "Battery : " + WorldModel.Instance.BatteryLife.ToString();
                    if (WorldModel.Instance.INSPose3D != null)
                    {
                        label6.Text = "X : " + WorldModel.Instance.INSPose3D.Position.X.ToString();
                        label7.Text = "Y : " + WorldModel.Instance.INSPose3D.Position.Y.ToString();
                        label8.Text = "Z : " + WorldModel.Instance.INSPose3D.Position.Z.ToString();
                    }
                    Visualizer.DrawLaser("laser1", WorldModel.Instance.CurrentScan, WorldModel.Instance.EstimatedPose, Color.GreenYellow);
                    Visualizer.DrawRobot("robot", WorldModel.Instance.EstimatedPose);
                    Visualizer.DrawPoint("centerRobot", WorldModel.Instance.EstimatedPose, Color.Green);
                    // Visualizer.DrawSonar("sonar", WorldModel.Instance.SonarManager, WorldModel.Instance.EstimatedPose, Color.DarkRed, Color.White, true);


                    CorrectPose(WorldModel.Instance.CurrentScan, WorldModel.Instance.EstimatedPose, new Pose2D(), WorldModel.Instance.SonarManager.GetSonarList());

                }
                else if (BaseRobot.Instance.ThisRobot.Type == "AirRobot")
                {
                    var XYZAcceleration = WorldModel.Instance.RawIMU.XYZAcceleration;
                    var AngularAccel = WorldModel.Instance.RawIMU.AngularAccel;
                    var AngularVel = WorldModel.Instance.RawIMU.AngularVel;
                    var Rotation = WorldModel.Instance.RawIMU.Rotation;

                    Visualizer.DrawText("IMU1", "XYZAcceleration : ( " + XYZAcceleration.X.ToString("0.##") + " , " + XYZAcceleration.Y.ToString("0.##") + " , " + XYZAcceleration.Z.ToString("0.##") + " )", new Pose2D(-1, -2.5, 0), Color.White);
                    Visualizer.DrawText("IMU2", "AngularAccel : ( " + AngularAccel.X.ToString("0.##") + " , " + AngularAccel.Y.ToString("0.##") + " , " + AngularAccel.Z.ToString("0.##") + " )", new Pose2D(-1, -3, 0), Color.Yellow);
                    Visualizer.DrawText("IMU3", "AngularVel : ( " + AngularVel.X.ToString("0.##") + " , " + AngularVel.Y.ToString("0.##") + " , " + AngularVel.Z.ToString("0.##") + " )", new Pose2D(-1, -3.5, 0), Color.White);
                    Visualizer.DrawText("IMU4", "Rotation : ( " + Rotation.X.ToString("0.##") + " , " + Rotation.Y.ToString("0.##") + " , " + Rotation.Z.ToString("0.##") + " )", new Pose2D(-1, -4, 0), Color.Yellow);
                }
                SlimControl1.invalid();

                Thread.Sleep(100);
            }
        }

        private Bitmap ReplaceColor(Bitmap bitmap, Color originalColor, Color replacementColor)
        {
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, y) == originalColor)
                    {
                        bitmap.SetPixel(x, y, replacementColor);
                    }
                }
            }
            return bitmap;
        }

        private void Viz_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Visualizer.ShowMap = checkBox1.Checked;
            SlimControl1.invalid();
        }



        private void CorrectPose(Laser laser, Pose2D currRobotPosition, Pose2D currRobotGoal, List<SonarSegment> sonarsList)
        {
            if (IsNeed2(laser, currRobotPosition, sonarsList))
            {

            }
        }

        public bool IsNeed2(Laser laser, Pose2D currRobotPosition, List<SonarSegment> sonarsList)
        {
            bool need = false;


            Visualizer.ClearStartWith("Danger");
            for (int i = 0; i < laser.fRanges.Length; i++)
            {
                double surveyBeam = laser.fRanges[i];
                double DangerThreashold = GetMaxSize_Rectangle(i);

                //draw Safe area
                var LaserPosition = Laser.GetLaserPosOnRobot(currRobotPosition);
                Line safeline = new Line(
                    new Position2D(LaserPosition),
                    new Position2D(LaserPosition) + Vector2D.FromAngleSize(currRobotPosition.Rotation + ((double)(90 - i)).ToRadian(), DangerThreashold),
                    Color.Green, 0.002f);
                Visualizer.DrawLine("Safe" + i, safeline);

                if (surveyBeam < DangerThreashold)
                {
                    //draw danger line
                    Pose2D BeamPos = laser.GetBeamPos(currRobotPosition, i);
                    Line Danger = new Line(new Position2D(Laser.GetLaserPosOnRobot(currRobotPosition)), new Position2D(BeamPos), Color.Red, 0.01f);
                    Visualizer.DrawLine("Danger" + i, Danger);
                    need = true;
                }
            }
            return need;
        }

        public bool IsNeed(Laser laser, Pose2D currRobotPosition, List<SonarSegment> sonarsList)
        {
            double RobotSize = 0.26d;
            double AvoidingThreashold = 0.05;
            double MaxDangerThreashold = 0.6;
            bool need = false;

            Visualizer.ClearStartWith("Danger");
            for (int i = 0; i < laser.fRanges.Length; i++)
            {
                double surveyBeam = laser.fRanges[i];

                //function
                double DangerThreashold = MaxDangerThreashold * (1d - ((double)Math.Abs(90 - i) / 90d));
                DangerThreashold += AvoidingThreashold + RobotSize;

                //draw Safe area
                Line safeline = new Line(new Position2D(Laser.GetLaserPosOnRobot(currRobotPosition)),
                    new Position2D(Laser.GetLaserPosOnRobot(currRobotPosition)) +
                    Vector2D.FromAngleSize(currRobotPosition.Rotation + ((double)(90 - i)).ToRadian(), DangerThreashold),
                    Color.Green, 0.002f);
                Visualizer.DrawLine("Safe" + i, safeline);

                if (surveyBeam < DangerThreashold)
                {
                    //draw danger line
                    Pose2D BeamPos = laser.GetBeamPos(currRobotPosition, i);
                    Line Danger = new Line(new Position2D(Laser.GetLaserPosOnRobot(currRobotPosition)), new Position2D(BeamPos), Color.Red, 0.01f);
                    Visualizer.DrawLine("Danger" + i, Danger);
                    need = true;
                }
            }
            return need;
        }

        public double GetMaxSize_Rectangle(int Angle)
        {
            double width = 0.6d;
            double height = 1d;
            double size = -1;

            Position2D Pleft = new Position2D(-width / 2, 0);
            Position2D Pright = new Position2D(width / 2, 0);
            Position2D Pleft_Up = Pleft + new Vector2D(0, height);
            Position2D Pright_Up = Pright + new Vector2D(0, height);
            Line LineRight = new Line(Pright, Pright_Up);
            Line LineLeft = new Line(Pleft, Pleft_Up);
            Line LineUp = new Line(Pleft_Up, Pright_Up);

            Line SampleLine = new Line(
                   new Position2D(0, 0),
                   new Position2D(0, 0) + Vector2D.FromAngleSize(((double)Angle).ToRadian(), 10), Color.Blue, 0.001f);

            Position2D? intersect = LineRight.IntersectWithLine(SampleLine);
            if (intersect == null) intersect = LineLeft.IntersectWithLine(SampleLine);
            if (intersect == null) intersect = LineUp.IntersectWithLine(SampleLine);

            if (intersect != null)
                size = Math.Sqrt((intersect.Value.X) * (intersect.Value.X) + (intersect.Value.Y) * (intersect.Value.Y));

            return size;
        }





        private void RecordBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Frames.Instance.SetFrame(WorldModel.Instance.EstimatedPose, WorldModel.Instance.SonarManager,
                    WorldModel.Instance.CurrentScan, ProjectCommons.config.sonarInfo,
                    currBitmap);

                Frames.Instance.Save(@"c:\log.xml");
                MessageBox.Show("Save Successfuls");
            }
            catch (Exception ex) { MessageBox.Show("Exeption-> Visualizer -> SaveFile : " + ex.ToString()); }
        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Frames.Instance.Load(@"c:\log.xml");
                ProjectCommons.config = new USARConfig() { sonarInfo = Frames.Instance.frame.sonarinfo };

                WorldModel.Instance.CurrentScan = Frames.Instance.frame.laser;
                WorldModel.Instance.SonarManager = Frames.Instance.frame.sonar;
                WorldModel.Instance.EstimatedPose = Frames.Instance.frame.currRobotPosition;
                Visualizer.DrawLaser("laser1", WorldModel.Instance.CurrentScan, WorldModel.Instance.EstimatedPose, Color.GreenYellow);
                Visualizer.DrawRobot("robot", WorldModel.Instance.EstimatedPose);
                Visualizer.DrawPoint("centerRobot", WorldModel.Instance.EstimatedPose, Color.Green);
                //   Visualizer.DrawSonar("sonar", WorldModel.Instance.SonarManager, WorldModel.Instance.EstimatedPose, Color.DarkRed, Color.White, true);
            }
            catch { MessageBox.Show("Exeption-> Visualizer -> OpenFile"); }
        }
    }
}
