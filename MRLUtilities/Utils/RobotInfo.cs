using System.Drawing;
using System.Linq;
using MRL.CustomMath;

namespace MRL.Utils
{
    public class RobotInfo
    {
        private float[] _iniPosition = new float[3]; // in meter
        private float[] _iniRotation = new float[3]; // in radian

        #region Propeties
        public string RobotIP { get; set; }
        public int RobotPort { get; set; }
        public string Name { get; set; }
        public bool Spawned { get; set; }
        public string Type { get; set; }
        public string NamePosition { get { return Name + "(" + Position3D + ")"; } }
        public int MountIndex { get; set; }
        public int MaxSpeed;
        public virtual Vector3 Position3D
        {
            get { return new Vector3(_iniPosition); }
            set { _iniPosition = value.FloatArray; }
        }

        public virtual Vector3 Rotation3D
        {
            get { return new Vector3(_iniRotation); }
            set { _iniRotation = value.FloatArray; }
        }

        public virtual string PositionString
        {
            get { return _iniPosition[0] + "," + _iniPosition[1] + "," + _iniPosition[2]; }
            set { _iniPosition = value.Split(',').Select(x => float.Parse(x.Trim())).ToArray(); }
        }

        public virtual string RotationString
        {
            get { return _iniRotation[0] + "," + _iniRotation[1] + "," + _iniRotation[2]; }
            set { _iniRotation = value.Split(',').Select(x => float.Parse(x.Trim())).ToArray(); }

        }

        public virtual PointF StartPoint { get { return new PointF(_iniPosition[0], _iniPosition[1]); } }
        #endregion

        #region Constructors
        public RobotInfo(string name)
            : this(name, "P3AT", new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 }, "127.0.0.1", "10000")
        {
        }

        public RobotInfo(RobotInfo r)
        {
            this.Name = r.Name;
            this.Type = r.Type;
            this.MountIndex = r.MountIndex;
            // copy instead of assign
            for (int i = 0; i < 3; i++)
            {
                this._iniPosition[i] = r._iniPosition[i];
                this._iniRotation[i] = r._iniRotation[i];
            }

            this.RobotIP = r.RobotIP;
            this.RobotPort = r.RobotPort;
            this.Spawned = r.Spawned;
        }

        public RobotInfo(string name, string type, float[] pos, float[] rot, string ip, string port)
        {
            this.Name = name;
            this.Type = type;

            // copy instead of assign
            for (int i = 0; i < 3; i++)
            {
                if (i < pos.Length)
                    _iniPosition[i] = pos[i];

                if (i < rot.Length)
                    _iniRotation[i] = rot[i];
            }

            this.RobotIP = ip;
            this.RobotPort = int.Parse(port);
            this.Spawned = false;
        }
        #endregion

        #region Methods
        public virtual void SetPosition(Pose2D p)
        {
            this._iniPosition[0] = (float)p.X;
            this._iniPosition[1] = (float)p.Y;
            this._iniPosition[2] = 0f;
            this._iniRotation[0] = 0f;
            this._iniRotation[1] = 0f;
            this._iniRotation[2] = (float)p.Rotation;
        }

        public virtual Pose3D GetPosition()
        {
            return new Pose3D(this._iniPosition[0], this._iniPosition[1], this._iniPosition[2],
                              this._iniRotation[0], this._iniRotation[1], this._iniRotation[2]);
        }

        public static RobotInfo parseInfo(string str)
        {
            USARParser up = new USARParser("Robot " + str);

            return new RobotInfo(up.getSegment("Name").Get("Name"), up.getSegment("Type").Get("Type"),
                                 USARParser.parseFloats(up.getSegment("Position").Get("Position"), ","),
                                 USARParser.parseFloats(up.getSegment("Rotation").Get("Rotation"), ","),
                                 up.getSegment("BotIP").Get("BotIP"), up.getSegment("BotPort").Get("BotPort"));
        }

        public override string ToString()
        {
            return "Robot {Name " + Name + " Type " + Type +
                   " Position " + PositionString + " Rotation " + RotationString +
                   " BotIP " + RobotIP + " BotPort " + RobotPort + " }";
        }

        public string GetINITCommand()
        {
            string initString = "INIT {ClassName USARBot." + Type + "} " +
                                "{Name " + Name + "} " +
                                "{Location " + _iniPosition[0] + "," + _iniPosition[1] + "," + _iniPosition[2] + "} " +
                                "{Rotation " + _iniRotation[0] + "," + _iniRotation[1] + "," + _iniRotation[2] + "}";

            return initString;
        }
        #endregion
    }
}
