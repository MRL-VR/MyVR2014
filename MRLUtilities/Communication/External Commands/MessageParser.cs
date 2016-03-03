using System;

using MRL.Utils;
using MRL.Commons;

namespace MRL.Communication.External_Commands
{
    public enum eSimulationMessageType
    {
        None = 0,
        State,
        INS,
        Laser,
        Sonar,
        Encoder,
        //VictimRFID,
        Odometdy,
        //RFID,
        //CameraPanTilt,
        //CameraStatus,
        Camera,
        GroundTruth, // Not Used in the competition But for SLAM Test
        //IR,
        //IRScanner,
        GPS, // Not Used
        //Touch,
        //RFIDTag,
        //HumanMotion,
        //Sound,
        Tachometer, // Not Used
        Acoustic,
        IMU
    };

    public class MessageParser
    {
        private string SimMsg = "";
        private int SimMsgType = 0;
        private object SimMsgData = null;

        private void ParseMessage(string message)
        {
            USARParser msg = new USARParser(message);
            try
            {
                if (msg.type.Equals("SEN"))
                {
                    //File.AppendAllText("C:\\" + Process.GetCurrentProcess().Id.ToString() + ".txt", message + "\r\n"); 
                    switch (msg.getSegment("Type").Get("Type"))
                    {
                        case "GroundTruth":
                            SimMsgType = (int)eSimulationMessageType.GroundTruth;
                            SimMsgData = new GroundTruth(msg);
                            break;
                        case "Odometry":
                            SimMsgType = (int)eSimulationMessageType.Odometdy;
                            SimMsgData = new Odometry(msg);
                            break;
                        case "INS":
                            SimMsgType = (int)eSimulationMessageType.INS;
                            SimMsgData = new INS(msg);
                            break;
                        case "Encoder":
                            SimMsgType = (int)eSimulationMessageType.Encoder;
                            SimMsgData = new Encoder(msg);
                            break;
                        case "RangeScanner":
                            SimMsgType = (int)eSimulationMessageType.Laser;
                            SimMsgData = new Laser(msg);
                            break;
                        case "Sonar":
                            SimMsgType = (int)eSimulationMessageType.Sonar;
                            SimMsgData = new Sonar(msg);
                            break;
                        case "IMU":
                            SimMsgType = (int)eSimulationMessageType.IMU;
                            SimMsgData = new IMU(msg);
                            break;
                        case "Camera":
                            SimMsgType = (int)eSimulationMessageType.Camera;
                            SimMsgData = null;
                            break;
                        case "Tachometer":
                            SimMsgType = (int)eSimulationMessageType.Tachometer;
                            SimMsgData = null;
                            break;
                        case "Acoustic":
                            SimMsgType = (int)eSimulationMessageType.Acoustic;
                            SimMsgData = null;
                            break;
                        case "AltitudeSensor":
                            SimMsgType = (int)eSimulationMessageType.GPS;
                            SimMsgData = null;
                            break;
                        default:
                            SimMsgType = (int)eSimulationMessageType.None;
                            SimMsgData = null;
                            Console.WriteLine("ParseMessage - Name: {0}\nType: {1}", msg.getSegment("Type").Get("Type"));
                            break;
                    }
                }
                else if (msg.type.Equals("STA"))
                {
                    SimMsgType = (int)eSimulationMessageType.State;
                    SimMsgData = new State(msg);
                }
            }
            catch (Exception)
            {
                USARLog.println("Unable parse message <" + message + "> into object: ", 5, "MessageParser");
            }
        }

        public MessageParser()
        {
            SimMsg = "";
            SimMsgType = -1;
        }

        public string SimulationMessage
        {
            get { return SimMsg; }
            set
            {
                if (!string.IsNullOrEmpty(value.Trim()))
                {
                    SimMsg = value;
                    ParseMessage(SimMsg);
                }
                else
                {
                    SimMsg = "";
                }
            }
        }

        public int MessageType
        {
            get { return SimMsgType; }
        }

        public object MessageData
        {
            get { return SimMsgData; }
        }

    }

}
