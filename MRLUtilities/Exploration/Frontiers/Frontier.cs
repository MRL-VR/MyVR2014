using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MRL.Utils;
using System.IO;
using MRL.CustomMath;
using MRL.Communication.External_Commands;

namespace MRL.Exploration.Frontiers
{
    public enum FrontierType
    {
        BY_INTERVAL,
        BY_START_END_POSITION
    }

    public class Frontier
    {

        private const double w0 = 0.2;
        private const double w1 = 1.5;
        private const double w2 = 0.5;
        private const double w3 = 1;

        public Pose2D FrontierPosition;
        public double Distance { set; get; }
        public double Weight { set; get; }
        public double Area { set; get; }
        public int StartRange { set; get; }
        public int EndRange { set; get; }
        public int RobotIndex { set; get; }
        public bool Visited { set; get; }



        public Frontier(int s, int e, Laser laser, double interval, double distance, Pose2D robotPos,FrontierType frontierType)
        {
            StartRange = s;
            EndRange = e;

            for (int i = s; i <= e; i++)
            {
                if (laser.fRanges[i] > interval)
                    Area = Area + (laser.fRanges[i] - interval);
            }

            Distance = distance;

            var d = ((double)CenterRange * Math.PI / 180d);
            var angle = 90 * Math.PI / 180d;

            var ds = ((double)s * Math.PI / 180d);
            var de = ((double)e * Math.PI / 180d);


            switch (frontierType)
            {
                case FrontierType.BY_INTERVAL:
                    FrontierPosition = Vector2.FromAngleSize(-d + robotPos.Rotation + angle, interval) + robotPos;
                    break;
                case FrontierType.BY_START_END_POSITION:
                    Pose2D pos1 = laser.GetBeamPos(robotPos, s);
                    Pose2D pos2 = laser.GetBeamPos(robotPos, e);
                    FrontierPosition = new Pose2D((pos1.Position.X + pos2.Position.X) / 2.0f, (pos1.Position.Y + pos2.Position.Y) / 2.0f,0);
                    break;
            }
            
            Weight = w0 + (w1 * Area) + (w2 * Distance) + (w3 * Widthness);
        }


        public int CenterRange
        {
            get
            {
                return (int)((StartRange + EndRange) / 2.0f);
            }
        }

        public double Widthness
        {
            get
            {
                return EndRange - StartRange;
            }
        }

        public Frontier()
        {
            FrontierPosition = new Pose2D();
        }
    }
}
