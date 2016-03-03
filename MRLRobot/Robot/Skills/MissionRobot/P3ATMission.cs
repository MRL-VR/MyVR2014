using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MRL.Commons;
using MRL.Communication.External_Commands;
using MRL.Communication.Internal_Objects;
using MRL.CustomMath;
using MRL.Exploration.ObstacleAvoidance;
using MRL.IDE.Base;
using MRL.Utils;
using MRL.Controller;
using MRL.Command.Drive;

namespace MRL.Robot.Skills.MissionRobot
{
    class P3ATMission : MissionBase
    {
        public P3ATMission(AbstractDrive ad)
        {
            driveController = ad;
            DEVIDE_SPEED = 5.0f;
        }
    }
}
