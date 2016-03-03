using System.Collections.Specialized;

using MRL.Utils;

namespace MRL.Communication.External_Commands
{

    public class State
    {
        //STA {Time 1553.55} {FrontSteer float} {RearSteer float} {SternPlaneAngle float}
        //    {RudderAngle float} {LightToggle False} {LightIntensity 0} {Battery 3597}
        public float fTime = 0.0f;
        public float fFrontSteer = 0.0f;
        public float fRearSteer = 0.0f;
        public float fSternPlaneAngle = 0.0f;
        public float fRudderAngle = 0.0f;
        public bool bLight = false;
        public int iLightIntensity = 0;
        public int iBatteryLife = 0;

        public State(State t)
        {
            if (t == null) return;

            this.fTime = t.fTime;
            this.fFrontSteer = t.fFrontSteer;
            this.fRearSteer = t.fRearSteer;
            this.fSternPlaneAngle = t.fSternPlaneAngle;
            this.fRudderAngle = t.fRudderAngle;
            this.bLight = t.bLight;
            this.iLightIntensity = t.iLightIntensity;
            this.iBatteryLife = t.iBatteryLife;
        }

        public State(USARParser msg)
        {
            if (msg.size == 0 || msg.segments == null) return;
            ParseState(msg);
        }

        private void ParseState(USARParser msg)
        {
            fTime = float.Parse(msg.getSegment("Time").Get("Time"));

            NameValueCollection segTemp = null;

            segTemp = msg.getSegment("FrontSteer");
            if (segTemp != null) fFrontSteer = float.Parse(segTemp.Get("FrontSteer"));

            segTemp = msg.getSegment("RearSteer");
            if (segTemp != null) fRearSteer = float.Parse(segTemp.Get("RearSteer"));

            segTemp = msg.getSegment("SternPlaneAngle");
            if (segTemp != null) fSternPlaneAngle = float.Parse(segTemp.Get("SternPlaneAngle"));

            segTemp = msg.getSegment("RudderAngle");
            if (segTemp != null) fRudderAngle = float.Parse(segTemp.Get("RudderAngle"));

            segTemp = msg.getSegment("LightToggle");
            if (segTemp != null) bLight = bool.Parse(segTemp.Get("LightToggle"));

            segTemp = msg.getSegment("LightIntensity");
            if (segTemp != null) iLightIntensity = int.Parse(segTemp.Get("LightIntensity"));

            segTemp = msg.getSegment("Battery");
            if (segTemp != null) iBatteryLife = int.Parse(segTemp.Get("Battery"));
        }
    }

}
