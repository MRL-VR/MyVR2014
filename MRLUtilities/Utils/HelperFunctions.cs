using System;

using MRL.CustomMath;

namespace MRL.Utils
{
    public static class HelperFunctions
    {

        public static double max(Pose2D q)
        {
            return Math.Max(Math.Max(q.X, q.Y), q.Rotation);
        }

        public static Pose2D abs(Pose2D q)
        {
            return new Pose2D(Math.Abs(q.X), Math.Abs(q.Y), Math.Abs(q.Rotation));
        }

        public static Pose2D compound(Pose2D t1, Pose2D t2)
        {
            Pose2D t_ret = new Pose2D();

            t_ret.X = t2.X * System.Math.Cos(t1.Rotation) - t2.Y * System.Math.Sin(t1.Rotation) + t1.X;
            t_ret.Y = t2.X * System.Math.Sin(t1.Rotation) + t2.Y * System.Math.Cos(t1.Rotation) + t1.Y;
            t_ret.Rotation = t1.Rotation + t2.Rotation;

            // Make angle [-pi,pi)
            t_ret.Rotation = t_ret.GetNormalizedRotation();

            return t_ret;
        }

        public static Pose2D compoundOnlyRot(Pose2D t1, Pose2D t2)
        {
            Pose2D t_ret = new Pose2D();

            t_ret.X = t2.X * System.Math.Cos(t1.Rotation) - t2.Y * System.Math.Sin(t1.Rotation);
            t_ret.Y = t2.X * System.Math.Sin(t1.Rotation) + t2.Y * System.Math.Cos(t1.Rotation);
            t_ret.Rotation = t2.Rotation;

            // Make angle [-pi,pi)
            t_ret.Rotation = t_ret.GetNormalizedRotation();

            return t_ret;
        }

    }
}
