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
    public class RobotWidget : WidgetBase
    {

        #region Variables

        private Dictionary<int, RobotShape> _robotsShape = new Dictionary<int, RobotShape>();
        private RobotInfo _selectedRobot;
        private System.Drawing.PointF ZeroPoint = new System.Drawing.PointF(0, 0);
        #endregion

        #region Constructor

        public RobotWidget()
        {
            // _drawState = new DrawState();
            try { this._drawState = ProjectCommons.config.Viewer.RobotWidget; }
            catch { this._drawState = new DrawState(); }
        }

        #endregion

        #region Methodes

        public override void Paint(WindowRenderTarget renderTarget)
        {
            if (!_visible)
                return;
            if (MouseData.SelectedObject != null && MouseData.SelectedObject.GetType() == typeof(RobotInfo))
                _selectedRobot = (RobotInfo)MouseData.SelectedObject;
            else
                _selectedRobot = null;
            if (ProjectCommons.Drawing2D.BaseBody.lastUpdated != ProjectCommons.Drawing2D.lastMapUpdated)
                ProjectCommons.Drawing2D.BaseBody.Initiate();

            List<RobotShape> rShapes;
            lock (((ICollection)_robotsShape).SyncRoot)
            {
                rShapes = new List<RobotShape>(_robotsShape.Values);
                int count = rShapes.Count;
                for (int i = 0; i < count; i++)
                    drawRobot(renderTarget, rShapes[i]);
            }
        }
        private void drawRobot(WindowRenderTarget renderTarget, RobotShape currentShape)
        {
            bool isSelected = (_selectedRobot != null && _selectedRobot.MountIndex == currentShape.RobotInfo.MountIndex);
            Brush backColor, borderColor;
            float borderWide;
            if (isSelected)
            {
                backColor = this._drawState.Brush_SelectedBackColor;
                borderColor = this._drawState.Brush_SelectedBorderColor;
                borderWide = this._drawState.SelectedBorderWide;
            }
            else
            {
                backColor = this._drawState.Brush_BackColor;
                borderColor = this._drawState.Brush_BorderColor;
                borderWide = this._drawState.BorderWide;
            }
            if (ProjectCommons.Drawing2D.BaseBody.lastUpdated != ProjectCommons.Drawing2D.lastMapUpdated)
            {
                currentShape.CanvasPose = ProjectCommons.Drawing2D.ChangeRealToCanvas(currentShape.RealPose);
            }
            ProjectCommons.Drawing2D.BaseBody.ellipse.Center = ZeroPoint;
            //renderTarget.Transform = Matrix3x2.Identity;
            renderTarget.Transform = ProjectCommons.Drawing2D.changePositionMatrix((float)currentShape.CanvasPose.X, (float)currentShape.CanvasPose.Y, (float)currentShape.CanvasPose.Rotation);
            renderTarget.FillEllipse(backColor, ProjectCommons.Drawing2D.BaseBody.ellipse);
            renderTarget.DrawEllipse(borderColor, ProjectCommons.Drawing2D.BaseBody.ellipse, borderWide);
            renderTarget.DrawLine(borderColor, ProjectCommons.Drawing2D.BaseBody.line.x1, ProjectCommons.Drawing2D.BaseBody.line.y1,
                                             ProjectCommons.Drawing2D.BaseBody.line.x2, ProjectCommons.Drawing2D.BaseBody.line.y2);
        }
        private RobotShape getShape(RobotInfo robot)
        {
            RobotShape ret = new RobotShape();
            ret.RealPose = new Pose3D();
            ret.RobotInfo = new RobotInfo(robot);
            return ret;
        }

        public void updateRobot(int mountIndex, Pose3D p)
        {
            RobotShape rs = null;
            lock (((ICollection)_robotsShape).SyncRoot)
            {
                if (_robotsShape.ContainsKey(mountIndex))
                {
                    rs = _robotsShape[mountIndex];
                    _robotsShape.Remove(mountIndex);
                    MouseData.removeObjectFromCollection(rs);
                }
                if (rs == null)
                {

                    int count = ProjectCommons.config.botInfo.Count;
                    for (int i = 0; i < count; i++)
                        if (ProjectCommons.config.botInfo[i].MountIndex == mountIndex)
                        {
                            rs = getShape(ProjectCommons.config.botInfo[i]);
                            break;
                        }
                }
                if (rs == null)
                    return;
                rs.RealPose = p;
                rs.CanvasPose = ProjectCommons.Drawing2D.ChangeRealToCanvas(p);
                rs.RobotInfo.Position3D = new Vector3(p.X, p.Y, p.Z);
                _robotsShape.Add(mountIndex, rs);
                MouseData.addObjectToCollection(rs);
            }
        }

        public override void recreateShape()
        {
            List<RobotShape> rShapes = null;
            lock (((ICollection)_robotsShape).SyncRoot)
            {
                rShapes = new List<RobotShape>(_robotsShape.Values);
                if (rShapes == null)
                    return;
                int count = rShapes.Count;
                for (int i = 0; i < count; i++)
                {
                    rShapes[i].CanvasPose = ProjectCommons.Drawing2D.ChangeRealToCanvas(rShapes[i].RealPose);
                }
                //createBasicShape(_robotRealRadius);
            }
        }
        private RobotShape contain(System.Drawing.Point point)
        {
            try
            {
                System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
                m.Translate(-ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusX,
                            -ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusY, System.Drawing.Drawing2D.MatrixOrder.Append);
                List<RobotShape> rShapes = null;
                lock (((ICollection)_robotsShape).SyncRoot)
                {
                    rShapes = new List<RobotShape>(_robotsShape.Values);
                    int count = rShapes.Count;
                    for (int i = 0; i < count; i++)
                    {
                        System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();

                        gp.AddEllipse(0, 0, 2 * ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusX, 2 * ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusY);
                        gp.Transform(m);
                        gp.Transform(ProjectCommons.Drawing2D.changePositionMatrix(rShapes[i].CanvasPose.X, rShapes[i].CanvasPose.Y));

                        System.Drawing.Rectangle rec = new System.Drawing.Rectangle(point.X, point.Y, 2, 2);

                        System.Drawing.Region f = new System.Drawing.Region(gp);
                        f.Intersect(rec);
                        if (f.Equals(new System.Drawing.Region(rec), ProjectCommons.Drawing2D.tempGraphic))
                            return rShapes[i];
                    }
                }
            }
            catch
            { ProjectCommons.writeConsoleMessage("EXCEPTION TO MOVE", ConsoleMessageType.Error); }

            return null;
        }
        #endregion

        #region Property

        public Dictionary<int, RobotShape> RobotsShape
        {
            get { return _robotsShape; }
            private set { _robotsShape = value; }
        }
        public RobotInfo SelectedRobot
        {
            get { return _selectedRobot; }
            set { _selectedRobot = value; }
        }
        public override WidgetTypes Type
        {
            get { return WidgetTypes.Robot; }
        }
        public List<RobotShape> TotalRobot
        {
            get { return new List<RobotShape>(_robotsShape.Values); }
        }

        #endregion

        #region events

        public override bool mouseClicked(EventArgs e)
        {
            return true;
        }

        public override bool mouseMoved(MouseEventArgs e)
        {

          //  ProjectCommons.writeConsoleMessage("OMAD", ConsolMessageType.Information);
            if (IsRaseHintNull) return true;
            RobotShape rShape = contain(MouseDataBase.CurrentPoint_Scaled);
            if (rShape != null)
            {
         //       ProjectCommons.writeConsoleMessage("NULL NaBOOD ", ConsolMessageType.Information);

                RaiseHint(rShape.getHint(), MouseDataBase.CurrentPoint_Scaled);
                return true;
            }
            return false;
        }

        public override bool mouseDClicked(EventArgs e) { return true; }
        public override bool mouseDown(MouseEventArgs e)
        {
            if (MouseData.SelectedObject != null) return false;

            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                RobotShape rShape = contain(MouseDataBase.CurrentPoint_Scaled);

                if (rShape != null)
                {
                    _selectedRobot = rShape.RobotInfo;
                    MouseData.SelectedObject = rShape.RobotInfo;
                    MouseData.SelectedShape = rShape;
                }
                return true;
            }
            return false;
        }
        public override bool mouseUp(MouseEventArgs e) { return true; }
        public override bool mouseWheelMoved(MouseEventArgs e) { return true; }

        public override bool keyDown(KeyEventArgs e) { return true; }
        public override bool keyUp(KeyEventArgs e) { return true; }

        #endregion

    }
}
