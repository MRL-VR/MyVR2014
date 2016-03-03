using System;
//using System.Drawing;

using System.Windows.Forms;
using MRL.Commons;
using MRL.Utils;
using SlimDX.Direct2D;
namespace MRL.Components.Tools.Widgets
{
    /**
        * This widget is responsible for drawing the map in the background.
        * @author M.Shirzadi
    **/
    public class MapWidget : WidgetBase
    {
        #region Variables

        Bitmap _map = null;

        #endregion

        #region Methodes

        public MapWidget()
        {
            try { this._drawState = ProjectCommons.config.Viewer.MapWidget; }
            catch { this._drawState = new DrawState(); }
        }

        public override void Paint(WindowRenderTarget renderTarget)
        {
            if (!_visible)
                return;

            if (_map == null)
                return;
            renderTarget.Transform = ProjectCommons.Drawing2D.changeMatrix(ProjectCommons.Drawing2D.WorldMatrix);
            renderTarget.DrawBitmap(_map, new System.Drawing.Rectangle(0, 0, (int)_map.Size.Width, (int)_map.Size.Height), 100, SlimDX.Direct2D.InterpolationMode.NearestNeighbor);
        }
        public void upadteMap(Bitmap bmp)
        {
            _map = bmp;
        }
        public override void recreateShape()
        {
        }

        #endregion

        #region Property

        public override WidgetTypes Type
        {
            get { return WidgetTypes.Map; }
        }

        #endregion

        #region events

        public override bool mouseClicked(EventArgs e) { return true; }
        public override bool mouseDClicked(EventArgs e) { return true; }
        public override bool mouseDown(MouseEventArgs e) { return true; }
        public override bool mouseUp(MouseEventArgs e)
        {
            return true;
        }
        public override bool mouseMoved(MouseEventArgs e)
        {
            //   RaiseHint("", MouseData.CurrentPoint);
            return true;
        }
        public override bool mouseWheelMoved(MouseEventArgs e) { return true; }

        public override bool keyDown(KeyEventArgs e) { return true; }
        public override bool keyUp(KeyEventArgs e) { return true; }

        #endregion
    }
}
