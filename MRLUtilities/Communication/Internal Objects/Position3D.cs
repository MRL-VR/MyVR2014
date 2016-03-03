
using System.IO;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{
    public class Position3D : BaseInternalObject
    {
        public Position3D()
        {

        }
        public Position3D(Pose3D position)
        {
            this.Position = position;
        }

        public Pose3D Position;

        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.Position3D; }
        }
        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);

                    mWriter.Write(Position.X);
                    mWriter.Write(Position.Y);
                    mWriter.Write(Position.Z);

                    mWriter.Write(Position.Rotation.X);
                    mWriter.Write(Position.Rotation.Y);
                    mWriter.Write(Position.Rotation.Z);
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
                    double _z = mReader.ReadDouble();
                    double _xR = mReader.ReadDouble();
                    double _yR = mReader.ReadDouble();
                    double _zR = mReader.ReadDouble();
                    Position = new Pose3D(new Vector3(_x, _y, _z),
                                          new Vector3(_xR,_yR, _zR));
                }
            }
        }
    }
}
