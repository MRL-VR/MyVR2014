using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Exploration;
using MRL.Communication.Internal_Objects;
using MRL.Robot.Skills.MissionRobot;
using MRL.Robot.Skills;
using MRL.Exploration.Frontiers;
using System.Threading.Tasks;
using MRL.Commons;
using System.Threading;
using MRLRobot.Robot.Skills.RobotAction;
using MRL.Utils;
using MRL.CustomMath;
using MRL.Communication.External_Commands;
using MRL.Communication.Tools;
using MRL.IDE.Robot;
using MRL.Exploration.PathPlannig;
using VisualizerLibrary;
using System.Drawing;
using MRL.Utils.GMath;

namespace MRLRobot.Exploration
{
    public enum LocalExpStatus
    {
        REACH_TO_FRONTIER, SELECT_FRONTIER, FAILED, IDLE, TIMEOUT, DANGER_AREA, DISCONNECTED, SELECT_MAPPER, REACH_TO_MAPPER, TURN_TIMEOUT, DRIFT, DRIFT_TIMEOUT
    }
    public enum TreeExpStatus
    {
        REACH_TO_FRONTIER, SELECT_FRONTIER, FAILED, IDLE, TIMEOUT, DANGER_AREA, DISCONNECTED, SELECT_MAPPER, REACH_TO_MAPPER, TURN_TIMEOUT, DRIFT, DRIFT_TIMEOUT, BACK_TO_PARENT
    }
    public abstract class BaseExploration
    {

        protected FrontierDetection frontierDetection;
        protected CancellationTokenSource cts;
        protected Task myTask;
        protected BaseAction actions;
        protected MissionBase missions;
        protected Random randomizer;
        protected Frontier selectedFrontier;
        protected RRTConnect rrtConnectPathPlanning;

        public abstract void Start();
        public abstract void Stop();

        public bool IsStarted()
        {
            if (myTask == null)
                return false;

            if (myTask.Status == TaskStatus.Running)
                return true;

            return false;
        }

        Random rand = new Random(DateTime.Now.Millisecond);
        protected bool decisionMakerByWeight(Laser l)
        {
            try
            {
                double d = rand.Next(4, 15);
                List<Frontier> fList = frontierDetection.GetStaticFrontiers2(new Laser(WorldModel.Instance.CurrentScan), new Pose2D(WorldModel.Instance.EstimatedPose), (int)d, 1);

                Visualizer.DrawListPoint("rrtPath", fList.Select(a => a.FrontierPosition).ToList(), Color.Yellow);

                if (fList.Count < 1)
                    return false;

                selectedFrontier = findBestFrontier(fList);

                if (selectedFrontier != null)
                {
                    List<Pose2D> path = rrtConnectPathPlanning.FindPathRRTConnect(WorldModel.Instance.EstimatedPose, selectedFrontier.FrontierPosition, l, 500);
                    if (path == null)
                        return false;

                    if (path.Count < 1)
                        return false;


                    Visualizer.DrawListPoint("rrtPath", path, Color.Cyan);

                    List<MRL.Utils.GMath.Line> tList = rrtConnectPathPlanning.myGraph.Select(a => new MRL.Utils.GMath.Line(new Position2D(a.Head), new Position2D(a.Tail) { DrawColor = Color.Black })).ToList();

                    Visualizer.ClearStartWith("tree");
                    for (int i = 0; i < tList.Count; i++)
                    {
                        Visualizer.DrawLine("tree" + i, tList[i].Head, tList[i].Tail, Color.Red, 0.05f);
                    }

                    List<MRL.Utils.GMath.Line> l_list = rrtConnectPathPlanning.myMap.Select(a => new MRL.Utils.GMath.Line(new Position2D(a.Head), new Position2D(a.Tail) { DrawColor = Color.Black })).ToList();

                    Visualizer.ClearStartWith("muli");
                    for (int i = 0; i < l_list.Count; i++)
                    {
                        Visualizer.DrawLine("muli" + i, l_list[i].Head, l_list[i].Tail, Color.Black, 0.01f);
                    }

                    reachPoint(path);

                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                ProjectCommons.writeConsoleMessage("Base Exploration->decisionMakerByWeight() ->> " + ex.ToString(), ConsoleMessageType.Error);
                return false;
            }
        }

        protected bool decisionMakerByMapper(Laser l)
        {
            List<Frontier> fList = frontierDetection.GetDynamicFrontiers(new Laser(WorldModel.Instance.CurrentScan), new Pose2D(WorldModel.Instance.EstimatedPose));
            ProjectCommons.writeConsoleMessage("Fronteris count(Mapper) = " + fList.Count, ConsoleMessageType.Information);

            if (fList.Count < 1)
                return false;

            selectedFrontier = findBestFrontierByMapperPos(fList);

            List<Pose2D> path = rrtConnectPathPlanning.FindPathRRTConnect(WorldModel.Instance.EstimatedPose, selectedFrontier.FrontierPosition, l, 500);

            List<MRL.Utils.GMath.Line> tList = rrtConnectPathPlanning.myGraph.Select(a => new MRL.Utils.GMath.Line(new Position2D(a.Head), new Position2D(a.Tail) { DrawColor = Color.Black })).ToList();

            Visualizer.ClearStartWith("tree");
            for (int i = 0; i < tList.Count; i++)
            {
                Visualizer.DrawLine("tree" + i, tList[i].Head, tList[i].Tail, Color.OrangeRed, 0.01f);
            }

            if (path == null)
                return false;
            if (path.Count < 1)
                return false;

            if (selectedFrontier != null)
            {
                reachPoint(path);
                return true;
            }

            return false;
        }

        protected Frontier findBestFrontierByWeight(List<Frontier> frontiers)
        {
            return frontiers.FirstOrDefault(a => a.Weight == (frontiers.Max(b => b.Weight)));
        }

        protected Frontier findBestFrontierByMapperPos(List<Frontier> frontiers)
        {
            Pose3D mapper3DPos = new Pose3D(ProjectCommons.config.mapperInfo.Position3D, ProjectCommons.config.mapperInfo.Rotation3D);
            Pose2D mapper2DPos = new Pose2D(mapper3DPos.X, mapper3DPos.Y, 0);

            foreach (var item in frontiers)
                item.Weight = 1.0 / (Math.Sqrt(Math.Pow((mapper2DPos.X - item.FrontierPosition.X), 2) + Math.Pow((mapper2DPos.Y - item.FrontierPosition.Y), 2)));

            return frontiers.FirstOrDefault(a => a.Weight == (frontiers.Max(b => b.Weight)));
        }

        protected ActionType sideDecision()
        {
            double rNum = randomizer.NextDouble();
            if (rNum > 0 && rNum <= 0.4)
                return ActionType.LEFT;
            if (rNum > 0.4 && rNum <= 0.8)
                return ActionType.RIGHT;
            return ActionType.BACKWARD;
        }

        protected ActionType driftDecision()
        {
            double rNum = randomizer.NextDouble();
            if (rNum > 0 && rNum <= 0.30)
                return ActionType.LEFT;
            if (rNum > 0.30 && rNum <= 0.60)
                return ActionType.RIGHT;
            return ActionType.BACKWARD;
        }

        protected Signal getMapperSignal()
        {
            return BaseRobot.Instance.GetRobotSignal(ProjectCommons.config.mapperInfo.Name);
        }

        protected void reachPoint(List<Pose2D> path)
        {
            Mission mission = new Mission();
            mission.pList.Add(new Pose2D());
            mission.pList.AddRange(path);
            missions.Start(mission);
        }

        protected Frontier findBestFrontier(List<Frontier> frontiers)
        {
            int index = (int)(randomizer.Next(0, frontiers.Count - 1));
            frontiers[index].Visited = true;
            return frontiers[index];
        }

    }
}
