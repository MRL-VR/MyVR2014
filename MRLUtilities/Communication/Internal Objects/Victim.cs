
using System.IO;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{

    public class Victim : BaseInternalObject
    {
        public Pose2D VictimPose;
        public float VictimProb;

        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.VictimRFID; }
        }

        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);

                    mWriter.Write(VictimPose.X);
                    mWriter.Write(VictimPose.Y);

                    mWriter.Write(VictimPose.Rotation);

                    mWriter.Write(VictimProb);
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
                    double _x = mReader.ReadDouble();
                    double _y = mReader.ReadDouble();
                    double _t = mReader.ReadDouble();
                    VictimPose = new Pose2D(_x,_y,_t);
                    VictimProb = mReader.ReadSingle();
                }
            }

        }
    }

}
