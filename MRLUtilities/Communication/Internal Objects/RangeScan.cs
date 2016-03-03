using System;
using System.IO;
using MRL.Commons;
using MRL.Communication.External_Commands;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{

    [Serializable()]
    public class RangeScan : BaseInternalObject, ILaserRangeData
    {
        public float fResolution = 0.0f;
        public float fFov = 0.0f;

        public Pose3D SensorGlobalPose3D;

        public Pose2D SensorGlobalPose;
        public Pose2D RobotGlobalPose;
        public double[] fRanges;

        public double[] fTheta;
        public bool[] bFilters;
        public float time;

        public RangeScan()
        {

        }

        public RangeScan(ILaserRangeData exlaser)
        {
            SetData(exlaser);
        }

        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.RangeScan; }
        }

        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);
                    mWriter.Write(time);
                    mWriter.Write(fResolution);
                    mWriter.Write(fFov);

                    mWriter.Write(SensorGlobalPose3D.X);
                    mWriter.Write(SensorGlobalPose3D.Y);
                    mWriter.Write(SensorGlobalPose3D.Z);

                    mWriter.Write(SensorGlobalPose3D.Rotation.X);
                    mWriter.Write(SensorGlobalPose3D.Rotation.Y);
                    mWriter.Write(SensorGlobalPose3D.Rotation.Z);

                    mWriter.Write(SensorGlobalPose.X);
                    mWriter.Write(SensorGlobalPose.Y);
                    mWriter.Write(SensorGlobalPose.Rotation);

                    mWriter.Write(RobotGlobalPose.X);
                    mWriter.Write(RobotGlobalPose.Y);
                    mWriter.Write(RobotGlobalPose.Rotation);

                    mWriter.Write(fRanges.Length);
                    foreach (double item in fRanges)
                        mWriter.Write(item);
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
                    time = mReader.ReadSingle();
                    fResolution = mReader.ReadSingle();
                    fFov = mReader.ReadSingle();

                    double _x = mReader.ReadDouble();
                    double _y = mReader.ReadDouble();
                    double _z = mReader.ReadDouble();
                    double _xR = mReader.ReadDouble();
                    double _yR = mReader.ReadDouble();
                    double _zR = mReader.ReadDouble();
                    SensorGlobalPose3D = new Pose3D(new Vector3(_x, _y, _z),
                                          new Vector3(_xR, _yR, _zR));

                    double _x1 = mReader.ReadDouble();
                    double _y1 = mReader.ReadDouble();
                    double _t1 = mReader.ReadDouble();
                    SensorGlobalPose = new Pose2D(_x1, _y1, _t1);

                    double _x2 = mReader.ReadDouble();
                    double _y2 = mReader.ReadDouble();
                    double _t2 = mReader.ReadDouble();
                    RobotGlobalPose = new Pose2D(_x2, _y2, _t2);

                    int bytesLength = mReader.ReadInt32();

                    double[] readFRanges = new double[bytesLength];

                    for (int i = 0; i < bytesLength; i++)
                        readFRanges[i] = mReader.ReadDouble();

                    fRanges = readFRanges;
                }
            }
        }

        private void SetData(ILaserRangeData exlaser)
        {
            fResolution = (float)exlaser.Resolution;
            fFov = (float)exlaser.FieldOfView;
            SensorGlobalPose3D = exlaser.SensorOffset3D;
            SensorGlobalPose = exlaser.SensorOffset;
            RobotGlobalPose = exlaser.RobotOffset;
            fRanges = exlaser.Range;
            fTheta = exlaser.RangeTheta;
            bFilters = exlaser.RangeFilters;
            time = exlaser.Time;
        }

        #region ILaserRangeData Members

        float ILaserRangeData.MaxRange
        {
            get { return Laser.max_range; }
        }

        float ILaserRangeData.MinRange
        {
            get { return Laser.min_range; }
        }

        public double Resolution
        {
            get { return fResolution; }
        }

        public double FieldOfView
        {
            get { return fFov; }
        }

        public Pose2D SensorOffset
        {
            get { return SensorGlobalPose; }
        }

        public Pose3D SensorOffset3D
        {
            get { return SensorGlobalPose3D; }
        }

        public Pose2D RobotOffset
        {
            get { return RobotGlobalPose; }
        }

        public double[] Range
        {
            get { return fRanges; }
        }

        public double[] RangeTheta
        {
            get { return fTheta; }
        }

        public bool[] RangeFilters
        {
            get { return bFilters; }
        }
        public float Time
        {
            get { return time; }
        }

        #endregion

    }
}
