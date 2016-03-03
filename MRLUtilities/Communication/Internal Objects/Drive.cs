using System.IO;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{
    public class Drive : BaseInternalObject
    {
        public Drive() { }
        public Drive(string cmd)
        { this.Command = cmd; }
        public string Command;

        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.Drive; }
        }

        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);
                    mWriter.Write(Command);
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
                    Command = mReader.ReadString();
                }
            }
        }
    }
}
