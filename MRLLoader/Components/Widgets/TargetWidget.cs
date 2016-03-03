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
    public class TargetWidget : WidgetBase
    {
        #region Variables

        private Dictionary<int, TargetShape> _targetsShape = new Dictionary<int, TargetShape>();
        private RobotInfo _selectedRobot;
        private Keys _shiftKey = Keys.D;
        private bool _isDrawing = false;
        private DrawState _drawState;
        private System.Drawing.PointF ZeroPoint = new System.Drawing.PointF(0, 0);

        #endregion

        #region Constructor

        public TargetWidget()
        {
            try
            {
                this._drawState = new DrawState();

                this._drawState.SelectedBackColor = System.Drawing.Color.Red;
                this._drawState.SelectedBorderColor = System.Drawing.Color.Black;

                this._drawState.BorderColor = System.Drawing.Color.Black;
                this._drawState.BorderColorAlpha = 100;
                this._drawState.BackColor = System.Drawing.Color.Brown;
                this._drawState.BackColorAlpha = 100;

            }
            catch { this._drawState = new DrawState(); }
        }

        #endregion

        #region Method
        public void addTarget(RobotInfo robot, Pose2D robotPos, System.Drawing.Point goalPos)
        {
            Pose2D p = new Pose2D(goalPos.X, goalPos.Y, 0);

            try
            {
                TargetShape newTarget = new TargetShape();
                newTarget.RobotInfo = robot;
                newTarget.TargetPosition = ProjectCommons.Drawing2D.ChangeCanvasToReal(p);
                newTarget.CanvasPose = p;
                _targetsShape.Add(robot.MountIndex, newTarget);

                ProjectCommons.targetWidget_OnNewPointReceived(robot.MountIndex, robotPos, newTarget.TargetPosition);
            }

            catch { _targetsShape[robot.MountIndex].TargetPosition = p; }
        }

        public void clearTarget(RobotInfo robot)
        {
            try
            {
                _targetsShape.Remove(robot.MountIndex);
            }
            catch { }
        }
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

            List<TargetShape> rShapes;
            lock (((ICollection)_targetsShape).SyncRoot)
            {
                rShapes = new List<TargetShape>(_targetsShape.Values);
                int count = rShapes.Count;
                for (int i = 0; i < count; i++)
                {
                    drawTarget(renderTarget, rShapes[i]);
                }
            }

        }
        private void drawTarget(WindowRenderTarget renderTarget, TargetShape currentShape)
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
            //renderTarget.Transform = ProjectCommons.Drawing2D.changePositionMatrix((float)currentShape.CanvasPose.X, (float)currentShape.CanvasPose.Y, (float)currentShape.CanvasPose.Rotation);
            renderTarget.Transform = ProjectCommons.Drawing2D.changePositionMatrix((float)currentShape.CanvasPose.X, (float)currentShape.CanvasPose.Y);
            
            renderTarget.FillEllipse(backColor, ProjectCommons.Drawing2D.BaseBody.ellipse);
            renderTarget.DrawEllipse(borderColor, ProjectCommons.Drawing2D.BaseBody.ellipse, borderWide);
        }
        private TargetShape getShape(RobotInfo robot)
        {
            TargetShape ret = _targetsShape[robot.MountIndex];
            return ret;
        }
        public override void recreateShape()
        {
            List<TargetShape> tShapes = null;
            lock (((ICollection)_targetsShape).SyncRoot)
            {
                tShapes = new List<TargetShape>(_targetsShape.Values);
                if (tShapes == null)
                    return;
                int count = tShapes.Count;
                for (int i = 0; i < count; i++)
                {
                    tShapes[i].CanvasPose = ProjectCommons.Drawing2D.ChangeRealToCanvas(tShapes[i].RealPose);
                }
                //createBasicShape(_robotRealRadius);
            }
        }
        private TargetShape contain(System.Drawing.Point point)
        {
            try
            {
                System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
                m.Translate(-ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusX,
                            -ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusY, System.Drawing.Drawing2D.MatrixOrder.Append);
                List<TargetShape> tShapes = null;
                lock (((ICollection)_targetsShape).SyncRoot)
                {
                    tShapes = new List<TargetShape>(_targetsShape.Values);
                    int count = tShapes.Count;
                    for (int i = 0; i < count; i++)
                    {
                        System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();

                        gp.AddEllipse(0, 0, 2 * ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusX, 2 * ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusY);
                        gp.Transform(m);
                        gp.Transform(ProjectCommons.Drawing2D.changePositionMatrix(tShapes[i].CanvasPose.X, tShapes[i].CanvasPose.Y));

                        System.Drawing.Rectangle rec = new System.Drawing.Rectangle(point.X, point.Y, 2, 2);

                        System.Drawing.Region f = new System.Drawing.Region(gp);
                        f.Intersect(rec);
                        if (f.Equals(new System.Drawing.Region(rec), ProjectCommons.Drawing2D.tempGraphic))
                            return tShapes[i];
                    }
                }
            }
            catch
            { ProjectCommons.writeConsoleMessage("EXCEPTION TO MOVE", ConsoleMessageType.Error); }

            return null;
        }

        #endregion

        #region Property
        public RobotInfo SelectedRobot
        {
            get { return _selectedRobot; }
            set { _selectedRobot = value; }
        }
        public override WidgetTypes Type
        {
            get { return WidgetTypes.Robot; }
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
            TargetShape tShape = contain(MouseDataBase.CurrentPoint_Scaled);
            if (tShape != null)
            {
                RaiseHint(tShape.getHint(), MouseDataBase.CurrentPoint_Scaled);
                return true;
            }
            return false;
        }

        public override bool mouseDClicked(EventArgs e) { return true; }
        public override bool mouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TargetShape tShape = contain(MouseDataBase.CurrentPoint_Scaled);

                if (tShape != null)
                {
                    _selectedRobot = tShape.RobotInfo;
                    MouseData.SelectedObject = tShape.RobotInfo;
                    MouseData.SelectedShape = tShape;
                }
                else if ((_selectedRobot != null) && _isDrawing)
                {
                    addTarget(_selectedRobot, _selectedRobot.Position3D, MouseDataBase.CurrentPoint_Scaled);

                }
                return true;
            }
            return false;
        }
        public override bool mouseUp(MouseEventArgs e) { return true; }
        public override bool mouseWheelMoved(MouseEventArgs e) { return true; }

        public override bool keyDown(KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == _shiftKey && MouseData.SelectedObject != null && !_isDrawing)
                {
                    if (!(_targetsShape.ContainsKey(_selectedRobot.MountIndex)))
                    {

                        _isDrawing = true;
                        MouseData.ForceForNoChangeSelectedObject = true;
                        ProjectCommons.keyboardCapturedByGeoViewer = true;
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    if (_selectedRobot != null)
                    {
                        List<Pose2D> p = new List<Pose2D>();
                        _targetsShape.Remove(_selectedRobot.MountIndex);
                        if (ProjectCommons.targetWidget_OnPointRemoved != null)
                            ProjectCommons.targetWidget_OnPointRemoved(_selectedRobot.MountIndex);
                    }
                }
            }
            catch { _selectedRobot = null; }

            return true;


        }
        public override bool keyUp(KeyEventArgs e)
        {

            if (e.KeyCode == _shiftKey)
            {
                _isDrawing = false;
                ProjectCommons.keyboardCapturedByGeoViewer = false;
            }
            if (e.KeyCode == _shiftKey || e.KeyCode == Keys.Delete)
            {
                MouseData.SelectedObject = null;
                MouseData.ForceForNoChangeSelectedObject = false;
            }

            return true;
        }
        #endregion

    }
}
