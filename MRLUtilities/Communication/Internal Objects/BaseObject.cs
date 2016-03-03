using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{
    
    public abstract class BaseInternalObject
    {
        public static BaseInternalObject CreateInternalObject(byte[] buffer)
        {
            InternalMessagesID mID = (InternalMessagesID)buffer[0];
            BaseInternalObject mObj = null;
            switch (mID)
            {
                case InternalMessagesID.Battery:
                    mObj = new Battery();
                    break;
                case InternalMessagesID.Position3D:
                    mObj = new Position3D();
                    break;
                case InternalMessagesID.RangeScan:
                    mObj = new RangeScan();
                    break;
                case InternalMessagesID.PortInfo:
                    mObj = new PortInfo();
                    break;
                case InternalMessagesID.ImageServer:
                    mObj = new ImageServer();
                    break;
                case InternalMessagesID.Drive:
                    mObj = new Drive();
                    break;
                case InternalMessagesID.Mission:
                    mObj = new Mission();
                    break;
                case InternalMessagesID.VictimRFID:
                    mObj = new Victim();
                    break;
                case InternalMessagesID.CommunicationGraph:
                    mObj = new CommGraph();
                    break;
                case InternalMessagesID.CameraImage:
                    mObj = new Camera();
                    break;
                case InternalMessagesID.AutonomousChange:
                    mObj = new AutonomousChange();
                    break;
                case InternalMessagesID.RoutedMessage:
                    mObj = new RoutedMessage();
                    break;
                case InternalMessagesID.RequestDVGraph:
                    mObj = new RequestGraph();
                    break;
                case InternalMessagesID.IMNode:
                    mObj = new IMNode();
                    break;
                case InternalMessagesID.JoyStickData:
                    mObj = new JoyStickData();
                    break;
                case InternalMessagesID.FrontierList:
                    mObj = new FrontierList();
                    break;
                case InternalMessagesID.PressedKeys:
                    mObj = new PressedKeys();
                    break;

            }

            mObj.Deserialize(buffer);
            return mObj;
        }
        public abstract byte MessageID { get; }
        public abstract byte[] Serialize();
        public abstract void Deserialize(byte[] buffer);
    }
}
