using System.Collections.Generic;
using System.Linq;
using MRL.Communication.External_Commands;
using System;
using System.Xml.Serialization;

namespace MRL.Utils
{
    [Serializable]
    public class SonarManager
    {
        [XmlIgnore]
        public Dictionary<string, SonarSegment> segments = new Dictionary<string, SonarSegment>();
        public string[] names = new string[0];

        public List<SegmentsSerializable> LSS;
        public void AfterLoadFromXml()
        {
            segments = LSS.ToDictionary(x => x.key, y => y.value);
        }
        public void BeforSaveToXml()
        {
            LSS = segments.Select(x => new SegmentsSerializable() { key = x.Key, value = x.Value }).ToList();
        }

        public SonarManager()
        {

        }

        public List<SonarSegment> GetSonarList()
        {
            lock (this)
            {
                return names.Select(x => segments[x]).ToList();
            }
        }

        public void Update(Sonar newSonarData)
        {
            lock (this)
            {
                int cnt = segments.Count;
                foreach (var item in newSonarData.fSonars)
                    segments[item.sName] = item;

                if (segments.Count != cnt)
                    names = segments.Keys.OrderBy(s => s).ToArray();
            }
        }
    }

    [Serializable]
    public class SegmentsSerializable
    {
        public string key;
        public SonarSegment value;
    }
}
