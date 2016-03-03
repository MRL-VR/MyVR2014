using System;
using System.IO;
using MRL.Communication.Internal_Objects;
using System.Collections.Generic;
using MRL.Utils;

namespace MRL.Communication.Tools
{
    public class Hop : ICloneable
    {
        public string RobotName;
        public int Signal;

        public object Clone()
        {
            return new Hop()
            {
                RobotName = this.RobotName,
                Signal = this.Signal
            };
        }
    }

    public class Packet : IComparable
    {
        public string SenderName;
        public string ReceiverName;
        public PacketType PacketType;
        public int ReceiveTime;
        public MessagePriority Priority;
        public int InQueueTime;
        public int SendOnSocketTime; //for debug
        public BaseInternalObject Data;
        public List<Hop> InterfaceHops = new List<Hop>();

        public Packet() { }
        public Packet(MessagePriority Priority, int StartTime, int ReceiveTime, string SenderName, string ReceiverName, PacketType PacketType, BaseInternalObject Data)
        {
            this.SenderName = SenderName;
            this.ReceiverName = ReceiverName;
            this.PacketType = PacketType;
            this.ReceiveTime = ReceiveTime;
            this.Priority = Priority;
            this.InQueueTime = StartTime;
            this.Data = Data;
        }

        public byte[] Serialize()
        {
            byte[] buffer;
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(m))
                {
                    mWriter.Write(SenderName);
                    mWriter.Write(ReceiverName);
                    mWriter.Write((byte)PacketType);
                    mWriter.Write(ReceiveTime);
                    mWriter.Write((byte)Priority);
                    mWriter.Write(InQueueTime);
                    mWriter.Write(SendOnSocketTime);

                    byte[] dataBytes = Data.Serialize();
                    mWriter.Write(dataBytes.Length);
                    mWriter.Write(dataBytes);

                    int hCount = InterfaceHops.Count;
                    mWriter.Write(hCount);
                    foreach (var item in InterfaceHops)
                    {
                        mWriter.Write(item.RobotName);
                        mWriter.Write(item.Signal);
                    }
                }
                buffer = m.ToArray();
            }
            return buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            using (MemoryStream m = new MemoryStream(buffer))
            {
                using (BinaryReader mReader = new BinaryReader(m))
                {
                    SenderName = mReader.ReadString();
                    ReceiverName = mReader.ReadString();
                    PacketType = (PacketType)mReader.ReadByte();
                    ReceiveTime = mReader.ReadInt32();
                    Priority = (MessagePriority)mReader.ReadByte();
                    InQueueTime = mReader.ReadInt32();
                    SendOnSocketTime = mReader.ReadInt32();
                    int dataLength = mReader.ReadInt32();
                    byte[] readBytes = mReader.ReadBytes(dataLength);
                    Data = BaseInternalObject.CreateInternalObject(readBytes);

                    int count = mReader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string botName = mReader.ReadString();
                        int signal = mReader.ReadInt32();
                        InterfaceHops.Add(new Hop() { RobotName = botName, Signal = signal });
                    }
                }
            }
        }

        public override string ToString()
        {
            string str = "SenderName:" + SenderName +
                "\r\nReceiverName:" + ReceiverName +
                "\r\nPacketType:" + PacketType +
                "\r\nReceiveTime:" + ReceiveTime +
                "\r\nPriority:" + Priority +
                "\r\nSendTime:" + InQueueTime;
            return str;
        }
        public int CompareTo(Object obj)
        {
            if (obj.GetType() != this.GetType())
                return 1;

            Packet path = (Packet)obj;
            if (path == null)
                return 1;

            int c1 = InQueueTime;
            int c2 = path.InQueueTime;
            return -c1.CompareTo(c2);
        }

    }
}