using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.CustomMath;
using MRL.Communication.External_Commands;
using MRL.Utils;
using MRL.Communication.Internal_Objects;

namespace MRL.Utils
{
    internal class WorldModel
    {
        public static WorldModel Instance { set; get; }

        public float BatteryMaximumeCapacity;
        public float BatteryLife;
        public bool LightState;
        public float ServerTime;
        public Pose3D GTHPose3D;
        public Pose3D INSPose3D;
        public Pose2D OdoPose2D;
        public Pose2D EstimatedPose;
        public Laser CurrentScan;
        public SonarManager SonarManager;
        public IMU RawIMU;

        static WorldModel()
        {
            Instance = new WorldModel();
        }
        
        public WorldModel()
        {
            SonarManager = new Utils.SonarManager();
        }
    }
}
