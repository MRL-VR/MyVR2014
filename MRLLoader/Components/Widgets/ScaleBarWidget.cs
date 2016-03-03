using System;
using System.Windows.Forms;
using MRL.Commons;
using MRL.Utils;
using SlimDX.Direct2D;

namespace MRL.Components.Tools.Widgets
{
    public class ScaleBarWidget : WidgetBase
    {
        #region  Variables

        float wide = -1f;

        #endregion

        #region Methods

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

            Brush brush = new SolidColorBrush(renderTarget, System.Drawing.Color.LightGray);
            System.Drawing.PointF p1 = new System.Drawing.PointF(0f, 0f), p2 = new System.Drawing.PointF(wide, 0f);

            renderTarget.Transform = ProjectCommons.Drawing2D.changePositionMatrix_noneWorld(x, y);
            renderTarget.DrawLine(brush, p1, p2);
            renderTarget.DrawLine(brush, new System.Drawing.PointF(p1.X, p1.Y - 1), new System.Drawing.PointF(p1.X, p1.Y + 1));
            renderTarget.DrawLine(brush, new System.Drawing.PointF(p2.X, p2.Y - 1), new System.Drawing.PointF(p2.X, p2.Y + 1));

            string strOneMeter = "1.0m";
            renderTarget.DrawText(strOneMeter, ProjectCommons.Drawing2D.textFormat, new System.Drawing.Rectangle(0, 0, 50, 20), brush);
        }
        public override void recreateShape()
        {
            //wide = change3DLenghtTo2DLenght(1);
        }

        #endregion

        #region Property

        public override WidgetTypes Type
        {
            get { return WidgetTypes.ScaleBar; }
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
