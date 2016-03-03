using System;
using System.Windows.Forms;
using MRL.Commons;
using MRL.Components.Tools.Shapes;
using MRL.CustomMath;
using MRL.Utils;
using SlimDX.Direct2D;

namespace MRL.Components.Tools.Widgets
{
    public class ComStationWidget : WidgetBase
    {
        #region Variables

        private RobotShape comStation;
        private RobotInfo _selectedCS;

        private Vector3 Vec3Zero = new Vector3();
        private System.Drawing.PointF ZeroPoint = new System.Drawing.PointF(0, 0);

        #endregion

        #region Constructor

        public ComStationWidget()
        {
            this._drawState = new DrawState();
            this._drawState.BorderWide = 0.3f;
            this._drawState.SelectedBorderWide = 0.3f;
        }

        #endregion

        #region Methods

        public override void recreateShape()
        {
            lock (comStation)
            { comStation.CanvasPose = ProjectCommons.Drawing2D.ChangeRealToCanvas(comStation.RealPose); }
        }

        public override void Paint(WindowRenderTarget renderTarget)
        {
            if (!_visible)
                return;

            if (comStation == null)
                updateComStation();

            if (MouseData.SelectedObject != null && MouseData.SelectedObject.GetType() == typeof(RobotInfo) && ((RobotInfo)MouseData.SelectedObject).Name == ProjectCommons.config.mapperInfo.Name)
                _selectedCS = (RobotInfo)MouseData.SelectedObject;
            else
                _selectedCS = null;

            ProjectCommons.Drawing2D.BaseBody.ellipse.Center = ZeroPoint;
            lock (comStation)
            { drawStation(renderTarget, comStation); }
        }
        private void drawStation(WindowRenderTarget renderTarget, RobotShape currentShape)
        {
            bool isSelected = (_selectedCS != null && _selectedCS.MountIndex == currentShape.RobotInfo.MountIndex);
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
                try { comStation.CanvasPose = ProjectCommons.Drawing2D.ChangeRealToCanvas(comStation.RealPose); }
                catch { return; }
                ProjectCommons.Drawing2D.BaseBody.Initiate();
            }
            renderTarget.Transform = ProjectCommons.Drawing2D.changePositionMatrix((float)currentShape.CanvasPose.X, (float)currentShape.CanvasPose.Y, (float)currentShape.CanvasPose.Rotation);

            renderTarget.FillEllipse(backColor, ProjectCommons.Drawing2D.BaseBody.ellipse);
            renderTarget.DrawEllipse(borderColor, ProjectCommons.Drawing2D.BaseBody.ellipse);
        }
        public void updateComStation()
        {
            comStation = new RobotShape();
            comStation.RobotInfo = ProjectCommons.config.mapperInfo;

            comStation.RealPose = new Pose3D(ProjectCommons.config.mapperInfo.Position3D, Vec3Zero);
        }
        private bool contain(System.Drawing.Point point)
        {
            try
            {
                System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
                m.Translate(-ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusX,
                            -ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusY, System.Drawing.Drawing2D.MatrixOrder.Append);
                System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();

                gp.AddEllipse(0, 0, 2 * ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusX, 2 * ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusY);
                gp.Transform(m);
                gp.Transform(ProjectCommons.Drawing2D.changePositionMatrix(comStation.CanvasPose.X, comStation.CanvasPose.Y));

                System.Drawing.Rectangle rec = new System.Drawing.Rectangle(point.X, point.Y, 2, 2);

                System.Drawing.Region f = new System.Drawing.Region(gp);
                f.Intersect(rec);
                bool isEqual = f.Equals(new System.Drawing.Region(rec), ProjectCommons.Drawing2D.tempGraphic);
                return isEqual;
            }
            catch
            { ProjectCommons.writeConsoleMessage("EXCEPTION TO MOVE", ConsoleMessageType.Error); }

            return false;
        }
        #endregion

        #region Property

        public override WidgetTypes Type
        {
            get { return WidgetTypes.ComStation; }
        }

        #endregion

        #region Events

        public override bool mouseClicked(EventArgs e) { return true; }
        public override bool mouseDClicked(EventArgs e) { return true; }
        public override bool mouseDown(MouseEventArgs e) { return true; }
        public override bool mouseUp(MouseEventArgs e) { return true; }
        public override bool mouseMoved(MouseEventArgs e)
        {
            if (IsRaseHintNull) return true;
            if (contain(MouseDataBase.CurrentPoint_Scaled))
            {
                RaiseHint(comStation.getHint(), MouseDataBase.CurrentPoint_Scaled);
                return true;
            }
            return false;
        }
        public override bool mouseWheelMoved(MouseEventArgs e) { return true; }
        public override bool keyDown(KeyEventArgs e) { return true; }
        public override bool keyUp(KeyEventArgs e) { return true; }

        #endregion
    }
}
