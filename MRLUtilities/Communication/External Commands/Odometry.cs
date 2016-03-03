using System;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Communication.External_Commands
{

    public class Odometry
    {
        //SEN {Type Odometry} {Name Odometry} {Pose 0.2415,0.0029,-0.5157}
        public float x = 0.0f;
        public float y = 0.0f;
        public float theta = 0.0f;

        public static implicit operator Pose2D(Odometry p)
        {
            return new Pose2D(p.x, p.y, p.theta);
        }

        public Pose2D GetPose2D()
        {
            return new Pose2D(x, y, theta);
        }

        public Odometry()
        {
        }
        public Odometry(Odometry t)
        {
            if (t == null) return;

            this.x = t.x;
            this.y = t.y;
            this.theta = t.theta;
        }

        public Odometry(USARParser msg)
        {
            if (msg.size == 0 || msg.segments == null) return;
            ParseState(msg);
        }

        private void ParseState(USARParser msg)
        {
            float[] curPose = USARParser.parseFloats(msg.getSegment("Pose").Get("Pose"), ",");
            x = curPose[0];
            y = curPose[1];
            theta = curPose[2];
        }

        public string ToString(int decimalsInPosition, int decimalsInRotation)
        {
            return String.Format(String.Format("{{0:f{0}}} , {{1:f{0}}} / {{2:f{1}}}", decimalsInPosition, decimalsInRotation), x, y, theta);
        }

        public string ToString(int decimalsInPosition)
        {
            return String.Format(String.Format("{{0:f{0}}} , {{1:f{0}}}", decimalsInPosition), x , y );
        }

        public override string ToString()
        {
            return this.ToString(5, 5);
        }

    }

}
