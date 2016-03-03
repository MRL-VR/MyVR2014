using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Command.Drive;
using MRL.Controller;

namespace MRLRobot.Robot.Skills.RobotAction
{
    public class KenafAction : BaseAction
    {
        public KenafAction(AbstractDrive ad, Control ctrlModule)
        {
            controller = ctrlModule;
            botDriver = ad;
            maxSpeed = 2.0;
            DEVIDE_SPEED = 10.0f;
        }
    }
}
