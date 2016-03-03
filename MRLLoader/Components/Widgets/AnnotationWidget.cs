using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using MRL.Commons;
using MRL.Components.Tools.Objects;
using MRL.Components.Tools.Shapes;
using MRL.CustomMath;
using MRL.Utils;
using SlimDX.Direct2D;

namespace MRL.Components.Tools.Widgets
{

    /**  --------------Not Completed
        * Allows the user to make arbitrary polygons and annotate them on the map.
        * @author M.Shirzadi
    */
    public class AnnotationWidget : WidgetBase
    {
        #region variables

        private Dictionary<int, AnnotationShape> _annotationsShape = new Dictionary<int, AnnotationShape>();
        private AnnotationState _selectedAnnotation;

        private List<Pose2D> _underDrawRectReal = new List<Pose2D>();
        private List<Pose2D> _underDrawRectCanvas = new List<Pose2D>();

        private Keys _shiftKey = Keys.A;
        private bool _isDrawing = false;
        private bool _getDescriptionAfterFinished = false;
        private Ellipse ellipse;
        private Brush brush_red = null;
        #endregion

        #region Constructor

        public AnnotationWidget()
        {
            try { this._drawState = ProjectCommons.config.Viewer.AnnotationWidget; }
            catch { this._drawState = new DrawState(); }
            try { brush_red = new SolidColorBrush(ProjectCommons.Drawing2D.windowRenderTarget, System.Drawing.Color.Red); }
            catch { }
            ellipse = new Ellipse();
            ellipse.Center = new System.Drawing.PointF();
            ellipse.RadiusX = 2;
            ellipse.RadiusY = 2;
        }

        #endregion

        #region Methods

        public override void Paint(WindowRenderTarget renderTarget)
        {
            if (!_visible) return;

            if ((MouseData.SelectedObject != null) && (MouseData.SelectedObject is AnnotationState))
                _selectedAnnotation = (AnnotationState)MouseData.SelectedObject;
            else
                _selectedAnnotation = null;
            renderTarget.Transform = ProjectCommons.Drawing2D.changePositionMatrix(0f, 0f);
            lock (((ICollection)_annotationsShape).SyncRoot)
            {
                foreach (AnnotationShape cur in _annotationsShape.Values)
                {
                    bool isSelected = (_selectedAnnotation != null && cur.AnnotationInfo.ID == _selectedAnnotation.ID);

                    Brush backColor, borderColor;
                    float borderWide;
                    if (isSelected)
                    {
                        //backColor = this._drawState.Brush_SelectedBackColor;
                        borderColor = this._drawState.Brush_SelectedBorderColor;
                        //borderWide = this._drawState.SelectedBorderWide;
                    }
                    else
                    { borderColor = this._drawState.Brush_BorderColor; }
                    if (cur.CanvasBody.Count > 2)
                    { Draw(renderTarget, borderColor, cur.CanvasBody.ToArray()); }
                }
            }

            Brush brush_BorderColor = _drawState.Brush_BorderColor;
            if (brush_red == null)
                brush_red = new SolidColorBrush(ProjectCommons.Drawing2D.windowRenderTarget, System.Drawing.Color.Red);
            float width = _drawState.BorderWide;
            lock (((ICollection)_underDrawRectCanvas).SyncRoot)
            {
                int i = 0,
                    count = _underDrawRectCanvas.Count;

                for (; i < count; i++)
                {
                    int j = i - 1;
                    if (j >= 0)
                    {
                        renderTarget.DrawLine(brush_BorderColor, _underDrawRectCanvas[j], _underDrawRectCanvas[i]);
                        ellipse.Center = _underDrawRectCanvas[j];
                        renderTarget.FillEllipse(brush_red, ellipse);
                    }
                }
                if (i > 0)
                {
                    renderTarget.DrawLine(brush_BorderColor, (float)_underDrawRectCanvas[i - 1].X, (float)_underDrawRectCanvas[i - 1].Y, MouseDataBase.CurrentPoint_Scaled.X, MouseDataBase.CurrentPoint_Scaled.Y);
                    ellipse.Center = _underDrawRectCanvas[i - 1];
                    renderTarget.FillEllipse(brush_red, ellipse);
                }
            }
        }
        private void Draw(WindowRenderTarget renderTarget, Brush brush, Pose2D[] p)
        {
            // ProjectCommons.writeConsoleMessage(p.Length.ToString(), ConsolMessageType.Error);
            int i = 0;
            int count = p.Length;
            for (; i < count; i++)
            {
                int j = i - 1;
                if (j >= 0)
                {
                    renderTarget.DrawLine(brush, p[j], p[i]);
                    ellipse.Center = p[j];
                    renderTarget.FillEllipse(brush_red, ellipse);
                }
            }
            if (i > 0)
            {
                renderTarget.DrawLine(brush, p[i - 1], p[0]);
                ellipse.Center = p[i - 1];
                renderTarget.FillEllipse(brush_red, ellipse);
            }
        }


        public override void recreateShape()
        {
            lock (((ICollection)_underDrawRectReal).SyncRoot)
            {
                lock (((ICollection)_underDrawRectCanvas).SyncRoot)
                {
                    _underDrawRectCanvas.Clear();
                    foreach (Pose2D p in _underDrawRectReal)
                        _underDrawRectCanvas.Add(ProjectCommons.Drawing2D.ChangeRealToCanvas(p));
                }
            }
            lock (((ICollection)_annotationsShape).SyncRoot)
            {
                foreach (AnnotationShape aShape in _annotationsShape.Values)
                {
                    aShape.CanvasBody.Clear();
                    foreach (Pose2D p in aShape.RealBody)
                        aShape.CanvasBody.Add(ProjectCommons.Drawing2D.ChangeRealToCanvas(p));
                }
            }
        }

        private AnnotationShape contain(System.Drawing.Point point)
        {
            try
            {
                System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
                List<AnnotationShape> rShapes = null;
                lock (((ICollection)_annotationsShape).SyncRoot)
                {
                    rShapes = new List<AnnotationShape>(_annotationsShape.Values);
                    int count = rShapes.Count;
                    for (int i = 0; i < count; i++)
                    {
                        //ProjectCommons.writeConsoleMessage("WRITE SHOD", ConsolMessageType.Exclamation);
                        System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                        System.Drawing.PointF[] ps = new System.Drawing.PointF[rShapes[i].CanvasBody.Count];
                        int count1 = rShapes[i].CanvasBody.Count;
                        for (int j = 0; j < count1; j++)
                            ps[j] = new System.Drawing.PointF((float)rShapes[i].CanvasBody[j].X, (float)rShapes[i].CanvasBody[j].Y);
                        gp.AddPolygon(ps);
                        gp.Transform(m);
                        gp.Transform(ProjectCommons.Drawing2D.changePositionMatrix((double)rShapes[i].Center.X, (double)rShapes[i].Center.Y));

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

        public bool IsEditing
        {
            get { return _isDrawing; }
            set { _isDrawing = value; }
        }
        public Keys ShiftKey
        {
            get { return _shiftKey; }
            set { _shiftKey = value; }
        }
        public override WidgetTypes Type
        {
            get { return WidgetTypes.Annotation; }
        }
        public bool GetDescriptionAfterFinished
        {
            get { return _getDescriptionAfterFinished; }
            set { _getDescriptionAfterFinished = value; }
        }
        public List<AnnotationShape> TotalShape
        {
            get { return new List<AnnotationShape>(_annotationsShape.Values); }
        }

        #endregion

        #region events

        public override bool mouseClicked(EventArgs e)
        {
            return false;
        }
        public override bool mouseDClicked(EventArgs e)
        {
            //AnnotationShape aShape = annotationContains(MouseData.CurrentPoint);

            //if (aShape != null)
            //{
            //    frmGetDescription frm = new frmGetDescription();
            //    frm.txtDescription.Text = aShape.AnnotationInfo.Description;

            //    ProjectCommons.keyboardCapturedByGeoViewer = true;
            //    frm.ShowDialog();
            //    ProjectCommons.keyboardCapturedByGeoViewer = false;

            //    if (frm.DialogResult == DialogResult.OK)
            //    {
            //        aShape.AnnotationInfo.Description = frm.txtDescription.Text;
            //    }
            //    frm.Dispose();
            //}
            return true;
        }
        public override bool mouseMoved(MouseEventArgs e)
        {
            if (IsRaseHintNull) return true;
            AnnotationShape aShape = contain(MouseDataBase.CurrentPoint_Scaled);
            if (aShape != null)
            {
                RaiseHint(aShape.getHint(), MouseDataBase.CurrentPoint_Scaled);
                return true;
            }

            //RaiseHint(_type + "=" + this.ToString(), MouseDataBase.CurrentPoint_Scaled);
            return false;
        }
        public override bool mouseDown(MouseEventArgs e)
        {
            if (_isDrawing)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _underDrawRectReal.Add(ProjectCommons.Drawing2D.ChangeCanvasToReal(MouseDataBase.CurrentPoint_Scaled));
                    // ProjectCommons.writeConsoleMessage(" " + MouseDataBase.CurrentPoint_Scaled.ToString(), ConsolMessageType.Error);
                    _underDrawRectCanvas.Add(new Pose2D(MouseDataBase.CurrentPoint_Scaled));
                }
                else if (e.Button == MouseButtons.Right)
                    if (_underDrawRectReal.Count > 0)
                    {
                        _underDrawRectReal.RemoveAt(_underDrawRectReal.Count - 1);
                        _underDrawRectCanvas.RemoveAt(_underDrawRectCanvas.Count - 1);
                    }
            }
            //else
            //{
            //    if (MouseData.SelectedObject != null) return false;


            //    if (e.Button == MouseButtons.Left && e.Clicks == 1)
            //    {
            //        AnnotationShape aShape = annotationContains(e.Location);
            //        if (aShape != null)
            //            MouseData.SelectedObject = aShape.AnnotationInfo;
            //        return true;
            //    }
            //}
            return true;
        }
        public override bool mouseUp(MouseEventArgs e) { return false; }
        public override bool mouseWheelMoved(MouseEventArgs e) { return true; }

        public override bool keyDown(KeyEventArgs e)
        {
            if (e.KeyCode == _shiftKey && Visible)
            {
                if (_selectedAnnotation != null && _underDrawRectReal.Count == 0 && _annotationsShape.ContainsKey(_selectedAnnotation.ID))
                {
                    _underDrawRectReal.AddRange((_annotationsShape[_selectedAnnotation.ID].RealBody));
                    _annotationsShape[_selectedAnnotation.ID].RealBody.Clear();
                    MouseData.ForceForNoChangeSelectedObject = true;
                }
                _isDrawing = true;
            }
            else if (e.KeyCode == Keys.Delete && _selectedAnnotation != null)
            {
                _annotationsShape.Remove(_selectedAnnotation.ID);
                MouseData.removeObjectFromCollection(_selectedAnnotation);
                MouseData.SelectedObject = null;
            }
            return true;
        }
        public override bool keyUp(KeyEventArgs e)
        {
            if (e.KeyCode != _shiftKey)
                return true;

            if (_underDrawRectCanvas.Count > 2)
            {
                if (_selectedAnnotation != null && _annotationsShape.ContainsKey(_selectedAnnotation.ID))
                {
                    _annotationsShape[_selectedAnnotation.ID].RealBody.AddRange(_underDrawRectReal);
                    _annotationsShape[_selectedAnnotation.ID].CanvasBody.AddRange(_underDrawRectCanvas);
                }
                else
                {
                    AnnotationShape aShape = new AnnotationShape();
                    AnnotationState aState = new AnnotationState(ProjectCommons.fastRandom.Next(100, 1000000));

                    //get description
                    if (_getDescriptionAfterFinished)
                    {
                        frmGetDescription frm = new frmGetDescription();
                        ProjectCommons.keyboardCapturedByGeoViewer = true;
                        frm.ShowDialog();
                        ProjectCommons.keyboardCapturedByGeoViewer = false;
                        if (frm.DialogResult == DialogResult.OK)
                            aState.Description = frm.txtDescription.Text;
                        frm.Dispose();
                    }

                    aShape.AnnotationInfo = aState;
                    aShape.RealBody = new List<Pose2D>();
                    aShape.CanvasBody = new List<Pose2D>();

                    aShape.RealBody.AddRange(_underDrawRectReal);
                    aShape.CanvasBody.AddRange(_underDrawRectCanvas);
                    _annotationsShape.Add(aShape.AnnotationInfo.ID, aShape);
                    MouseData.addObjectToCollection(aShape.AnnotationInfo);
                }
            }
            else if (_selectedAnnotation != null && _annotationsShape.ContainsKey(_selectedAnnotation.ID))
            {
                _annotationsShape.Remove(_selectedAnnotation.ID);
                MouseData.removeObjectFromCollection(_selectedAnnotation);
                _selectedAnnotation = null;
            }

            _underDrawRectReal.Clear();
            _underDrawRectCanvas.Clear();
            MouseData.ForceForNoChangeSelectedObject = false;
            _isDrawing = false;
            return true;
        }


        #endregion

    }
}
