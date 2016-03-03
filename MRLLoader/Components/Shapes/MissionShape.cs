using System.Collections.Generic;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Components.Tools.Shapes
{
    public class MissionShape
    {
        private RobotInfo _robotInfo;
        private List<Pose2D> _polysReal = new List<Pose2D>();
        private List<Pose2D> _polysCanvas = new List<Pose2D>();

        public MissionShape()
        {

        }

        public RobotInfo RobotInfo
        {
            get { return _robotInfo; }
            set { _robotInfo = value; }
        }

        public List<Pose2D> RealBody
        {
            get { return _polysReal; }
            set { _polysReal = value; }
        }

        public List<Pose2D> CanvasBody
        {
            get { return _polysCanvas; }
            set { _polysCanvas = value; }
        }
    }
}
