using System;
using System.Drawing;

namespace MRL.CustomMath
{
    [Serializable]
    public class Pose3D
    {
        protected const double PI = System.Math.PI;
        private Vector3 _Position;
        private Vector3 _Rotation;

        public Vector3 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
            }
        }
        public Vector3 Rotation
        {
            get
            {
                return _Rotation;
            }
            set
            {
                _Rotation = value;
            }
        }

        public double X
        {
            get
            {
                return _Position.X;
            }
            set
            {
                _Position.X = value;
            }
        }
        public double Y
        {
            get
            {
                return _Position.Y;
            }
            set
            {
                _Position.Y = value;
            }
        }
        public double Z
        {
            get
            {
                return _Position.Z;
            }
            set
            {
                _Position.Z = value;
            }
        }

        public static Pose3D operator -(Pose3D t1, Pose3D t2)
        {
            Vector3 dP = t1._Position - t2._Position;
            Vector3 dR = t1._Rotation - t2._Rotation;
            return new Pose3D(dP, dR);
        }

        public static implicit operator Pose2D(Pose3D t)
        {
            return new Pose2D(t.X, t.Y, t.NormalizedRoll());
        }

        public static implicit operator PointF(Pose3D p)
        {
            return new PointF((float)p.X, (float)p.Y);
        }

        public static Pose3D operator +(Pose3D p1, Pose3D p2)
        {
            return new Pose3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z, 0, 0, 0);
        }
        public Pose3D()
            : this(new Vector3(0.0, 0.0, 0.0), new Vector3(0.0, 0.0, 0.0))
        {
        }

        public Pose3D(double x, double y, double z, double yaw, double phi, double roll)
            : this(new Vector3(x, y, z), new Vector3(yaw, phi, roll))
        {
        }

        public Pose3D(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public double NormalizedYaw()
        {
            return MRL.Utils.MathHelper.correctAngle(Rotation[0]);
        }
        public double NormalizedPhi()
        {
            return MRL.Utils.MathHelper.correctAngle(Rotation[1]);
        }
        public double NormalizedRoll()
        {
            return MRL.Utils.MathHelper.correctAngle(Rotation[2]);
        }
        public float getDistance2D(Pose3D p1)
        {
            return getDistance2D(this, p1);
        }
        public static float getDistance2D(Pose3D p1, Pose3D p2)
        {
            return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        public static Pose3D applyRotateShift(Pose3D point, float theta, Pose3D rotateCenter, Pose3D shift)
        {
            double nx, ny;
            double dx, dy;
            dx = point.X - rotateCenter.X;
            dy = point.Y - rotateCenter.Y;
            nx = dx * (float)Math.Cos(theta) - dy * (float)Math.Sin(theta) + rotateCenter.X;
            ny = dx * (float)Math.Sin(theta) + dy * (float)Math.Cos(theta) + rotateCenter.Y;
            nx += shift.X;
            ny += shift.Y;
            double nz = rotateCenter.Z;
            return new Pose3D(nx, ny, nz, 0, 0, 0);
        }
        public string ToMatlabString()
        {
            return X.ToString() + " " + Y.ToString() + " " + Z.ToString();
        }

        public string ToFormattedString()
        {
            return X.ToString("#0.####") + " , " + Y.ToString("#0.####") + " , " + Z.ToString("#0.####");
        }

        public string ToString(int decimalsInPosition, int decimalsInRotation)
        {
            return String.Format(String.Format("{{0:f{0}}},{{1:f{0}}},{{2:f{0}}}/{{3:f{1}}},{{4:f{1}}},{{5:f{1}}}",
                                 decimalsInPosition, decimalsInRotation), X, Y, Z, Rotation[0], Rotation[1], Rotation[2]);
        }

        public string ToString(int decimalsInPosition)
        {
            return String.Format(String.Format("{{0:f{0}}},{{1:f{0}}},{{2:f{0}}}", decimalsInPosition), X, Y, Z);
        }

        public override string ToString()
        {
            return this.ToString(5, 5);
        }
    }
}
