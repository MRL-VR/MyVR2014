using System;
using System.Drawing;
using System.Windows.Forms;
using MRL.Commons;
using MRL.Utils;
using SlimDX.Direct2D;

namespace MRL.Components.Tools.Widgets
{
    public abstract class WidgetBase : IWidget
    {
        #region Variables

        public event ProjectCommons._hint Hint;
        protected bool _visible = true;
        protected DrawState _drawState;

        #endregion

        #region Property

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        public DrawState DrawState
        {
            get { return _drawState; }
            set { _drawState = value; }
        }
        public abstract WidgetTypes Type { get; }

        #endregion
        public abstract void recreateShape();
        #region Event

        protected bool IsRaseHintNull
        { get { return (Hint == null); } }
        protected virtual void RaiseHint(String msg, Point location)
        {
            if (Hint != null)
                Hint(msg, location);
        }

        #endregion

        #region Widget Members

        public abstract void Paint(WindowRenderTarget g);

        public abstract bool mouseClicked(EventArgs e);
        public abstract bool mouseDClicked(EventArgs e);
        public abstract bool mouseDown(MouseEventArgs e);
        public abstract bool mouseUp(MouseEventArgs e);
        public abstract bool mouseMoved(MouseEventArgs e);
        public abstract bool mouseWheelMoved(MouseEventArgs e);

        public abstract bool keyDown(KeyEventArgs e);
        public abstract bool keyUp(KeyEventArgs e);

        #endregion
    }
}
