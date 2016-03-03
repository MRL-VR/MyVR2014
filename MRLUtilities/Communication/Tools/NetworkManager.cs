using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MRL.Commons;
using MRL.Communication.Internal_Objects;
using MRL.Utils;
using MRL.Communication.Tools;

namespace MRL.Communication.Tools
{
    public class NetworkManager
    {
        public delegate void OnNewPacketReceivedDlg(BaseInternalObject newPacket, string senderName, List<Hop> InfaterfaceHop);
        public OnNewPacketReceivedDlg OnNewPacketReceived;


        private WSSControlLink wssService;
        private RoutingGraph commGraphService;

        //private ConcurrentQueue<Packet> cmdQueue = null;
        //private ConcurrentQueue<Packet> imgQueue = null;

        private BlockingCollection<Packet> cmdCollection = null;
        private BlockingCollection<Packet> imgCollection = null;
        private CancellationTokenSource cts;

        private string thisRobot;
        private int wssListenPort;
        private int imageServerPort;
        private List<LinkProperties> RegisteredPorts;

        public List<LinkProperties> GetRegisteredPorts
        {
            get
            {
                return RegisteredPorts;
            }
        }

        public NetworkManager(string robotName, int wssListenPort)
        {
            thisRobot = robotName;
            this.wssListenPort = wssListenPort;
            this._isStarted = false;

            imgCollection = new BlockingCollection<Packet>();
            cmdCollection = new BlockingCollection<Packet>();

        }

        public void NewPacketReceivedByWSS(Packet newPacket)
        {
            newPacket.ReceiveTime = Environment.TickCount;
            //ProjectCommons.writeConsoleMessage(cmdCollection.Count + " " + imgCollection.Count, ConsoleMessageType.Exclamation);
            
            switch (newPacket.Data.MessageID)
            {
                case (byte)InternalMessagesID.RoutedMessage:
                    {
                        RoutedMessage rm = newPacket.Data as RoutedMessage;
                        
                        string nextInterfaceRobot = commGraphService.GetInterfaceRobot(rm.receiverName);
                        Packet p = null;
                        if (nextInterfaceRobot.Equals(thisRobot))
                        {
                            p = new Packet(newPacket.Priority, 0, 0, rm.senderName, rm.receiverName, newPacket.PacketType, rm.packetData);
                        }
                        else
                        {
                            p = new Packet(newPacket.Priority, 0, 0, thisRobot, nextInterfaceRobot, newPacket.PacketType, rm);
                        }

                        int signal = GetSignalRobot(newPacket.SenderName).Value;
                        p.InterfaceHops.AddRange(newPacket.InterfaceHops);
                        p.InterfaceHops.Add(new Hop() { RobotName = thisRobot, Signal = signal });
                        cmdCollection.Add(p);
                    }
                    break;
                case (byte)InternalMessagesID.RequestDVGraph:
                    {
                        commGraphService.sendDVForwardTable(newPacket.SenderName);
                    }
                    break;
                case (byte)InternalMessagesID.CommunicationGraph:
                    {

                        CommGraph commGraph = newPacket.Data as CommGraph;
                        commGraphService.updateDVTable(commGraph);
                    }
                    break;
                default:
                    {
                        //int signal = GetSignalRobot(newPacket.SenderName).Value;
                        //newPacket.InterfaceHops.Add(new Hop() { RobotName = thisRobot, Signal = signal });
                        OnNewPacketReceived(newPacket.Data, newPacket.SenderName, newPacket.InterfaceHops);
                    }
                    break;
            }

        }


        int skipFrame = 0;
        int SKIP_COUNT = 2;
        public bool SendVideoToBot(string toRobot, int toPort, BaseInternalObject msg, MessagePriority priority = MessagePriority.INP_MSG_LOW)
        {
            if (!_isStarted) return false;

            imageServerPort = toPort;
            string interfaceRobot = commGraphService.GetInterfaceRobot(toRobot);

            if (interfaceRobot != "")
            {
                Packet p = null;
                if (interfaceRobot.Equals(thisRobot))
                {
                    //stright link
                    p = new Packet(priority, Environment.TickCount, 0, thisRobot, toRobot, PacketType.BITMAP_PACKET, msg);
                    p.InterfaceHops.Add(new Hop() { RobotName = thisRobot, Signal = 0 });

                    imgCollection.Add(p);
                }
                else
                {
                    if (skipFrame % SKIP_COUNT == 0)
                    {
                        RoutedMessage rm = new RoutedMessage()
                        {
                            senderName = thisRobot,
                            receiverName = toRobot,
                            receiverPort = toPort,
                            packetData = msg
                        };

                        p = new Packet(priority, Environment.TickCount, 0, thisRobot, interfaceRobot, PacketType.BITMAP_PACKET, rm);
                        p.InterfaceHops.Add(new Hop() { RobotName = thisRobot, Signal = 0 });
                        cmdCollection.Add(p);
                    }

                    skipFrame++;
                }

                return true;
            }
            else
                return false;
        }

        public bool SendStringToBot(string toRobot, BaseInternalObject msg, MessagePriority priority = MessagePriority.INP_MSG_LOW)
        {
            if (!_isStarted) return false;

            string interfaceRobot = commGraphService.GetInterfaceRobot(toRobot);

            if (interfaceRobot != "")
            {
                Packet p = null;

                if (interfaceRobot.Equals(thisRobot))
                {
                    //stright link
                    p = new Packet(priority, Environment.TickCount, 0, thisRobot, toRobot, PacketType.STRING_PACKET, msg);
                }
                else
                {
                    RoutedMessage rm = new RoutedMessage()
                    {
                        senderName = thisRobot,
                        receiverName = toRobot,
                        packetData = msg,
                    };

                    p = new Packet(priority, Environment.TickCount, 0, thisRobot, interfaceRobot, PacketType.STRING_PACKET, rm);
                }
             
                p.InterfaceHops.Add(new Hop() { RobotName = thisRobot, Signal = 0 });

                cmdCollection.Add(p);

                return true;
            }
            else
                return false;
        }

        public void Start(params RobotLinkType[] types)
        {
            try
            {
                wssService = new WSSControlLink(thisRobot, ProjectCommons.config.wssHost, ProjectCommons.config.wssPort, wssListenPort);
                wssService.OnNewPacketReceived += new WSSControlLink.OnNewPacketReceivedDlg(NewPacketReceivedByWSS);
                bool isConncted = wssService.Connect();

                if (isConncted)
                {
                    List<LinkProperties> regPorts = wssService.RegisterAndListen(types);

                    if (regPorts != null)
                        RegisteredPorts = regPorts;

                    commGraphService = new RoutingGraph(wssService, thisRobot);
                    commGraphService.OnSendDVForwardTable += new RoutingGraph.OnSendDVForwardTableDlg(DVTableProducedByCG);

                    //ThreadPool.QueueUserWorkItem(new WaitCallback(ControlSendPacket));
                    cts = new CancellationTokenSource();

                    Task.Factory.StartNew(() =>
                    {
                        foreach (var item in cmdCollection.GetConsumingEnumerable())
                        {
                            if (cts.Token.IsCancellationRequested)
                                break;

                            //USARLog.println("In Queue : " + (Environment.TickCount - item.InQueueTime), "Command");
                            wssService.SendPacket(item);
                        }
                    }, cts.Token);

                    Task.Factory.StartNew(() =>
                    {
                        foreach (var item in imgCollection.GetConsumingEnumerable())
                        {
                            if (cts.Token.IsCancellationRequested)
                                break;

                            //USARLog.println("In Queue : " + (Environment.TickCount - item.InQueueTime), "Image");

                            wssService.SendPacket(item, imageServerPort);
                        }
                    }, cts.Token);


                    _isStarted = true;
                }
                USARLog.println(" >> NetworkManager started successfully!", this.ToString());
                ProjectCommons.writeConsoleMessage("NetworkManager started successfully!", ConsoleMessageType.Information);
            }
            catch (Exception e)
            {
                USARLog.println(" >> " + e.ToString(), this.ToString());
                ProjectCommons.writeConsoleMessage("Starting NetworkManager failed!", ConsoleMessageType.Error);
            }
        }

        public void Stop()
        {
            try
            {
                if (cts != null)
                    cts.Cancel();
                cmdCollection.CompleteAdding();
                imgCollection.CompleteAdding();

                if (wssService != null)
                    wssService.Disconnect();

                if (commGraphService != null)
                    commGraphService.CloseCommunication();
            }
            catch (Exception)
            {
            }
            _isStarted = false;
        }

        public Signal GetSignalRobot(string destRobot)
        {
            if (commGraphService == null) return null;

            return commGraphService.GetCostRobot(destRobot);

        }

        public void DVTableProducedByCG(Packet newPacket)
        {
            cmdCollection.Add(newPacket);
        }

        private bool _isStarted;
        public bool IsManagerStarted
        {
            get { return _isStarted; }
        }

    }
}
