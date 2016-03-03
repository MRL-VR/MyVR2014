using System.Collections.Generic;
using System.IO;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{
    public class ComGraphLink
    {
        public string destRobotName;
        public string interfaceRobot;
        public bool signalState;
        public int hob;
        
        public ComGraphLink()
        {

        }

        public ComGraphLink(string destRobotName, string interfaceRobot, bool signalState, int hob)
        {
            this.destRobotName = destRobotName;
            this.interfaceRobot = interfaceRobot;
            this.signalState = signalState;
            this.hob = hob;
        }
    }

    public class CommGraph : BaseInternalObject
    {
        public string startName;
        public Pose2D startPose;

        public List<ComGraphLink> links = new List<ComGraphLink>();

        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.CommunicationGraph; }
        }

        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);
                    mWriter.Write(startName);

                    //write the Pose2D on Memory Stream
                    mWriter.Write(startPose.X);
                    mWriter.Write(startPose.Y);
                    mWriter.Write(startPose.Rotation);

                    //write the links list on Memory Stream
                    mWriter.Write(links.Count);
                    foreach (ComGraphLink item in links)
                    {
                        mWriter.Write(item.destRobotName);
                        mWriter.Write(item.signalState);
                        mWriter.Write(item.interfaceRobot);
                        mWriter.Write(item.hob);
                    }

                }
                return mStream.ToArray();
            }

        }
        public override void Deserialize(byte[] buffer)
        {
            using (MemoryStream mStream = new MemoryStream(buffer))
            {
                links = new List<ComGraphLink>();
                using (BinaryReader mReader = new BinaryReader(mStream))
                {
                    mReader.ReadByte();
                    startName = mReader.ReadString();

                    double _x = mReader.ReadDouble();
                    double _y = mReader.ReadDouble();
                    double _t = mReader.ReadDouble();
                    startPose = new Pose2D(_x, _y, _t);

                    int listCount = mReader.ReadInt32();
                    for (int i = 0; i < listCount; i++)
                    {
                        string _dest = mReader.ReadString();
                        bool _state = mReader.ReadBoolean();
                        string _interface = mReader.ReadString();
                        int _hob = mReader.ReadInt32();

                        links.Add(new ComGraphLink(_dest, _interface, _state, _hob));
                    }
                }
            }
        }
        public override string ToString()
        {
            string tableStr = "\r\nFT " + this.startName + "\r\n";
            tableStr += "D\t\tS\r\n";
            tableStr += "-----------------------------------------------------------\r\n";
            foreach (var item in links)
                tableStr += string.Format("{0}\t\t{1}\t\t{2}\t\t{3}\r\n", item.destRobotName, item.signalState, item.interfaceRobot, item.hob);
            tableStr += "-----------------------------------------------------------\r\n";

            return tableStr;
        }
    }
}
