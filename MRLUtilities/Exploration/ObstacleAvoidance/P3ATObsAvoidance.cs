using System;
using System.Collections.Generic;
using System.Linq;
using MRL.Commons;
using MRL.Communication.External_Commands;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Exploration.ObstacleAvoidance
{
    //public class P3ATObsAvoidance : ObstacleAvoidance
    //{
    //    #region Enums

    //    public enum ObsStatus
    //    {
    //        NONE = 0,
    //        OBS_MODE
    //    }

    //    #endregion

    //    #region Variables

    //    private const double maxDistance = 5.0f;
    //    private const double safetyDistance = 0.3f;
    //    private const double obsRadius = 0.16f;
    //    private const double minThreshold = 0.5f; // must be check 
    //    private const double maxThreshold = 0.4f;
    //    private Pose2D robotcenter = new Pose2D(0, 0, 0);
    //    private const double sameSideDirection = 0.0f;
    //    private const double minDifAngle = 0.17f;
    //    // private double dn = 0.6f;

    //    //StreamWriter sw = new StreamWriter("D:\\ObsAvoidance.log");

    //    #endregion

    //    #region Public Methods



        //public override ObstacleResult CorrectPose(Laser laser, Pose2D currRobotPosition, Pose2D currRobotGoal, List<SonarSegment> sonarsList)
        //{
        //    try
        //    {
        //        currPosition = currRobotPosition;
        //        goalPosition = currRobotGoal;
        //        this.sonarlist = sonarsList;
        //        ObstacleResult or = new ObstacleResult() { Status = ObstacleStatus.CLEAR };

        //        if (currPosition == null || goalPosition == null || sonarlist == null)
        //            return or;

        //        if (sonarlist.Count < 1)
        //            return or;

        //        if (!checkSonarsValidation())
        //            return or;

        //        //double dis = Math.Sqrt((Math.Pow(currPosition.X - robotGoal.X, 2) + Math.Pow(currPosition.Y - robotGoal.Y, 2)));
        //        Pose2D correctPos = null;

        //        ObsStatus obsStatus = DecisionMakingForObsAvoidance();

        //        if (obsStatus == ObsStatus.OBS_MODE)
        //        {
        //            correctPos = ObsAvoidanceMode(currPosition, goalPosition);
        //        }
        //        //else if (dis < nearToObstacle)
        //        //{
        //        //    correctPos = obs.ObsAvoidanceMode(currRobotPose, currGoal);
        //        //}


        //        if (correctPos != null)
        //        {
        //            or.CorrectedPose = correctPos;
        //            or.Status = ObstacleStatus.OBSTACLE;
        //        }
        //        else
        //        {
        //            or.CorrectedPose = null;
        //            or.Status = ObstacleStatus.CLEAR;
        //        }

        //        return or;
        //    }
        //    catch (Exception e)
        //    {
        //        ProjectCommons.writeConsoleMessage("Exception in P3ATObsAvoidance->GetCorrectedPosition", ConsoleMessageType.Error);
        //        USARLog.println("Exception in GetCorrectedPosition -> " + e, "P3ATObsAvoidance");
        //        return null;
        //    }
        //}

        //#endregion

    //    #region Private Methods

    //    private bool checkSonarsValidation()
    //    {
    //        return sonarlist.Where(a => a.fRange <= (float)minThreshold).ToList().Count == 0 ? false : true;
    //    }

    //    private Pose2D ObsAvoidanceMode(Pose2D RobotPose, Pose2D GoalPose)
    //    {
    //        Pose2D newPose = new Pose2D();
    //        double dsi = 0.0;
    //        double thetaSi = 0.0;
    //        string nameSi;
    //        int sonarCount = sonarlist.Count;
    //        int mid = (int)sonarCount / 2;
    //        int start, i;
    //        //string cmd = "";
    //        bool flag = false;
    //        Pose2D sonarPose = new Pose2D();
    //        int outSetDirectionTurn;


    //        outSetDirectionTurn = SetDirectionTurn();

    //        try
    //        {
    //            if (outSetDirectionTurn == 1)
    //            {
    //                //cmd += "obstacle is Left" + "  ";
    //                start = mid - 1;
    //                for (i = start; i >= 0; i--)
    //                {
    //                    if (JudgeSensorReading(i))
    //                    {
    //                        dsi = sonarlist[i].fRange;
    //                        //thetaSi = ProjectCommons.config.sonarInfo[i].Direction.Rotation;
    //                        thetaSi = ProjectCommons.config.sonarInfo[i].Direction.GetNormalizedRotation();
    //                        //cmd += "ThetaSi" + i + "    " + thetaSi + Environment.NewLine;
    //                        nameSi = sonarlist[i].sName;
    //                        sonarPose.X = ProjectCommons.config.sonarInfo[i].Position.X;
    //                        sonarPose.Y = ProjectCommons.config.sonarInfo[i].Position.Y;
    //                        flag = true;
    //                        break;
    //                    }

    //                }
    //            }
    //            else if (outSetDirectionTurn == 0)
    //            {
    //                //cmd += "obstacle is Right" + "  ";
    //                start = mid;
    //                for (i = start; i < sonarCount; i++)
    //                {
    //                    if (JudgeSensorReading(i))
    //                    {
    //                        dsi = sonarlist[i].fRange;
    //                        thetaSi = ProjectCommons.config.sonarInfo[i].Direction.Rotation;
    //                        thetaSi = ProjectCommons.config.sonarInfo[i].Direction.GetNormalizedRotation();
    //                        //cmd += "ThetaSi" + i + "    " + thetaSi + Environment.NewLine;
    //                        //thetaSi = sonarlist[i].dir;
    //                        nameSi = sonarlist[i].sName;
    //                        sonarPose.X = ProjectCommons.config.sonarInfo[i].Position.X;
    //                        sonarPose.Y = ProjectCommons.config.sonarInfo[i].Position.Y;
    //                        flag = true;
    //                        break;
    //                    }
    //                }

    //            }
    //            else
    //                return null;
    //        }
    //        catch (Exception e)
    //        {
    //            ProjectCommons.writeConsoleMessage("Error of ObsAvoidanceMode" + e.ToString(), ConsoleMessageType.Error);
    //        }

    //        if (flag)
    //        {
    //            double thetaSafe = obsRadius / (dsi - safetyDistance);
    //            double thetaG = Math.Atan(thetaSafe);
    //            //cmd += "thetaSafe" + thetaG + Environment.NewLine;
    //            double thetaCorrection = currPosition.GetNormalizedRotation() + GetNormalizedRotation((thetaG - thetaSi));
    //            double dn = Math.Sqrt((Math.Pow(robotcenter.X - sonarPose.X, 2) + Math.Pow(robotcenter.Y - sonarPose.Y, 2)));
    //            dn = dn + safetyDistance;
    //            newPose.X = currPosition.X + dn * (Math.Cos(thetaCorrection));
    //            newPose.Y = currPosition.Y + dn * (Math.Sin(thetaCorrection));
    //            newPose.Rotation = thetaG;//Must be check

    //            //cmd += "CurrPosDir=" + currPosition.GetNormalizedRotation() + "CurrPosX= " + currPosition.X + " CurrPosY = " + currPosition.Y;
    //            //cmd += Environment.NewLine + "CorrectDir=" + thetaCorrection + " CorrectX = " + newPose.X + " CorrectY = " + newPose.Y + Environment.NewLine;

    //            //cmd += Environment.NewLine;
    //            //sw.WriteLine(cmd);
    //            //sw.Flush();

    //            flag = false;
    //        }
    //        else
    //            newPose = null;

    //        return newPose;
    //    }

    //    private int SetDirectionTurn()
    //    {
    //        int sonarCount = sonarlist.Count;
    //        List<double> dmi = new List<double>();
    //        int flag;
    //        int mid = (int)sonarCount / 2;

    //        double sumDmiLeft = 0.0f;
    //        double sumDmiRight = 0.0f;

    //        for (int i = 0; i < sonarCount; i++)
    //            dmi.Add(maxDistance - sonarlist[i].fRange);

    //        for (int i = 0; i < mid; i++)
    //            sumDmiLeft += dmi[i];

    //        for (int j = mid; j < sonarCount; j++)
    //            sumDmiRight += dmi[j];

    //        if (sumDmiLeft < sumDmiRight)
    //            flag = 0; // Flag 0 means that obstacle is right
    //        else if (sumDmiLeft > sumDmiRight)
    //            flag = 1; // flag 1 means that obstacle is left
    //        else
    //            flag = 2;

    //        return flag;
    //    }

    //    private bool JudgeSensorReading(int IndexStart)
    //    {
    //        //if (sonarlist[IndexStart].fRange) >= minThreshold && sonarlist[IndexStart].fRange <= maxThreshold)
    //        if ((maxDistance - sonarlist[IndexStart].fRange) > minThreshold)
    //            return true; // This Sonar Reading is acceptable
    //        else
    //            return false; // This sonar Reading is not acceptable
    //    }

    //    private double DirectionSide()
    //    {
    //        double dif = (currPosition.Y - goalPosition.Y) / (currPosition.X - goalPosition.X);
    //        double theta = Math.Atan(dif);
    //        double direcSide = currPosition.GetNormalizedRotation() - GetNormalizedRotation(theta);
    //        //direcSide = GetNormalizedRotation(direcSide);

    //        return direcSide;
    //    }

    //    private ObsStatus DecisionMakingForObsAvoidance()
    //    {
    //        ObsStatus status = ObsStatus.NONE;
    //        int result = SetDirectionTurn();
    //        switch (result)
    //        {
    //            case 0:
    //                if (DirectionSide() <= sameSideDirection)
    //                    status = ObsStatus.NONE;
    //                else
    //                    status = ObsStatus.OBS_MODE;
    //                break;
    //            case 1:
    //                if (DirectionSide() < sameSideDirection)
    //                    status = ObsStatus.OBS_MODE;
    //                else
    //                    status = ObsStatus.NONE; ;
    //                break;
    //        }
    //        return status;
    //    }

    //    //private ObsStatus DecisionMakingForObsAvoidance()
    //    //{
    //    //    ObsStatus status = ObsStatus.NONE;
    //    //    double theta=currPosition.GetNormalizedRotation();
    //    //    double theta0=DirectionSide();

    //    //  //  cmd += "RobotDirection" + theta + "DirectionSide" + theta0 + Environment.NewLine;

    //    //    if (((theta > 0 && theta0 > 0) || (theta < 0 && theta0 < 0)) && (Math.Abs(theta - theta0) < minDifAngle))
    //    //        status = ObsStatus.NONE;
    //    //    else
    //    //        status = ObsStatus.OBS_MODE;
    //    //    return status;
    //    //}

    //    private double GetNormalizedRotation(double rotation)
    //    {
    //        double radians = rotation;

    //        while (radians > Math.PI)
    //            radians -= 2 * Math.PI;

    //        while (radians <= -Math.PI)
    //            radians += 2 * Math.PI;

    //        return radians;
    //    }

    //    #endregion

    //}
}
