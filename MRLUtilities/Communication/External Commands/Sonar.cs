using System;
using System.Collections.Specialized;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Communication.External_Commands
{
    [Serializable]
    public struct SonarSegment
    {
        public float dir;
        public Pose2D pos;   //carmen frame
        public Pose2D posUT; //UT frame
        public float fRange;
        public string sName;

        public static implicit operator float(SonarSegment t)
        {
            return t.fRange;
        }

        public static implicit operator string(SonarSegment t)
        {
            return t.sName;
        }

        public SonarSegment(string name, float range)
        {
            sName = name;
            fRange = range;
            dir = 0f;
            pos = new Pose2D();
            posUT = new Pose2D();
        }
    }

    public struct SonarConfig
    {
        public Vector3 posInRobot;
        public float direction;

        public SonarConfig(Vector3 pos, float dir)
        {
            posInRobot = new Vector3(pos);
            direction = dir;
        }
    }

    public class Sonar : ICloneable
    {
        //SEN {Time 1816.8206} {Type Sonar} {Name F1 Range 1.5560} {Name F2 Range 1.6349} {Name F3 Range 1.5453} {Name F4 Range 1.6907} {Name F5 Range 0.5629} {Name F6 Range 0.1550}
        //SEN {Time 26.35} {Type Sonar} {Name F1 Range 4.5645} {Name F2 Range 2.5976} {Name F3 Range 1.9675} {Name F4 Range 1.7088} {Name F5 Range 0.7870} {Name F6 Range 0.8180} {Name F7 Range 1.1635} {Name F8 Range 2.0633}
        public SonarSegment[] fSonars;
        public float fTime = -1.0f;

        public static implicit operator float[](Sonar p)
        {
            float[] ranges = new float[p.fSonars.Length - 1];
            for (int i = 0; i < p.fSonars.Length; i++)
            {
                ranges[i] = (float)p.fSonars[i];
            }
            return ranges;
        }

        public static implicit operator string[](Sonar p)
        {
            string[] names = new string[p.fSonars.Length - 1];

            for (int i = 0; i < p.fSonars.Length; i++)
            {
                names[i] = (string)p.fSonars[i];
            }

            return names;
        }

        public Sonar(Sonar t)
        {
            if (t == null) return;

            this.fSonars = new SonarSegment[t.fSonars.Length - 1];
            for (int i = 0; i < this.fSonars.Length; i++)
            {
                this.fSonars[i] = new SonarSegment(t.fSonars[i].sName, t.fSonars[i].fRange);
            }
            this.fTime = t.fTime;
        }

        public Sonar(USARParser msg)
        {
            if (msg.size == 0 || msg.segments == null) return;
            ParseState(msg);
        }

        private void ParseState(USARParser msg)
        {
            int shift;
            NameValueCollection segTime = msg.getSegment("Time");
            
            if (segTime == null)
            {
                shift = 1;
                fTime = -1f;
            }
            else
            {
                shift = 2;
                fTime = float.Parse(segTime.Get("Time"));
            }

            string name = msg.getSegment("Name").Get("Name");
            float range = float.Parse(msg.getSegment("Range").Get("Range"));

            fSonars = new SonarSegment[1];
            fSonars[0] = new SonarSegment(name, range);

            //try
            //{
            //    for (int i = 0; i < msg.segments.Length; i++)
            //    {
            //        string name = msg.segments[i].Get("Name");
            //        float range = float.Parse(msg.segments[i].Get("Range"));
            //        fSonars[i - shift] = new SonarSegment(name, range);
            //    }
            //}
            //catch(Exception ex)
            //{
            //    ProjectCommons.writeConsoleMessage("Parsing " +ex.ToString(), ConsoleMessageType.Information);
            //}

        }

        /*   private void ParseStatewithwrite(USARParser msg)
           {
               int shift;
               NameValueCollection segTime = msg.getSegment("Time");

               if (segTime == null)
               {
                   shift = 1;
                   fTime = -1f;
               }
               else
               {
                   shift = 2;
                   fTime = float.Parse(segTime.Get("Time"));
               }

               List<SonarSegment> lst = new List<SonarSegment>();
               String s = "";

               for (int i = shift; i < msg.segments.Length; i += 2)
               {
                   string name = msg.segments[i].Get("Name");
                   float range = float.Parse(msg.segments[i + 1].Get("Range"));
                   lst.Add(new SonarSegment(name, range));
                   s += string.Format("{0}: {1}; ", name, range);
               }
               fSonars = lst.ToArray();
               Console.WriteLine(s);
           }*/

        public object Clone()
        {
            Sonar s = new Sonar(this);
            return s;
        }

        public int FindIndexByName(string sensorName)
        {
            for (int i = 0; i < fSonars.Length; i++)
            {
                if (fSonars[i].sName.Equals(sensorName))
                    return i;
            }

            return -1;
        }
    }

}
