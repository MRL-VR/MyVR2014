using System.IO;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{
    public class AutonomousChange : BaseInternalObject
    {
        public string Tag;
        public override byte MessageID
        { get { return (byte)InternalMessagesID.AutonomousChange; } }
        
        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);
                    mWriter.Write(Tag);
                }
                return mStream.ToArray();
            }
        }
        public override void Deserialize(byte[] buffer)
        {
            using (MemoryStream mStream = new MemoryStream(buffer))
            {
                using (BinaryReader mReader = new BinaryReader(mStream))
                {
                    mReader.ReadByte();
                    Tag = mReader.ReadString();
                }
            }
        }
    }
}
