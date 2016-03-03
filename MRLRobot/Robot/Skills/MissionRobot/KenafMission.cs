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
    class KenafMission : MissionBase
    {
        public KenafMission(AbstractDrive ad)
            : base()
        {
            driveController = ad;
            DEVIDE_SPEED = 6.0f;
        }

    }
}
