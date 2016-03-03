using MRL.CustomMath;
using MRL.IDE.Base;
using MRL.Utils;
using MRL.Command;
using MRL.Robot.Skills;
using MRL.Command.Drive;
using MRL.Command.Joystick;
using MRLRobot.Exploration;

namespace MRL.IDE.Robot
{
    public class Kenaf : BaseRobot
    {
        public Kenaf(RobotInfo me, int mountIndex)
            : base(me, mountIndex)
        {
            RangeScannerLocalPos = new Pose2D();
            RobotHasLaser = true;
            RobotLocalizationMode = LocalizationMode.LM_RAW_INS;


            //This is driver for ForWheeledDrive that is not for Kenaf robots, Because at the peresent the driver of kenaf is same as p3at
            mMovingDriver = new KenafDrive();
            joystickController = new KenafCmd();
            SkillManager.Instance = new KenafSkillMgr();
            robotExploration = new LocalObsExploration();
            semiAutonomous = new SemiAutonomous();
            localMap = new Mapping.EGMap();
        }
    }
}
