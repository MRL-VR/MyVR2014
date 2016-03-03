using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using MRL.Communication.Tools;
using MRL.Components;
using MRL.Components.Tools.Objects;
using MRL.Components.Tools.Shapes;
using MRL.CustomMath;
using MRL.Mapping;
using MRL.Utils;

namespace MRL.Commons
{
    public struct LinkProperties
    {
        public int portNumber;
        public RobotLinkType linkContentType;

        public LinkProperties(int pn, RobotLinkType ct)
        {
            portNumber = pn;
            linkContentType = ct;
        }
    }

    public static class ProjectCommons
    {
        public delegate void StringEventHandler(string msg);
        public delegate void PacketEventHandler(Packet packet);
        public delegate void _addConsoleMessage(string msg, ConsoleMessageType type);

        public delegate void _newMissionReceived(int botIndex, List<Pose2D> missionPoints);
        public delegate void _electromagnetic_Update(Pose2D pG, Pose2D pE, Pose2D pT);
        public delegate void _geoRefrencedMap_Updated(Bitmap map);
        public delegate void _robotCameraImage_Received(Bitmap full);

        public delegate void _newTargetReceived(int botIndex, Pose2D start,Pose2D goal);
        public delegate void _TargetRemoved(int botIndex);

        public delegate void _repaint();
        public delegate void _hint(string msg, Point location);

        public delegate void _panoViewer_OnVictimSelected(int id);
        public delegate void _imageWidget_OnSelected(PanoState p);
        public delegate void _imageWidget_OnVictim(VictimShape v);

        public delegate void _mouseClicked(EventArgs e);
        public delegate void _mouseDown(MouseEventArgs e);
        public delegate void _mouseUp(MouseEventArgs e);
        public delegate void _mouseMoved(MouseEventArgs e);
        public delegate void _mouseWheelMoved(MouseEventArgs e);

        public delegate void _keyDown(KeyEventArgs e);
        public delegate void _keyUp(KeyEventArgs e);

        public delegate void _updateGraphLink(string start, string end, double ss, int updateTime);// this delegate help us to update our communication graph links.
        public delegate void _updateGraphNode(string startName, Pose2D startPose, int updateTime);// this delegate help us to update our communication graph links.
        //public delegate void _pathFinderDebugHandler(PathFinderNode mCur);

        public static USARConfig config;
        public static StringEventHandler AddNewMessage;
        public static _addConsoleMessage ConsoleMessage;
        public static _imageWidget_OnSelected imageWidget_OnSelected;
        public static _imageWidget_OnVictim imageWidget_OnVictimUpdated;
        public static _imageWidget_OnVictim imageWidget_OnVictimDeleted;
        public static _panoViewer_OnVictimSelected panoViewer_OnVictimSelected;
        public static _newMissionReceived missionWidget_OnNewListReceived;
        public static _updateGraphLink updateGraphLink; // this var help us to update our communication graph links.
        public static _updateGraphNode updateGraphNode; // this var help us to update our communication graph nodes.
        public static _newTargetReceived targetWidget_OnNewPointReceived;
        public static _TargetRemoved targetWidget_OnPointRemoved;

        public static bool keyboardCapturedByGeoViewer = false;
        public static string currentResultPath = "";

        public static Pose2D ConvertCarmenToGlobal(Pose2D p)
        {
            return new Pose2D(p.X, -p.Y, -p.Rotation);
        }

        public static Pose2D ConvertGlobalToCarmen(Pose2D p)
        {
            return new Pose2D(p.X, -p.Y, -p.Rotation);
        }

        public static IPAddress LocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    return ip;
                }
            }
            return Dns.Resolve("localhost").AddressList[0];
        }

        public static void writeConsoleMessage(string message, ConsoleMessageType type)
        {
            if (ConsoleMessage != null)
                ConsoleMessage(message, type);
        }

        public static RobotInfo getRobot(string robotName)
        {
            robotName = robotName.ToLower();
            foreach (RobotInfo r in config.botInfo)
                if (r.Name.ToLower().Equals(robotName))
                    return r;
            return null;
        }

        public static bool UserLog
        {
            get { return config.userLog; }
        }

        public static FastRandom fastRandom = new FastRandom(Environment.TickCount);

        public static class RegisteryManager
        {
            public static string CurrentRegisteryAddress = "Software\\MRLVirtual\\";
            public static string DefaultConfigAddress = "DefaultConfigAddress";
            public static string lstAgentsItemsSelected = "lstAgentsItemsSelected";
            public static string lstSpawnItemsSelected = "lstSpawnItemsSelected";
        }
        public static class Commands
        {
            public static string getSignalStrength(string destinationRobotName)
            { return string.Format("GETSS {{Robot {0}}}\r\n", destinationRobotName); }
            public static double getSignalStrengthValue(string receivedMessage)
            {
                USARParser parser = new USARParser(receivedMessage);
                if (parser.type.Equals("SS"))
                    return double.Parse(parser.getString("Strength"));
                else
                    return double.MaxValue;
            }
            public static string getDNS(string destinationRobotName)
            { return string.Format("DNS {{Robot {0}}}\r\n", destinationRobotName); }
            public static string getDNS(string destinationRobotName, int onCustomPort)
            { return string.Format("DNS {{Robot {0}}} {{Port {1}}}\r\n", destinationRobotName, onCustomPort); }
            public static int getDNSValue(string receivedMessage)
            {
                USARParser parser = new USARParser(receivedMessage);
                return int.Parse(parser.getString("Port"));
            }
            public static string getReverseDNS(int destinationRobotPort)
            { return string.Format("REVERSEDNS {{Port {0}}}\r\n", destinationRobotPort); }

            public static string getReverseDNSValue(string receivedMessage)
            {
                USARParser parser = new USARParser(receivedMessage);
                return parser.getString("Robot");
            }

            public static string getRegisterPortINIT(string robotName, string ports)
            {
                return string.Format("INIT {{Robot {0}}} {{Port {1}}}\r\n", robotName, ports);
            }
        }

        public static class Drawing2D
        {
            public static class BaseBody
            {
                public static int lastUpdated = -1;
                public struct BodyLine { public float x1, y1, x2, y2;}
                public static SlimDX.Direct2D.Ellipse ellipse = new SlimDX.Direct2D.Ellipse();
                public static BodyLine line = new BodyLine();
                public static int lenght;
                public static void Initiate()
                {
                    Initiate(0.8f);
                }
                public static void Initiate(float radious)
                {
                    try
                    {
                        float len = ChangeRealLenghtToCanvasLenght(radious);
                        ellipse.RadiusX = len; ellipse.RadiusY = len;

                        line.x1 = 0; line.y1 = 0; line.x2 = len; line.y2 = 0;
                        lenght = 5;
                        //ProjectCommons.writeConsoleMessage("Initiated Len : " + len, MRL.Utils.ConsolMessageType.Information);
                    }
                    catch { }
                }
            }

            static Drawing2D()
            {
                try
                {
                   textFormat = new SlimDX.DirectWrite.TextFormat(new SlimDX.DirectWrite.Factory( SlimDX.DirectWrite.FactoryType.Shared), "tahoma", SlimDX.DirectWrite.FontWeight.Normal, SlimDX.DirectWrite.FontStyle.Normal, SlimDX.DirectWrite.FontStretch.Normal, 10f, "");
                }
                catch (Exception)
                {
                }
            }

            public static IEGMap GlobalMap;
            public static volatile int lastMapUpdated = -1;
            public const int defaultRadius = 10;
            public static SlimDX.Vector2 scaleCenter = SlimDX.Vector2.Zero;
            public static SlimDX.Direct2D.WindowRenderTarget windowRenderTarget;
            public static SlimDX.Matrix WorldMatrix = SlimDX.Matrix.Identity;
            public static PointF Center = new PointF(0f, 0f);
            public static float Scale = 1f;
            public static SlimDX.DirectWrite.TextFormat textFormat;
            private static SlimDX.Vector2 unitVector = new SlimDX.Vector2(1f, 1f);

            public static Graphics tempGraphic = Graphics.FromImage(new System.Drawing.Bitmap(10, 10));


            public static SlimDX.Direct2D.Bitmap Convert(System.Drawing.Bitmap bmp)
            {
                if (windowRenderTarget == null)
                    return null;
                System.Drawing.Imaging.BitmapData bitmapData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                SlimDX.DataStream stream = new SlimDX.DataStream(bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, true, false);
                SlimDX.Direct2D.PixelFormat format = new SlimDX.Direct2D.PixelFormat(SlimDX.DXGI.Format.B8G8R8A8_UNorm, SlimDX.Direct2D.AlphaMode.Premultiplied);
                SlimDX.Direct2D.BitmapProperties bitmapProperties = new SlimDX.Direct2D.BitmapProperties();
                bitmapProperties.HorizontalDpi = bitmapProperties.VerticalDpi = 96;
                bitmapProperties.PixelFormat = format;
                SlimDX.Direct2D.Bitmap bitmap = new SlimDX.Direct2D.Bitmap(windowRenderTarget, new System.Drawing.Size(bmp.Width, bmp.Height), stream, bitmapData.Stride, bitmapProperties);
                bmp.UnlockBits(bitmapData);
                return bitmap;
            }
            public static SlimDX.Matrix3x2 changePositionMatrix_noneWorldAndScale(float x, float y)
            {
                SlimDX.Matrix m = SlimDX.Matrix.Transformation2D(SlimDX.Vector2.Zero, 0f, new SlimDX.Vector2(1, 1), SlimDX.Vector2.Zero, 0f, new SlimDX.Vector2(x, y));
                return changeMatrix(m);

            }
            public static SlimDX.Matrix3x2 changePositionMatrix_noneWorld(float x, float y)
            {
                SlimDX.Matrix m = SlimDX.Matrix.Transformation2D(SlimDX.Vector2.Zero, 0f, new SlimDX.Vector2(Scale, Scale), SlimDX.Vector2.Zero, 0f, new SlimDX.Vector2(x, y));
                return changeMatrix(m);

            }
            public static SlimDX.Matrix3x2 changePositionMatrix_noneScale(float x, float y)
            {
                SlimDX.Matrix m = SlimDX.Matrix.Transformation2D(SlimDX.Vector2.Zero, 0f, new SlimDX.Vector2(1 / Scale, 1 / Scale), SlimDX.Vector2.Zero, 0f, new SlimDX.Vector2(x, y));
                return changeMatrix(SlimDX.Matrix.Multiply(m, WorldMatrix));

            }

            public static System.Drawing.Drawing2D.Matrix changePositionMatrix(double x, double y)
            {
                System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
                m.Translate((float)x, (float)y, System.Drawing.Drawing2D.MatrixOrder.Append);
                return m;
            }
            public static SlimDX.Matrix3x2 changePositionMatrix(float x, float y)
            {
                SlimDX.Matrix m = SlimDX.Matrix.Transformation2D(SlimDX.Vector2.Zero, 0f, unitVector, SlimDX.Vector2.Zero, 0f, new SlimDX.Vector2(x, y));
                return changeMatrix(SlimDX.Matrix.Multiply(m, WorldMatrix));
            }
            public static SlimDX.Matrix3x2 changePositionMatrix(float x, float y, float theta)
            {
                SlimDX.Matrix m = SlimDX.Matrix.Transformation2D(SlimDX.Vector2.Zero, 0f, unitVector, SlimDX.Vector2.Zero, theta, new SlimDX.Vector2(x, y));
                return changeMatrix(SlimDX.Matrix.Multiply(m, WorldMatrix));
            }
            public static void changeMatrix()
            {
                WorldMatrix = SlimDX.Matrix.Identity;
                WorldMatrix = SlimDX.Matrix.Transformation2D(scaleCenter, 0f, new SlimDX.Vector2(Scale, Scale), SlimDX.Vector2.Zero, 0f,
                    new SlimDX.Vector2(MouseDataBase.DrawPoint.X + MouseDataBase.Offset.X, MouseDataBase.DrawPoint.Y + MouseDataBase.Offset.Y));
            }
            public static SlimDX.Matrix3x2 changeMatrix(SlimDX.Matrix matrix)
            {
                SlimDX.Matrix3x2 retValue = SlimDX.Matrix3x2.Identity;
                retValue.M11 = matrix.M11;
                retValue.M12 = matrix.M12;
                retValue.M21 = matrix.M21;
                retValue.M22 = matrix.M22;
                retValue.M31 = matrix.M41;
                retValue.M32 = matrix.M42;
                return retValue;
            }
            public static Size mesureString(string text, Font font)
            {
                Size ret = new Size(0, 0);
                System.Drawing.SizeF sf = tempGraphic.MeasureString(text, font);
                ret.Width = (int)sf.Width;
                ret.Height = (int)sf.Height;
                return ret;
            }
            public static Pose2D ChangeCanvasToReal(Pose2D canvas)
            {
                if (GlobalMap == null)
                { ProjectCommons.writeConsoleMessage("ChangeCanvasToReal --> GlobalMap is null.", ConsoleMessageType.Error); return canvas; }
                Pose2D p = new Pose2D(canvas);
                p = GlobalMap.TransposeCanvas2Real(p);
                p = ProjectCommons.ConvertCarmenToGlobal(p);
                return p;
            }
            public static Pose2D ChangeCanvasToReal(Point canvas)
            {
                return ChangeCanvasToReal(new Pose2D(canvas.X, canvas.Y, 0));
            }

            public static Pose2D ChangeRealToCanvas(Pose2D real)
            {
                if (GlobalMap == null)
                {
                    ProjectCommons.writeConsoleMessage("ChangeRealToCanvas --> GlobalMap is null.", ConsoleMessageType.Error);
                    return real;
                }

                Pose2D p = new Pose2D(real);
                p = ProjectCommons.ConvertGlobalToCarmen(p);
                p = GlobalMap.TransposeRealToCanvas(p);
                p.Rotation = -p.Rotation - Math.PI / 2.0;
                return p;
            }

            public static float ChangeRealLenghtToCanvasLenght(float l)
            {
                if (GlobalMap == null)
                {
                    ProjectCommons.writeConsoleMessage("ChangeRealLenghtToCanvasLenght --> GlobalMap is null.", ConsoleMessageType.Error);
                    return l;
                }

                Pose2D p1 = new Pose2D(0, 0, 0);
                p1 = ProjectCommons.ConvertGlobalToCarmen(p1);
                p1 = GlobalMap.TransposeRealToCanvas(p1);

                Pose2D p2 = new Pose2D(1, 0, 0);
                p2 = ProjectCommons.ConvertGlobalToCarmen(p2);
                p2 = GlobalMap.TransposeRealToCanvas(p2);

                float oneMeter = (float)Math.Abs(p2.X - p1.X);

                if (oneMeter == 0f)
                    oneMeter = 1f / (float)(EGMap.egmParam[EGMap.RESOLUTION] * EGMap.egmParam[EGMap.DOWNSAMPLE]);
                return l * oneMeter;
            }
        }
    }
}