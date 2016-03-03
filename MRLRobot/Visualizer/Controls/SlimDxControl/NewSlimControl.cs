using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DWriteFactory = SlimDX.DirectWrite.Factory;
using PointF = System.Drawing.PointF;
using Color = System.Drawing.Color;
using SizeF = System.Drawing.SizeF;
using RectangleF = System.Drawing.RectangleF;
using SlimDX;

using System.Threading.Tasks;
using SlimDX.Direct2D;
using SlimDX.DirectWrite;
using System.Drawing.Imaging;
using System.IO;
using MRL.Commons;
using MRL.IDE.Robot;
using MRL.Utils;
using MRL.Utils.GMath;

namespace VisualizerLibrary
{
    public partial class NewSlimControl : newD2DControl
    {

        public NewSlimControl()
            : base()
        {
            InitializeComponent();

            dwriteFactory = new DWriteFactory(SlimDX.DirectWrite.FactoryType.Isolated);
            this.MouseMove += new MouseEventHandler(SlimControl_MouseMove);
            this.MouseDown += new MouseEventHandler(SlimControl_MouseDown);
            this.MouseUp += new MouseEventHandler(SlimControl_MouseUp);

        }

        public void invalid() { Invalidate(); }

        void SlimControl_MouseUp(object sender, MouseEventArgs e)
        {

        }

        void SlimControl_MouseLeave(object sender, EventArgs e)
        {

        }

        void SlimControl_MouseMove(object sender, MouseEventArgs e)
        {

        }

        void SlimControl_MouseDown(object sender, MouseEventArgs e)
        {
            Position2D p = PixelToMetric(e.Location);
            if (MouseDownEvent != null && (Control.MouseButtons & MouseButtons.Right) != 0)
                MouseDownEvent(new MRL.CustomMath.Pose2D(p.X, p.Y, 0));
        }

        public delegate void MouseDownDel(MRL.CustomMath.Pose2D P);
        public event MouseDownDel MouseDownEvent;

        public override void InitializeTransform()
        {

            // Transform = MatrixCalculator.Rotate(new Position2D(0f, 0f), Transform.Value, 90, System.Drawing.Drawing2D.MatrixOrder.Append);
            // Transform = MatrixCalculator.Translate(Transform.Value, 1000, 120, System.Drawing.Drawing2D.MatrixOrder.Append);
            // Transform = MatrixCalculator.Scale(Transform.Value, 0.5f, 0.5f, System.Drawing.Drawing2D.MatrixOrder.Append);

            // Transform = Matrix.Transformation2D(SlimDX.Vector2.Zero, 0f, new SlimDX.Vector2(1, 1), SlimDX.Vector2.Zero, 0f, new SlimDX.Vector2(0, 0)).ToMatrix3x2();
            // Transform = new Matrix3x2() { M11 = 0, M12 = 100, M21 = -100, M22 = 0, M31 = 260, M32 = 340 };
            Transform = MatrixCalculator.Rotate(new Position2D(0, 0), Transform.Value, 180, System.Drawing.Drawing2D.MatrixOrder.Append);
            Transform = MatrixCalculator.Scale(Transform.Value, -1f, 1f, System.Drawing.Drawing2D.MatrixOrder.Append);
            Transform = MatrixCalculator.Translate(Transform.Value, 550, 200, System.Drawing.Drawing2D.MatrixOrder.Append);
            // Transform = Matrix.Transformation2D(SlimDX.Vector2.Zero, 0f, new SlimDX.Vector2(1, 1), SlimDX.Vector2.Zero, 0f, new SlimDX.Vector2(0, 0)).ToMatrix3x2();
        }

        DWriteFactory dwriteFactory;

        protected override void OnPaintContent()
        {
            DrawField();
            DrawAllObj();
        }

        void DrawField()
        {

            var WhiteBrush = new SolidColorBrush(renderTarget, new Color4(Color.White));

            renderTarget.DrawLine(WhiteBrush, new PointF(0f, -1f), new PointF(0f, 1f), 0.02f);
            renderTarget.DrawLine(WhiteBrush, new PointF(-1f, 0f), new PointF(1f, 0f), 0.02f);

            DrawText("(0,1)", WhiteBrush, new Position2D(0, 1 + 0.3));
            DrawText("(0,-1)", WhiteBrush, new Position2D(0, -1 - 0.3));
            DrawText("(1,0)", WhiteBrush, new Position2D(1 + 0.3, 0));
            DrawText("(-1,0)", WhiteBrush, new Position2D(-1 - 0.6 + 0.2, 0));

            WhiteBrush.Dispose();
        }

        private void DrawAllObj()
        {
            //------------------kind : List<Position2D> , Position2D , Circle ,Line----------------------

            //-------------------------------------------------------------------------Create Copy-------
            if (DrawingObjects.drawingObject.Count == 0) return;
            Dictionary<string, object> copy = new Dictionary<string, object>();
            DrawingObjects.drawingObject.ToList().ForEach(p =>
            {
                if (p.Value != null && p.Key != null)
                    copy[p.Key] = p.Value;
            });

            //--------------------------------------------------------------------------Drawing----------
            foreach (var item in copy)
            {
                if (item.Value.GetType() == typeof(System.Drawing.Bitmap) && DrawingObjects.ShowMap)
                {
                    var map = item.Value.As<System.Drawing.Bitmap>();
                    var _map = Convert(map);

                    var n1 = Canvas2Real(new MRL.CustomMath.Pose2D(0, 0, 0));
                    var n2 = Canvas2Real(new MRL.CustomMath.Pose2D(map.Size.Width, 0, 0));
                    var n3 = Canvas2Real(new MRL.CustomMath.Pose2D(map.Size.Width, map.Size.Height, 0));
                    var n4 = Canvas2Real(new MRL.CustomMath.Pose2D(0, map.Size.Height, 0));

                    var rect = new System.Drawing.RectangleF(n1, new SizeF((float)(n2 - n1).Size, (float)(n2 - n3).Size));
                    renderTarget.DrawBitmap(_map, rect, 1f, SlimDX.Direct2D.InterpolationMode.NearestNeighbor);
                    _map.Dispose();
                }
            }

            foreach (var item in copy)
            {
                if (item.Value.GetType() == typeof(List<Position2D>))
                {
                    List<Position2D> poses = item.Value.As<List<Position2D>>();
                    foreach (var item2 in poses)
                    {
                        renderTarget.DrawEllipse(item2.DrawColor.ToBrush(renderTarget), new Ellipse().GetEllipse(item2, 0.025f, 0.025f), 0.05f);
                    }
                }
                //-------------------------------------------------------------
                if (item.Value.GetType() == typeof(LaserModel))
                {
                    LaserModel laser = item.Value.As<LaserModel>();
                    Position2D mypos = laser.RobotPosition;
                    foreach (var item2 in laser.LaserScan)
                    {
                        renderTarget.DrawLine(laser.color.ToBrush(renderTarget), mypos, item2, 0.02f);
                    }
                }
                //-------------------------------------------------------------
                if (item.Value.GetType() == typeof(StringDraw))
                {
                    StringDraw st = item.Value.As<StringDraw>();
                    DrawText(st.Content, st.TextColor.ToBrush(renderTarget), st.Posiotion);
                }
                //-------------------------------------------------------------
                if (item.Value.GetType() == typeof(SonarModel))
                {
                    SonarModel Smodel = item.Value.As<SonarModel>();
                    foreach (Line item2 in Smodel.Sonar)
                    {
                        renderTarget.DrawLine(Smodel.SonarColor.ToBrush(renderTarget), item2.Tail, item2.Head, item2.strock);
                    }
                    foreach (StringDraw item2 in Smodel.SonarText)
                    {
                        DrawText(item2.Content, item2.TextColor.ToBrush(renderTarget), item2.Posiotion);
                    }

                }
                //-------------------------------------------------------------
                if (item.Value.GetType() == typeof(Position2D))
                {
                    Position2D pos = item.Value.As<Position2D>();
                    renderTarget.DrawEllipse(pos.DrawColor.ToBrush(renderTarget), new Ellipse().GetEllipse(pos, 0.025f, 0.025f), 0.05f);
                }
                //-------------------------------------------------------------
                else if (item.Value.GetType() == typeof(Circle))
                {
                    Circle circle = item.Value.As<Circle>();
                    var OrangeRedBrush = Color.OrangeRed.ToBrush(renderTarget);
                    renderTarget.DrawEllipse(OrangeRedBrush, new Ellipse().GetEllipse(circle.Center, (float)circle.Radious, (float)circle.Radious), 0.3f);
                    OrangeRedBrush.Dispose();
                }
                //-------------------------------------------------------------
                else if (item.Value.GetType() == typeof(Line))
                {
                    Line line = item.Value.As<Line>();
                    renderTarget.DrawLine(line.DrawColor.ToBrush(renderTarget), line.Tail, line.Head, line.strock);
                }
                //-------------------------------------------------------------
                // else if (item.Value.GetType() == typeof(System.Drawing.Bitmap) && DrawingObjects.ShowMap)
                // {
                //     var map = item.Value.As<System.Drawing.Bitmap>();
                //     var _map = Convert(map);
                //
                //     var n1 = Canvas2Real(new MRL.CustomMath.Pose2D(0, 0, 0));
                //     var n2 = Canvas2Real(new MRL.CustomMath.Pose2D(map.Size.Width, 0, 0));
                //     var n3 = Canvas2Real(new MRL.CustomMath.Pose2D(map.Size.Width, map.Size.Height, 0));
                //     var n4 = Canvas2Real(new MRL.CustomMath.Pose2D(0, map.Size.Height, 0));
                //
                //     var rect = new System.Drawing.RectangleF(n1, new SizeF((float)(n2 - n1).Size, (float)(n2 - n3).Size));
                //     renderTarget.DrawBitmap(_map, rect, 0.5f, SlimDX.Direct2D.InterpolationMode.NearestNeighbor);
                // }
                //-------------------------------------------------------------
                else if (item.Value.GetType() == typeof(RobotModel))
                {
                    RobotModel Robot = item.Value.As<RobotModel>();
                    DrawRobot(Robot.position, Robot.position.angle);
                    if (Robot.haveLaser) ;
                    if (Robot.haveSonar) ;
                    if (Robot.havePositionText) ;
                }
            }


        }


        private void DrawToken(Position2D pos)
        {
            Line l1 = new Line(new Position2D(pos.X - 0.02, pos.Y), new Position2D(pos.X + 0.02, pos.Y), new System.Drawing.Pen(System.Drawing.Brushes.Black, 0.01f));
            Line l2 = new Line(new Position2D(pos.X, pos.Y - 0.02), new Position2D(pos.X, pos.Y + 0.02), new System.Drawing.Pen(System.Drawing.Brushes.Black, 0.01f));
            Brush b = ToBrush(pos.DrawColor);
            renderTarget.DrawLine(b, l1.Head, l1.Tail, 0.005f);
            renderTarget.DrawLine(b, l2.Head, l2.Tail, 0.005f);
            renderTarget.DrawEllipse(b, new Ellipse().GetEllipse(pos, 0.02f, 0.02f), 2.01f);
            b.Dispose();
        }

        private void DrawRobot(Position2D Location, double? Angle)
        {
            if (Location == null) return;
            //----------------------------------------------
            float RobotWidth = 0.3f, RobotHeight = 0.5f;

            var _map = Convert(MRLRobot.Resource.Resources.robot);
            renderTarget.Transform = MatrixCalculator.Rotate(Location, renderTarget.Transform, 90 + (float)Location.angle.ToDegree(), System.Drawing.Drawing2D.MatrixOrder.Append);
            renderTarget.DrawBitmap(_map, new System.Drawing.RectangleF(Location - new Vector2D(RobotWidth, RobotWidth), new SizeF(2 * RobotWidth, 2 * RobotWidth)), 100, SlimDX.Direct2D.InterpolationMode.NearestNeighbor);
            renderTarget.Transform = MatrixCalculator.Rotate(Location, renderTarget.Transform, -1 * (90 + (float)Location.angle.ToDegree()), System.Drawing.Drawing2D.MatrixOrder.Append);
            _map.Dispose();
        }

        public SlimDX.Direct2D.Bitmap Convert(System.Drawing.Bitmap bmp)
        {
            if (renderTarget == null)
                return null;
            System.Drawing.Imaging.BitmapData bitmapData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            SlimDX.DataStream stream = new SlimDX.DataStream(bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, true, false);
            SlimDX.Direct2D.PixelFormat format = new SlimDX.Direct2D.PixelFormat(SlimDX.DXGI.Format.B8G8R8A8_UNorm, SlimDX.Direct2D.AlphaMode.Premultiplied);
            SlimDX.Direct2D.BitmapProperties bitmapProperties = new SlimDX.Direct2D.BitmapProperties();
            bitmapProperties.HorizontalDpi = bitmapProperties.VerticalDpi = 96;
            bitmapProperties.PixelFormat = format;
            SlimDX.Direct2D.Bitmap bitmap = new SlimDX.Direct2D.Bitmap(renderTarget, new System.Drawing.Size(bmp.Width, bmp.Height), stream, bitmapData.Stride, bitmapProperties);
            bmp.UnlockBits(bitmapData);

            stream.Dispose();

            return bitmap;
        }

        private Position2D Canvas2Real(MRL.CustomMath.Pose2D pos)
        {
            var p1 = (BaseRobot.Instance.localMap.TransposeCanvas2Real(pos));
            p1 = ProjectCommons.ConvertGlobalToCarmen(p1);
            p1.Rotation = -p1.Rotation - Math.PI / 2.0;
            var n1 = new Position2D(p1);
            return n1;
        }

        public SlimDX.Direct2D.Bitmap LoadBitmap(System.Drawing.Bitmap drawingBitmap)
        {
            SlimDX.Direct2D.Bitmap result = null;

            //Lock the gdi resource
            System.Drawing.Imaging.BitmapData drawingBitmapData = drawingBitmap.LockBits(new System.Drawing.Rectangle(0, 0, drawingBitmap.Width, drawingBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //Prepare loading the image from gdi resource
            DataStream dataStream = new DataStream(drawingBitmapData.Scan0, drawingBitmapData.Stride * drawingBitmapData.Height, true, false);
            SlimDX.Direct2D.BitmapProperties properties = new SlimDX.Direct2D.BitmapProperties();
            properties.PixelFormat = new SlimDX.Direct2D.PixelFormat(SlimDX.DXGI.Format.B8G8R8A8_UNorm, SlimDX.Direct2D.AlphaMode.Premultiplied);

            //Load the image from the gdi resource
            result = new SlimDX.Direct2D.Bitmap(renderTarget, new System.Drawing.Size(drawingBitmap.Width, drawingBitmap.Height), dataStream, drawingBitmapData.Stride, properties);

            //Unlock the gdi resource
            drawingBitmap.UnlockBits(drawingBitmapData);

            return result;
        }

        private Position2D PixelToMetric(System.Drawing.Point MouseLocation)
        {
            double X = MouseLocation.X;
            double Y = MouseLocation.Y;

            // X = (X - Transform.Value.M31) / Transform.Value.M21;
            // Y = (Y - Transform.Value.M32) / Transform.Value.M12;
            // return new Position2D(Y, X);

            X = (X - Transform.Value.M31) / Transform.Value.M11;
            Y = (Y - Transform.Value.M32) / Transform.Value.M22;
            return new Position2D(X, Y);
        }

        private void DrawText(string text, Brush brush, Position2D pos)
        {
            SlimDX.DirectWrite.TextFormat textFormat;
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(pos, new System.Drawing.SizeF(text.Length * 0.2f, 0.1f));
            rect.Location = new System.Drawing.PointF(rect.Location.X - rect.Width / 2, rect.Location.Y - rect.Height / 2);
            textFormat = new SlimDX.DirectWrite.TextFormat(dwriteFactory, "Berlin Sans FB Demi",
                SlimDX.DirectWrite.FontWeight.Bold
                , SlimDX.DirectWrite.FontStyle.Normal
                , SlimDX.DirectWrite.FontStretch.Normal, 0.25f, "t");
            textFormat.TextAlignment = SlimDX.DirectWrite.TextAlignment.Center;
            textFormat.ParagraphAlignment = SlimDX.DirectWrite.ParagraphAlignment.Center;
            //   renderTarget.Transform = MatrixCalculator.Rotate(new Position2D(rect.Location.X + rect.Width / 2, rect.Location.Y + rect.Height / 2), renderTarget.Transform, -90, System.Drawing.Drawing2D.MatrixOrder.Append);
            renderTarget.DrawText(text, textFormat, rect, brush);
            //  renderTarget.Transform = MatrixCalculator.Rotate(new Position2D(rect.Location.X + rect.Width / 2, rect.Location.Y + rect.Height / 2), renderTarget.Transform, 90, System.Drawing.Drawing2D.MatrixOrder.Append);
            textFormat.Dispose();
        }

    }


}

