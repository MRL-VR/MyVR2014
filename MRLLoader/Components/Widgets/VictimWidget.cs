using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MRL.Commons;
using MRL.Components.Tools.Objects;
using MRL.Components.Tools.Shapes;
using MRL.CustomMath;
using MRL.Utils;
using SlimDX.Direct2D;

namespace MRL.Components.Tools.Widgets
{
    public class VictimWidget : WidgetBase
    {
        #region Variables

        private Dictionary<int, VictimShape> _victimsShape = new Dictionary<int, VictimShape>();
        private Victim _selectedVictim;
        private static GraphicsPath _victimBody;
        private static float _victimRealRadius = 0.3f;
        private int userVictimAddedIndex = 0;
        private volatile int _victimIndex = 1;
        private int panoSelectedVictim = -1;
        private SolidColorBrush panoSelectedBackColor = null;
        private SolidColorBrush panoSelectedBorderColor = null;

        #endregion

        #region Constructor

        public VictimWidget()
        {
            try { this._drawState = ProjectCommons.config.Viewer.VictimWidget; }
            catch { this._drawState = new DrawState(); }
            ProjectCommons.panoViewer_OnVictimSelected += new ProjectCommons._panoViewer_OnVictimSelected(PanoVictimSelectedChange);
        }
        #endregion

        #region Methodes

        public override void Paint(WindowRenderTarget renderTarget)
        {
            if (!_visible) return;
            if (panoSelectedBackColor == null)
            {
                panoSelectedBackColor = new SolidColorBrush(ProjectCommons.Drawing2D.windowRenderTarget, System.Drawing.Color.Yellow);
                panoSelectedBorderColor = new SolidColorBrush(ProjectCommons.Drawing2D.windowRenderTarget, System.Drawing.Color.Red);
            }
            try { _selectedVictim = (Victim)MouseData.SelectedObject; }
            catch { _selectedVictim = null; }
            if (ProjectCommons.Drawing2D.BaseBody.lastUpdated != ProjectCommons.Drawing2D.lastMapUpdated)
                ProjectCommons.Drawing2D.BaseBody.Initiate();
            List<VictimShape> vShapes;
            lock (((ICollection)_victimsShape).SyncRoot)
            {
                vShapes = new List<VictimShape>(_victimsShape.Values);
            }
            int count = vShapes.Count;
            for (int i = 0; i < count; i++)
                drawVictim(renderTarget, vShapes[i]);
        }
        protected void drawVictim(WindowRenderTarget renderTarget, VictimShape currentShape)
        {
            bool isSelected = (_selectedVictim != null && _selectedVictim.ID == currentShape.VictimInfo.ID);
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
            if (currentShape.VictimInfo.ID == panoSelectedVictim)
            {
                backColor = panoSelectedBackColor;
                borderColor = panoSelectedBorderColor;
                borderWide = 0.5f;
            }
            if (ProjectCommons.Drawing2D.BaseBody.lastUpdated != ProjectCommons.Drawing2D.lastMapUpdated)
            {
                currentShape.CanvasPose = ProjectCommons.Drawing2D.ChangeRealToCanvas(currentShape.RealPose);
            }
            renderTarget.Transform = ProjectCommons.Drawing2D.changePositionMatrix((float)currentShape.CanvasPose.X, (float)currentShape.CanvasPose.Y);
            renderTarget.FillEllipse(backColor, ProjectCommons.Drawing2D.BaseBody.ellipse);
            renderTarget.DrawEllipse(borderColor, ProjectCommons.Drawing2D.BaseBody.ellipse);
        }
        private VictimShape getShape(Victim v)
        {
            VictimShape ret = new VictimShape();
            ret.VictimInfo = v;
            return ret;
        }
        public void updateVictim(int index, Pose3D p, double probability)
        {
            lock (((ICollection)_victimsShape).SyncRoot)
            {
                VictimShape vShape = null;
                if (_victimsShape.ContainsKey(index))
                {
                    vShape = _victimsShape[index];
                    _victimsShape.Remove(index);
                    MouseData.removeObjectFromCollection(vShape);
                }
                if (vShape == null)
                {
                    Victim v = new Victim(index, "Victim" + _victimIndex++, "", "", 0);
                    vShape = getShape(v);
                }
                if (vShape == null)
                    return;
                vShape.RealPose = p;
                vShape.CanvasPose = ProjectCommons.Drawing2D.ChangeRealToCanvas(p);
                vShape.VictimInfo.Position = p.ToString(2);
                vShape.VictimInfo.Probability = probability;

                _victimsShape.Add(index, vShape);
                MouseData.addObjectToCollection(vShape);

                if (ProjectCommons.imageWidget_OnVictimUpdated != null)
                    ProjectCommons.imageWidget_OnVictimUpdated(vShape);
            }
        }
        public void addVictim(System.Drawing.Point p2D)
        {
            int index = int.Parse("12" + userVictimAddedIndex.ToString());
            userVictimAddedIndex++;
            VictimShape vShape = null;
            if (_victimsShape.ContainsKey(index))
            {
                vShape = _victimsShape[index];
                _victimsShape.Remove(index);
                MouseData.removeObjectFromCollection(vShape);
            }
            if (vShape == null)
            {
                Victim v = new Victim(index, "Victim" + _victimIndex++, "", "", 0);
                vShape = getShape(v);
            }
            if (vShape == null)
                return;
            Pose2D p = new Pose2D(p2D.X, p2D.Y, 0);
            vShape.CanvasPose = p;
            vShape.RealPose = ProjectCommons.Drawing2D.ChangeCanvasToReal(p);
            vShape.VictimInfo.Position = vShape.RealPose.ToString(2);
            vShape.VictimInfo.Probability = 1.0;
            _victimsShape.Add(index, vShape);
            MouseData.addObjectToCollection(vShape);

            if (ProjectCommons.imageWidget_OnVictimUpdated != null)
                ProjectCommons.imageWidget_OnVictimUpdated(vShape);
        }

        public void removeAllVictims()
        {
            _victimsShape.Clear();
            userVictimAddedIndex = 0;
        }

        private void PanoVictimSelectedChange(int ID)
        {
            panoSelectedVictim = ID;
        }
        public override void recreateShape()
        {
            List<VictimShape> vShapes = null;
            lock (((ICollection)_victimsShape).SyncRoot)
            {
                vShapes = new List<VictimShape>(_victimsShape.Values);
            }
            if (vShapes == null)
                return;
            int count = vShapes.Count;
            for (int i = 0; i < count; i++)
            {
                vShapes[i].CanvasPose = ProjectCommons.Drawing2D.ChangeRealToCanvas(vShapes[i].RealPose);
            }
        }

        private VictimShape contain(System.Drawing.Point point)
        {
            try
            {
                System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
                m.Translate(-ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusX,
                            -ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusY, System.Drawing.Drawing2D.MatrixOrder.Append);
                List<VictimShape> vShapes = null;
                lock (((ICollection)_victimsShape).SyncRoot)
                {
                    vShapes = new List<VictimShape>(_victimsShape.Values);
                    int count = vShapes.Count;
                    for (int i = 0; i < count; i++)
                    {
                        System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();

                        gp.AddEllipse(0, 0, 2 * ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusX, 2 * ProjectCommons.Drawing2D.BaseBody.ellipse.RadiusY);
                        gp.Transform(m);
                        gp.Transform(ProjectCommons.Drawing2D.changePositionMatrix(vShapes[i].CanvasPose.X, vShapes[i].CanvasPose.Y));

                        System.Drawing.Rectangle rec = new System.Drawing.Rectangle(point.X, point.Y, 2, 2);

                        System.Drawing.Region f = new System.Drawing.Region(gp);
                        f.Intersect(rec);
                        if (f.Equals(new System.Drawing.Region(rec), ProjectCommons.Drawing2D.tempGraphic))
                            return vShapes[i];
                    }
                }
            }
            catch
            { ProjectCommons.writeConsoleMessage("EXCEPTION TO MOVE", ConsoleMessageType.Error); }

            return null;
        }
        #endregion

        #region Property
        public override WidgetTypes Type
        {
            get { return WidgetTypes.Victim; }
        }
        public Dictionary<int, VictimShape> VictimsShape
        {
            get { return _victimsShape; }
            private set { _victimsShape = value; }
        }
        public Victim SelectedVictim
        {
            get { return _selectedVictim; }
            set { _selectedVictim = value; }
        }
        public List<VictimShape> TotalVictims
        {
            get { return new List<VictimShape>(_victimsShape.Values); }
        }

        #endregion

        #region events

        public override bool mouseClicked(EventArgs e) { return true; }
        public override bool mouseMoved(MouseEventArgs e)
        {
            if (IsRaseHintNull) return true;
            VictimShape vShape = contain(MouseDataBase.CurrentPoint_Scaled);
            if (vShape != null)
            {
                RaiseHint(vShape.getHint(), MouseDataBase.CurrentPoint_Scaled);
                return true;
            }
            return false;
        }

        public override bool mouseDClicked(EventArgs e)
        {
            //VictimShape vShape = victimContains(MouseData.CurrentPoint);

            //if (vShape != null)
            //{
            //    frmGetVictimInfo frm = new frmGetVictimInfo();
            //    frm.txtID.Text = vShape.VictimInfo.ID.ToString();
            //    frm.txtName.Text = vShape.VictimInfo.Name;
            //    frm.txtPosition2D.Text = vShape.CanvasPose.ToString();
            //    frm.txtPosition3D.Text = vShape.RealPose.ToString();
            //    frm.txtStatus.Text = vShape.VictimInfo.Status;
            //    frm.txtProbability.Text = vShape.VictimInfo.Probability.ToString();

            //    ProjectCommons.keyboardCapturedByGeoViewer = true;
            //    frm.ShowDialog();
            //    ProjectCommons.keyboardCapturedByGeoViewer = false;

            //    if (frm.DialogResult == DialogResult.OK)
            //    {
            //        vShape.VictimInfo.ID = int.Parse(frm.txtID.Text);
            //        vShape.VictimInfo.Name = frm.txtName.Text;
            //        //vShape.Pose2D = frm.txtPosition2D.Text;
            //        //vShape.Pose3D = frm.txtPosition2D.Text;
            //        vShape.VictimInfo.Status = frm.txtStatus.Text;
            //        vShape.VictimInfo.Probability = double.Parse(frm.txtProbability.Text);
            //        if (ProjectCommons.imageWidget_OnVictimUpdated != null)
            //            ProjectCommons.imageWidget_OnVictimUpdated(vShape.VictimInfo);

            //    }
            //    frm.Dispose();
            //}
            return true;
        }
        public override bool mouseDown(MouseEventArgs e)
        {
            if (MouseData.SelectedObject != null) return false;

            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                VictimShape vShape = contain(MouseDataBase.CurrentPoint_Scaled);
                if (vShape != null)
                {
                    _selectedVictim = vShape.VictimInfo;
                    MouseData.SelectedObject = vShape.VictimInfo;
                    MouseData.SelectedShape = vShape;
                }
                return true;
            }
            return false;
        }
        public override bool mouseUp(MouseEventArgs e) { return true; }
        public override bool mouseWheelMoved(MouseEventArgs e) { return true; }

        public override bool keyDown(KeyEventArgs e)
        {
            if (_selectedVictim != null && _victimsShape.ContainsKey(_selectedVictim.ID))
            {
                if (e.KeyCode == Keys.Delete)
                {
                    MouseData.removeObjectFromCollection(_victimsShape[_selectedVictim.ID]);
                    _victimsShape.Remove(_selectedVictim.ID);

                    if (ProjectCommons.imageWidget_OnVictimDeleted != null)
                        ProjectCommons.imageWidget_OnVictimDeleted(_victimsShape[_selectedVictim.ID]);

                    _selectedVictim = null;
                }
            }
            return true;
        }
        public override bool keyUp(KeyEventArgs e) { return true; }

        #endregion

    }
}
