using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRL.Robot.Skills
{
    public abstract class SkillManager
    {
        public Skill MissionSkill;
        public Skill ActionSkill;
        public static SkillManager Instance { set; get; }
    }
}
