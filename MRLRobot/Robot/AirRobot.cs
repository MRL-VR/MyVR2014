using MRL.IDE.Base;
using MRL.Utils;
using MRL.Command;
using MRL.Robot.Skills;
using MRL.Command.Joystick;
using MRL.Command.Drive;
using MRLRobot.Exploration;
using MRL.Localization;
using MRL.Commons;
using MRL.CustomMath;

namespace MRL.IDE.Robot
{
    public class AirRobot : BaseRobot
    {

        public AirRobot(RobotInfo me, int mountIndex)
            : base(me, mountIndex)
        {
            RobotHasLaser = false;

            mMovingDriver = new AirBotDrive();
            joystickController = new AirRobotCmd();
            SkillManager.Instance = new P3ATSkillMgr();
            robotExploration = new LocalObsExploration();
            semiAutonomous = new SemiAutonomous();
            double rotation = ProjectCommons.config.botInfo[ThisRobot.MountIndex].Rotation3D.Z;
            imuLocalization = new IMULocalization(new Pose2D(this.ThisRobot.StartPoint.X, this.ThisRobot.StartPoint.Y, rotation));
        }
    }
}
