using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Components.Tools.Widgets;
using MRL.Utils;
using MRL.CustomMath;
using MRL.Components.Tools.Shapes;
using System.Windows.Forms;
using MRL.Commons;
using System.Drawing;
using SlimDX.Direct2D;
using System.Collections;
using MRL.Communication.Tools;

namespace MRL.Components.Tools.Widgets
{
    public class SignalWidget : WidgetBase
    {

        #region Variables

        private List<SignalShape> _signalShape = new List<SignalShape>();
        private bool _isDrawing = false;
        private DrawState _drawState;
        private SlimDX.Direct2D.Brush drawingBrush = null;
        private SlimDX.Direct2D.Brush drawingBrush_Font = null;

        #endregion

        #region Constructor

        public SignalWidget()
        {
            try
            {
                this._drawState = ProjectCommons.config.Viewer.SignalWidget;

            }
            catch { this._drawState = new DrawState(); }
        }

        #endregion

        #region Methods

        public void updateSignals(List<SignalLine> signals)
        {
            try
            {
                if (_signalShape == null) _signalShape = new List<SignalShape>();

                _signalShape.Clear();

                foreach (var item in signals)
                {
                    _signalShape.Add(new SignalShape() { SignalInfo = item });
                }
            }
            catch
            { }
        }


        public override void Paint(WindowRenderTarget renderTarget)
        {
            if (!_visible)
                return;

            if (drawingBrush == null)
                drawingBrush = _drawState.Brush_Singal;
            if (drawingBrush_Font == null)
                drawingBrush_Font = _drawState.Brush_Singal_Font;

            if (ProjectCommons.Drawing2D.BaseBody.lastUpdated != ProjectCommons.Drawing2D.lastMapUpdated)
            {
                ProjectCommons.Drawing2D.BaseBody.Initiate();
                recreateShape();
            }

            renderTarget.Transform = ProjectCommons.Drawing2D.changePositionMatrix(0f, 0f);
            lock (((ICollection)_signalShape).SyncRoot)
            {
                foreach (var item in _signalShape)
                {
                    //drawingBrush.Opacity = (float)item.SignalInfo.SignalByPercent / 100.0f + 0.40f;
                    renderTarget.DrawLine(drawingBrush, item.SignalInfo.ConvasHead, item.SignalInfo.ConvasTail);
                    Pose2D center = new Pose2D();
                    center.X = (item.SignalInfo.ConvasHead.X + item.SignalInfo.ConvasTail.X) / 2.0f;
                    center.Y = (item.SignalInfo.ConvasHead.Y + item.SignalInfo.ConvasTail.Y) / 2.0f;
                    center.Rotation = (item.SignalInfo.ConvasHead.Y - item.SignalInfo.ConvasTail.Y) / (item.SignalInfo.ConvasHead.X - item.SignalInfo.ConvasTail.X);
                    DrawText(item.SignalInfo.SignalByPercent + "%", drawingBrush_Font, center, renderTarget);
                }
            }
        }

        private void DrawText(string text, SlimDX.Direct2D.Brush brush, Pose2D pos, RenderTarget renderTarget)
        {
            SlimDX.DirectWrite.TextFormat textFormat;
            SlimDX.DirectWrite.Factory dwriteFactory = new SlimDX.DirectWrite.Factory(SlimDX.DirectWrite.FactoryType.Isolated);
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(pos, new System.Drawing.SizeF(text.Length * 10.2f, 10.1f));
            rect.Location = new System.Drawing.PointF(rect.Location.X - rect.Width / 2, rect.Location.Y - rect.Height / 2);
            // renderTarget.DrawRectangle(brush, rect);
            textFormat = new SlimDX.DirectWrite.TextFormat(dwriteFactory, "Berlin Sans FB Demi",
                SlimDX.DirectWrite.FontWeight.Bold
                , SlimDX.DirectWrite.FontStyle.Normal
                , SlimDX.DirectWrite.FontStretch.Normal, 5.0f, "t");
            textFormat.TextAlignment = SlimDX.DirectWrite.TextAlignment.Center;
            textFormat.ParagraphAlignment = SlimDX.DirectWrite.ParagraphAlignment.Center;

            //  renderTarget.Transfor
            renderTarget.DrawText(text, textFormat, rect, brush);
            //  renderTarget.transform =

            textFormat.Dispose();
        }

        #endregion

        #region Property

        public override WidgetTypes Type
        {
            get { return WidgetTypes.SignalWidget; }
        }

        #endregion

        #region events

        public override bool mouseClicked(EventArgs e) { return false; }
        public override bool mouseDClicked(EventArgs e) { return false; }
        public override bool mouseMoved(MouseEventArgs e) { return false; }
        public override bool mouseDown(MouseEventArgs e) { return false; }
        public override bool mouseUp(MouseEventArgs e) { return false; }
        public override bool mouseWheelMoved(MouseEventArgs e) { return false; }
        public override bool keyDown(KeyEventArgs e) { return false; }
        public override bool keyUp(KeyEventArgs e) { return false; }

        #endregion

        public override void recreateShape()
        {

        }
    }
}
