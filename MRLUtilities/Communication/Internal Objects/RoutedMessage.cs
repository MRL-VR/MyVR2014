
using System.IO;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{
    public class RoutedMessage : BaseInternalObject
    {
        public BaseInternalObject packetData;
        public string senderName;
        public string receiverName;
        public int receiverPort;

        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.RoutedMessage; }
        }
        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);

                    mWriter.Write(senderName);
                    mWriter.Write(receiverName);
                    mWriter.Write(receiverPort);

                    byte[] dataBytes = packetData.Serialize();
                    mWriter.Write(dataBytes.Length);
                    mWriter.Write(dataBytes);
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
                    senderName = mReader.ReadString();
                    receiverName = mReader.ReadString();
                    receiverPort = mReader.ReadInt32();
                    int bytesLength = mReader.ReadInt32();
                    byte[] readBytes = mReader.ReadBytes(bytesLength);
                    packetData = BaseInternalObject.CreateInternalObject(readBytes);
                }
            }
        }
    }

}
