using System.IO;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{
    public class ImageServer : BaseInternalObject
    {
        public bool Status;
        public int PortNumber;

        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.ImageServer; }
        }

        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);
                    mWriter.Write(Status);
                    mWriter.Write(PortNumber);
                }
                return mStream.ToArray();
            }
        }
        public override void Deserialize(byte[] buffer)
        {
            using (MemoryStream mStream = new MemoryStream(buffer))
                using (BinaryReader mReader = new BinaryReader(mStream))
                {
                    mReader.ReadByte();
                    Status = mReader.ReadBoolean();
                    PortNumber = mReader.ReadInt32();
                }
        }
    }
}
