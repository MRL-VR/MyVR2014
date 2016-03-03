using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SlimDX.Direct2D;
using SlimDX;
using MRL.Utils.GMath;

namespace VisualizerLibrary
{
    public static class WindowExtensions
    {
        public static T As<T>(this object objectToCast)
        {
            if (objectToCast == null)
                return default(T);
            else
            {
                return (T)objectToCast;
            }
        }

        public static SlimDX.Direct2D.Ellipse GetEllipse(this SlimDX.Direct2D.Ellipse source, Position2D center, float xradius, float yradius)
        {
            return new SlimDX.Direct2D.Ellipse()
            {
                Center = center,
                RadiusX = xradius,
                RadiusY = yradius
            };
        }

        public static Brush ToBrush(this System.Drawing.Color source, WindowRenderTarget renderTarget)
         {
             SlimDX.Direct2D.Brush b = new SlimDX.Direct2D.SolidColorBrush(renderTarget, new Color4(1f, source.R / 255f, source.G / 255f, source.B / 255f));
             return b;
         }

        public static Matrix3x2 ToMatrix3x2(this Matrix m)
        {
            SlimDX.Matrix3x2 retValue = SlimDX.Matrix3x2.Identity;
            retValue.M11 = m.M11;
            retValue.M12 = m.M12;
            retValue.M21 = m.M21;
            retValue.M22 = m.M22;
            retValue.M31 = m.M41;
            retValue.M32 = m.M42;
            return retValue;
        }
    }
}
