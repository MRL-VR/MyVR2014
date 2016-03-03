using System.Collections.Generic;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Communication.External_Commands
{

    public class VictSensor
    {
        public double prob = -1;
        public string partName = "";
        public Vector3 p3Loc = new Vector3();

        public VictSensor(string part, Vector3 loc)
        {
            partName = part;
            p3Loc = new Vector3(loc);
            prob = -1;
        }
    }

    public class VictRFID
    {
        //SEN {Time 3370.4606} {Type VictSensor} {Status NoVictims}
        //SEN {Type VictSensor} {Name VictimSensor} {PartName Leg} {Location 2.05,0.33,0.46} {PartName Leg} {Location 2.51,0.44,0.39}
        public List<VictSensor> victParts = new List<VictSensor>();
        public bool hasStatus;

        public Vector3 meanPos;
        public double meanProb;

        public VictRFID(VictRFID v)
        {
            if (v == null) return;

            foreach (VictSensor s in v.victParts)
            {
                victParts.Add(s);
            }

            this.hasStatus = v.hasStatus;
            this.meanPos = new Vector3(v.meanPos);
            this.meanProb = v.meanProb;
        }

        public VictRFID(USARParser msg)
        {
            if (msg.size == 0 || msg.segments == null) return;
            ParseState(msg);
        }

        private void ParseState(USARParser msg)
        {
            hasStatus = (msg.getSegment("Status") != null);

            int shift = (msg.getSegment("Time") == null) ? 2 : 3;

            try
            {
                for (int i = shift; i < msg.segments.Length; i += 2)
                {
                    string name = msg.segments[i].Get("PartName");
                    float[] curLocation = USARParser.parseFloats(msg.segments[i + 1].Get("Location"), ",");

                    victParts.Add(new VictSensor(name, new Vector3(curLocation[0], curLocation[1], curLocation[2])));
                }
            }
            catch
            {

            }
        }

        public override string ToString()
        {
            return "Victim <" + meanPos.ToString() + ">(" + meanProb.ToString() + ")";
        }
    }

}
