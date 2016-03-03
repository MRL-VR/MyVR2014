using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.CustomMath;
using System.Threading;
using System.Threading.Tasks;
using MRL.Utils;
using MRL.Controller;
using MRL.Robot.Skills;
using MRL.Command.Drive;
using MRL.IDE.Robot;
using MRL.Commons;
using System.Drawing;
using MRL.Communication.External_Commands;
using MRL.Exploration.Frontiers;

namespace MRLRobot.Robot.Skills.RobotAction
{
    public enum ActionType { LEFT, RIGHT, FORWARD, BACKWARD, STOP, RICH_POINT, FINISH, GOTO_MAPPER }
    public enum DirectionStatus { LEFT_TO_RIGHT, RIGHT_TO_LEFT, SAME_LINE }
    public abstract class BaseAction : Skill
    {
        private CancellationTokenSource ctsExit = new CancellationTokenSource();
        private ActionType actionType;
        private double gDegree;
        private Pose2D goalPos;
        private int repeatValue;

        private const double THRESHOLD_NEAR_TO_GOAL = 0.5f;
        private const double THRESHOLD_NEAR_TO_ROTATION = 0.5f;

        protected double DEVIDE_SPEED;

        protected Control controller;
        protected AbstractDrive botDriver;
        protected double maxSpeed;

        public override void Start()
        {

        }

        public override void Resume()
        {

        }

        public override void Pause()
        {

        }

        public override void Stop()
        {
            action(ActionType.STOP);
        }

        public void Left(int degree)
        {
            int fDegree = splitDirection(degree);
            gDegree = Math.Round((currDegree() + fDegree) % 360f);
            action(ActionType.LEFT);
        }

        public void Right(int degree)
        {
            int fDegree = splitDirection(degree);
            gDegree = Math.Round((currDegree() - fDegree) % 360f);
            action(ActionType.RIGHT);
        }

        public void Forward(double meter)
        {
            Pose2D currPos = WorldModel.Instance.EstimatedPose;
            double x = currPos.X + (meter * Math.Cos(currPos.Rotation));
            double y = currPos.Y + (meter * Math.Sin(currPos.Rotation));
            goalPos = new Pose2D(x, y, 0);
            action(ActionType.FORWARD);
        }

        public void ForwardAndLeft(int degree, double meter)
        {

        }

        public void ForwardAndRight(int degree, double meter)
        {

        }

        public void Backward(double meter)
        {
            Pose2D currPos = WorldModel.Instance.EstimatedPose;
            double x = currPos.X - (meter * Math.Cos(currPos.Rotation));
            double y = currPos.Y - (meter * Math.Sin(currPos.Rotation));
            goalPos = new Pose2D(x, y, 0);
            action(ActionType.BACKWARD);
        }

        public void ReachPoint(Pose2D destPoint)
        {
            goalPos = destPoint;
            action(ActionType.RICH_POINT);
        }

        private void action(ActionType aType)
        {
            actionType = ActionType.FINISH;
            if (ctsExit != null)
                ctsExit.Cancel();
            Thread.Sleep(10);
            ctsExit = new CancellationTokenSource();
            actionType = aType;
            Task.Factory.StartNew(() => run());
        }

        private void run()
        {
            while (!ctsExit.Token.IsCancellationRequested)
            {
                switch (actionType)
                {
                    case ActionType.LEFT:
                        DirectionStatus lds = getDirection(currDegree(), gDegree);

                        if (lds == DirectionStatus.RIGHT_TO_LEFT)
                        {
                            string cmd = "DRIVE {Left " + -maxSpeed + "} {Right " + maxSpeed + "} {Normalized False}";
                            if (Actuators != null)
                                Actuators(cmd);
                        }
                        else
                        {
                            if (repeatValue > 0)
                            {
                                gDegree = Math.Round((currDegree() + repeatValue) % 360f);
                                repeatValue = 0;
                            }
                            else
                                actionType = ActionType.STOP;
                        }
                        break;
                    case ActionType.RIGHT:
                        DirectionStatus rds = getDirection(currDegree(), gDegree);
                        if (rds == DirectionStatus.LEFT_TO_RIGHT)
                        {
                            string cmd = "DRIVE {Left " + maxSpeed + "} {Right " + -maxSpeed + "} {Normalized False}";
                            Actuators(cmd);
                        }
                        else
                        {
                            if (repeatValue > 0)
                            {
                                gDegree = Math.Round((currDegree() - repeatValue) % 360f);
                                repeatValue = 0;
                            }
                            else
                                actionType = ActionType.STOP;
                        }
                        break;
                    case ActionType.STOP:
                        {
                            actionType = ActionType.FINISH;
                            ctsExit.Cancel();
                            string cmd = "DRIVE {Left 0} {Right 0} {Normalized False}";
                            Actuators(cmd);

                            if (Reports != null)
                                Reports(SkillStatus.STOP);
                        }
                        break;
                    case ActionType.FORWARD:
                        {
                            Pose2D currPos = WorldModel.Instance.EstimatedPose;
                            if (reachPosition(currPos, goalPos))
                                actionType = ActionType.STOP;
                            else
                            {
                                string cmd = "DRIVE {Left " + maxSpeed + "} {Right " + maxSpeed + "} {Normalized False}";
                                Actuators(cmd);
                            }
                        }
                        break;
                    case ActionType.BACKWARD:
                        {
                            Pose2D currPos = WorldModel.Instance.EstimatedPose;
                            if (reachPosition(currPos, goalPos))
                                actionType = ActionType.STOP;
                            else
                            {
                                string cmd = "DRIVE {Left " + -maxSpeed + "} {Right " + -maxSpeed + "} {Normalized False}";
                                Actuators(cmd);
                            }
                        }
                        break;
                    case ActionType.RICH_POINT:
                        {
                            Pose2D currPos = WorldModel.Instance.EstimatedPose;
                            if (reachPosition(currPos, goalPos))
                                actionType = ActionType.STOP;
                            else
                            {
                                Vector3 speed = controller.CheckSpeed(new Vector2(currPos.X, currPos.Y), new Vector2(goalPos.X, goalPos.Y), currPos.GetNormalizedRotation());
                                double leftSpeed = speed.X / DEVIDE_SPEED;
                                double rightSpeed = speed.Y / DEVIDE_SPEED;
                                string cmd = "DRIVE {Left " + leftSpeed + "} {Right " + rightSpeed + "} {Normalized False}";
                                Actuators(cmd);
                            }
                        }
                        break;
                }
                Thread.Sleep(10);
            }
        }

        private bool reachPosition(Pose2D src, Pose2D dest)
        {
            double delta = Math.Sqrt(Math.Pow(src.X - dest.X, 2) + Math.Pow(src.Y - dest.Y, 2));
            if (THRESHOLD_NEAR_TO_GOAL >= delta)
                return true;
            return false;
        }

        private int splitDirection(int degree)
        {
            repeatValue = 0;
            int eDegree = 0;

            if (degree > 359)
                eDegree = 359;

            if (degree < 0)
                eDegree = 0;

            ProjectCommons.writeConsoleMessage("Final Degree = " + eDegree, ConsoleMessageType.Information);
            if (eDegree > 180)
            {
                repeatValue = ((eDegree % 180) == 180) ? 179 : (eDegree % 180);
                return 179;
            }

            return (degree == 180) ? 179 : degree;
        }

        private double currDegree()
        {
            if (WorldModel.Instance.EstimatedPose != null)
            {
                double cDegree = WorldModel.Instance.EstimatedPose.Rotation * 180.0 / Math.PI;
                cDegree = cDegree < 0 ? (cDegree + 360) : cDegree;
                return 360 - Math.Round(cDegree);
            }
            return double.NaN;
        }

        private DirectionStatus getDirection(double sDegree, double gDegree)
        {
            double s = (sDegree / 180.0) * Math.PI;
            double g = (gDegree / 180.0) * Math.PI;

            PointF p1 = new PointF();
            PointF p3 = new PointF();

            p1.X = (float)(10.0 * Math.Cos(s));
            p1.Y = (float)(10.0 * Math.Sin(s));

            p3.X = (float)(10.0 * Math.Cos(g));
            p3.Y = (float)(10.0 * Math.Sin(g));

            double f = p1.X * (-p3.Y) - p1.Y * (-p3.X);

            if (f > 0)
                return DirectionStatus.LEFT_TO_RIGHT;
            if (f < 0)
                return DirectionStatus.RIGHT_TO_LEFT;

            return DirectionStatus.SAME_LINE;
        }
    }
}
