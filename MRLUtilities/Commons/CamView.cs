using System;

using MRL.CustomMath;

namespace MRL.Commons
{

    [Serializable()]
    public class CamView
    {
        public double x, y, theta, fov;

        public CamView ( Pose2D p, double fov )
        {
            this.x = p.X;
            this.y = p.Y;
            this.theta = p.Rotation;
            this.fov = fov;
        }

        public CamView ( double x, double y, double theta, double fov )
        {
            this.x = x;
            this.y = y;
            this.theta = theta;
            this.fov = fov;
        }

        public override string ToString ( )
        {
            return "CamView [" + x + "," + y + "," + theta + "," + fov + "]";
        }
    }

}
