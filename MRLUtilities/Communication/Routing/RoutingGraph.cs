using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MRL.Commons;
using MRL.Communication.Internal_Objects;
using MRL.CustomMath;
using MRL.Utils;
using MRL.Communication.Tools;

namespace MRL.Communication.Tools
{
    public class RoutingGraph
    {
        internal class SyncEvents
        {
            private EventWaitHandle _newFPacketEvent;
            private EventWaitHandle _exitThreadEvent;

            private WaitHandle[] _eventArray1;

            public SyncEvents()
            {
                _newFPacketEvent = new AutoResetEvent(false);
                _exitThreadEvent = new AutoResetEvent(false);

                _eventArray1 = new WaitHandle[2];
                _eventArray1[0] = _newFPacketEvent;
                _eventArray1[1] = _exitThreadEvent;
            }

            public EventWaitHandle ExitThreadEvent
            {
                get { return _exitThreadEvent; }
            }
            public EventWaitHandle NewFPacketEvent
            {
                get { return _newFPacketEvent; }
            }
            public WaitHandle[] EventArrayForControllingPackets
            {
                get { return _eventArray1; }
            }
        }

        private DVTable routingTable = new DVTable();

        private WSSControlLink comLink = null;
        private string myRobotName = "";
        public int robotsCount = 0;

        private SyncEvents _syncEvents = new SyncEvents();

        public delegate void OnSendDVForwardTableDlg(Packet com);
        public OnSendDVForwardTableDlg OnSendDVForwardTable;

        public string getRobotname
        {
            get
            {
                return myRobotName;
            }
        }

        public RoutingGraph(WSSControlLink comLink, string thisRobot)
        {
            try
            {
                myRobotName = thisRobot;

                List<string> nodesList = new List<string>();

                foreach (RobotInfo r in ProjectCommons.config.botInfo)
                {
                    if (!r.Spawned) continue;
                    nodesList.Add(r.Name);
                }

                nodesList.Add(ProjectCommons.config.mapperInfo.Name);

                this.comLink = comLink;

                //createDVTable()
                foreach (var node in nodesList)
                {
                    if (!node.Equals(myRobotName))
                    {
                        bool cost = false;
                        double sigPower = 0.0;

                        sigPower = this.comLink.GETSS(node);
                        cost = (!double.IsNaN(sigPower) && (sigPower > ProjectCommons.config.wireLink_Danger_Strength));

                        DVLink dvlink = new DVLink(myRobotName, node, cost, 1, sigPower);
                        routingTable.items.Add(dvlink);
                    }
                }

                robotsCount = ProjectCommons.config.botInfo.Count + 1;

                Task.Factory.StartNew(() => controlDVTable());
                Task.Factory.StartNew(() => sendForwardTable());

                _syncEvents.NewFPacketEvent.Set();

                USARLog.println(" >> CommunicationGraph started successfully!", this.ToString());
                ProjectCommons.writeConsoleMessage("CommunicationGraph started successfully!", ConsoleMessageType.Information);
            }
            catch (Exception e)
            {
                USARLog.println(" >> " + e.ToString(), this.ToString());
                ProjectCommons.writeConsoleMessage("Starting CommunicationGraph  failed!", ConsoleMessageType.Error);
            }
        }

        private void Start()
        {
            Task.Factory.StartNew(() => controlDVTable());
            Task.Factory.StartNew(() => sendForwardTable());

            _syncEvents.NewFPacketEvent.Set();
        }
        public void CloseCommunication()
        {
            _syncEvents.ExitThreadEvent.Set();
        }

        public string GetInterfaceRobot(string destRobot)
        {
            DVLink dvLink = routingTable[destRobot];
            if (dvLink == null) return "";

            lock (dvLink)
            {
                if (dvLink.cost)
                    return dvLink.interfaceNode;
                else
                    return "";
            }
        }

        public Signal GetCostRobot(string destRobot)
        {
            DVLink dvLink = routingTable[destRobot];
            if (dvLink == null) return null;

            lock (dvLink)
            {
                return new Signal()
                {
                    Status = dvLink.cost,
                    Value = dvLink.SignalPercentage,
                    Type = (dvLink.interfaceNode.Equals(myRobotName) ? SignalType.DIRECT : SignalType.ROUTED)
                };
            }
        }

        public void updateDVTable(CommGraph comGraph)
        {
            try
            {
                bool isStateChanged = false;

                bool costSToJ = false;
                DVLink dvLink = routingTable[comGraph.startName];
                lock (dvLink)
                {
                    costSToJ = dvLink.cost;
                }

                foreach (var src in routingTable.items)
                {
                    lock (src)
                    {
                        if (src.cost && src.interfaceNode.Equals(myRobotName))
                            continue;

                        bool costSToN = src.cost;
                        string intSToN = src.interfaceNode;

                        foreach (var link in comGraph.links)
                        {
                            if (link.destRobotName.Equals(src.destNode))
                            {
                                bool costJToN = link.signalState;
                                bool V = costSToJ && costJToN;
                                bool W = costSToN;

                                if (intSToN.Equals(comGraph.startName))
                                {
                                    if (V)
                                    {
                                        if (src.hob != (link.hob + 1))
                                            isStateChanged = true;

                                        src.hob = link.hob + 1;
                                        src.cost = V;
                                    }
                                    else
                                    {
                                        src.hob = 1;
                                        src.cost = false;
                                        //src.interfaceNode = myRobotName;
                                        //isStateChanged = true;
                                    }
                                }
                                else
                                {
                                    if ((V && src.hob > (link.hob + 1)) || (!src.cost && V))
                                    {
                                        if (!src.interfaceNode.Equals(link.interfaceRobot))
                                        {
                                            src.hob = link.hob + 1;
                                            src.cost = true;
                                            src.interfaceNode = comGraph.startName;
                                            isStateChanged = true;
                                        }
                                    }
                                }

                                if (src.hob > robotsCount)
                                {
                                    src.hob = 1;
                                    src.cost = false;
                                }

                                break;
                            }
                        }
                    }
                }
                if (isStateChanged)
                {
                    sendDVTable();
                }
                //ProjectCommons.writeConsoleMessage("After ...", ConsoleMessageType.Error);
                //ProjectCommons.writeConsoleMessage(this.ToString(), ConsoleMessageType.Error);
            }
            catch (Exception e)
            {



            }

        }
        private bool updateDVTable()
        {
            bool isStateChanged = false;
            int dcCount = 0;
            foreach (var item in routingTable.items)
            {
                lock (item)
                {
                    double sigPower = this.comLink.GETSS(item.destNode);
                    bool cost = (!double.IsNaN(sigPower) && (sigPower > ProjectCommons.config.wireLink_Danger_Strength));
                    if (!cost)
                        dcCount++;

                    if (item.interfaceNode.Equals(myRobotName))
                    {
                        item.signalStrength = sigPower;

                        if (cost != item.cost)
                        {
                            item.cost = cost;
                            item.interfaceNode = myRobotName;
                            item.hob = 1;
                            isStateChanged = true;
                        }
                    }
                    else
                    {

                        if (cost)
                        {
                            item.signalStrength = sigPower;

                            item.cost = cost;
                            item.interfaceNode = myRobotName;
                            item.hob = 1;
                            isStateChanged = true;
                        }
                        else
                        {
                            double sigInterfacePower = this.comLink.GETSS(item.interfaceNode);
                            bool interfaceCost = (!double.IsNaN(sigInterfacePower) && (sigInterfacePower > ProjectCommons.config.wireLink_Danger_Strength));

                            item.signalStrength = sigInterfacePower;

                            if (!interfaceCost)
                            {
                                item.cost = false;
                                item.interfaceNode = myRobotName;
                                item.hob = 1;
                                isStateChanged = true;
                            }
                        }
                    }
                }
            }

            if (dcCount == routingTable.items.Count)
            {
                isStateChanged = false;
                foreach (var item in routingTable.items)
                {
                    lock (item)
                    {
                        item.cost = false;
                        //item.interfaceNode = myRobotName;
                        item.signalStrength = -104f;
                        item.hob = 1;
                    }
                }
            }
            return isStateChanged;
        }

        private void controlDVTable()
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (true)
            {
                bool isStateChanged = updateDVTable();
                if (isStateChanged || sw.ElapsedMilliseconds > 2000)
                {
                    if (isStateChanged)
                        sendDVTable();
                    else
                        _syncEvents.NewFPacketEvent.Set();
                    sw.Restart();
                }

                //ProjectCommons.writeConsoleMessage(DateTime.Now.ToString() + "\n" + this.ToString(), ConsoleMessageType.Information);
                Thread.Sleep(1000);
            }
        }

        private void sendForwardTable()
        {
            while (WaitHandle.WaitAny(_syncEvents.EventArrayForControllingPackets) != 1)
            {
                foreach (var item in routingTable.items)
                {
                    bool cost = false;
                    string destNode = "";
                    string interfaceNode = "";

                    lock (item)
                    {
                        cost = item.cost;
                        destNode = item.destNode;
                        interfaceNode = item.interfaceNode;
                    }

                    if ((interfaceNode.Equals(myRobotName)) && (cost))
                    {
                        sendRequestDVTable(destNode);
                    }
                }
            }
        }

        private void sendRequestDVTable(string destRobot)
        {
            try
            {
                RequestGraph rg = new RequestGraph();

                Packet p = new Packet();

                p.Data = rg;
                p.SenderName = myRobotName;
                p.ReceiverName = destRobot;

                OnSendDVForwardTable(p);
            }
            catch (Exception ex)
            {
            }
        }

        private void sendDVTable()
        {
            foreach (var item in routingTable.items)
            {
                bool cost = false;
                string destNode = "";
                string interfaceNode = "";

                lock (item)
                {
                    cost = item.cost;
                    destNode = item.destNode;
                    interfaceNode = item.interfaceNode;
                }

                if ((interfaceNode.Equals(myRobotName)) && (cost))
                {
                    sendDVForwardTable(destNode);
                }
            }

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool sendDVForwardTable(string destRobot)
        {
            try
            {
                CommGraph comGraph = new CommGraph();
                comGraph.startName = myRobotName;
                comGraph.startPose = new Pose2D();

                bool cost = false;
                string destNode = "";
                string interfaceNode = "";
                int hob = 0;

                foreach (var item in routingTable.items)
                {
                    lock (item)
                    {
                        cost = item.cost;
                        destNode = item.destNode;
                        interfaceNode = item.interfaceNode;
                        hob = item.hob;
                    }

                    if (interfaceNode.Equals(destRobot))
                        comGraph.links.Add(new ComGraphLink(destNode, interfaceNode, false, 0));
                    else
                        comGraph.links.Add(new ComGraphLink(destNode, interfaceNode, cost, hob));
                }

                Packet p = new Packet();

                p.Data = comGraph;
                p.SenderName = myRobotName;
                p.ReceiverName = destRobot;

                OnSendDVForwardTable(p);

                return true;
            }
            catch { return false; }
        }

        public override string ToString()
        {

            string tableStr = "\r\nDVTable " + myRobotName + "     " + DateTime.Now + "  \r\n";
            tableStr += "D\t\tI\t\tC\t\tH\r\n";
            tableStr += "-----------------------------------------------------------\r\n";

            bool cost = false;
            string destNode = "";
            string interfaceNode = "";
            int hob;
            string signal;

            foreach (var item in routingTable.items)
            {
                lock (item)
                {
                    cost = item.cost;
                    destNode = item.destNode;
                    interfaceNode = item.interfaceNode;
                    hob = item.hob;
                    signal = item.SignalPercentage + "%";
                }

                tableStr += string.Format("{0}\t\t{1}\t\t{2}\t\t{3}\t\t{4}\r\n", destNode, interfaceNode, cost, hob, signal);
            }
            tableStr += "-----------------------------------------------------------\r\n";

            return tableStr;
        }

    }

}
