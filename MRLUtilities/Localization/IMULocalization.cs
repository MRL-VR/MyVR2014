using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.CustomMath;
using MRL.Communication.External_Commands;

namespace MRL.Localization
{
    public class IMULocalization
    {
        public IMULocalization(Pose2D initialPos)
        {

        }

        public Pose2D CurrPos{ get; set; }

        public void Start() { }

        public void Stop() { }

        public void InsertNewIMU(IMU newPacket)
        { 
        
        }

    }
}
