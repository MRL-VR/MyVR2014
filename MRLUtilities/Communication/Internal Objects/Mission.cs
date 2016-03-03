using System;
using System.Collections.Generic;
using System.IO;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{
    [Serializable()]
    public class Mission : BaseInternalObject
    {
        public List<Pose2D> pList = new List<Pose2D>();
        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.Mission; }
        }
        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);
                    mWriter.Write(pList.Count);
                    foreach (var item in pList)
                    {
                        mWriter.Write(item.X);
                        mWriter.Write(item.Y);
                        mWriter.Write(item.Rotation);
                    }

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
                    pList = new List<Pose2D>();
                    pList.Clear();

                    double _x, _y, _z;

                    mReader.ReadByte();
                    int listCount = mReader.ReadInt32();
                    for (int i = 0; i < listCount; i++)
                    {
                        _x = mReader.ReadDouble();
                        _y = mReader.ReadDouble();
                        _z = mReader.ReadDouble();
                        pList.Add(new Pose2D(_x, _y, _z));
                    }
                }
            }
        }
    }
}
