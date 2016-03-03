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
    public class GoalPointsWidget : WidgetBase
    {
        #region Variables

        private List<GoalPointShape> _pointShapes = new List<GoalPointShape>(4); //in this collection will be points that are converted in 2D 
        private Vector3 Vec3Zero = new Vector3();

        #endregion

        #region Constructor

        public GoalPointsWidget()
        {
            //add it's special color from projectsCommons
            this._drawState = new DrawState();
            this._drawState.BorderColor = System.Drawing.Color.Red;
            this._drawState.BorderColorAlpha = 100;
            this._drawState.BackColor = System.Drawing.Color.Red;
            this._drawState.BackColorAlpha = 30;
            
            try
            {
                for (int i = ProjectCommons.config.GoalPoints.Count - 1; i >= 0; i--)
                    _pointShapes.Add(new GoalPointShape(new Pose3D(ProjectCommons.config.GoalPoints[i], Vec3Zero)));
                    //this._pointShapes.Add(null);
            }
            catch { }
        }

        #endregion

        #region Methodes

        public override void Paint(WindowRenderTarget renderTarget)
        {
            if (!Visible)
                return;
            if (ProjectCommons.Drawing2D.BaseBody.lastUpdated != ProjectCommons.Drawing2D.lastMapUpdated)
                ProjectCommons.Drawing2D.BaseBody.Initiate();
            lock (((ICollection)_pointShapes).SyncRoot)
            {
                int count = _pointShapes.Count;
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        if (ProjectCommons.Drawing2D.BaseBody.lastUpdated != ProjectCommons.Drawing2D.lastMapUpdated)
                        {
                            Pose2D p2 = _pointShapes[i].RealPose;
                            _pointShapes[i].CanvasPose = ProjectCommons.Drawing2D.ChangeRealToCanvas(p2);
                        }
                    }
                    catch { ProjectCommons.writeConsoleMessage("EXCEPTION", ConsoleMessageType.Exclamation); }

                    drawPoint(renderTarget, _pointShapes[i]);
                }
            }
        }
        public void drawPoint(WindowRenderTarget renderTarget, GoalPointShape gps)
        {
            Brush borderBrush = this._drawState.Brush_BorderColor;
            Brush backBrush = this._drawState.Brush_BackColor;
            renderTarget.Transform = ProjectCommons.Drawing2D.changePositionMatrix((float)gps.CanvasPose.X, (float)gps.CanvasPose.Y);
            renderTarget.DrawEllipse(borderBrush, ProjectCommons.Drawing2D.BaseBody.ellipse);
            renderTarget.FillEllipse(backBrush, ProjectCommons.Drawing2D.BaseBody.ellipse);
        }
        public override void recreateShape()
        {
            //int count = ProjectCommons.config.GoalPoints.Count;
            //for (int i = 0; i < count; i++)
            //{
            //    Pose2D t = new Pose2D(ProjectCommons.config.GoalPoints[i].X,ProjectCommons.config.GoalPoints[i].Y, 0);
            //    Pose2D p = ProjectCommons.Drawing2D.ChangeRealToCanvas(t);
            //    this._pointShapes[i] = p;
            //}
        }
        private GoalPointShape contain(System.Drawing.Point point)
        {
            try
            {
                System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
                m.Translate(-ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusX,
                            -ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusY, System.Drawing.Drawing2D.MatrixOrder.Append);
                List<GoalPointShape> gpShapes = null;
                lock (((ICollection)_pointShapes).SyncRoot)
                {
                    gpShapes = new List<GoalPointShape>(_pointShapes);
                    int count = gpShapes.Count;
                    for (int i = 0; i < count; i++)
                    {
                        System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();

                        gp.AddEllipse(0, 0, 2 * ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusX, 2 * ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusY);
                        gp.Transform(m);
                        gp.Transform(ProjectCommons.Drawing2D.changePositionMatrix(gpShapes[i].CanvasPose.X, gpShapes[i].CanvasPose.Y));

                        System.Drawing.Rectangle rec = new System.Drawing.Rectangle(point.X, point.Y, 2, 2);

                        System.Drawing.Region f = new System.Drawing.Region(gp);
                        f.Intersect(rec);
                        if (f.Equals(new System.Drawing.Region(rec), ProjectCommons.Drawing2D.tempGraphic))
                            return gpShapes[i];
                    }
                }
            }
            catch
            { ProjectCommons.writeConsoleMessage("EXCEPT----------ION TO MOVE", ConsoleMessageType.Error); }

            return null;
        }
        #endregion

        #region Property

        public override WidgetTypes Type
        {
            get { return WidgetTypes.GoalPoint; }
        }

        #endregion

        #region events

        public override bool mouseClicked(EventArgs e) { return true; }
        public override bool mouseDClicked(EventArgs e) { return true; }
        public override bool mouseDown(MouseEventArgs e) { return true; }
        public override bool mouseUp(MouseEventArgs e) { return true; }
        public override bool mouseMoved(MouseEventArgs e) 
        {
            if (IsRaseHintNull) return true;
            GoalPointShape gpShape = contain(MouseDataBase.CurrentPoint_Scaled);

            if (gpShape != null)
            {
                RaiseHint(gpShape.getHint(), MouseDataBase.CurrentPoint_Scaled);
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
