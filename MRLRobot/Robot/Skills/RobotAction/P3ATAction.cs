using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Command.Drive;
using MRL.Controller;

namespace MRLRobot.Robot.Skills.RobotAction
{
    public class P3ATAction : BaseAction
    {
        public P3ATAction(AbstractDrive ad, Control ctrlModule)
        {
            controller = ctrlModule;
            botDriver = ad;
            maxSpeed = 0.4;
            DEVIDE_SPEED = 6.0f;
        }
    }
}
