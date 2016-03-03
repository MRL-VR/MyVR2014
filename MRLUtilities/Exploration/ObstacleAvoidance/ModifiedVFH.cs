using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.CustomMath;
using MRL.Utils;
using MRL.Commons;
using MRL.Communication.External_Commands;


namespace MRL.Exploration.ObstacleAvoidance
{
    public class ModifiedVFH : ObstacleAvoidance
    {
        private Laser laserData;
        private List<Gaps> listLaserMapping = new List<Gaps>();
        private static int obstacleCounter = 0;

        public override ObstacleResult CorrectPose(Laser laser, Pose2D currRobotPosition, Pose2D currRobotGoal, List<SonarSegment> sonarsList)
        {
            currPosition = currRobotPosition;
            goalPosition = currRobotGoal;
            this.sonarlist = sonarsList;
            laserData = laser;
            Pose2D correctedPos = new Pose2D();
            correctedPos = null;
            double obThreashold = detectObThreashold();
           
            for (int i = 0; i < 180; i++)
            {
                if (laserData.fRanges[i] < obThreashold)
                {
                    correctedPos = finalPosition();
                    obstacleCounter++;
                    break;
                }
            }
            ObstacleResult obsResult = new ObstacleResult() { Status = ObstacleStatus.CLEAR };
            if (obstacleCounter > 10)
            {
                obsResult.Status = ObstacleStatus.FAILD;
                obstacleCounter = 0;
            }
            else if (obstacleCounter > 5)
            {
                obsResult.Status = ObstacleStatus.TRY;
                obsResult.CorrectedPose = correctedPos;
            }
            else if (obstacleCounter > 0)
            {
                obsResult.Status = ObstacleStatus.OBSTACLE;
                obsResult.CorrectedPose = correctedPos;
            }
            else
            {
                obsResult.Status = ObstacleStatus.CLEAR;
                obsResult.CorrectedPose = correctedPos;
                obstacleCounter = 0;
            }

            if (obsResult.CorrectedPose == null)
            {
                obsResult.Status = ObstacleStatus.CLEAR;
                obsResult.CorrectedPose = correctedPos;
                obstacleCounter = 0;
            }
            return obsResult;
        }

        private float detectObThreashold()
        {
            int p = 0; float obThreashold = 0.0f;
            for (int i = 0; i < 180; i++)
                if (laserData.fRanges[i] < 1.5)
                    p++;
            if (p > 90)
                obThreashold = 1.0f;
            else
                obThreashold = 0.5f;
            return obThreashold;
        }

        private Pose2D finalPosition()
        {
            generateMap();
            int nearestGap2Goal = nearestGap();
            double disCorrectPose = detectdisCorrectPose();
            Pose2D finalPoint = new Pose2D(map(nearestGap2Goal, disCorrectPose));
            return finalPoint;
        }

        private void generateMap()
        {

            listLaserMapping.Clear();
            if (dicOfMinObs() == "right")
            {
                int p = 0;
                for (int i = 90; i < 180; i++)
                {
                    if (laserData.fRanges[i] >= 0.6)
                    {
                        p++;
                        if (p == 50)
                        {
                            listLaserMapping.Add(new Gaps(0, i - 25, distance2Goal(i - 25, 1)));
                            p = 0;
                        }
                    }
                    else
                        if (i + 50 > 180)
                            break;
                        else
                            p = 0;
                }
                p = 0;
                for (int i = 90; i >= 0; i--)
                {
                    if (laserData.fRanges[i] >= 1.5)
                    {
                        p++;
                        if (p == 75)
                        {
                            listLaserMapping.Add(new Gaps(0, i + 50, distance2Goal(i + 50, 1)));
                            p = 0;
                        }
                    }
                    else
                        if (i - 75 < 0)
                            break;
                        else
                            p = 0;
                }
                p = 0;
            }
            if (dicOfMinObs() == "left")
            {
                int q = 0;
                for (int i = 90; i < 180; i++)
                {
                    if (laserData.fRanges[i] >= 1.5)
                    {
                        q++;
                        if (q == 75)
                        {
                            listLaserMapping.Add(new Gaps(0, i - 50, distance2Goal(i - 50, 1)));
                            q = 0;
                        }
                    }
                    else
                        if (i + 75 > 180)
                            break;
                        else
                            q = 0;
                }
                q = 0;
                for (int i = 90; i >= 0; i--)
                {
                    if (laserData.fRanges[i] >= 0.6)
                    {
                        q++;
                        if (q == 50)
                        {
                            listLaserMapping.Add(new Gaps(0, i + 25, distance2Goal(i + 25, 1)));
                            q = 0;
                        }
                    }
                    else
                        if (i - 50 < 0)
                            break;
                        else
                            q = 0;
                }
                q = 0;
            }
        }

        private double distance2Goal(int i, double j)
        {
            Pose2D gapPoint = new Pose2D(map(i, j));
            double d2G = Math.Sqrt(Math.Pow(goalPosition.X - gapPoint.X, 2) + Math.Pow(goalPosition.Y - gapPoint.Y, 2));
            return d2G;
        }

        private int nearestGap()
        {
            double minFrangeLaser = double.MaxValue;
            int nearGap = 0;
            if (listLaserMapping.Count == 0)
                if (dicOfMinObs() == "right")
                    nearGap = 130;
                else
                    nearGap = 50;
            else
            {
                for (int i = 0; i < listLaserMapping.Count; i++)
                    if (listLaserMapping[i].DisFromCenter < minFrangeLaser)
                        minFrangeLaser = listLaserMapping[i].DisFromCenter;
                for (int i = 0; i < listLaserMapping.Count; i++)
                    if (listLaserMapping[i].DisFromCenter == minFrangeLaser)
                        nearGap = listLaserMapping[i].indexNod;
            }
            return nearGap;
        }

        private double detectdisCorrectPose()
        {
            double disR2G = Math.Sqrt(Math.Pow(currPosition.X - goalPosition.X, 2) + Math.Pow(currPosition.Y - goalPosition.Y, 2));
            if (disR2G < 1.2)
                return disR2G;
            else
                return 1.2;
        }

        private string dicOfMinObs()
        {
            double nearestObstacle = double.MaxValue;
            string obDirection = null;
            int obDic = 0;
            for (int i = 0; i < 180; i++)
                if (laserData.fRanges[i] < nearestObstacle)
                    nearestObstacle = laserData.fRanges[i];
            for (int i = 0; i < 180; i++)
                if (laserData.fRanges[i] == nearestObstacle)
                {
                    obDic = i;
                    break;
                }
            if (obDic < 90)
                obDirection = "right";
            else
                obDirection = "left";
            return obDirection;
        }

        private Pose2D map(int q, double d)
        {
            Pose2D localPo = new Pose2D();
            Pose2D globalPo = new Pose2D();
            double angle = 0.0175f * (90 - q);
            localPo.X = d * Math.Cos(angle);
            localPo.Y = d * Math.Sin(angle);
            globalPo = globalPoint(localPo.X, localPo.Y, angle);
            return globalPo;
        }

        private Pose2D globalPoint(double localX, double localY, double degree)
        {
            Pose2D globalPosition = new Pose2D();
            double distance = Math.Sqrt(Math.Pow(localX, 2) + Math.Pow(localY, 2));
            double globalAngle = currPosition.Rotation + degree;
            globalPosition.X = (distance * (Math.Cos(globalAngle))) + currPosition.X;
            globalPosition.Y = (distance * (Math.Sin(globalAngle))) + currPosition.Y;
            return globalPosition;
        }
    }
}
