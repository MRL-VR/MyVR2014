using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MRL.Communication.External_Commands;
using MRL.Communication.Internal_Objects;
using MRL.CustomMath;
using MRL.Utils;
using MRL.Commons;
using MRL.Mapping;
using MRL.Exploration.Frontiers;

namespace MRL.Exploration.Frontiers
{


    public class FrontierDetection
    {
        private const int maxLaserDistance = 20;
        private const double robotWidth = 0.52d;
        private Pose2D laserPosOnRobot = new Pose2D(0.2085, 0, -0.2);

        private Pose2D robotPos;

        public List<Frontier> GetStaticFrontiers2(Laser laser, Pose2D currPosition, int maxMeter, int minMeter)
        {
            List<Frontier> frontiers = new List<Frontier>();

            robotPos = currPosition;

            for (int i = maxMeter; i > minMeter; i--)
            {
                frontiers.AddRange(GetFrontiers(laser, i, FrontierType.BY_START_END_POSITION));
                if (frontiers.Count > 0)
                    break;
            }

            return frontiers;
        }

        public List<Frontier> GetStaticFrontiers(Laser laser, Pose2D currPosition, int minMeter, int maxMeter)
        {
            List<Frontier> frontiers = new List<Frontier>();

            robotPos = currPosition;

            for (int i = minMeter; i < maxMeter; i++)
            {
                frontiers.AddRange(GetFrontiers(laser, i, FrontierType.BY_START_END_POSITION));
                if (frontiers.Count > 0)
                    break;
            }

            return frontiers;
        }

        public List<Frontier> GetDynamicFrontiers(Laser laser, Pose2D currPosition)
        {
            List<Frontier> frontiers = new List<Frontier>();

            robotPos = currPosition;

            for (int i = 14; i > 0; i--)
            {
                frontiers.AddRange(GetFrontiers(laser, i, FrontierType.BY_START_END_POSITION));
                if (frontiers.Count > 0)
                    break;
            }

            return frontiers;
        }

        public List<Frontier> GetDynamicFrontiersSoroosh(Laser laser, Pose2D currPosition)
        {
            List<Frontier> frontiers = new List<Frontier>();

            // diffrence between beams
            int d = 1;
            // max check radious
            int rMax = 10;
            // UnreachedPlace
            int UnreachedPlace = 3;

            double LastValue = -1;
            List<int> ChangedBeam = new List<int>();
            // add first beam
            ChangedBeam.Add(0);
            for (int i = 0; i < laser.fRanges.Length; i++)
            {
                double CurrValue = laser.fRanges[i];
                CurrValue = (CurrValue > rMax) ? rMax : CurrValue;
                // at first : LastValue = CurrValue
                LastValue = LastValue == -1 ? CurrValue : LastValue;
                if (Math.Abs(LastValue - CurrValue) > d)
                {
                    ChangedBeam.Add(i);
                }
                LastValue = CurrValue;
            }
            // add last beam
            if (!ChangedBeam.Exists(x => x == laser.fRanges.Length - 1)) ChangedBeam.Add(laser.fRanges.Length - 1);

            // add to frontiers
            // ChangedBeam.Count - 1 => (-1) is for not checking last beam
            for (int i = 0; i < ChangedBeam.Count - 1; i++)
            {
                // is last frontier?
                bool isLast = i == ChangedBeam.Count - 2;
                // in normal :  ChangedBeam[i + 1] - 1
                // if last :  ChangedBeam[i + 1]
                int startRng = ChangedBeam[i];
                int endRng = isLast ? ChangedBeam[i + 1] : ChangedBeam[i + 1] - 1;

                // improvement
                if (Math.Abs(startRng - endRng) < 2) continue;
                // add if this packet have nay unreached beam
                for (int j = startRng; j <= endRng; j++)
                    if (laser.fRanges[j] > UnreachedPlace)
                    {
                        frontiers.Add(new Frontier()
                        {
                            StartRange = startRng,
                            EndRange = endRng
                        });
                        break;
                    }
            }

            return frontiers;
        }


        public List<Frontier> GetDynamicFrontiers2(Laser laser, Pose2D currPosition)
        {
            List<Frontier> frontiers = new List<Frontier>();

            robotPos = currPosition;

            for (double i = 8f; i > 1.5d; i-=0.5d)
            {
                frontiers.AddRange(GetFrontiers(laser, i, FrontierType.BY_START_END_POSITION));
                if (frontiers.Count > 0)
                    break;
            }

            return frontiers;
        }

        public Frontier CalcForObstacleAvoidance(Laser laser, Pose2D currPosition,Pose2D goalPosition)
        {
            List<Frontier> frontiers = new List<Frontier>();

            robotPos = currPosition;

            for (double i =3.0d; i > 0.5d; i -= 0.5d)
            {
                frontiers.AddRange(GetFrontiersForObstacle(laser, i, FrontierType.BY_START_END_POSITION));
                //if (frontiers.Count > 0)
                //    break;
            }
            if (frontiers.Count>0)
            {
                int selectedFrontier = 0;
                double minDistance = double.MaxValue;
                for (int i = 0; i < frontiers.Count; i++)
                {
                    double dist = goalPosition.DistanceFrom(frontiers[i].FrontierPosition);
                    if (dist < minDistance)
                    {
                        selectedFrontier = i;
                        minDistance = dist;
                    }
                }

                return frontiers[selectedFrontier];
            }

            return null;
        }


        public List<Frontier> GetFrontiers(Laser laser, double frontierIterval,FrontierType frontierType)
        {
            if (laser == null)
                return null;

            List<Frontier> fList = new List<Frontier>();
            bool inFrontier = false;
            int sFrontier = 0;
            for (int i = 0; i < laser.fRanges.Length; i++)
            {
                if (laser.fRanges[i] >= frontierIterval && !inFrontier)
                {
                    inFrontier = true;
                    sFrontier = i;
                }
                else if (((laser.fRanges.Length == i + 1) || (laser.fRanges[i] <= frontierIterval)) && inFrontier)
                {
                    sFrontier = sFrontier > 0 ? sFrontier - 1 : 0;

                    Pose2D pos1 = laser.GetBeamPos(new Pose2D(), sFrontier);
                    Pose2D pos2 = laser.GetBeamPos(new Pose2D(), i);
                    double distance = Math.Sin((double)(i - sFrontier) * Math.PI / 180d) * (double)Math.Min(laser.fRanges[i], laser.fRanges[sFrontier]);//pos1.DistanceFrom(pos2);
                    
                    if (distance > (robotWidth) && (Math.Abs(sFrontier - i) > 1))
                        fList.Add(new Frontier(sFrontier, i, laser, frontierIterval, distance, robotPos, frontierType));

                    inFrontier = false;
                }
            }

            return fList;
        }

        public List<Frontier> GetFrontiersForObstacle(Laser laser, double frontierIterval, FrontierType frontierType)
        {
            if (laser == null)
                return null;

            List<Frontier> fList = new List<Frontier>();
            bool inFrontier = false;
            int sFrontier = 0;
            for (int i = 0; i < laser.fRanges.Length; i++)
            {
                if (laser.fRanges[i] >= frontierIterval && !inFrontier)
                {
                    inFrontier = true;
                    sFrontier = i;
                }
                else if (((laser.fRanges.Length == i + 1) || (laser.fRanges[i] <= frontierIterval)) && inFrontier)
                {
                    sFrontier = sFrontier > 0 ? sFrontier - 1 : 0;

                    Pose2D pos1 = laser.GetBeamPos(new Pose2D(), sFrontier);
                    Pose2D pos2 = laser.GetBeamPos(new Pose2D(), i);
                    double distance = Math.Sin((double)(i - sFrontier) * Math.PI / 180d) * (double)Math.Min(laser.fRanges[i], laser.fRanges[sFrontier]);//pos1.DistanceFrom(pos2);

                    if (distance > (robotWidth / 2.0d) && (Math.Abs(sFrontier - i) > 1))
                        fList.Add(new Frontier(sFrontier, i, laser, frontierIterval, distance, robotPos, frontierType));

                    inFrontier = false;
                }
            }

            return fList;
        }
    }
}
