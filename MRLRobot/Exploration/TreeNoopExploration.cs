//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Diagnostics;
//using MRL.CustomMath;
//using MRL.Exploration.Frontiers;
//using MRL.Robot.Skills;
//using MRLRobot.Robot.Skills.RobotAction;
//using MRL.Robot.Skills.MissionRobot;
//using MRL.Communication.Tools;
//using System.Threading;
//using System.Threading.Tasks;
//using MRL.Utils;
//using MRL.Commons.Tree;
//using MRL.Commons;
//using MRL.Communication.External_Commands;
//namespace MRLRobot.Exploration
//{
//    public class TreeNoopExploration : BaseExploration
//    {

//        private volatile TreeExpStatus localExpStatus = TreeExpStatus.IDLE;

//        private Stopwatch timeoutHandler = new Stopwatch();
//        private Stopwatch signalCheckHandler = new Stopwatch();
//        private Stopwatch turnTimeoutHandler = new Stopwatch();
//        private Stopwatch failedTimeoutHandler = new Stopwatch();
//        private Stopwatch reachTimeoutHandler = new Stopwatch();
//        private Stopwatch driftHandler = new Stopwatch();
//        private Stopwatch driftTimeoutHandler = new Stopwatch();
//        private Pose2D lastPosition;
//        private int driftCount = 0;

//        private const int MAX_TIMEOUT = 5000;
//        private const int MAX_FAILED_TIMEOUT = 1000;
//        private const int MAX_TURN_TIMEOUT = 10000;
//        private const int MAX_REACH_TIMEOUT = 50000;
//        private const int MAX_DRIFT_TIMEOUT = 5000;
//        private GTree<Frontier> frontierTree;

//        public TreeNoopExploration()
//        {
//            frontierDetection = new FrontierDetection();
//            randomizer = new Random(DateTime.Now.Millisecond);
//            actions = SkillManager.Instance.ActionSkill as BaseAction;
//            missions = SkillManager.Instance.MissionSkill as MissionBase;
//            frontierTree = new GTree<Frontier>();
//            SkillManager.Instance.MissionSkill.Reports += new Action<SkillStatus>(MissionSkill_Reports);
//            SkillManager.Instance.ActionSkill.Reports += new Action<SkillStatus>(ActionSkill_Reports);
//        }

//        public override void Start()
//        {
//            localExpStatus = TreeExpStatus.SELECT_FRONTIER;
//            frontierTree = new GTree<Frontier>();
//            if (cts != null)
//            {
//                cts.Cancel();
//                cts = null;
//            }
//            cts = new CancellationTokenSource();
//            myTask = new Task(() => run());
//            myTask.Start();
//        }


//        public override void Stop()
//        {
//            if (cts != null)
//                cts.Cancel();
//            SkillManager.Instance.MissionSkill.Stop();

//            localExpStatus = TreeExpStatus.IDLE;
//            myTask = null;
//        }

//        private bool addFrontiersToTree()
//        {
//            List<Frontier> fList = frontierDetection.GetStaticFrontiers(new Laser(WorldModel.Instance.CurrentScan), new Pose2D(WorldModel.Instance.EstimatedPose), 15, 2);
//            ProjectCommons.writeConsoleMessage("Fronteris count(Weight) = " + fList.Count, ConsoleMessageType.Information);

//            if (fList.Count < 1)
//                return false;

//            selectedFrontier = findBestFrontier(fList);
//            if (selectedFrontier != null)
//            {
//                reachPoint(selectedFrontier.FrontierPosition);
//                foreach (var item in fList)
//                    if (item.Visited)
//                        frontierTree.Insert(new GNode<Frontier>() { Data = item });

//                frontierTree.Insert(new GNode<Frontier>() { Data = selectedFrontier }, true);
//                return true;
//            }

//            return false;
//        }


//        private void run()
//        {
//            signalCheckHandler.Restart();
//            while (!cts.Token.IsCancellationRequested)
//            {
//                switch (localExpStatus)
//                {
//                    case TreeExpStatus.SELECT_FRONTIER:
//                        if (addFrontiersToTree())
//                        {
//                            reachTimeoutHandler.Restart();
//                            driftCount = 0;
//                            driftHandler.Restart();
//                            localExpStatus = TreeExpStatus.REACH_TO_FRONTIER;
//                        }
//                        else
//                        {
//                            missions.Stop();
//                            failedTimeoutHandler.Restart();
//                            localExpStatus = TreeExpStatus.BACK_TO_PARENT;
//                        }
//                        break;

//                    case TreeExpStatus.BACK_TO_PARENT:
//                        GNode<Frontier> pFrontier = frontierTree.GotoParentNode();
//                        if (pFrontier == null)
//                        {

//                        }

//                        if (pFrontier.Childs.Count < 1)
//                            break;

//                        List<GNode<Frontier>> unvisitedList = pFrontier.Childs.Where(a => a.Data.Visited == false).ToList();

//                        if (unvisitedList.Count > 1)
//                        {
//                            Frontier sF = findBestFrontier(unvisitedList);

//                            if (sF != null)
//                            {
//                                reachPoint(selectedFrontier.FrontierPosition);
//                                reachTimeoutHandler.Restart();
//                                driftCount = 0;
//                                driftHandler.Restart();
//                                localExpStatus = TreeExpStatus.REACH_TO_FRONTIER;
//                            }
//                        }
//                        break;
//                    case TreeExpStatus.SELECT_MAPPER:
//                        if (decisionMakerByMapper())
//                        {
//                            reachTimeoutHandler.Restart();
//                            driftCount = 0;
//                            driftHandler.Restart();
//                            localExpStatus = TreeExpStatus.REACH_TO_MAPPER;
//                        }
//                        else
//                        {
//                            missions.Stop();

//                            failedTimeoutHandler.Restart();
//                            localExpStatus = TreeExpStatus.FAILED;
//                        }
//                        break;
//                    case TreeExpStatus.REACH_TO_FRONTIER:
//                        if (reachTimeoutHandler.ElapsedMilliseconds > MAX_REACH_TIMEOUT)
//                        {
//                            reachTimeoutHandler.Stop();
//                            failedTimeoutHandler.Restart();
//                            localExpStatus = TreeExpStatus.FAILED;
//                        }

//                        if (driftCheck())
//                        {
//                            reachTimeoutHandler.Stop();
//                            localExpStatus = TreeExpStatus.DRIFT;
//                        }
//                        break;
//                    case TreeExpStatus.REACH_TO_MAPPER:
//                        if (reachTimeoutHandler.ElapsedMilliseconds > MAX_REACH_TIMEOUT)
//                        {
//                            reachTimeoutHandler.Stop();
//                            failedTimeoutHandler.Restart();
//                            localExpStatus = TreeExpStatus.FAILED;
//                        }

//                        if (driftCheck())
//                        {
//                            reachTimeoutHandler.Stop();
//                            localExpStatus = TreeExpStatus.DRIFT;
//                        }
//                        break;
//                    case TreeExpStatus.FAILED:
//                        if (failedTimeoutHandler.ElapsedMilliseconds > MAX_FAILED_TIMEOUT)
//                        {
//                            failedTimeoutHandler.Stop();
//                            turnTimeoutHandler.Restart();

//                            missions.Stop();
//                            actions.Stop();

//                            ActionType side = sideDecision();

//                            int sDegree = (int)randomizer.Next(91, 179);
//                            switch (side)
//                            {
//                                case ActionType.LEFT:
//                                    actions.Left(sDegree);
//                                    break;
//                                case ActionType.RIGHT:
//                                    actions.Right(sDegree);
//                                    break;
//                                case ActionType.BACKWARD:
//                                    actions.Backward(1);
//                                    break;
//                            }
//                            localExpStatus = TreeExpStatus.TURN_TIMEOUT;
//                        }
//                        break;
//                    case TreeExpStatus.DRIFT:
//                        {
//                            missions.Stop();
//                            actions.Stop();

//                            ActionType side = driftDecision();

//                            int sDegree = (int)randomizer.Next(45, 150);
//                            switch (side)
//                            {
//                                case ActionType.LEFT:
//                                    actions.Left(sDegree);
//                                    break;
//                                case ActionType.RIGHT:
//                                    actions.Right(sDegree);
//                                    break;
//                                case ActionType.BACKWARD:
//                                    actions.Backward(1);
//                                    break;
//                            }
//                            localExpStatus = TreeExpStatus.DRIFT_TIMEOUT;
//                        }
//                        break;
//                    case TreeExpStatus.IDLE:
//                        break;
//                    case TreeExpStatus.TIMEOUT:
//                        if (timeoutHandler.ElapsedMilliseconds > MAX_TIMEOUT)
//                        {
//                            timeoutHandler.Stop();
//                            turnTimeoutHandler.Restart();

//                            actions.Left(179);

//                            localExpStatus = TreeExpStatus.TURN_TIMEOUT;
//                        }
//                        break;
//                    case TreeExpStatus.TURN_TIMEOUT:
//                        if (turnTimeoutHandler.ElapsedMilliseconds > MAX_TURN_TIMEOUT)
//                        {
//                            turnTimeoutHandler.Stop();
//                            failedTimeoutHandler.Restart();
//                            localExpStatus = TreeExpStatus.FAILED;
//                        }
//                        break;
//                    case TreeExpStatus.DRIFT_TIMEOUT:
//                        if (driftTimeoutHandler.ElapsedMilliseconds > MAX_DRIFT_TIMEOUT)
//                        {
//                            driftTimeoutHandler.Stop();
//                            localExpStatus = TreeExpStatus.DRIFT;
//                        }
//                        break;
//                }

//                checkMySignal();

//                Thread.Sleep(10);
//                //ProjectCommons.writeConsoleMessage("RobotExploration Machine State = " + localExpStatus, ConsoleMessageType.Information);
//            }
//        }

//        private void checkMySignal()
//        {
//            if (signalCheckHandler.ElapsedMilliseconds > 200)
//            {
//                signalCheckHandler.Restart();
//                Signal mapperSignal = getMapperSignal();
//                if (mapperSignal != null)
//                {
//                    if (!mapperSignal.Status && (localExpStatus == TreeExpStatus.SELECT_FRONTIER || localExpStatus == TreeExpStatus.REACH_TO_FRONTIER))
//                    {
//                        localExpStatus = TreeExpStatus.TIMEOUT;
//                        missions.Stop();
//                        actions.Stop();
//                        timeoutHandler.Restart();
//                    }
//                    else if (mapperSignal.Status &&
//                            (localExpStatus == TreeExpStatus.TIMEOUT || localExpStatus == TreeExpStatus.SELECT_MAPPER || localExpStatus == TreeExpStatus.REACH_TO_MAPPER))
//                    {
//                        localExpStatus = TreeExpStatus.SELECT_FRONTIER;
//                        missions.Stop();
//                        actions.Stop();
//                    }
//                }
//            }
//        }


//        private void MissionSkill_Reports(SkillStatus status)
//        {
//            switch (localExpStatus)
//            {
//                case TreeExpStatus.REACH_TO_FRONTIER:
//                    switch (status)
//                    {
//                        case SkillStatus.STOP:
//                        case SkillStatus.FAILED:
//                            localExpStatus = TreeExpStatus.SELECT_FRONTIER;
//                            break;
//                    }
//                    break;
//                case TreeExpStatus.REACH_TO_MAPPER:
//                    switch (status)
//                    {
//                        case SkillStatus.STOP:
//                        case SkillStatus.FAILED:
//                            localExpStatus = TreeExpStatus.SELECT_MAPPER;
//                            break;
//                    }
//                    break;
//            }
//        }

//        public bool driftCheck()
//        {
//            if (driftHandler.ElapsedMilliseconds > 200)
//            {
//                driftHandler.Restart();
//                Pose2D currPosition = new Pose2D(WorldModel.Instance.EstimatedPose);
//                if (lastPosition != null)
//                {
//                    double xD = currPosition.X - lastPosition.X;
//                    double yD = currPosition.Y - lastPosition.Y;

//                    double dist = Math.Sqrt((xD * xD) + (yD * yD));

//                    if (dist <= 0.05f)
//                        driftCount++;
//                    else
//                        driftCount = 0;

//                    if (driftCount > 50)
//                    {
//                        driftCount = 0;
//                        return true;
//                    }
//                }
//                lastPosition = currPosition;
//            }
//            return false;
//        }

//        private void ActionSkill_Reports(SkillStatus status)
//        {
//            if (localExpStatus == TreeExpStatus.TURN_TIMEOUT || localExpStatus == TreeExpStatus.DRIFT_TIMEOUT)
//            {
//                switch (status)
//                {
//                    case SkillStatus.STOP:
//                    case SkillStatus.FAILED:
//                        turnTimeoutHandler.Stop();
//                        Signal s = getMapperSignal();
//                        if (s != null)
//                        {
//                            if (s.Status)
//                                localExpStatus = TreeExpStatus.SELECT_FRONTIER;
//                            else
//                                localExpStatus = TreeExpStatus.SELECT_MAPPER;
//                        }
//                        break;
//                }
//            }
//        }

//        protected Frontier findBestFrontier(List<GNode<Frontier>> frontiers)
//        {
//            int index = (int)(randomizer.Next(0, frontiers.Count - 1));
//            frontiers[index].Data.Visited = true;
//            return frontiers[index].Data;
//        }

//    }
//}
