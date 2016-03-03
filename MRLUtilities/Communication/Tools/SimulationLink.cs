using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using MRL.Commons;
using MRL.Utils;

namespace MRL.Communication.Tools
{

    /// <summary>
    /// class for connecting to simulation
    /// and retriving sensors information to the major class
    /// </summary>
    public class SimulationLink
    {
        public delegate void USARMessage(string msg);
        public event USARMessage OnUSARMessage_Received;

        #region Internal Class Declaration

        /// <summary>
        /// packet structure used to get simulation (sensors)
        /// </summary>
        internal class StringPacketObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 256;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder allMessage = new StringBuilder();
        }

        #endregion

        #region Variables Declaration

        private const int RECEIVE_TIMEOUT = 4000;

        // boolean to end the reading from socket
        //private volatile bool exitSignal = false;

        private Socket connection;
        private NetworkStream nStream;

        // for feedback the major class to read the inbox
        //private EventWaitHandle msgAction;

        // temorary used for saving splited packets in socket
        private string strRead = "";

        // connection properties
        private string ip;
        private int port;

        // Inbox for saving 
        //public Queue<string> Inbox = new Queue<string>();

        #endregion

        #region Property Declaration

        /// <summary>
        /// is connection alive
        /// </summary>
        public bool IsConnected
        {
            get 
            {
                return (connection.Connected) && (nStream != null);
            }
        }

        #endregion

        #region Public Functions Declaration

        /// <summary>
        /// create new simulation connection controller
        /// for retriving sensors information
        /// </summary>
        /// <param name="ci">simulation connection properties</param>
        /// <param name="messageReceivedAction">signal action for reativating major thread 
        /// to processing received packet</param>
        public SimulationLink(string host, int port)
        {
            this.ip = host;
            this.port = port;
        }

        public void Connect()
        {
            try
            {
                connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                connection.Connect(ip, port);
                nStream = new NetworkStream(connection, true);

                USARLog.println(" >> SimulationServer connected successfully!", this.ToString());
                ProjectCommons.writeConsoleMessage("SimulationServer connected successfully!", ConsoleMessageType.Information);

                Receive(connection);
            }
            catch (Exception e)
            {
                USARLog.println(" >> " + e.ToString(), this.ToString());
                ProjectCommons.writeConsoleMessage("Connecting to SimulationServer failed.", ConsoleMessageType.Error);
            }
        }

        /// <summary>
        /// disconnect from simulation
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected)
            {
                //exitSignal = true;
                //connection.Close();

                nStream.Close();
                nStream = null;
            }
        }

        /// <summary>
        /// Sending command to Simulation (Driving command)
        /// </summary>
        /// <param name="command">command to be sent</param>
        /// <returns>if it success returns True</returns>
        public bool Send(string command)
        {
            try
            {
                command += Environment.NewLine;
                byte[] commBytes = Encoding.ASCII.GetBytes(command);
                nStream.Write(commBytes, 0, commBytes.Length);
                nStream.Flush();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string SendAndReceive(string command)
        {
            try
            {
                command += Environment.NewLine;

                byte[] buffer = Encoding.ASCII.GetBytes(command);
                nStream.Write(buffer, 0, buffer.Length);
                nStream.Flush();

                buffer = new byte[1024];
                nStream.ReadTimeout = RECEIVE_TIMEOUT;
                int len = nStream.Read(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer, 0, len);

                return response;
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region Private Functions Declarations

        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StringPacketObject state = new StringPacketObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StringPacketObject.BufferSize, 0,
                                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                USARLog.println(" >> " + e.ToString(), this.ToString());
            }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                //if (exitSignal) return;

                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StringPacketObject state = (StringPacketObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.allMessage.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    SplitRAWData(state.allMessage);

                    state.allMessage = new StringBuilder();
                    state.buffer = new byte[StringPacketObject.BufferSize];

                    // Get the rest of the data.
                    //if (!exitSignal)
                        client.BeginReceive(state.buffer, 0, StringPacketObject.BufferSize, 0,
                                            new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    SplitRAWData(state.allMessage);

                    state.allMessage = new StringBuilder();
                    state.buffer = new byte[StringPacketObject.BufferSize];

                    // re Begin receiving the data from the remote device.
                    //if (!exitSignal)
                        client.BeginReceive(state.buffer, 0, StringPacketObject.BufferSize, 0,
                                            new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch (Exception e)
            {
                USARLog.println(" >> " + e.ToString(), this.ToString());
                ProjectCommons.writeConsoleMessage("Error In SimulationLink", ConsoleMessageType.Error);
            }
        }
        private void SplitRAWData(StringBuilder netCompleteMessage)
        {
            strRead += netCompleteMessage.ToString();

            string[] sp = strRead.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < sp.Length - 1; i++)
                RaiseEvent(sp[i]);

            if (strRead.EndsWith("\r\n"))
            {
                RaiseEvent(sp[sp.Length - 1]);
                strRead = "";
            }
            else
                strRead = sp[sp.Length - 1];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void RaiseEvent(string msg)
        {
            if (OnUSARMessage_Received != null)
                OnUSARMessage_Received(msg);
        }

        #endregion

    }

}
