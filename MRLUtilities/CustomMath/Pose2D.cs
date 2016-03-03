using System;
using System.Drawing;
using MRL.Commons;

namespace MRL.CustomMath
{

    [Serializable]
    public class Pose2D
    {
        public static Pose2D operator +(Vector2 V, Pose2D P)
        {
            return new Pose2D(P.X + V.X, P.Y + V.Y, 0);
        }

        public static implicit operator DPoint(Pose2D p)
        {
            return new DPoint(p.X, p.Y);
        }
        public static implicit operator Pose3D(Pose2D t)
        {
            return new Pose3D(t.X, t.Y, 0, 0, 0, t.GetNormalizedRotation());
        }

        public static implicit operator PointF(Pose2D t)
        {
            return new PointF((float)t.X, (float)t.Y);
        }

        public static Pose2D operator -(Pose2D t1, Pose2D t2)
        {
            return new Pose2D(t1.Position - t2.Position,
                              t1.Rotation - t2.Rotation);
        }

        public double DistanceFrom(Pose2D From)
        {
            return Math.Sqrt((this.X - From.X) * (this.X - From.X) + (this.Y - From.Y) * (this.Y - From.Y));
        }

        protected const double PI = System.Math.PI;
        private Vector2 _Position;
        private double _Rotation;

        public Vector2 Position
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
        public double Rotation
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

        public Pose2D()
            : this(new Vector2(0.0, 0.0), 0.0)
        {
        }

        public Pose2D(Point p)
            : this(p.X, p.Y, 0)
        {
        }

        public Pose2D(Pose2D t)
            : this(t._Position, t._Rotation)
        {
        }

        public Pose2D(double rotation)
            : this(new Vector2(0.0, 0.0), rotation)
        {
        }

        public Pose2D(double x, double y, double rotation)
            : this(new Vector2(x, y), rotation)
        {
        }

        public Pose2D(Vector2 position, double rotation)
        {
            Position = new Vector2(position);
            Rotation = rotation;
        }

        public double GetNormalizedRotation()
        {
            double radians = Rotation;

            while (radians > PI)
                radians -= 2 * PI;

            while (radians <= -PI)
                radians += 2 * PI;

            return radians;
        }

        public Pose2D ToGlobal(Pose2D currentOrigin)
        {
            TMatrix2D rotmx = currentOrigin.ToGlobalMatrix();
            return rotmx * this;
        }

        public TMatrix2D ToGlobalMatrix()
        {
            return new TMatrix2D(Position, Rotation);
        }

        public Pose2D ToLocal(Pose2D targetOrigin)
        {
            TMatrix2D rotmx = targetOrigin.ToLocalMatrix();
            return rotmx * this;
        }

        public TMatrix2D ToLocalMatrix()
        {
            TMatrix2D mx = new TMatrix2D(-Rotation);
            Pose2D merotated = mx * this;
            mx.Translation = -merotated.Position;
            return mx;
        }

        public string ToString(int decimalsInPosition, int decimalsInRotation)
        {
            return String.Format(String.Format("{{0:f{0}}},{{1:f{0}}}/{{2:f{1}}}", decimalsInPosition, decimalsInRotation), X, Y, Rotation);
        }

        public string ToString(int decimalsInPosition)
        {
            return String.Format(String.Format("{{0:f{0}}},{{1:f{0}}}", decimalsInPosition), X, Y);
        }

        public override string ToString()
        {
            return this.ToString(5, 5);
        }

    } // class Pose2D

}

