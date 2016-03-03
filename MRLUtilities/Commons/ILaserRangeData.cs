using MRL.CustomMath;

namespace MRL.Commons
{
    public interface ILaserRangeData
    {
        float MaxRange { get; }
        float MinRange { get; }

        Pose3D SensorOffset3D { get; }
        Pose2D SensorOffset { get; }
        Pose2D RobotOffset { get; }

        double Resolution { get; }
        double FieldOfView { get; }

        double[] Range { get; }
        double[] RangeTheta { get; }
        bool[] RangeFilters { get; }
        float Time { get; }
    }

}
