using MRL.CustomMath;

namespace MRL.Exploration
{
    public class Line
    {
        public Pose3D head;
        public Pose3D tail;
        public float a;
        public float b;
        public float err;
        public int laser_start;
        public int laser_end;
        public Line()
        {

        }

        public Line(Line t)
        {
            head = t.head;
            tail = t.tail;
            a = t.a;
            b = t.b;
            err = t.err;
            laser_start = t.laser_start;
            laser_end = t.laser_end;
        }

        public static float getAngleBetweenLines(Line l1, Line l2)
        {
            return 0;
        }

        public string ToMatlabString()
        {
            return head.ToMatlabString() + " " + tail.ToMatlabString();
        }

        public override string ToString()
        {
            return "Head : " + head.ToFormattedString() + " (to) Tail : " + tail.ToFormattedString();
        }
    }
}
