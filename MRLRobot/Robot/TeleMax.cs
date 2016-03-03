using MRL.IDE.Base;
using MRL.Utils;
using MRL.Command.Drive;

namespace MRL.IDE.Robot
{
    public class TeleMax : BaseRobot
    {
        public TeleMax(RobotInfo me, int mountIndex)
            : base(me, mountIndex)
        {
            RobotHasLaser = false;
            RobotLocalizationMode = LocalizationMode.LM_GROUNDTRUTH;

            mMovingDriver = new FourWheeledDrive();
        }
    }
}
