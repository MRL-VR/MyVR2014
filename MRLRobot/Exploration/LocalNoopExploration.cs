using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.IDE.Robot;
using System.Threading;
using MRL.Exploration;
using MRL.Exploration.Frontiers;
using MRL.Communication.External_Commands;
using MRL.Utils;
using MRL.Communication.Internal_Objects;
using MRL.CustomMath;
using MRL.Robot.Skills;
using System.Threading.Tasks;
using MRL.Commons;
using MRLRobot.Robot.Skills.RobotAction;
using MRL.Robot.Skills.MissionRobot;

namespace MRLRobot.Exploration
{
    public class LocalNoopExploration : BaseExploration
    {
        private LocalExpStatus expStatus = LocalExpStatus.IDLE;
        private Random randomizer;
        
        private List<Frontier> safeFrontiers;
        private Frontier selectedFrontier;

        public LocalNoopExploration()
        {
            frontierDetection = new FrontierDetection();
            safeFrontiers = new List<Frontier>();
            randomizer = new Random(DateTime.Now.Millisecond);
            SkillManager.Instance.MissionSkill.Reports += new Action<SkillStatus>(MissionSkill_Reports);
            actions = SkillManager.Instance.ActionSkill as BaseAction;
            missions = SkillManager.Instance.MissionSkill as MissionBase;
        }

        public override void Start()
        {
            expStatus = LocalExpStatus.SELECT_FRONTIER;
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
            cts = new CancellationTokenSource();
            Task.Factory.StartNew(() => run());
            BaseAction bc = SkillManager.Instance.ActionSkill as BaseAction;

        }

        public override void Stop()
        {
            if (cts != null)
                cts.Cancel();
            SkillManager.Instance.MissionSkill.Stop();

            expStatus = LocalExpStatus.IDLE;
        }

        private void run()
        {
            while (!cts.Token.IsCancellationRequested)
            {
                switch (expStatus)
                {
                    case LocalExpStatus.REACH_TO_FRONTIER:
                        break;
                    case LocalExpStatus.SELECT_FRONTIER:
                        decisionMaker();
                        expStatus = LocalExpStatus.REACH_TO_FRONTIER;
                        break;
                    case LocalExpStatus.FAILED:
                        break;
                    case LocalExpStatus.IDLE:
                        break;
                    case LocalExpStatus.TIMEOUT:
                        break;
                    default:
                        break;
                }
                Thread.Sleep(10);
            }
        }

        private void MissionSkill_Reports(SkillStatus status)
        {
            switch (status)
            {
                case SkillStatus.STOP:
                case SkillStatus.FAILED:
                    expStatus = LocalExpStatus.SELECT_FRONTIER;
                    break;
            }
        }

        private void decisionMaker()
        {
            List<Frontier> fList = frontierDetection.GetDynamicFrontiers(new Laser(WorldModel.Instance.CurrentScan), new Pose2D(WorldModel.Instance.EstimatedPose));
            ProjectCommons.writeConsoleMessage("Fronteris count = " + fList.Count, ConsoleMessageType.Information);
            selectedFrontier = findBestFrontier(fList);

            richPoint(selectedFrontier.FrontierPosition);
        }

        private Frontier findBestFrontier(List<Frontier> frontiers)
        {
            int index = (int)(randomizer.Next(0, frontiers.Count - 1));
            return frontiers[index];
        }

        private Frontier findBestFrontierByWeight(List<Frontier> frontiers)
        {
            return frontiers.FirstOrDefault(a => a.Weight == (frontiers.Max(b => b.Weight)));
        }

        private void richPoint(Pose2D richPoint)
        {
            Mission mission = new Mission();
            mission.pList.Add(new Pose2D());
            mission.pList.Add(richPoint);
            missions.Start(mission);
        }
    }
}
