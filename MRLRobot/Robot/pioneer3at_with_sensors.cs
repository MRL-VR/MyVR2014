using MRL.Command.Drive;
using MRL.Command.Joystick;
using MRL.CustomMath;
using MRL.CustomMath;
using MRL.IDE.Base;
using MRL.Utils;
using MRL.Command;
using MRLRobot.Exploration;
using MRL.Robot.Skills;
using MRL.Command.Joystick;
using MRL.Command.Drive;

namespace MRL.IDE.Robot
{
    public class pioneer3at_with_sensors : BaseRobot
    {
        public pioneer3at_with_sensors(RobotInfo me, int mountIndex)
            : base(me, mountIndex)
        {
            RangeScannerLocalPos = new Pose2D();
            RobotHasLaser = true;

            mMovingDriver = new FourWheeledDrive();
            joystickController = new P3ATCmd();
            SkillManager.Instance = new P3ATSkillMgr();
            robotExploration = new LocalObsExploration();
            semiAutonomous = new SemiAutonomous();
            localMap = new Mapping.EGMap();
        }
    }
}
