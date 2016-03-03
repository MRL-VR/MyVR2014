using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Exploration.PathPlannig;
using MRL.Communication.External_Commands;
using MRL.CustomMath;
using MRL.Utils.GMath;
using System.Drawing;

namespace MRL.Exploration.ObstacleAvoidance
{
    public class MotionOriented : ObstacleAvoidance
    {
        private RRTConnect motionPlanning = new RRTConnect();
        public override ObstacleResult CorrectPose(Communication.External_Commands.Laser laser, CustomMath.Pose2D currRobotPosition, CustomMath.Pose2D currRobotGoal, List<Communication.External_Commands.SonarSegment> sonarsList)
        {
            ObstacleResult or = new ObstacleResult();

            or.Status = Utils.ObstacleStatus.CLEAR;

            if (IsNeed2(laser, currRobotPosition, sonarlist))
            {
                List<Pose2D> path = motionPlanning.FindPathRRTConnect(currRobotPosition, currRobotGoal, laser, 500);

                if (path != null)
                {
                    if (path.Count > 0)
                    {
                        or.Alorithm = Utils.ObstacleAlgorthm.MOTION;
                        or.CorrectedPath = path;
                        or.Status = Utils.ObstacleStatus.OBSTACLE;
                    }
                    else
                    {
                        or.Status = Utils.ObstacleStatus.FAILD;                    
                    }
                }
                else
                {
                    or.Status = Utils.ObstacleStatus.FAILD;
                }
            }

            return or;
        }

        private bool IsNeed2(Laser laser, Pose2D currRobotPosition, List<SonarSegment> sonarsList)
        {
            for (int i = 0; i < laser.fRanges.Length; i++)
            {
                double surveyBeam = laser.fRanges[i];
                double DangerThreashold = GetMaxSize_Rectangle(i);
                if (surveyBeam < DangerThreashold)
                    return true;
            }
            return false;
        }


        private double GetMaxSize_Rectangle(int Angle)
        {
            double width = 0.6d;
            double height = 1d;
            double size = -1;

            Position2D Pleft = new Position2D(-width / 2, 0);
            Position2D Pright = new Position2D(width / 2, 0);
            Position2D Pleft_Up = Pleft + new Vector2D(0, height);
            Position2D Pright_Up = Pright + new Vector2D(0, height);
            MRL.Utils.GMath.Line LineRight = new MRL.Utils.GMath.Line(Pright, Pright_Up);
            MRL.Utils.GMath.Line LineLeft = new MRL.Utils.GMath.Line(Pleft, Pleft_Up);
            MRL.Utils.GMath.Line LineUp = new MRL.Utils.GMath.Line(Pleft_Up, Pright_Up);

            MRL.Utils.GMath.Line SampleLine = new MRL.Utils.GMath.Line(
                   new Position2D(0, 0),
                   new Position2D(0, 0) + Vector2D.FromAngleSize(((double)Angle).ToRadian(), 10), Color.Blue, 0.001f);

            Position2D? intersect = LineRight.IntersectWithLine(SampleLine);
            if (intersect == null) intersect = LineLeft.IntersectWithLine(SampleLine);
            if (intersect == null) intersect = LineUp.IntersectWithLine(SampleLine);

            if (intersect != null)
                size = Math.Sqrt((intersect.Value.X) * (intersect.Value.X) + (intersect.Value.Y) * (intersect.Value.Y));

            return size;
        }
    }
}
