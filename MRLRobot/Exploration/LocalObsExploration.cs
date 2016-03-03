using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Exploration.Frontiers;
using System.Threading;
using MRL.Robot.Skills;
using MRL.Commons;
using MRL.Communication.Internal_Objects;
using MRL.CustomMath;
using System.Threading.Tasks;
using MRLRobot.Robot.Skills.RobotAction;
using MRL.Communication.External_Commands;
using MRL.Utils;
using MRL.Communication.Tools;
using MRL.IDE.Robot;
using System.Diagnostics;
using MRL.Robot.Skills.MissionRobot;
using MRL.Exploration.PathPlannig;

namespace MRLRobot.Exploration
{
    public class LocalObsExploration : BaseExploration
    {
        private LocalExpStatus localExpStatus = LocalExpStatus.IDLE;

        private Stopwatch timeoutHandler = new Stopwatch();
        private Stopwatch signalCheckHandler = new Stopwatch();
        private Stopwatch turnTimeoutHandler = new Stopwatch();
        private Stopwatch failedTimeoutHandler = new Stopwatch();
        private Stopwatch reachTimeoutHandler = new Stopwatch();
        private Stopwatch driftHandler = new Stopwatch();
        private Stopwatch driftTimeoutHandler = new Stopwatch();
        private Pose2D lastPosition;
        private int driftCount = 0;

        private const int MAX_TIMEOUT = 5000;
        private const int MAX_FAILED_TIMEOUT = 1000;
        private const int MAX_TURN_TIMEOUT = 10000;
        private const int MAX_REACH_TIMEOUT = 50000;
        private const int MAX_DRIFT_TIMEOUT = 5000;


        public LocalObsExploration()
        {
            frontierDetection = new FrontierDetection();
            randomizer = new Random(DateTime.Now.Millisecond);
            actions = SkillManager.Instance.ActionSkill as BaseAction;
            missions = SkillManager.Instance.MissionSkill as MissionBase;
            SkillManager.Instance.MissionSkill.Reports += new Action<SkillStatus>(MissionSkill_Reports);
            SkillManager.Instance.ActionSkill.Reports += new Action<SkillStatus>(ActionSkill_Reports);
            rrtConnectPathPlanning = new RRTConnect();
        }

        public override void Start()
        {
            localExpStatus = LocalExpStatus.SELECT_FRONTIER;
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
            cts = new CancellationTokenSource();
            myTask = new Task(() => run());
            myTask.Start();
        }


        public override void Stop()
        {
            if (cts != null)
                cts.Cancel();
            SkillManager.Instance.MissionSkill.Stop();

            localExpStatus = LocalExpStatus.IDLE;
            myTask = null;
        }

        private void run()
        {
            try
            {
                signalCheckHandler.Restart();
                while (!cts.Token.IsCancellationRequested)
                {

                    switch (localExpStatus)
                    {
                        case LocalExpStatus.SELECT_FRONTIER:
                            if (decisionMakerByWeight(WorldModel.Instance.CurrentScan))
                            {
                                reachTimeoutHandler.Restart();
                                driftCount = 0;
                                driftHandler.Restart();
                                localExpStatus = LocalExpStatus.REACH_TO_FRONTIER;
                            }
                            else
                            {
                                missions.Stop();
                                failedTimeoutHandler.Restart();
                                localExpStatus = LocalExpStatus.FAILED;
                            }
                            break;
                        case LocalExpStatus.SELECT_MAPPER:
                            if (decisionMakerByMapper(WorldModel.Instance.CurrentScan))
                            {
                                reachTimeoutHandler.Restart();
                                driftCount = 0;
                                driftHandler.Restart();
                                localExpStatus = LocalExpStatus.REACH_TO_MAPPER;
                            }
                            else
                            {
                                missions.Stop();

                                failedTimeoutHandler.Restart();
                                localExpStatus = LocalExpStatus.FAILED;
                            }
                            break;
                        case LocalExpStatus.REACH_TO_FRONTIER:
                            if (reachTimeoutHandler.ElapsedMilliseconds > MAX_REACH_TIMEOUT)
                            {
                                reachTimeoutHandler.Stop();
                                failedTimeoutHandler.Restart();
                                localExpStatus = LocalExpStatus.FAILED;
                            }

                            if (driftCheck())
                            {
                                reachTimeoutHandler.Stop();
                                localExpStatus = LocalExpStatus.DRIFT;
                            }
                            break;
                        case LocalExpStatus.REACH_TO_MAPPER:
                            if (reachTimeoutHandler.ElapsedMilliseconds > MAX_REACH_TIMEOUT)
                            {
                                reachTimeoutHandler.Stop();
                                failedTimeoutHandler.Restart();
                                localExpStatus = LocalExpStatus.FAILED;
                            }

                            if (driftCheck())
                            {
                                reachTimeoutHandler.Stop();
                                localExpStatus = LocalExpStatus.DRIFT;
                            }
                            break;
                        case LocalExpStatus.FAILED:
                            if (failedTimeoutHandler.ElapsedMilliseconds > MAX_FAILED_TIMEOUT)
                            {
                                failedTimeoutHandler.Stop();
                                turnTimeoutHandler.Restart();

                                missions.Stop();
                                actions.Stop();

                                ActionType side = sideDecision();

                                int sDegree = (int)randomizer.Next(91, 179);
                                switch (side)
                                {
                                    case ActionType.LEFT:
                                        actions.Left(sDegree);
                                        break;
                                    case ActionType.RIGHT:
                                        actions.Right(sDegree);
                                        break;
                                    case ActionType.BACKWARD:
                                        actions.Backward(1);
                                        break;
                                }
                                localExpStatus = LocalExpStatus.TURN_TIMEOUT;
                            }
                            break;
                        case LocalExpStatus.DRIFT:
                            {
                                missions.Stop();
                                actions.Stop();

                                ActionType side = driftDecision();

                                int sDegree = (int)randomizer.Next(45, 150);
                                switch (side)
                                {
                                    case ActionType.LEFT:
                                        actions.Left(sDegree);
                                        break;
                                    case ActionType.RIGHT:
                                        actions.Right(sDegree);
                                        break;
                                    case ActionType.BACKWARD:
                                        actions.Backward(1);
                                        break;
                                }
                                driftTimeoutHandler.Restart();
                                localExpStatus = LocalExpStatus.DRIFT_TIMEOUT;
                            }
                            break;
                        case LocalExpStatus.IDLE:
                            break;
                        case LocalExpStatus.TIMEOUT:
                            if (timeoutHandler.ElapsedMilliseconds > MAX_TIMEOUT)
                            {
                                timeoutHandler.Stop();
                                turnTimeoutHandler.Restart();

                                actions.Left(179);

                                localExpStatus = LocalExpStatus.TURN_TIMEOUT;
                            }
                            break;
                        case LocalExpStatus.TURN_TIMEOUT:
                            if (turnTimeoutHandler.ElapsedMilliseconds > MAX_TURN_TIMEOUT)
                            {
                                turnTimeoutHandler.Stop();
                                failedTimeoutHandler.Restart();
                                localExpStatus = LocalExpStatus.FAILED;
                            }
                            break;
                        case LocalExpStatus.DRIFT_TIMEOUT:
                            if (driftTimeoutHandler.ElapsedMilliseconds > MAX_DRIFT_TIMEOUT)
                            {
                                driftTimeoutHandler.Stop();
                                localExpStatus = LocalExpStatus.DRIFT;
                            }
                            break;
                    }

                    checkMySignal();

                    Thread.Sleep(10);
                    ProjectCommons.writeConsoleMessage("RobotExploration Machine State = " + localExpStatus, ConsoleMessageType.Information);
                }

            }
            catch (Exception ex)
            {
                ProjectCommons.writeConsoleMessage("LocalObsExploration->" + ex.ToString(), ConsoleMessageType.Error);
            }
        }

        private void checkMySignal()
        {
            if (signalCheckHandler.ElapsedMilliseconds > 200)
            {
                signalCheckHandler.Restart();
                Signal mapperSignal = getMapperSignal();
                if (mapperSignal != null)
                {
                    if (!mapperSignal.Status && (localExpStatus == LocalExpStatus.SELECT_FRONTIER || localExpStatus == LocalExpStatus.REACH_TO_FRONTIER))
                    {
                        localExpStatus = LocalExpStatus.TIMEOUT;
                        missions.Stop();
                        actions.Stop();
                        timeoutHandler.Restart();
                    }
                    else if (mapperSignal.Status &&
                            (localExpStatus == LocalExpStatus.TIMEOUT || localExpStatus == LocalExpStatus.SELECT_MAPPER || localExpStatus == LocalExpStatus.REACH_TO_MAPPER))
                    {
                        localExpStatus = LocalExpStatus.SELECT_FRONTIER;
                        missions.Stop();
                        actions.Stop();
                    }
                }
            }
        }


        private void MissionSkill_Reports(SkillStatus status)
        {
            switch (localExpStatus)
            {
                case LocalExpStatus.REACH_TO_FRONTIER:
                    switch (status)
                    {
                        case SkillStatus.STOP:
                        case SkillStatus.FAILED:
                            {
                                localExpStatus = LocalExpStatus.SELECT_FRONTIER;
                                break;
                            }
                    }
                    break;
                case LocalExpStatus.REACH_TO_MAPPER:
                    switch (status)
                    {
                        case SkillStatus.STOP:
                        case SkillStatus.FAILED:
                            localExpStatus = LocalExpStatus.SELECT_MAPPER;
                            break;
                    }
                    break;
            }
        }

        public bool driftCheck()
        {
            if (driftHandler.ElapsedMilliseconds > 200)
            {
                driftHandler.Restart();
                Pose2D currPosition = new Pose2D(WorldModel.Instance.EstimatedPose);
                if (lastPosition != null)
                {
                    double xD = currPosition.X - lastPosition.X;
                    double yD = currPosition.Y - lastPosition.Y;

                    double dist = Math.Sqrt((xD * xD) + (yD * yD));

                    if (dist <= 0.05f)
                        driftCount++;
                    else
                        driftCount = 0;

                    if (driftCount > 50)
                    {
                        driftCount = 0;
                        return true;
                    }
                }
                lastPosition = currPosition;
            }
            return false;
        }

        private void ActionSkill_Reports(SkillStatus status)
        {
            if (localExpStatus == LocalExpStatus.TURN_TIMEOUT || localExpStatus == LocalExpStatus.DRIFT_TIMEOUT)
            {
                switch (status)
                {
                    case SkillStatus.STOP:
                    case SkillStatus.FAILED:
                        turnTimeoutHandler.Stop();
                        Signal s = getMapperSignal();
                        if (s != null)
                        {
                            if (s.Status)
                                localExpStatus = LocalExpStatus.SELECT_FRONTIER;
                            else
                                localExpStatus = LocalExpStatus.SELECT_MAPPER;
                        }
                        break;
                }
            }
        }

    }
}
