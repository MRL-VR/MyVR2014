using System.Collections.Generic;
using System.Collections.Specialized;

using MRL.Utils;

namespace MRL.Communication.External_Commands
{

    public struct EncoderSensor
    {
        public string ecName;
        public int ecValue;

        public EncoderSensor(string name, int value)
        {
            ecName = name;
            ecValue = value;
        }
    }

    public class Encoder
    {
        //SEN {Type Encoder} {Name ECLeft Tick -24} {Name ECRight Tick -23}
        public List<EncoderSensor> sensors = new List<EncoderSensor>();

        public Encoder(USARParser msg)
        {
            if (msg.size == 0 || msg.segments == null) return;
            ParseState(msg);
        }

        public int getEncoderValue(string senName)
        {
            foreach (EncoderSensor es in sensors)
            {
                if (es.ecName.Equals(senName))
                {
                    return es.ecValue;
                }
            }
            return -1;
        }

        private void ParseState(USARParser msg)
        {
            for (int i = 0; i < msg.size; i++)
            {
                NameValueCollection segTemp = msg.getSegment(i);
                if (segTemp.AllKeys.Length <= 1) continue;

                sensors.Add(new EncoderSensor(segTemp.Get(0), int.Parse(segTemp.Get(1))));
            }
        }

    }

}
