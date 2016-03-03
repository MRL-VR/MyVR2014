using System;
using MRL.Commons;
using MRL.Communication.Internal_Objects;
using MRL.CustomMath;
using MRL.IDE.Base;
using MRL.Utils;

namespace MRL.Communication.External_Commands
{
    [Serializable]
    public class Laser : ICloneable, ILaserRangeData
    {
        public Laser()
        {

        }
        public static float min_range = 0.0f;
        public static float max_range = 17f;
        public static Pose2D laserPosOnRobot = new Pose2D(0.2085, 0, -0.2);

        //SEN {Time 2202.98} {Type RangeScanner} {Name Scanner1} {Resolution 0.0174} {FOV 3.1415} {Range 2.1951,2.1962,2.1956,2.1956,2.1993,2.2015,2.2067,2.2097,2.2144,2.2203,2.2294,2.2334,2.2413,2.2541,2.2613,2.2699,2.2846,2.2954,2.3065,2.3206,2.3368,2.3494,2.3688,2.3849,2.4041,2.4215,2.4408,2.4648,2.4878,2.5092,2.5368,2.5589,2.5872,2.6163,2.6484,2.6780,1.4325,1.4145,1.3820,1.3527,1.3227,1.2958,1.2705,1.2473,1.2244,1.2025,1.1810,1.1626,1.1425,1.1242,1.1087,1.0923,1.0774,1.0629,1.0485,1.0343,1.0219,1.0097,0.9982,0.9879,0.9782,0.9684,0.9586,0.9509,0.9417,0.9334,0.9259,0.9187,0.9130,0.9070,0.9007,0.8949,0.8889,0.8851,0.8797,0.8752,0.8715,0.8681,0.8554,0.8513,0.9473,1.0547,1.1911,1.3619,1.6718,1.8432,1.8444,1.8429,1.8407,1.8433,1.8417,1.8410,1.8438,1.8451,1.8445,1.8496,1.8535,1.8543,1.8614,1.8627,1.8689,1.8747,1.8845,1.8907,1.8967,1.9074,1.9146,1.9261,1.9387,1.9499,1.9607,1.9737,1.9861,2.0013,2.0177,2.0338,2.0519,2.0693,2.0870,2.1078,2.1294,2.1485,2.1733,2.1978,2.2253,2.2507,2.2809,2.3089,2.3404,2.2683,2.3286,2.3899,2.4588,2.5207,2.5459,2.6038,2.6365,2.6646,2.7001,1.6763,1.6999,1.7366,1.7747,1.8151,1.8604,1.9063,1.9553,2.0100,2.0627,2.1237,2.1878,2.2837,2.3634,2.4398,2.4830,4.3756,4.5520,4.7475,4.9563,4.9250,4.7569,4.6105,4.4652,4.3296,4.2058,4.0943,3.9799,4.8007,4.7817,4.7704,4.7562,4.7421,4.7276,4.7165,4.7069,4.6971,4.6928,4.6877,4.6875,4.6913,4.6849}
        public double[] fTheta;
        public double[] fRanges;
        public bool[] bFilters;

        public string fName = "";
        public float fResolution = 0.0f;
        public float fFov = 0.0f;
        public float fTime = -1.0f;
        public float fAng0 = 0.0f;

        /// <summary>
        /// laser position in global frame
        /// </summary>
        public Pose2D SensorGlobalPose = new Pose2D(); //global frame
        public Pose3D SensorGlobalPose3D = new Pose3D(); //global frame

        /// <summary>
        /// robot position in global frame
        /// </summary>
        public Pose2D RobotGlobalPose = new Pose2D(); //global frame

        private string validateString = "";
        private string thetaString = "";
        private string rangesString = "";
        private static Pose2D posRangeScanner = new Pose2D();

        public Laser(Laser t)
        {
            if (t == null) return;

            fRanges = (double[])t.fRanges.Clone();
            fTheta = (double[])t.fTheta.Clone();
            bFilters = (bool[])t.bFilters.Clone();

            this.rangesString = t.rangesString;
            this.fName = t.fName;
            this.fAng0 = t.fAng0;
            this.fTime = t.fTime;
            this.fFov = t.fFov;
            this.fResolution = t.fResolution;
            this.SensorGlobalPose = new Pose2D(t.SensorGlobalPose);
            this.RobotGlobalPose = new Pose2D(t.RobotGlobalPose);
        }

        public Laser(RangeScan rs)
        {
            this.bFilters = rs.bFilters;
            this.fFov = rs.fFov;
            this.fRanges = rs.fRanges;
            this.fResolution = rs.fResolution;
            this.fTheta = rs.fTheta;
            this.fTime = rs.Time;
            this.RobotGlobalPose = rs.RobotGlobalPose;
            this.SensorGlobalPose = rs.SensorGlobalPose;
            this.SensorGlobalPose3D = rs.SensorGlobalPose3D;
        }

        public Laser(USARParser msg)
        {
            if (msg.size == 0 || msg.segments == null) return;
            ParseState(msg);
        }

        private void ParseState(USARParser msg)
        {
            fName = msg.getSegment("Name").Get("Name");
            fFov = float.Parse(msg.getSegment("FOV").Get("FOV"));
            fResolution = float.Parse(msg.getSegment("Resolution").Get("Resolution"));

            rangesString = msg.getString("Range");
            fRanges = USARParser.parseDoubles(rangesString, ",");

            validateString = msg.getString("Valid");
            if (!string.IsNullOrEmpty(validateString))
            {
                bFilters = USARParser.parseBools(validateString, ",", true);
            }
            else
            {
                bFilters = new bool[fRanges.Length];
                int c = fRanges.Length;
                for (int i = 0; i < c; i++)
                    bFilters[i] = fRanges[i] < min_range || fRanges[i] > max_range;
            }

            if (!string.IsNullOrEmpty(thetaString))
                fAng0 = (float)fTheta[0];
            else
                fAng0 = ((float)System.Math.PI / 2 - fFov / 2);

            thetaString = msg.getString("Theta");
            if (!string.IsNullOrEmpty(thetaString))
            {
                fTheta = USARParser.parseDoubles(thetaString, ",");
            }
            else
            {
                fTheta = new double[fRanges.Length];
                int c = fRanges.Length;
                for (int i = 0; i < c; i++)
                    fTheta[i] = fAng0 + i * fResolution;
            }

            if (msg.getSegment("Time") != null)
                fTime = float.Parse(msg.getSegment("Time").Get("Time"));

        }

        public Pose2D GetBeamPos(Pose2D robotPos, int Beam)
        {
            double rotation = robotPos.GetNormalizedRotation();

            int count = this.fRanges.Length;

            float bAngle = 0;
            int i = Beam;

            if (Beam > count) return new Pose2D();

            Pose2D laserBeamPose = new Pose2D(0, 0, 0);
            Pose2D rawPose = new Pose2D(0, 0, 0);
            Pose2D centerPose = new Pose2D(0, 0, 0);

            bAngle = -0.0175f * (i - 90);

            rawPose.X = this.fRanges[i] * Math.Cos(bAngle);
            rawPose.Y = this.fRanges[i] * Math.Sin(bAngle);

            centerPose.X = rawPose.X + laserPosOnRobot.X;
            centerPose.Y = rawPose.Y + laserPosOnRobot.Y;

            laserBeamPose.X = robotPos.X + centerPose.X * Math.Cos(rotation) + centerPose.Y * (-Math.Sin(rotation));
            laserBeamPose.Y = robotPos.Y + centerPose.X * Math.Sin(rotation) + centerPose.Y * (Math.Cos(rotation));
            return laserBeamPose;

        }
        /// <summary>
        /// Robot Position += Size of Delta
        /// </summary>
        /// <param name="robotPos"></param>
        /// <param name="Beam"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public Pose2D GetBeamPos(Pose2D robotPos, int Beam, double delta)
        {
            double rotation = robotPos.GetNormalizedRotation();

            int count = this.fRanges.Length;

            float bAngle = 0;
            int i = Beam;

            if (Beam > count) return new Pose2D();

            Pose2D laserBeamPose = new Pose2D(0, 0, 0);
            Pose2D rawPose = new Pose2D(0, 0, 0);
            Pose2D centerPose = new Pose2D(0, 0, 0);

            bAngle = -0.0175f * (i - 90);

            rawPose.X = (this.fRanges[i] + delta) * Math.Cos(bAngle);
            rawPose.Y = (this.fRanges[i] + delta) * Math.Sin(bAngle);

            centerPose.X = rawPose.X + laserPosOnRobot.X;
            centerPose.Y = rawPose.Y + laserPosOnRobot.Y;

            laserBeamPose.X = robotPos.X + centerPose.X * Math.Cos(rotation) + centerPose.Y * (-Math.Sin(rotation));
            laserBeamPose.Y = robotPos.Y + centerPose.X * Math.Sin(rotation) + centerPose.Y * (Math.Cos(rotation));
            return laserBeamPose;

        }
        public static Pose2D GetLaserPosOnRobot(Pose2D robotPos)
        {
            double rotation = robotPos.GetNormalizedRotation();

            Pose2D laserBeamPose = new Pose2D(0, 0, 0);
            Pose2D centerPose = new Pose2D(0, 0, 0);

            centerPose.X = laserPosOnRobot.X;
            centerPose.Y = laserPosOnRobot.Y;

            laserBeamPose.X = robotPos.X + centerPose.X * Math.Cos(rotation) + centerPose.Y * (-Math.Sin(rotation));
            laserBeamPose.Y = robotPos.Y + centerPose.X * Math.Sin(rotation) + centerPose.Y * (Math.Cos(rotation));
            return laserBeamPose;
        }

        public object Clone()
        {
            Laser obj = new Laser(this);
            return obj;
        }
        public static Pose3D getLaserVector(RangeScan l, Pose3D robotPos, Pose3D robotOri, int iIndex)
        {
            double z = robotPos.Z;
            double robotRoll = robotOri.Z;
            {
                float fTheta = 0;
                fTheta = -0.0175f * (iIndex - 90);

                Pose3D vLaser;
                double fVecLength = l.fRanges[iIndex];

                vLaser = new Pose3D(fVecLength * (double)Math.Cos(fTheta),
                                     fVecLength * (double)Math.Sin(fTheta), z, 0f, 0f, 0f);

                Pose3D vF = new Pose3D(-posRangeScanner.X, -posRangeScanner.Y, z, 0f, 0f, 0f);
                Pose3D vSub = new Pose3D(vLaser.X - vF.X, vLaser.Y - vF.Y, z, 0f, 0f, 0f);
                Pose3D vSubF = new Pose3D(0f, 0f, 0f, 0f, 0f, 0f);

                vSubF.X = robotPos.X + (float)Math.Cos(robotRoll) * vSub.X + (float)(-Math.Sin(robotRoll)) * vSub.Y;
                vSubF.Y = robotPos.Y + (float)Math.Sin(robotRoll) * vSub.X + (float)(Math.Cos(robotRoll)) * vSub.Y;
                vSubF.Z = z;

                return vSubF;
            }
        }

        internal static double GetMaxRange(ILaserRangeData ls)
        {
            double max = -1;

            foreach (double d in ls.Range)
                if (d < max_range)
                    max = Math.Max(max, d);

            return max;
        }

        #region ILaserRangeData Members

        float ILaserRangeData.MaxRange
        {
            get { return max_range; }
        }

        float ILaserRangeData.MinRange
        {
            get { return min_range; }
        }

        Pose2D ILaserRangeData.SensorOffset
        {
            get { return SensorGlobalPose; }
        }

        Pose3D ILaserRangeData.SensorOffset3D
        {
            get { return SensorGlobalPose3D; }
        }

        Pose2D ILaserRangeData.RobotOffset
        {
            get { return RobotGlobalPose; }
        }

        double ILaserRangeData.Resolution
        {
            get { return fResolution; }
        }

        double ILaserRangeData.FieldOfView
        {
            get { return fFov; }
        }

        double[] ILaserRangeData.Range
        {
            get { return fRanges; }
        }

        double[] ILaserRangeData.RangeTheta
        {
            get { return fTheta; }
        }

        bool[] ILaserRangeData.RangeFilters
        {
            get { return bFilters; }
        }
        float ILaserRangeData.Time
        {
            get { return fTime; }
        }
        #endregion

    }

}
