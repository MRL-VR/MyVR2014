using System.IO;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{
    public class Battery : BaseInternalObject
    {
        public float Life;
        
        public Battery()
        {

        }
        public Battery(float life)
        {
            this.Life = life;
        }
        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.Battery; }
        }
        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);
                    mWriter.Write(Life);
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
                    Life = mReader.ReadSingle();
                }
            }
        }
    }
}
