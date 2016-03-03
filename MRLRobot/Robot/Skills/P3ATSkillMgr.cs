using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Robot.Skills.MissionRobot;
using MRL.Command.Drive;
using MRLRobot.Robot.Skills.RobotAction;
using MRL.Controller;

namespace MRL.Robot.Skills
{
    public class P3ATSkillMgr : SkillManager
    {
        public P3ATSkillMgr()
        {
            MissionSkill = new P3ATMission(new FourWheeledDrive());
            ActionSkill = new P3ATAction(new FourWheeledDrive(), new Control());
        }


    }
}
