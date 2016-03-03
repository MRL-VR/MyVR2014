using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Communication.External_Commands
{
    public class IMU
    {
        //SEN {Time 3843.9387} {Type IMU} {Name IMU} {XYZAcceleration 0.0000,0.0000,-0.0000} {AngularVel 0.0000,0.0000,0.0000} {AngularAccel 0.0000,0.0000,0.0000} {Rotation 0.0000,0.0000,0.0000}
        public Vector3 XYZAcceleration = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 AngularVel = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 AngularAccel = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 Rotation = new Vector3(0.0f, 0.0f, 0.0f);

        public IMU(USARParser msg)
        {
            if (msg.size == 0 || msg.segments == null) return;

            ParseState(msg);
        }
        public IMU(IMU t)
        {
            if (t == null) return;

            this.XYZAcceleration = new Vector3(t.XYZAcceleration);
            this.AngularVel = new Vector3(t.AngularVel);
            this.AngularAccel = new Vector3(t.AngularAccel);
            this.Rotation = new Vector3(t.Rotation);
        }

        private void ParseState(USARParser msg)
        {
            float[] curXYZAcceleration = USARParser.parseFloats(msg.getSegment("XYZAcceleration").Get("XYZAcceleration"), ",");
            float[] curAngularVel = USARParser.parseFloats(msg.getSegment("AngularVel").Get("AngularVel"), ",");
            float[] curAngularAccel = USARParser.parseFloats(msg.getSegment("AngularAccel").Get("AngularAccel"), ",");
            float[] curRotation = USARParser.parseFloats(msg.getSegment("Rotation").Get("Rotation"), ",");

            XYZAcceleration = new Vector3(curXYZAcceleration[0], curXYZAcceleration[1], curXYZAcceleration[2]);
            AngularVel = new Vector3(curAngularVel[0], curAngularVel[1], curAngularVel[2]);
            AngularAccel = new Vector3(curAngularAccel[0], curAngularAccel[1], curAngularAccel[2]);
            Rotation = new Vector3(curRotation[0], curRotation[1], curRotation[2]);
        }

        public override string ToString()
        {
            string cmd = "\nIMU :\nXYZAcceleration : " + XYZAcceleration.ToString() +
                         "\nAngularVel : " + AngularVel.ToString() +
                         "\nAngularAccel : " + AngularAccel.ToString() +
                         "\nRatation : " + Rotation.ToString() + "\n";

            return cmd;
        }
    }
}
