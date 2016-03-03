using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Robot.Skills.MissionRobot;
using MRL.Robot.Skills;
using MRL.Communication.Internal_Objects;
using MRL.Commons;
using MRLRobot.Robot.Skills.RobotAction;
using MRL.Communication.Tools;
using MRL.IDE.Robot;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using MRL.CustomMath;
using MRL.Utils;

namespace MRLRobot.Exploration
{
    public enum SemiAutomonousStaus { IDLE, CONNECTED, DISCONNECT, DO_BACKWARD, TRY_BACKWARD, WAIT_FOR_ROUTING }
    public class SemiAutonomous : BaseExploration
    {
        private volatile SemiAutomonousStaus semiAutoStatus = SemiAutomonousStaus.IDLE;
        private Stopwatch backwardTimeoutHandler = new Stopwatch();
        private Stopwatch signalCheckHandler = new Stopwatch();
        private Stopwatch disconnectHandler = new Stopwatch();

        private volatile int backwardCount = 0;

        private const int MAX_BACKWARD_TIMEOUT = 10000;
        private const int MAX_DISCONNECT_TIMEOUT = 5000;

        public SemiAutonomous()
        {
            actions = SkillManager.Instance.ActionSkill as BaseAction;
            missions = SkillManager.Instance.MissionSkill as MissionBase;
            SkillManager.Instance.ActionSkill.Reports += new Action<SkillStatus>(ActionSkill_Reports);
        }

        public override void Start()
        {
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

            myTask = null;
        }

        public void StartMission(Mission mission)
        {
            missions.Start(mission);
        }

        public void StopMission()
        {
            missions.Stop();
        }

        private void checkMySignal()
        {
            if (signalCheckHandler.ElapsedMilliseconds > 500)
            {
                signalCheckHandler.Restart();
                Signal curSig = getMapperSignal();
                if (curSig != null && semiAutoStatus != SemiAutomonousStaus.IDLE)
                {
                    if (!curSig.Status && semiAutoStatus == SemiAutomonousStaus.CONNECTED)
                    {
                        backwardCount = 0;
                        disconnectHandler.Restart();
                        semiAutoStatus = SemiAutomonousStaus.DISCONNECT;
                    }
                    else if (curSig.Status && semiAutoStatus != SemiAutomonousStaus.CONNECTED)
                    {
                        actions.Stop();
                        backwardCount = 0;
                        semiAutoStatus = SemiAutomonousStaus.CONNECTED;
                    }
                }
            }
        }

        private void run()
        {
            semiAutoStatus = SemiAutomonousStaus.WAIT_FOR_ROUTING;
            signalCheckHandler.Restart();
            while (!cts.Token.IsCancellationRequested)
            {
                switch (semiAutoStatus)
                {
                    case SemiAutomonousStaus.IDLE:
                        break;
                    case SemiAutomonousStaus.CONNECTED:
                        break;
                    case SemiAutomonousStaus.WAIT_FOR_ROUTING:
                        break;
                    case SemiAutomonousStaus.DISCONNECT:
                        if (disconnectHandler.ElapsedMilliseconds > MAX_DISCONNECT_TIMEOUT)
                        {
                            disconnectHandler.Stop();
                            missions.Stop();
                            backwardCount = 0;
                            semiAutoStatus = SemiAutomonousStaus.DO_BACKWARD;
                        }
                        break;
                    case SemiAutomonousStaus.DO_BACKWARD:
                        if (backwardCount < 2)
                        {
                            backwardCount++;
                            actions.Backward(1.5f);
                            backwardTimeoutHandler.Restart();
                            semiAutoStatus = SemiAutomonousStaus.TRY_BACKWARD;
                        }
                        else
                        {
                            backwardCount = 0;
                            semiAutoStatus = SemiAutomonousStaus.WAIT_FOR_ROUTING;
                            actions.Stop();
                        }
                        break;
                    case SemiAutomonousStaus.TRY_BACKWARD:
                        if (backwardTimeoutHandler.ElapsedMilliseconds > MAX_BACKWARD_TIMEOUT)
                        {
                            backwardTimeoutHandler.Stop();
                            actions.Stop();
                            semiAutoStatus = SemiAutomonousStaus.DO_BACKWARD;
                        }
                        break;
                }
                checkMySignal();

                Thread.Sleep(10);
                //ProjectCommons.writeConsoleMessage("SemiAutonomous Machine State = " + semiAutoStatus + "=" + backwardCount, ConsoleMessageType.Information);
            }
        }

        private void ActionSkill_Reports(SkillStatus status)
        {
            if (semiAutoStatus == SemiAutomonousStaus.TRY_BACKWARD)
            {
                switch (status)
                {
                    case SkillStatus.STOP:
                    case SkillStatus.FAILED:
                        Signal s = getMapperSignal();
                        if (s != null)
                        {
                            if (s.Status)
                            {
                                semiAutoStatus = SemiAutomonousStaus.CONNECTED;
                                backwardCount = 0;
                            }
                            else
                                semiAutoStatus = SemiAutomonousStaus.DO_BACKWARD;
                        }
                        break;
                }
            }
        }
    }
}
