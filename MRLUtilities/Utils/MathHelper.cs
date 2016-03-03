using System;
using System.Drawing;

using MRL.CustomMath;
using MRL.Exploration;

namespace MRL.Utils
{

    public class MathHelper
    {
        private static float PI = (float)Math.PI;

        /// <summary>
        /// convert 0-360 to (left)-pi~pi(right)
        /// </summary>
        /// <param name="x">x axis value in global frame</param>
        /// <param name="y">y axis value in global frame</param>
        /// <returns></returns>
        public static double calcTheta(double x, double y)
        {
            double theta = -(Math.Atan2(y, x) - (Math.PI / 2.0));
            return theta = normalAngle(theta, UnitType.UNIT_RAD);
        }

        /// <summary> 
        /// clalculate the difference between angle d and s.
        /// </summary>
        /// <param name="s">source angle ranged in -pi~pi or -180~180.
        /// </param>
        /// <param name="d">destination angle rangned in -pi~pi or -180~180.
        /// </param>
        /// <param name="type">the unit
        /// </param>
        public static double angleDif(double s, double d, UnitType type)
        {
            double dif = d - s;
            double pi;
            if (type == UnitType.UNIT_DEG)
                pi = 180.0;
            else if (type == UnitType.UNIT_RAD || 
                     type == UnitType.UNIT_RAW)
                pi = Math.PI;
            else
                pi = 32768.0;

            if (dif >= pi)
                dif -= 2.0 * pi;
            if (dif < -pi)
                dif += 2.0 * pi;

            return dif;
        }

        /// <summary> Normalize angle in -pi~pi</summary>
        /// <param name="a">the angle
        /// </param>
        /// <param name="type">the unit
        /// </param>
        public static double normalAngle(double a, UnitType type)
        {
            double pi;
            if (type == UnitType.UNIT_DEG)
                pi = 180.0;
            else if (type == UnitType.UNIT_RAD || 
                     type == UnitType.UNIT_RAW)
                pi = Math.PI;
            else
                pi = 32768.0;

            if (a > 2 * pi || a < (-2) * pi)
                a = a % (2 * pi);
            if (a >= pi)
                a -= 2 * pi;
            if (a < -pi)
                a += 2 * pi;

            return a;
        }

        public static float normalAngle(float a, UnitType type)
        {
            float pi;
            if (type == UnitType.UNIT_DEG)
                pi = 180.0f;
            else if (type == UnitType.UNIT_RAD || 
                     type == UnitType.UNIT_RAW)
            {
                pi = (float)Math.PI;
            }
            else
                pi = 32768.0f;

            if (a > 2 * pi || a < (-2) * pi)
                a = a % (2 * pi);
            if (a >= pi)
                a -= 2 * pi;
            if (a < -pi)
                a += 2 * pi;

            return a;
        }

        /// <summary> Normalize rotation in -pi~pi</summary>
        /// <param name="r">the rotation
        /// </param>
        /// <param name="type">the unit
        /// </param>
        public static void normalRotation(double[] r, UnitType type)
        {
            double pi;
            if (type == UnitType.UNIT_DEG)
                pi = 180.0;
            else if (type == UnitType.UNIT_RAD || 
                     type == UnitType.UNIT_RAW)
                pi = Math.PI;
            else
                pi = 32768.0;

            for (int i = 0; i < r.Length; i++)
            {
                if (r[i] > 2 * pi || r[i] < (-2) * pi)
                    r[i] = r[i] % (2 * pi);
                if (r[i] >= pi)
                    r[i] -= 2.0 * pi;
                if (r[i] < -pi)
                    r[i] += 2.0 * pi;
            }
        }

        public static void normalRotation(float[] r, UnitType type)
        {
            float pi;
            if (type == UnitType.UNIT_DEG)
                pi = 180.0f;
            else if (type == UnitType.UNIT_RAD ||
                     type == UnitType.UNIT_RAW)
            {
                pi = (float)Math.PI;
            }
            else
                pi = 32768.0f;

            for (int i = 0; i < r.Length; i++)
            {
                if (r[i] > 2 * pi || r[i] < (-2) * pi)
                    r[i] = r[i] % (2 * pi);
                if (r[i] >= pi)
                    r[i] -= 2 * pi;
                if (r[i] < -pi)
                    r[i] += 2 * pi;
            }
        }

        /// <summary> return middle point between two points of p1 & p2.</summary>
        public static Vector3 normalize(Vector3 p1, Vector3 p2)
        {
            Vector3 n = new Vector3();
            n = p1 + p2;
            n.X /= 2f;
            n.Y /= 2f;
            n.Z /= 2f;
            return n;
        }
        
        public static Pose3D normalize(Pose3D p1, Pose3D p2)
        {
            Pose3D n = new Pose3D();
            n = p1 + p2;
            n.X /= 2f;
            n.Y /= 2f;
            n.Z /= 2f;
            return n;
        }

        /// <summary> get the distance between two points.</summary>
        public static double getDistance(double sx, double sy, double dx, double dy)
        {
            double a = dx - sx;
            double b = dy - sy;
            return Math.Sqrt(a * a + b * b);
        }

        /// <summary> get the distance between two pose.</summary>
        public static double getDistance(Pose2D p1, Pose2D p2)
        {
            double a = p1.X - p2.X;
            double b = p1.Y - p2.Y;
            return Math.Sqrt(a * a + b * b);
        }

        /// <summary> compare two points using error tolerance
        /// .......|.................
        /// . -1 ..+-----+...........
        /// .......+  0  +...........
        /// .......+-----+....... 1 .
        /// .............|...........
        /// </summary>
        static public int comparePoint(PointF p1, PointF p2, double tolerance)
        {
            if (p1.X < p2.X - tolerance)
                return -1;
            else if (p1.X > p2.X + tolerance)
                return 1;
            else
            {
                if (p1.Y < p2.Y - tolerance)
                    return -1;
                else if (p1.Y > p2.Y + tolerance)
                    return 1;
                else
                    return 0;
            }
        }

        /// <summary>
        /// calculate the distance between two points. The sign of the distance is
        /// decided by the current moving direction, the theta.
        /// </summary>
        public static double posDif(double sx, double sy, double dx, double dy, double theta)
        {
            double a = dx - sx;
            double b = dy - sy;
            double dif = Math.Sqrt(a * a + b * b);
            double alpha = angleDif(theta, Math.Atan2(b, a), UnitType.UNIT_RAD);
            if (alpha > Math.PI / 2.0 || alpha < (-Math.PI) / 2.0)
                dif = -dif;
            return dif;
        }

        public static double boundVel(double v, double ceil, double floor)
        {
            double res;
            if (v < 0)
            {
                res = Math.Max(v, (-1) * ceil);
                return Math.Min(res, (-1) * floor);
            }
            else
            {
                res = Math.Min(v, ceil);
                return Math.Max(res, floor);
            }
        }

        public static double binaryVel(double v, double threshold, double val1, double val2)
        {
            if (v > 0)
                return (v > threshold) ? val1 : val2;
            else
                return (v < -threshold) ? -val1 : -val2;
        }


        public static float VectorRadian(PointF vFrom, PointF vTo)
        {
            float dp = DotProduct(vFrom, vTo),
                  vfl = VectorLength(vFrom),
                  vtl = VectorLength(vTo);

            float th = dp / (vfl * vtl);
            float fAlpha = (float)Math.Acos(th);

            PointF vT = new PointF();
            vT.X = vTo.X * vFrom.X + vTo.Y * vFrom.Y;
            vT.Y = vTo.X * (-vFrom.Y) + vTo.Y * vFrom.X;

            if (vT.Y < 0) { fAlpha = -fAlpha; }

            return fAlpha;
        }

        public static float AngleBetweenLines(PointF l1, PointF l2) // > 0 , <= PI/2
        {
            float angle = Math.Abs(VectorRadian(l1, l2));
            if (angle > PI / 2)
                angle = PI - angle;
            return angle;
        }
        public static float AngleBetweenLines(Line l1, Line l2) // > 0 , <= PI/2
        {
            Pose3D p1 = l1.head - l1.tail, p2 = l2.head - l2.tail;
            return AngleBetweenLines(p1, p2);
        }

        public static float DistanceBTP(PointF p1, PointF p2)
        {
            return (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2.0) + Math.Pow(p2.Y - p1.Y, 2.0));
        }

        public static double GetDistance(Vector2 p1, Vector2 p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2.0) + Math.Pow(p2.Y - p1.Y, 2.0));
        }

        public static float VectorLength(PointF vBase)
        {
            return (float)Math.Sqrt(Math.Pow(vBase.X, 2) + Math.Pow(vBase.Y, 2));
        }

        public static float DotProduct(PointF vBase, PointF vTo)
        {
            return (vBase.X * vTo.X) + (vBase.Y * vTo.Y);
        }

        public static float DegToRad(float deg)
        {
            return deg * ((float)Math.PI / 180.0f);
        }

        public static float RadToDeg(float rad)
        {
            return rad * (180.0f / (float)Math.PI);
        }

        public static int GetCycleDelta(int delta)
        {
            if (Math.Abs(delta) >= 180)
            {
                int y = delta % 180;
                return y - Math.Sign(y) * 180;
            }
            else
            {
                return delta;
            }
        }

        public static float GetCycleDelta(float delta)
        {
            if (Math.Abs(delta) >= (float)Math.PI)
            {
                float y = delta % (float)Math.PI;
                return y - Math.Sign(y) * (float)Math.PI;
            }
            else
            {
                return delta;
            }
        }

        public static double correctAngle(double th)
        {
            double radians = th;

            while (radians > PI)
                radians -= 2 * PI;

            while (radians <= -PI)
                radians += 2 * PI;

            return radians;
        }

        public static Pose2D CorrectAngle(Pose2D l, double cangle)
        {
            double correction_angle = cangle;
            double X = l.X * Math.Cos(correction_angle) - l.Y * Math.Sin(correction_angle);
            double Y = l.X * Math.Sin(correction_angle) + l.Y * Math.Cos(correction_angle);
            l.X = X;
            l.Y = Y;
            return l;
        }
        public static Pose2D CorrectAngle(Pose2D l)
        {
            return CorrectAngle(l, -Math.PI / 2);
        }

    }

}
