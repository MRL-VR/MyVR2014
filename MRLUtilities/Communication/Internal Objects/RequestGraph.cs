using System.IO;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{

    public class RequestGraph : BaseInternalObject
    {

        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.RequestDVGraph; }
        }

        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);
                }

                return mStream.ToArray();
            }
        }

        public override void Deserialize(byte[] buffer)
        {

        }

    }
}
