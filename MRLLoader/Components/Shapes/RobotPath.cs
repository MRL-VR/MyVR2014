using System.Collections.Generic;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Components.Tools.Shapes
{
    public class RobotPath
    {
        #region Variables

        public List<Pose2D> RealPath = new List<Pose2D>(1000);
        public List<Pose2D> CanvasPath = new List<Pose2D>(1000);
        public RobotInfo RobotInfo;

        #endregion

        #region Property

        #endregion
    }
}
