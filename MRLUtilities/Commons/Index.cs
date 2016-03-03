using System;

namespace MRL.Commons
{
    public class Index 
    {
        int r = 0;
        int c = 0;

        public int x
        {
            get { return r; }
            set { r = value; }
        }

        public int y
        {
            get { return c; }
            set { c = value; }
        }

        public Index(Index t)
        {
            this.r = t.r;
            this.c = t.c;
        }

        public Index(int x, int y)
        {
            r = x;
            c = y;
        }

        public float getDistance(Index next)
        {
            return (float)Math.Sqrt(Math.Pow(r - next.r, 2) + Math.Pow(c - next.c, 2));
        }

        public override string ToString()
        {
            string pfnStr = string.Format("(X:{0},Y:{1})", r, c);
            return pfnStr;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType()) return false;
            Index mOther = (Index)obj;
            return (mOther.r == this.r && mOther.c == this.c);
        }
    }
}
