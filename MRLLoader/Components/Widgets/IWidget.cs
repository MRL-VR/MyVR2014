using System;
using System.Windows.Forms;
using SlimDX.Direct2D;

namespace MRL.Components.Tools.Widgets
{
    public interface IWidget
    {
        void Paint(WindowRenderTarget g);

        bool mouseClicked(EventArgs e);
        bool mouseDClicked(EventArgs e);
        bool mouseDown(MouseEventArgs e);
        bool mouseUp(MouseEventArgs e);
        bool mouseMoved(MouseEventArgs e);
        bool mouseWheelMoved(MouseEventArgs e);

        bool keyDown(KeyEventArgs e);
        bool keyUp(KeyEventArgs e);
        //bool dispatchKeyEvent(KeyEventArgs e);

    }
}

