using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Communication.Internal_Objects;
using MRL.CustomMath;
using MRL.Communication.External_Commands;
using System.Drawing;
using MRL.Utils;
using MRL.Mapping;
using MRL.Commons;
using MRL.Utils.GMath;

namespace VisualizerLibrary
{
    public static class Visualizer
    {
        //------------------------------------------Base--------------------------------------
        public static void DrawPoint(string key, Pose2D pos, Color color)
        {
            DrawingObjects.AddObject(key, new Position2D(pos, color));
        }

        public static void DrawLine(string key, Pose2D head, Pose2D tail, Color color, float strock)
        {
            DrawingObjects.AddObject(key, new Line(new Position2D(head), new Position2D(tail), color, strock));
        }

        public static void DrawLine(string key, Line line)
        {
            DrawingObjects.AddObject(key, new Line(line.Head, line.Tail, line.DrawColor, line.strock));
        }

        //------------------------------------------List<Base>-----------------------------------

        public static void DrawListPoint(string key, List<Pose2D> poses, Color color)
        {
            DrawingObjects.AddObject(key, poses.Select(x => new Position2D(x, color)).ToList());
        }

        //------------------------------------------Other----------------------------------------

        public static void DrawText(string key, string Content, Pose2D pos, Color color)
        {
            StringDraw sd = new StringDraw(Content, color, new Position2D(pos));
            DrawingObjects.AddObject(key, sd);
        }

        public static void DrawMap(Bitmap Map)
        {
            DrawingObjects.AddObject("map", Map);
        }

        public static bool ShowMap { set { DrawingObjects.ShowMap = value; } }

        //------------------------------------------Robot----------------------------------------

        public static void DrawRobot(string key, Pose2D position)
        {
            RobotModel robot = new RobotModel(false, false, false);
            robot.position = new Position2D(position);
            DrawingObjects.AddObject(key, robot);
        }

        public static void DrawSonar(string key, SonarManager sonarmanager, Pose2D rPos, Color SonarColor, Color TextColor, bool IsShowLable)
        {
            var sonars = sonarmanager.GetSonarList();
            double rotation = rPos.GetNormalizedRotation();
            List<Line> lines = new List<Line>();
            List<StringDraw> Texts = new List<StringDraw>();

            foreach (var item in sonars)
            {
                SonarInfo si = ProjectCommons.config.sonarInfo.FirstOrDefault(a => a.Name.Equals(item.sName));

                Pose2D SensorPoseOnRobot = si.Position;
                Pose2D SenSorPoseReal = new Pose2D(0, 0, 0);
                SenSorPoseReal.X = rPos.X + SensorPoseOnRobot.X * Math.Cos(rotation) + SensorPoseOnRobot.Y * (-Math.Sin(rotation));
                SenSorPoseReal.Y = rPos.Y + SensorPoseOnRobot.X * Math.Sin(rotation) + SensorPoseOnRobot.Y * (Math.Cos(rotation));

                Pose2D sB = new Pose2D(0, 0, 0);
                sB.X = item.fRange * Math.Cos(si.Direction.Rotation);
                sB.Y = item.fRange * Math.Sin(si.Direction.Rotation);

                sB.X = sB.X + SensorPoseOnRobot.X;
                sB.Y = sB.Y + SensorPoseOnRobot.Y;
                Pose2D finalpos = new Pose2D(0, 0, 0);
                finalpos.X = rPos.X + sB.X * Math.Cos(rotation) + sB.Y * (-Math.Sin(rotation));
                finalpos.Y = rPos.Y + sB.X * Math.Sin(rotation) + sB.Y * (Math.Cos(rotation));

                Line L = new Line(new Position2D(finalpos), new Position2D(SenSorPoseReal));
                L.strock = 0.07f;
                lines.Add(L);
                if (IsShowLable)
                    Texts.Add(new StringDraw(item.fRange.ToString(), TextColor, new Position2D(finalpos)));
            }

            SonarModel SM = new SonarModel(SonarColor, TextColor, lines, Texts);
            DrawingObjects.AddObject(key, SM);
        }

        public static void DrawLaser(string key, Laser currLaser, Pose2D RobotPosition, Color color)
        {
            List<Pose2D> poses = getLaserPosition(currLaser, RobotPosition);
            List<Position2D> lp = poses.Select(x => new Position2D(x, color)).ToList();
            LaserModel LM = new LaserModel(lp, new Position2D(Laser.GetLaserPosOnRobot(RobotPosition)), color);
            DrawingObjects.AddObject(key, LM);
        }

        //------------------------------------------Utiliti----------------------------------------

        public static void ClearStartWith(string word)
        {
            var keys = DrawingObjects.drawingObject.Where(x => x.Key.StartsWith(word)).Select(x => x.Key).ToList();
            foreach (var item in keys)
                DrawingObjects.drawingObject.Remove(item);
        }

        private static List<Pose2D> getLaserPosition(Laser laser, Pose2D robotPos)
        {
            double rotation = robotPos.GetNormalizedRotation();
            List<Pose2D> LaserPositions = new List<Pose2D>();
            int count = laser.fRanges.Length;

            float bAngle = 0;

            for (int i = 0; i < count; i++)
            {
                Pose2D laserBeamPose = new Pose2D(0, 0, 0);
                Pose2D rawPose = new Pose2D(0, 0, 0);
                Pose2D centerPose = new Pose2D(0, 0, 0);

                bAngle = -0.0175f * (i - 90);

                rawPose.X = laser.fRanges[i] * Math.Cos(bAngle);
                rawPose.Y = laser.fRanges[i] * Math.Sin(bAngle);

                centerPose.X = rawPose.X + Laser.laserPosOnRobot.X;
                centerPose.Y = rawPose.Y + Laser.laserPosOnRobot.Y;

                laserBeamPose.X = robotPos.X + centerPose.X * Math.Cos(rotation) + centerPose.Y * (-Math.Sin(rotation));
                laserBeamPose.Y = robotPos.Y + centerPose.X * Math.Sin(rotation) + centerPose.Y * (Math.Cos(rotation));
                LaserPositions.Add(laserBeamPose);
            }

            return LaserPositions;
        }



    }
}
