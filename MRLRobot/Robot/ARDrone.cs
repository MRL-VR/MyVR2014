using MRL.IDE.Base;
using MRL.Utils;
using MRL.Command.Drive;
namespace MRL.IDE.Robot
{
    public class ARDrone : BaseRobot
    {
        public ARDrone(RobotInfo me, int mountIndex)
            : base(me, mountIndex)
        {
            RobotHasLaser = false;

            mMovingDriver = new AirBotDrive();
            //skillManager = new P3ATSkillMgr();
        }
    }
}
