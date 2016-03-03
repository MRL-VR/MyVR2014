using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRL.Exploration.ObstacleAvoidance
{
    public class Gaps
    {
        public double SizeNod;
        public int indexNod;
        public double DisFromCenter;
        public Gaps()
        {
        }
        public Gaps(double Size, int index, double DFC)
        {
            SizeNod = Size;
            indexNod = index;
            DisFromCenter = DFC;
        }
    }
}
