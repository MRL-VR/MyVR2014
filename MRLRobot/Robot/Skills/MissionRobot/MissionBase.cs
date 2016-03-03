using System.Collections.Generic;
using System.Threading;
using MRL.Communication.External_Commands;
using MRL.Communication.Internal_Objects;
using MRL.CustomMath;
using MRL.Exploration.ObstacleAvoidance;
using MRL.Utils;
using MRL.Controller;
using System.Threading.Tasks;
using MRL.Commons;
using System;
using MRL.Command.Drive;
using VisualizerLibrary;
using System.Drawing;

namespace MRL.Robot.Skills.MissionRobot
{
    public abstract class MissionBase : Skill
    {
        private MissionState missionState;
        private MissionState obstacleMisionState;
        private Mission MissionData;
        private CancellationTokenSource ctsExit = new CancellationTokenSource();
        private Dictionary<string, SonarSegment> sonarsList;
        private ObstacleAvoidance obsAvoidance;
        private ObstacleResult obstacleResult = null;
        private Mission missionData;


        private double THRESHOLD_NEAR_TO_GOAL = 0.2f;

        private Control speedControl = new Control();
        //private WheeledControl speedControl = new WheeledControl(7, -7, 5, ControlMode.ROT_F, false);

        private int id = 0;
        private int obsID = 0;
        private double arivingTime = 0;
        private Pose2D selectedPoint;
        private Pose2D selectedObstaclePoint;
        private double rightSpeed;
        private double leftSpeed;
        private double rotation;
        private Task task = null;

        protected AbstractDrive driveController;
        protected double DEVIDE_SPEED;

        public MissionBase()
        {
            sonarsList = new Dictionary<string, SonarSegment>();
            obsAvoidance = new MotionOriented();
            SkillType = Skills.SkillType.MISSION;
        }

        public void Start(Mission mission)
        {
            missionData = mission;
            Start();
        }

        public void SetMission(Mission mission)
        {
            missionData = mission;
        }

        public override void Start()
        {
            try
            {
                if (missionData == null) return;
                if (missionData.pList.Count < 2) return;


                Pose2D currRobotPose = new Pose2D(WorldModel.Instance.EstimatedPose);

                if (ctsExit != null)
                    ctsExit.Cancel();

                if (missionData.pList.Count < 0) return;

                this.MissionData = missionData;

                rotation = currRobotPose.GetNormalizedRotation();

                id = 1;
                missionState = MissionState.SELECT;

                ctsExit = new CancellationTokenSource();
                task = new Task(() =>
                {
                    try
                    {
                        while (!ctsExit.IsCancellationRequested)
                        {
                            switch (missionState)
                            {
                                case MissionState.SELECT:
                                    {
                                        if (MissionData.pList.Count <= id)
                                        {
                                            ctsExit.Cancel();
                                        }
                                        else
                                        {
                                            selectedPoint = MissionData.pList[id++];
                                            missionState = MissionState.GOTO;
                                        }
                                    }
                                    break;
                                case MissionState.GOTO:
                                    {
                                        gotoXY(selectedPoint.X, selectedPoint.Y);
                                    }
                                    break;
                                case MissionState.OBS_AVOIDANCE:
                                    {
                                        if (obstacleResult.Alorithm == ObstacleAlgorthm.MOTION)
                                            gotoObsPath();
                                        else
                                            gotoObsPos();
                                    }
                                    break;
                                default:
                                    break;
                            }
                            Thread.Sleep(10);
                            arivingTime += 10;
                        }
                        Stop();
                    }
                    catch (Exception ex)
                    {
                        ProjectCommons.writeConsoleMessage("Exception in MissionCtrl->StartTask() >> " + ex.ToString(), ConsoleMessageType.Error);
                        USARLog.println("Exception in StartTask()", "MissionCtrl");
                    }
                }, ctsExit.Token);
                task.Start();
            }
            catch
            {
                ProjectCommons.writeConsoleMessage("Exception in MissionCtrl->Start()", ConsoleMessageType.Error);
                USARLog.println("Exception in Start()", "MissionCtrl");
            }
        }

        public override void Stop()
        {
            if (Reports != null)
                Reports(SkillStatus.STOP);

            ctsExit.Cancel();
            stopRobot();
            missionData = null;
        }

        public override void Resume()
        {
            throw new System.NotImplementedException();
        }

        public override void Pause()
        {
            throw new System.NotImplementedException();
        }

        private bool reach(double x, double y, ObstacleStatus obsStatus)
        {
            double delta = Math.Sqrt(Math.Pow(WorldModel.Instance.EstimatedPose.X - x, 2) + Math.Pow(WorldModel.Instance.EstimatedPose.Y - y, 2));

            switch (obsStatus)
            {
                case ObstacleStatus.CLEAR:
                case ObstacleStatus.FAILD:
                case ObstacleStatus.OBSTACLE:
                    if (THRESHOLD_NEAR_TO_GOAL >= delta)
                        return true;
                    break;
                //case ObstacleStatus.TRY:
                //    if (MAX_THRESHOLD_NEAR_TO_GOAL >= delta)
                //        return true;
                //    break;
            }

            return false;
        }

        private void gotoObsPos()
        {
            try
            {
                Pose2D currRobotPose = new Pose2D(WorldModel.Instance.EstimatedPose);
                rotation = currRobotPose.GetNormalizedRotation();
                //List<SonarSegment> tmpList = sonarManager.GetSonarList();

                if (obstacleResult.CorrectedPose == null)
                {
                    missionState = MissionState.GOTO;
                    return;
                }

                Vector3 speed = speedControl.CheckSpeed(new Vector2(currRobotPose.X, currRobotPose.Y), new Vector2(obstacleResult.CorrectedPose.X, obstacleResult.CorrectedPose.Y), rotation);

                leftSpeed = speed.X / DEVIDE_SPEED;
                rightSpeed = speed.Y / DEVIDE_SPEED;

                if (reach(obstacleResult.CorrectedPose.X, obstacleResult.CorrectedPose.Y, obstacleResult.Status))
                {
                    missionState = MissionState.GOTO;
                    obstacleResult = null;
                }
                else
                {
                    string utCmd = driveController.GetCommand("DRIVE {Left " + leftSpeed + "} {Right " + rightSpeed + "} {Normalized True}");
                    Actuators(utCmd);
                }
            }
            catch (Exception e)
            {
                ProjectCommons.writeConsoleMessage("P3ATMission.gotoObsPos() >> " + e.ToString(), ConsoleMessageType.Error);
            }
        }

        private void gotoObsPath()
        {
            try
            {
                Pose2D currRobotPose = new Pose2D(WorldModel.Instance.EstimatedPose);
                rotation = currRobotPose.GetNormalizedRotation();

                switch (obstacleMisionState)
                {

                    case MissionState.SELECT:
                        {
                            if (obsID > obstacleResult.CorrectedPath.Count - 1)
                            {
                                missionState = MissionState.GOTO;
                            }
                            else
                            {
                                selectedObstaclePoint = obstacleResult.CorrectedPath[obsID++];
                                obstacleMisionState = MissionState.GOTO;
                            }
                        }
                        break;
                    case MissionState.GOTO:
                        {
                            Vector3 speed = speedControl.CheckSpeed(new Vector2(currRobotPose.X, currRobotPose.Y), new Vector2(selectedObstaclePoint.X, selectedObstaclePoint.Y), rotation);

                            leftSpeed = speed.X / DEVIDE_SPEED;
                            rightSpeed = speed.Y / DEVIDE_SPEED;

                            if (reach(selectedObstaclePoint.X, selectedObstaclePoint.Y, obstacleResult.Status))
                            {
                                obstacleMisionState = MissionState.SELECT;
                            }
                            else
                            {
                                string utCmd = driveController.GetCommand("DRIVE {Left " + leftSpeed + "} {Right " + rightSpeed + "} {Normalized True}");
                                Actuators(utCmd);
                            }
                        }

                        break;
                }
            }

            catch (Exception e)
            {
                ProjectCommons.writeConsoleMessage("P3ATMission.gotoObsPos() >> " + e.ToString(), ConsoleMessageType.Error);
            }
        }


        private void gotoXY(double x, double y)
        {
            Pose2D currRobotPose = new Pose2D(WorldModel.Instance.EstimatedPose);
            rotation = currRobotPose.GetNormalizedRotation();
            //ProjectCommons.writeConsoleMessage("rot : " + rotation, ConsoleMessageType.Information);
            Pose2D goalPos = new Pose2D(x, y, 0);
            //string cmd = "";

            //This part will be work when we turn on ObstacleAvoidance from config of project
            if (ProjectCommons.config.OBSTACLE_AVOIDANCE_STATUS == 1)
            {
                List<SonarSegment> tmpList = WorldModel.Instance.SonarManager.GetSonarList();

                obstacleResult = obsAvoidance.CorrectPose(new Laser(WorldModel.Instance.CurrentScan), currRobotPose, goalPos, tmpList); // detects obtacle avoidance mode
                
                Laser l = new Laser(WorldModel.Instance.CurrentScan);
                switch (obstacleResult.Status)
                {
                    case ObstacleStatus.OBSTACLE:
                        missionState = MissionState.OBS_AVOIDANCE;
                        if (obstacleResult.Alorithm == ObstacleAlgorthm.MOTION)
                        {
                            Visualizer.DrawListPoint("ObstaclePath", obstacleResult.CorrectedPath, Color.Pink);
                            obstacleMisionState = MissionState.SELECT;
                            obsID = 1;
                        }
                        return;
                    case ObstacleStatus.FAILD:
                        Stop();
                        if (Reports != null)
                        {
                            Reports(SkillStatus.FAILED);
                        }
                        break;
                }

            }

            Vector3 speed = speedControl.CheckSpeed(new Vector2(currRobotPose.X, currRobotPose.Y), new Vector2(goalPos.X, goalPos.Y), rotation);
            leftSpeed = speed.X / DEVIDE_SPEED;
            rightSpeed = speed.Y / DEVIDE_SPEED;

            if (reach(x, y, ObstacleStatus.CLEAR))
            {
                missionState = MissionState.SELECT;
            }
            else
            {
                string utCmd = driveController.GetCommand("DRIVE {Left " + leftSpeed + "} {Right " + rightSpeed + "} {Normalized True}");
                //string utCmd = driveController.GetCommand("DRIVE {Left 3.0} {Right 3.0} {Normalized True}");
                Actuators(utCmd);

            }
        }

        private void stopRobot()
        {
            string utCmd = driveController.GetCommand("DRIVE {Left 0.0} {Right 0.0} {Normalized True}");
            Actuators(utCmd);
        }

    }
}
