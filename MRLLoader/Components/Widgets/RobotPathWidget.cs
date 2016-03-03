using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using MRL.Commons;
using MRL.Components.Tools.Shapes;
using MRL.CustomMath;
using MRL.Utils;
using SlimDX.Direct2D;

namespace MRL.Components.Tools.Widgets
{
    public class RobotPathWidget : WidgetBase
    {
        #region Variables

        private Dictionary<int, RobotPath> _robotsPath = new Dictionary<int, RobotPath>();
        #endregion

        #region Constructor

        public RobotPathWidget()
        {
            try { this._drawState = ProjectCommons.config.Viewer.RobotPathWidget; }
            catch { this._drawState = new DrawState(); }
        }

        #endregion

        #region Methods

        public override void Paint(WindowRenderTarget renderTarget)
        {
            if (!_visible)
                return;

            List<RobotPath> rPaths;
            lock (((ICollection)_robotsPath).SyncRoot)
            {
                rPaths = new List<RobotPath>(_robotsPath.Values);
                int count = rPaths.Count;
                for (int i = 0; i < count; i++)
                    drawPath(renderTarget, rPaths[i]);
            }
        }
        private void drawPath(WindowRenderTarget renderTarget, RobotPath rp)
        {
            Brush borderColor = this._drawState.Brush_BorderColor;
            float borderWide = this._drawState.BorderWide;

            List<Pose2D> cur = rp.RealPath;
            int count = cur.Count;
            for (int i = 1; i < count; i++)
            {
                int j = i - 1;
                if (j >= 0)
                {
                    Pose2D p1 = ProjectCommons.Drawing2D.ChangeRealToCanvas(cur[i]);
                    Pose2D p2 = ProjectCommons.Drawing2D.ChangeRealToCanvas(cur[j]);
                    renderTarget.DrawLine(borderColor, p1, p2, borderWide);
                }
            }
        }
        public override void recreateShape()
        {
            lock (((ICollection)_robotsPath).SyncRoot)
            {
                foreach (RobotPath rp in _robotsPath.Values)
                {
                    rp.CanvasPath.Clear();
                    foreach (Pose2D p in rp.RealPath)
                        rp.CanvasPath.Add(ProjectCommons.Drawing2D.ChangeRealToCanvas(p));
                }
            }
        }
        public void addPosition(int robotIndex, Pose3D p)
        {
            RobotPath rp;
            if (_robotsPath.ContainsKey(robotIndex))
                rp = _robotsPath[robotIndex];
            else
            {
                rp = new RobotPath();
                rp.RobotInfo = ProjectCommons.config.botInfo[robotIndex];
                _robotsPath.Add(robotIndex, rp);
            }
            if (rp.RealPath.Count > 0)
            {
                Pose2D p1 = rp.RealPath[rp.RealPath.Count - 1];
                double dis = MathHelper.getDistance(p, p1);

                if (dis > 0.5)
                {
                    rp.CanvasPath.Add(ProjectCommons.Drawing2D.ChangeRealToCanvas(p));
                    rp.RealPath.Add(p);
                }
            }
            else
            {
                rp.CanvasPath.Add(ProjectCommons.Drawing2D.ChangeRealToCanvas(p));
                rp.RealPath.Add(p);
            }
        }

        #endregion

        #region Property

        public override WidgetTypes Type
        {
            get { return WidgetTypes.RobotPath; }
        }
        public List<RobotPath> TotalPath
        {
            get { return new List<RobotPath>(_robotsPath.Values); }
        }

        #endregion


        #region events

        public override bool mouseClicked(EventArgs e) { return true; }
        public override bool mouseDClicked(EventArgs e) { return true; }
        public override bool mouseDown(MouseEventArgs e) { return true; }
        public override bool mouseUp(MouseEventArgs e) { return true; }
        public override bool mouseMoved(MouseEventArgs e) { return true; }
        public override bool mouseWheelMoved(MouseEventArgs e) { return true; }

        public override bool keyDown(KeyEventArgs e) { return true; }
        public override bool keyUp(KeyEventArgs e) { return true; }

        #endregion
    }

    public class Lines : WidgetBase
    {

        #region  Variables

        float wide = -1f;

        #endregion

        #region Methods

        public Dictionary<string, List<Pose2D>> lines = new Dictionary<string, List<Pose2D>>();

        public Communication.Tools.Signal signal;

        public override void Paint(WindowRenderTarget renderTarget)
        {
            if (wide == -1)
                wide = ProjectCommons.Drawing2D.ChangeRealLenghtToCanvasLenght(1);

            float x = MouseDataBase.DrawPoint.X + MouseDataBase.Offset.X
                , y = MouseDataBase.DrawPoint.Y + MouseDataBase.Offset.Y;
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;

            SolidColorBrush brush = new SolidColorBrush(renderTarget, System.Drawing.Color.OrangeRed);
            System.Drawing.PointF p1 = new System.Drawing.PointF(0f, 0f), p2 = new System.Drawing.PointF(wide, 0f);

            renderTarget.Transform = ProjectCommons.Drawing2D.changePositionMatrix_noneWorld(x, y);

            foreach (var item in lines)
            {
                if (signal.Value > 75)
                    brush.Color = System.Drawing.Color.FromArgb(255, 0, 255, 0);
                else if (signal.Value > 50)
                    brush.Color = System.Drawing.Color.FromArgb(255, 255, 0, 0);
                else if (signal.Value > 25)
                    brush.Color = System.Drawing.Color.FromArgb(255, 0, 0, 255);
                else
                    brush.Color = System.Drawing.Color.FromArgb(255, 0, 255, 255);

                Pose2D old = null;
                foreach (var li in item.Value)
                {
                    if (old == null)
                    {
                        old = li;
                    }
                    else
                    {
                        renderTarget.DrawLine(brush, ProjectCommons.Drawing2D.ChangeRealToCanvas(old), ProjectCommons.Drawing2D.ChangeRealToCanvas(li), 2);
                        old = li;
                    }
                }
            }
        }
        public override void recreateShape()
        {
            //wide = change3DLenghtTo2DLenght(1);
        }

        #endregion

        #region Property

        public override WidgetTypes Type
        {
            get { return WidgetTypes.lines; }
        }

        #endregion

        #region events

        public override bool mouseClicked(EventArgs e) { return true; }
        public override bool mouseDClicked(EventArgs e) { return true; }
        public override bool mouseDown(MouseEventArgs e) { return true; }
        public override bool mouseUp(MouseEventArgs e) { return true; }
        public override bool mouseMoved(MouseEventArgs e) { return true; }
        public override bool mouseWheelMoved(MouseEventArgs e) { return true; }

        public override bool keyDown(KeyEventArgs e) { return true; }
        public override bool keyUp(KeyEventArgs e) { return true; }

        #endregion

    }
}
