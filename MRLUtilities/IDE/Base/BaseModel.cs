using System;
using System.Collections.Generic;
using MRL.Commons;
using MRL.Communication.Internal_Objects;
using MRL.Communication.Tools;
using MRL.Utils;

namespace MRL.IDE.Base
{
    public abstract class BaseModel
    {
        public SimulationLink mSimulationLink;
        public NetworkManager mNetworkManager;

        public List<LinkProperties> RegisteredPorts;

        public RobotInfo ThisRobot { get; protected set; }
        public bool RobotSpawned { get; protected set; }


        public BaseModel(RobotInfo me)
        {
            ThisRobot = new RobotInfo(me);
            RegisteredPorts = new List<LinkProperties>();

            RobotSpawned = false;
        }

        public abstract void Mount();
        public abstract void Unmount();


        protected internal abstract void RobotUSARMessage_Received(string msg);
        protected internal abstract void RobotWSSStringPacket_Received(BaseInternalObject newPacket, string senderName, List<Hop> interfaceHops);

        protected void Run_RobotWSSStringPacket_Received(BaseInternalObject newPacket, string senderName, List<Hop> interfaceHops)
        {
            try
            {
                RobotWSSStringPacket_Received(newPacket, senderName, interfaceHops);
            }
            catch
            {

            }
        }
    }

}
