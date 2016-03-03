using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Utils;
using MRL.CustomMath;

namespace MRL.Exploration.ObstacleAvoidance
{
    public class ObstacleResult
    {
        public ObstacleAlgorthm Alorithm = ObstacleAlgorthm.SINGL_POINT;
        public ObstacleStatus Status;
        public Pose2D CorrectedPose;
        public List<Pose2D> CorrectedPath;
    }
}
