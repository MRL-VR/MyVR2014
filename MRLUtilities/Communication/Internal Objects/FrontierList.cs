using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Exploration.Frontiers;
using MRL.Utils;
using System.IO;
using MRL.CustomMath;

namespace MRL.Communication.Internal_Objects
{
    public class FrontierList:BaseInternalObject
    {
        public List<Frontier> FrontiersList = new List<Frontier>();

        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.FrontierList; }
        }

        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);

                    mWriter.Write(FrontiersList.Count);
                    foreach (var item in FrontiersList)
                    {
                        mWriter.Write(item.StartRange);
                        mWriter.Write(item.EndRange);
                        mWriter.Write(item.FrontierPosition.X);

                        mWriter.Write(item.FrontierPosition.Y);
                        mWriter.Write(item.FrontierPosition.Rotation);
                        mWriter.Write(item.Distance);
                        mWriter.Write(item.Widthness);
                        mWriter.Write(item.Weight);    
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
                    int count = mReader.ReadInt32();

                    for (int i = 0; i < count; i++)
                    {
                        Frontier frontier = new Frontier();
                        frontier.StartRange = mReader.ReadInt32();
                        frontier.EndRange = mReader.ReadInt32();
                        double _x = mReader.ReadDouble();
                        double _y = mReader.ReadDouble();
                        double _rot = mReader.ReadDouble();
                        frontier.FrontierPosition = new Pose2D(_x, _y, _rot);
                        frontier.Distance = mReader.ReadDouble();
                        frontier.Weight = mReader.ReadDouble();
                        FrontiersList.Add(frontier);
                    }
                }
            }
        }
    }
}
