using System.Collections.Generic;
using MRL.Communication.External_Commands;
using MRL.CustomMath;

namespace MRL.Exploration.ObstacleAvoidance
{
    public abstract class ObstacleAvoidance
    {
        public List<SonarSegment> sonarlist;
        public Pose2D currPosition;
        public Pose2D goalPosition;

        public abstract ObstacleResult CorrectPose(Laser laser, Pose2D currRobotPosition, Pose2D currRobotGoal, List<SonarSegment> sonarsList);
    }
}
