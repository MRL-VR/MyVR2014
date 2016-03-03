using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.CustomMath;
using MRL.Communication.Internal_Objects;

namespace MRL.Robot.Skills
{
    public enum SkillType
    {
        MISSION, VISIT_ROOM, LEAVE_ROOM, STOP_NEAR_VICTIM, VISIT_HALL, RECOVERY
    }

    public enum SkillStatus
    {
        START, STOP, RESUME, CONTUNUE, FAILED
    }

    public abstract class Skill
    {
        public SkillType SkillType;

        public  Action<string> Actuators;
        public  Action<SkillStatus> Reports;

        public abstract void Start();
        public abstract void Stop();
        public abstract void Resume();
        public abstract void Pause();
    }
}
