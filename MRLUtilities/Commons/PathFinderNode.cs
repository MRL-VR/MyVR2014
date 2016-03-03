using System;

namespace MRL.Commons
{
    public class PathFinderNode : IComparable
    {
        public int F;
        public int G;
        public int H;
        public Index This;
        public PathFinderNode Parent;
        public double lastDir;

        public PathFinderNode(Index mPos, PathFinderNode mParent)
        {
            this.This = new Index(mPos);
            this.Parent = mParent;
            this.F = 0;
            this.G = 0;
            this.H = 0;
        }

        public PathFinderNode(Index mPos, PathFinderNode mParent, int mG, int mH)
        {
            this.This = new Index(mPos);
            this.Parent = mParent;
            this.G = mG;
            this.H = mH;
            this.F = mG + mH;
        }

        public override string ToString()
        {
            int PX, PY;
            PX = (Parent != null) ? Parent.This.x : 0;
            PY = (Parent != null) ? Parent.This.y : 0;

            string pfnStr = string.Format("(X:{0},Y:{1})-(PX:{2},PY:{3})-(F:{4},G:{5},H:{6})",
                                          This.x, This.y, PX, PY, F, G, H);
            return pfnStr;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            PathFinderNode nodeY = (PathFinderNode)obj;
            return (this.This.x == nodeY.This.x && this.This.y == nodeY.This.y);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();            
        }

        public int CompareTo(Object obj)
        {
            if (obj.GetType() != this.GetType())
            {
                return 1;
            }

            PathFinderNode path = (PathFinderNode)obj;
            if (path == null)
            {
                return 1;
            }

            int c1 = F;
            int c2 = path.F;
            return -c1.CompareTo(c2);
        }

    }
}
