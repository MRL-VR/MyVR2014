using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using MRL.Commons;
using MRL.Utils;
using MRL.CustomMath;
using System.IO;

namespace MRL.Communication.Internal_Objects
{
    [Serializable]
    public class IMNode : BaseInternalObject
    {
        public Pose2D sourcePose;
        public float x, y;      // in meter
        public float direction; // in radians
        public List<Pose2D> path = new List<Pose2D>();
        public int priority = int.MinValue;
        //public ProxyID generator;
        public int exploredCount;
        public int commandIndex;
        private bool visited = false;


        public static string generateExplNodesWCSMsg(List<IMNode> el, int gID)
        {
            string ret = "";
            string nodesString = "";

            foreach (IMNode n in el)
            {
                nodesString += n.NodePosition.ToString().Replace(" ", "#") + ",";
            }
            if (nodesString.EndsWith(",")) nodesString = nodesString.Substring(0, nodesString.Length - 1);
            if (string.IsNullOrEmpty(nodesString)) nodesString = "[]";

            ret = string.Format("WCSExplNode {{Nodes {0}}} {{GoalID {1}}}", nodesString, gID);
            //USARLog.println(ret, 0, "IMNode---->");
            return ret;
        }


        virtual public int Priority
        {
            get { return priority; }
            set { this.priority = value; }
        }

        virtual public bool Visited
        {
            get { return visited; }
            set { visited = value; }
        }

        virtual public List<Pose2D> Path
        {
            get { return path; }

            // NOTE: we copy the data into path
            set
            {
                foreach (Pose2D p in value)
                {
                    path.Add(new Pose2D(p));
                }
            }
        }

        /// <summary>
        /// get node position in carmen frame
        /// </summary>
        virtual public Pose2D NodePosition
        {
            get { return new Pose2D(x, y, direction); }
        }

        virtual public DPoint Source
        {
            get
            {
                if (path == null)
                    return null; // should not happen

                DPoint p = (DPoint)path[0];
                return p;
            }
        }

        virtual public DPoint Destination
        {
            get { return new DPoint(x, y); }
        }

        public IMNode()
        {
            x = 0;
            y = 0;
            direction = 0;
            path = null;
        }

        /// <summary>Creates a new instance of IMNode </summary>
        public IMNode(float x, float y, float dir)
        {
            sourcePose = new Pose2D(x, y, dir);
            this.x = x;
            this.y = y;
            direction = dir;
            path = new List<Pose2D>();
            exploredCount = 0;
            commandIndex = -1;
        }

        public IMNode(double x, double y, double dir)
            : this((float)x, (float)y, (float)dir)
        {
        }

        public IMNode(Pose2D p)
            : this((float)p.X, (float)p.Y, (float)p.Rotation)
        {
        }

        public virtual void addWaypoint(double x, double y)
        {
            path.Add(new Pose2D(x, y, 0));
        }

        public override string ToString()
        {
            string res = "Node X=" + x + " Y=" + y + " Dir=" + direction + " Path=";

            if (path == null)
                res += "null";
            else
            {
                foreach (Pose2D p in path)
                {
                    res += ("(" + p.X + "," + p.Y + ")");
                }
            }

            return res + " Priority=" + priority;
        }

        public override byte MessageID { get { return (byte)InternalMessagesID.IMNode; } }
        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);
                    mWriter.Write(sourcePose.X);
                    mWriter.Write(sourcePose.Y);
                    mWriter.Write(sourcePose.Rotation);
                    mWriter.Write(x);
                    mWriter.Write(y);
                    mWriter.Write(direction);

                    mWriter.Write(path.Count);

                    foreach (var item in path)
                    {
                        mWriter.Write(item.X);
                        mWriter.Write(item.Y);
                        mWriter.Write(item.Rotation);
                    }

                    mWriter.Write(priority);
                    mWriter.Write(exploredCount);
                    mWriter.Write(commandIndex);
                    mWriter.Write(visited);
                }
                return mStream.ToArray();
            }
        }
        public override void Deserialize(byte[] buffer)
        {
            using (MemoryStream mStream = new MemoryStream(buffer))
            {
                using (BinaryReader mReader = new BinaryReader(mStream))
                {
                    mReader.ReadByte();

                    double _x = mReader.ReadDouble();
                    double _y = mReader.ReadDouble();
                    double _r = mReader.ReadDouble();
                    sourcePose = new Pose2D(_x, _y, _r);
                    x = mReader.ReadSingle();
                    y = mReader.ReadSingle();
                    direction = mReader.ReadSingle();

                    int count = mReader.ReadInt32();
                    path = new List<Pose2D>();
                    path.Clear();
                    for (int i = 0; i < count; i++)
                    {
                        _x = mReader.ReadDouble();
                        _y = mReader.ReadDouble();
                        _r = mReader.ReadDouble();
                        path.Add(new Pose2D(_x,_y,_r));
                    }
                    
                    priority = mReader.ReadInt32();
                    exploredCount = mReader.ReadInt32();
                    commandIndex = mReader.ReadInt32();
                    visited = mReader.ReadBoolean();
                }
            }
        
        }
    }

}
