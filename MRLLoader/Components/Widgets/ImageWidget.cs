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
    public class ImageWidget : WidgetBase
    {

        #region Variables

        private Dictionary<int, ImageShape> _imagesShape = new Dictionary<int, ImageShape>();
        private PanoState _selectedImage;
        private int sSize = 10, sAngle = 15;
        private float _imageRealRadius = 2.0f;
        #endregion

        #region Constructor

        public ImageWidget()
        {
            try { this._drawState = ProjectCommons.config.Viewer.ImageWidget; }
            catch { this._drawState = new DrawState(); }
        }

        #endregion

        #region Methodes

        public override void Paint(WindowRenderTarget renderTarget)
        {
            if (!_visible) return;
            if (ProjectCommons.Drawing2D.BaseBody.lastUpdated != ProjectCommons.Drawing2D.lastMapUpdated)
                ProjectCommons.Drawing2D.BaseBody.Initiate();

            if (MouseData.SelectedObject != null && MouseData.SelectedObject.GetType() == typeof(PanoState))
                _selectedImage = (PanoState)MouseData.SelectedObject;
            else
                _selectedImage = null;

            try
            {
                List<ImageShape> iShapes;
                lock (((ICollection)_imagesShape).SyncRoot)
                { iShapes = new List<ImageShape>(_imagesShape.Values); }
                int count = iShapes.Count;
                for (int i = 0; i < count; i++)
                    drawPano(renderTarget, iShapes[i]);
            }
            catch (Exception e)
            { ProjectCommons.writeConsoleMessage("Exception in ImageWidget:" + e.Message, ConsoleMessageType.Error); }
        }


        private void drawPano(WindowRenderTarget renderTarget, ImageShape currentShape)
        {
            bool isSelected = (this._selectedImage != null && currentShape.ImageInfo.ID == this._selectedImage.ID);

            Brush backColor, borderColor;
            float borderWide;
            if (isSelected)
            {
                backColor = this._drawState.Brush_BackColor;
                borderColor = new SolidColorBrush(ProjectCommons.Drawing2D.windowRenderTarget, System.Drawing.Color.Red);
                borderWide = this._drawState.BorderWide;
            }
            else
            {
                backColor = this._drawState.Brush_SelectedBackColor;
                borderColor = this._drawState.Brush_BorderColor;
                borderWide = this._drawState.SelectedBorderWide;
            }
            if (ProjectCommons.Drawing2D.BaseBody.lastUpdated != ProjectCommons.Drawing2D.lastMapUpdated)
                currentShape.CanvasPose = ProjectCommons.Drawing2D.ChangeRealToCanvas(currentShape.RealPose);

            renderTarget.Transform = ProjectCommons.Drawing2D.changePositionMatrix((float)currentShape.CanvasPose.X, (float)currentShape.CanvasPose.Y, (float)currentShape.CanvasPose.Rotation);


            renderTarget.DrawLine(borderColor, 0, 0, ProjectCommons.Drawing2D.BaseBody.lenght, -ProjectCommons.Drawing2D.BaseBody.lenght);
            renderTarget.DrawLine(borderColor, ProjectCommons.Drawing2D.BaseBody.lenght, -ProjectCommons.Drawing2D.BaseBody.lenght, ProjectCommons.Drawing2D.BaseBody.lenght, ProjectCommons.Drawing2D.BaseBody.lenght);
            renderTarget.DrawLine(borderColor, ProjectCommons.Drawing2D.BaseBody.lenght, ProjectCommons.Drawing2D.BaseBody.lenght, 0, 0);
        }
        
        private ImageShape getShape(PanoState p)
        {
            ImageShape ret = new ImageShape();
            ret.ImageInfo = p;
            ret.RealPose = new Pose3D();
            return ret.Clone();
        }

        private ImageShape contain(System.Drawing.Point point)
        {
            try
            {
                if (ProjectCommons.Drawing2D.BaseBody.lastUpdated != ProjectCommons.Drawing2D.lastMapUpdated)
                    ProjectCommons.Drawing2D.BaseBody.Initiate();

                List<ImageShape> vShapes = null;
                lock (((ICollection)_imagesShape).SyncRoot)
                {
                    vShapes = new List<ImageShape>(_imagesShape.Values);
                    int count = vShapes.Count;
                    for (int i = 0; i < count; i++)
                    {
                        System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();

                        gp.StartFigure();
                        gp.AddPolygon(new System.Drawing.Point[] { new System.Drawing.Point(0, 0), new System.Drawing.Point(ProjectCommons.Drawing2D.BaseBody.lenght, -ProjectCommons.Drawing2D.BaseBody.lenght),
                            new System.Drawing.Point(ProjectCommons.Drawing2D.BaseBody.lenght, ProjectCommons.Drawing2D.BaseBody.lenght) });
                        gp.CloseFigure();

                        System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
                        m.Rotate(MathHelper.RadToDeg((float)vShapes[i].CanvasPose.Rotation), MatrixOrder.Append);
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
        public void updateRobotImage(int id, System.Drawing.Bitmap bmp, Pose3D p, string name)
        {
            ImageShape iShape = null;
            lock (((ICollection)_imagesShape).SyncRoot)
            {
                if (_imagesShape.ContainsKey(id))
                {
                    iShape = _imagesShape[id];
                    _imagesShape.Remove(id);
                }
                if (iShape == null)
                {
                    PanoState ps = new PanoState(id);
                    iShape = getShape(ps);
                    iShape.ImageInfo.Image = bmp;
                }
                iShape.RealPose = p;
                iShape.CanvasPose = ProjectCommons.Drawing2D.ChangeRealToCanvas(p);
                iShape.ImageInfo.Source = name;
                _imagesShape.Add(id, iShape);
            }
        }
        public override void recreateShape()
        {
            List<ImageShape> iShapes = null;
            lock (((ICollection)_imagesShape).SyncRoot)
            {
                iShapes = new List<ImageShape>(_imagesShape.Values);
            }
            if (iShapes == null)
                return;
            int count = iShapes.Count;
            for (int i = 0; i < count; i++)
            {
                iShapes[i].CanvasPose = ProjectCommons.Drawing2D.ChangeRealToCanvas(iShapes[i].RealPose);
            }
        }

        #endregion

        #region Property

        public Dictionary<int, ImageShape> ImagesShape
        {
            get { return _imagesShape; }
            private set { _imagesShape = value; }
        }
        public PanoState SelectedImage
        {
            get { return _selectedImage; }
            set { _selectedImage = value; }
        }
        public DrawState DrawingState
        {
            get { return _drawState; }
            set { _drawState = value; }
        }
        public override WidgetTypes Type
        {
            get { return WidgetTypes.CameraImage; }
        }

        #endregion

        #region events

        public override bool mouseClicked(EventArgs e) { return true; }
        public override bool mouseMoved(MouseEventArgs e)
        {
            if (IsRaseHintNull) return true;
            ImageShape iShape = contain(MouseDataBase.CurrentPoint_Scaled);
            if (iShape != null)
            {
                RaiseHint(iShape.getHint(), MouseDataBase.CurrentPoint_Scaled);
                return true;
            }
            return false;
        }
        public override bool mouseDClicked(EventArgs e) { return true; }
        public override bool mouseDown(MouseEventArgs e)
        {
            if (MouseData.SelectedObject != null) return true;

            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                ImageShape image = contain(MouseDataBase.CurrentPoint_Scaled);
                if (image != null)
                {
                    _selectedImage = image.ImageInfo;
                    image.isViewed = true;
                    MouseData.SelectedObject = image.ImageInfo;
                    if (ProjectCommons.imageWidget_OnSelected != null)
                        ProjectCommons.imageWidget_OnSelected(image.ImageInfo);
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
                if (e.KeyCode == Keys.Delete && _selectedImage != null)
                {
                    if (_imagesShape.ContainsKey(_selectedImage.ID))
                        _imagesShape.Remove(_selectedImage.ID);
                }
            }
            catch { }
            return true;
        }
        public override bool keyUp(KeyEventArgs e) { return true; }

        #endregion

    }
}
