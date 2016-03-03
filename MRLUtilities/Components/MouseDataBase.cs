using System.Drawing;

namespace MRL.Components
{
    public static class MouseDataBase
    {
        public static Point Offset = new Point(0, 0);
        public static Point StartPoint = new Point(-1, -1); //point that mouse clicked
        public static Point DrawPoint = new Point(0, 0);//this is the starting point that we should draw our image
        public static Point CurrentPoint = new Point(0, 0);//mouse current position on control
        public static Point CurrentPoint_Scaled = new Point(0, 0);
    }
}
