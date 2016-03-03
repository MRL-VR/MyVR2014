using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Utils;
using SlimDX.DirectInput;
using System.IO;

namespace MRL.Communication.Internal_Objects
{
    public class PressedKeys : BaseInternalObject
    {
        public List<Key> keysList = new List<Key>();
        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.PressedKeys; }
        }

        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);
                    int count = keysList.Count;
                    mWriter.Write(count);
                    foreach (var item in keysList)
                    {
                        mWriter.Write((int)item);
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
                    mReader.ReadByte();
                    int listCount = mReader.ReadInt32();
                    for (int i = 0; i < listCount; i++)
                        keysList.Add((Key)mReader.ReadInt32());
                }
            }
        }
    }
}
