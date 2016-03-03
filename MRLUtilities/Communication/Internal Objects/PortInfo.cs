using System.IO;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{
    public class PortInfo : BaseInternalObject
    {
        public int ImagePort;

        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.PortInfo; }
        }
        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);
                    mWriter.Write(ImagePort);
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
                    ImagePort = mReader.ReadInt32();
                }
            }
        }
    }
}
