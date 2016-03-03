using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Robot.Skills.MissionRobot;
using MRL.Command.Drive;
using MRL.Controller;
using MRLRobot.Robot.Skills.RobotAction;

namespace MRL.Robot.Skills
{
    public class KenafSkillMgr : SkillManager
    {
        public KenafSkillMgr()
        {
            MissionSkill = new KenafMission(new FourWheeledDrive());
            ActionSkill = new KenafAction(new FourWheeledDrive(), new Control());
        }
    }
}
