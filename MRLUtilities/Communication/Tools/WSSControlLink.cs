using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using MRL.Commons;
using MRL.Utils;

namespace MRL.Communication.Tools
{
    public class WSSControlLink
    {
        #region Variable Declaration

        public delegate void OnNewPacketReceivedDlg(Packet newPacket);
        public OnNewPacketReceivedDlg OnNewPacketReceived;

        private const int RECEIVE_TIMEOUT = 4000;

        private string ip;
        private int port;

        private int botInitialPort;
        private string myName;

        private Socket wssConnection;
        private NetworkStream wssNetStream;
        private BinaryWriter wssBinWriter;
        private BinaryReader wssBinReader;

        private bool isListeningToPort;
        private volatile bool exitSignal;

        private List<TcpListener> socketListeners = new List<TcpListener>(2);

        #endregion

        #region Property Declaration

        public bool IsConnected
        {
            get
            {
                return (wssNetStream != null);
            }
        }

        #endregion

        #region Public Functions Declaration

        public WSSControlLink(string myName, string wssIP, int wssPort, int InitialPort)
        {
            this.myName = myName;
            ip = wssIP;
            port = wssPort;
            botInitialPort = InitialPort;
        }
        public bool Connect()
        {
            try
            {
                wssConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                wssConnection.Connect(ip, port);

                wssNetStream = new NetworkStream(wssConnection, true);
                wssBinWriter = new BinaryWriter(wssNetStream);
                wssBinReader = new BinaryReader(wssNetStream);

                exitSignal = false;

                USARLog.println(" >> WSSControlLink connected successfully!", this.ToString());
                ProjectCommons.writeConsoleMessage("WssControlLink connected successfully!", ConsoleMessageType.Information);

                return true;
            }
            catch (Exception e)
            {
                USARLog.println(" >> " + e.ToString(), this.ToString());
                ProjectCommons.writeConsoleMessage("Error In WSSControlLink.Connect() : " + e.InnerException, ConsoleMessageType.Error);

                return false;
            }
        }
        public void Disconnect()
        {
            if (IsConnected)
            {
                wssBinWriter.Close();
                wssBinWriter = null;

                wssBinReader.Close();
                wssBinReader = null;

                wssNetStream.Close();
                wssNetStream = null;

                if (isListeningToPort)
                {
                    exitSignal = true;

                    foreach (TcpListener s in socketListeners)
                        s.Stop();
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<LinkProperties> RegisterAndListen(params RobotLinkType[] linkArray)
        {
            if (linkArray.Length == 0)
                linkArray = (RobotLinkType[])Enum.GetValues(typeof(RobotLinkType));

            List<LinkProperties> propes;
            while (true)
            {
                propes = getFreePorts(linkArray);

                string response = sendAndReceive(getInitCommand(propes));
                if (response.Contains("INITREPLY"))
                    break;
            }

            isListeningToPort = true;
            foreach (LinkProperties lp in propes)
                ThreadPool.QueueUserWorkItem(new WaitCallback(WaitForAcceptAnotherConnections), lp);

            return propes;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public double GETSS(string toRobot)
        {
            try
            {
                string reply = sendAndReceive(ProjectCommons.Commands.getSignalStrength(toRobot));
                //USARLog.println(myName + " GETSS to " + toRobot + " reply : " + reply, "WSS");
                //ProjectCommons.writeConsoleMessage(myName + " GETSS to " + toRobot + " reply : " + reply, ConsoleMessageType.Information);
                return ProjectCommons.Commands.getSignalStrengthValue(reply);
            }
            catch
            { return double.MaxValue; }
        }

        Dictionary<string, int> DNSPoool = new Dictionary<string, int>();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetDNS(string otherRobotName, int customPortOn)
        {
            try
            {
                var key = otherRobotName + "/" + customPortOn;
                if (!DNSPoool.ContainsKey(key))
                {
                    string DNScommand = "";
                    string reply;
                    if (customPortOn == -1)
                        DNScommand = ProjectCommons.Commands.getDNS(otherRobotName);
                    else
                        DNScommand = ProjectCommons.Commands.getDNS(otherRobotName, customPortOn);
                    reply = sendAndReceive(DNScommand);
                    DNSPoool.Keys.Where(x => x.StartsWith(otherRobotName + "/")).ToList().ForEach(x => DNSPoool.Remove(x));
                    DNSPoool.Add(key, ProjectCommons.Commands.getDNSValue(reply));
                }
                return DNSPoool[key];
            }
            catch { return -1; }
        }

        public bool SendPacket(Packet sendPacket)
        {
            return SendPacket(sendPacket, sendPacket.ReceiverName, -1);
        }

        public bool SendPacket(Packet sendPacket, int toPort)
        {
            return SendPacket(sendPacket, sendPacket.ReceiverName, toPort);
        }

        public bool SendPacket(Packet sendPacket, string toRobot)
        {
            return SendPacket(sendPacket, toRobot, -1);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool SendPacket(Packet sendPacket, string toRobot, int toPort)
        {
            try
            {
                double d = GETSS(toRobot);

                if (Double.IsNaN(d) || (d == Double.MaxValue)) { d = 0.0; }

                if (d >= ProjectCommons.config.wireLink_Danger_Strength)
                {
                    int otherRobotPortInWSS = GetDNS(toRobot, toPort);

                    if (otherRobotPortInWSS > -1)
                    {
                        //Socket socketConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        //socketConnection.Connect(ip, otherRobotPortInWSS);

                        //if (socketConnection.Connected)
                        //{
                        //    using (NetworkStream networkStream = new NetworkStream(socketConnection, true))
                        //        using (BinaryWriter writer = new BinaryWriter(networkStream))
                        //        {
                        //            byte[] serialized = sendPacket.Serialize();

                        //            writer.Write(serialized.Length);
                        //            writer.Flush();
                        //            writer.Write(serialized);
                        //            writer.Flush();
                        //        }

                        //    return true;
                        //}
                        //else
                        //    throw new Exception();

                        using (TcpClient tc = new TcpClient(ip, otherRobotPortInWSS))
                        using (NetworkStream networkStream = tc.GetStream())
                        using (BinaryWriter writer = new BinaryWriter(networkStream))
                        {
                            byte[] serialized = sendPacket.Serialize();

                            writer.Write(serialized.Length);
                            writer.Flush();
                            writer.Write(serialized);
                            writer.Flush();
                        }

                        return true;
                    }
                    else
                        throw new Exception();
                }
                return false;
            }
            catch (Exception e)
            {
                DNSPoool.Keys.Where(x => x.StartsWith(toRobot + "/")).ToList().ForEach(x => DNSPoool.Remove(x));
                return false;
            }
        }

        #endregion

        #region Private Functions Declaration

        //private string sendAndReceive(string command)
        //{
        //    try
        //    {
        //        if (wssNetStream == null)
        //            return "";

        //        //ProjectCommons.writeConsoleMessage(command, ConsoleMessageType.Information);

        //        wssBinWriter.Write(Encoding.ASCII.GetBytes(command));
        //        wssBinWriter.Flush();

        //        List<byte> responseBytes = new List<byte>();
        //        byte cr = Encoding.ASCII.GetBytes("\r")[0];
        //        byte read = 0;
        //        //wssNetStream.ReadTimeout = RECEIVE_TIMEOUT;
        //        while (read != cr)
        //        {
        //            read = wssBinReader.ReadByte();
        //            responseBytes.Add(read);
        //        }

        //        string response = Encoding.ASCII.GetString(responseBytes.ToArray());

        //        //ProjectCommons.writeConsoleMessage(response, ConsoleMessageType.Information);
        //        return response;
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}        

        object wssLock = new object();

        private string sendAndReceive(string command)
        {
            try
            {
                lock (wssLock)
                {
                    if (wssNetStream == null)
                        return "";
                    byte[] buffer = Encoding.ASCII.GetBytes(command);
                    wssNetStream.Write(buffer, 0, buffer.Length);
                    wssNetStream.Flush();

                    buffer = new byte[1024];
                    wssNetStream.ReadTimeout = RECEIVE_TIMEOUT;
                    int len = wssNetStream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.ASCII.GetString(buffer, 0, len);
                    return response;
                }
            }
            catch
            {
                return "";
            }
        }

        private void WaitForAcceptAnotherConnections(object state)
        {
            LinkProperties lp = (LinkProperties)state;
            TcpListener socketListener = new TcpListener(ProjectCommons.LocalIPAddress(), lp.portNumber);

            socketListener.Start();
            socketListeners.Add(socketListener);

            while (!exitSignal)
            {
                try
                {
                    ThreadPool.QueueUserWorkItem(newPacketReceived, socketListener.AcceptTcpClient());
                }
                catch (Exception e)
                {
                    ProjectCommons.writeConsoleMessage("WSSControlLink.WaitForAcceptAnotherConnections() : " +
                                                       e.ToString(), ConsoleMessageType.Exclamation);
                    USARLog.println(e.ToString(), "WSSControlLink.WaitForAcceptAnotherConnections");
                }
            }
        }
        private void newPacketReceived(object newSocket)
        {
            try
            {
                using (var tc = newSocket as TcpClient)
                using (var receivedStream = tc.GetStream())
                using (var binReader = new BinaryReader(receivedStream))
                    if (receivedStream != null)
                    {
                        int packetSize = binReader.ReadInt32();
                        byte[] packetBuffer = binReader.ReadBytes(packetSize);

                        Packet packet = new Packet();
                        packet.Deserialize(packetBuffer);
                        OnNewPacketReceived(packet);
                        packet = null;
                    }
            }
            catch (Exception ex)
            {
                //ProjectCommons.writeConsoleMessage("WSSControlLink.newPacketReceived() : " +
                //                                    ex.ToString(), ConsoleMessageType.Error);
                USARLog.println(ex.ToString(), "WSSControlLink.newPacketReceived");
            }
        }

        private string getInitCommand(List<LinkProperties> propes)
        {
            if (propes.Count > 0)
            {
                string ports = "";
                foreach (LinkProperties p in propes)
                { ports += p.portNumber.ToString() + ","; }
                ports = ports.Substring(0, ports.Length - 1);
                return ProjectCommons.Commands.getRegisterPortINIT(myName, ports);
            }
            else
                return ProjectCommons.Commands.getRegisterPortINIT(myName, botInitialPort.ToString());
        }

        private List<LinkProperties> getFreePorts(params RobotLinkType[] linkArray)
        {
            List<LinkProperties> retValue = new List<LinkProperties>();
            foreach (RobotLinkType rlt in linkArray)
            {
                while (true)
                {
                    if (!isFreePort())
                    {
                        botInitialPort += 10;
                        continue;
                    }
                    else
                    {
                        retValue.Add(new LinkProperties(botInitialPort, rlt));
                        botInitialPort += 10;
                        break;
                    }
                }
            }
            return retValue;
        }
        private bool isFreePort()
        {
            try
            {
                IPAddress ipAddress = ProjectCommons.LocalIPAddress();
                TcpListener tcpListener = new TcpListener(ipAddress, botInitialPort);
                tcpListener.Start();
                tcpListener.Stop();
                tcpListener = null;
                return true;
            }
            catch { return false; }
        }

        #endregion
    }
}
