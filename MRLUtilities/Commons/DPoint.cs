using System;
using System.Drawing;

namespace MRL.Commons
{
    [Serializable]
    public class DPoint : ICloneable
    {
        public double x = 0, y = 0;

        public static implicit operator PointF(DPoint p)
        {
            return new PointF((float)p.x, (float)p.y);
        }

        public DPoint()
        {
        }

        public DPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "(x=" + x + ",y=" + y + ")";
        }

        public virtual Object Clone()
        {
            DPoint p = new DPoint();
            p.x = x;
            p.y = y;
            return p;
        }

        public override bool Equals(Object obj)
        {
            DPoint p2 = (DPoint)obj;
            if (p2 == null)
                return false;
            return (p2.x == x && p2.y == y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
