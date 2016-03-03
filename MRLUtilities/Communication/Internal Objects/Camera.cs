using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{
    public class Camera : BaseInternalObject
    {
        public int sequenceNumber;
        public List<byte[]> Images;
        public bool isMultiView;

        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.CameraImage; }
        }
        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);
                    mWriter.Write(sequenceNumber);
                    mWriter.Write(isMultiView);
                    mWriter.Write(Images.Count);
                    foreach (byte[] item in Images)
                    {
                        mWriter.Write(item.Length);
                        mWriter.Write(item);
                    }
                }
                return mStream.ToArray();
            }
        }
        public override void Deserialize(byte[] buffer)
        {
            using (MemoryStream mStream = new MemoryStream(buffer))
            {
                Images = new List<byte[]>();
                using (BinaryReader mReader = new BinaryReader(mStream))
                {
                    mReader.ReadByte();
                    sequenceNumber = mReader.ReadInt32();
                    isMultiView = mReader.ReadBoolean();
                    int listCount = mReader.ReadInt32();
                    for (int i = 0; i < listCount; i++)
                    {
                        int bytesLength = mReader.ReadInt32();
                        byte[] roadBytes = mReader.ReadBytes(bytesLength);
                        Images.Add(roadBytes);
                    }
                }
            }
        }

        public Bitmap[] GetImages()
        {
            return Images.Select(x => x.Length == 0 ? null : new Bitmap(new MemoryStream(x))).ToArray(); ;
        }
    }
}
