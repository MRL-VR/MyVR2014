using System;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Communication.External_Commands
{

    public class INS
    {
        //SEN {Type INS} {Name INS} {Location 4.50,1.90,1.80} {Orientation 0.00,0.00,0.00}
        public Vector3 p3Ori = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 p3Loc = new Vector3(0.0f, 0.0f, 0.0f);

        public static implicit operator Pose3D(INS s)
        {
            return new Pose3D(s.p3Loc, s.p3Ori);
        }

        public Pose2D GetPose2D()
        {
            return new Pose2D(this.p3Loc.X, this.p3Loc.Y, this.p3Ori.Z);
        }

        public Pose3D GetPose3D()
        {
            return new Pose3D(p3Loc, p3Ori);
        }

        public INS(INS t)
        {
            if (t == null) return;

            this.p3Ori = new Vector3(t.p3Ori);
            this.p3Loc = new Vector3(t.p3Loc);
        }

        public INS(USARParser msg)
        {
            if (msg.size == 0 || msg.segments == null) return;
            ParseState(msg);
        }

        private void ParseState(USARParser msg)
        {
            float[] curLocation = USARParser.parseFloats(msg.getSegment("Location").Get("Location"), ",");
            float[] curRotation = USARParser.parseFloats(msg.getSegment("Orientation").Get("Orientation"), ",");
            MathHelper.normalRotation(curRotation, UnitType.UNIT_RAD);

            p3Loc = new Vector3(curLocation[0], curLocation[1], curLocation[2]);
            p3Ori = new Vector3(curRotation[0], curRotation[1], curRotation[2]);
        }

        public string ToString(int decimalsInPosition, int decimalsInRotation)
        {
            return String.Format(String.Format("{{0:f{0}}} , {{1:f{0}}} , {{2:f{0}}} / {{3:f{1}}} , {{4:f{1}}} , {{5:f{1}}}",
                                 decimalsInPosition, decimalsInRotation), p3Loc[0], p3Loc[1], p3Loc[2], p3Ori[0], p3Ori[1], p3Ori[2]);
        }

        public string ToString(int decimalsInPosition)
        {
            return String.Format(String.Format("{{0:f{0}}} , {{1:f{0}}} , {{2:f{0}}}", decimalsInPosition),
                                 p3Loc[0], p3Loc[1], p3Loc[2]);
        }

        public override string ToString()
        {
            return this.ToString(5, 5);
        }

    }

}
