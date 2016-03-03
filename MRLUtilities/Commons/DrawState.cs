using System;
using MRL.Utils;
using SlimDX.Direct2D;

namespace MRL.Commons
{
    public class DrawState
    {
        public System.Drawing.Font HintFont = new System.Drawing.Font("Calibri (Body)", 8.25f, System.Drawing.FontStyle.Regular);

        public System.Drawing.Color BackColor = System.Drawing.Color.LightBlue;
        public System.Drawing.Color BorderColor = System.Drawing.Color.Black;

        public System.Drawing.Color SelectedBackColor = System.Drawing.Color.LightGray;
        public System.Drawing.Color SelectedBorderColor = System.Drawing.Color.Black;

        public System.Drawing.Color HintFontColor = System.Drawing.Color.LightBlue;
        public System.Drawing.Color HintFontBackColor = System.Drawing.Color.LightYellow;

        public int BackColorAlpha = 255;
        public int BorderColorAlpha = 255;

        public int SelectedBackColorAlpha = 255;
        public int SelectedBorderColorAlpha = 255;

        public int HintFontColorAlpha = 255;
        public int HintFontBackColorAlpha = 255;

        public float BorderWide = 1.2f;
        public float SelectedBorderWide = 2.0f;

        public DrawState()
        {
        }

        public DrawState(System.Drawing.Color backColor, int backColorAlpha, System.Drawing.Color selectedBackColor, int selectedBackColorAlpha, System.Drawing.Color borderColor
            , int borderColorAlpha, System.Drawing.Color selectedBorderColor, int selectedBorderColorAlpha, float borderWide, float selectedBorderWide
            , System.Drawing.Font hintFont, System.Drawing.Color hintFontColor, int hintFontColorAlpha, System.Drawing.Color hintFontBackColor, int hintFontBackColorAlpha)
        {
            this.BackColor = backColor;
            this.BackColorAlpha = backColorAlpha;

            this.SelectedBackColor = selectedBackColor;
            this.SelectedBackColorAlpha = selectedBackColorAlpha;

            this.BorderColor = borderColor;
            this.BorderColorAlpha = borderColorAlpha;

            this.SelectedBorderColor = selectedBorderColor;
            this.SelectedBorderColorAlpha = selectedBorderColorAlpha;

            this.BorderWide = borderWide;
            this.SelectedBorderWide = selectedBorderWide;

            this.HintFont = hintFont;
            this.HintFontColor = hintFontColor;
            this.HintFontColorAlpha = hintFontColorAlpha;
            this.HintFontBackColor = hintFontBackColor;
            this.HintFontBackColorAlpha = hintFontBackColorAlpha;
        }

        public Brush Brush_BackColor
        {
            get { return new SolidColorBrush(ProjectCommons.Drawing2D.windowRenderTarget, System.Drawing.Color.FromArgb(BackColorAlpha, BackColor)); }
        }
        public Brush Brush_BorderColor
        {
            get { return new SolidColorBrush(ProjectCommons.Drawing2D.windowRenderTarget, System.Drawing.Color.FromArgb(BorderColorAlpha, BorderColor)); ; }
        }
        public Brush Brush_SelectedBackColor
        {
            get { return new SolidColorBrush(ProjectCommons.Drawing2D.windowRenderTarget, System.Drawing.Color.FromArgb(SelectedBackColorAlpha, SelectedBackColor)); ; }
        }
        public Brush Brush_SelectedBorderColor
        {
            get { return new SolidColorBrush(ProjectCommons.Drawing2D.windowRenderTarget, System.Drawing.Color.FromArgb(SelectedBorderColorAlpha, SelectedBorderColor)); ; }
        }

        public Brush Brush_Singal
        {
            get { return new SolidColorBrush(ProjectCommons.Drawing2D.windowRenderTarget, System.Drawing.Color.FromArgb(255, System.Drawing.Color.OrangeRed)); ; }
        }

        public Brush Brush_Singal_Font
        {
            get { return new SolidColorBrush(ProjectCommons.Drawing2D.windowRenderTarget, System.Drawing.Color.FromArgb(255, System.Drawing.Color.DarkGreen)); ; }
        }
        public static DrawState parseInfo(string str)
        {
            System.Drawing.Font defaultFont = new System.Drawing.Font("Calibri (Body)", 8.25f, System.Drawing.FontStyle.Regular);
            if (str == null)
                return new DrawState(System.Drawing.Color.LightBlue, 255, System.Drawing.Color.LightBlue, 255, System.Drawing.Color.Black, 255, System.Drawing.Color.Black, 255, 1.2f, 3f, defaultFont, System.Drawing.Color.Black, 255, System.Drawing.Color.Yellow, 255);
            USARParser up = new USARParser(str);

            int backCAlpha = Int32.Parse(up.getSegment("BackColorAlpha").Get("BackColorAlpha"))
                , sbackCAlpha = Int32.Parse(up.getSegment("SelectedBackColorAlpha").Get("SelectedBackColorAlpha"))
                , borderCAlpha = Int32.Parse(up.getSegment("BorderColorAlpha").Get("BorderColorAlpha"))
                , sborderCAlpha = Int32.Parse(up.getSegment("SelectedBorderColorAlpha").Get("SelectedBorderColorAlpha"))
                , hintFontColorAlpha = int.Parse(up.getSegment("HintFontColorAlpha").Get("HintFontColorAlpha"))
                , hintFontBackColorAlpha = int.Parse(up.getSegment("HintFontBackColorAlpha").Get("HintFontBackColorAlpha"));

            System.Drawing.Color backC = System.Drawing.Color.FromArgb(Int32.Parse(up.getSegment("BackColor").Get("BackColor")))
                , sbackC = System.Drawing.Color.FromArgb(Int32.Parse(up.getSegment("SelectedBackColor").Get("SelectedBackColor")))
                , borderC = System.Drawing.Color.FromArgb(Int32.Parse(up.getSegment("BorderColor").Get("BorderColor")))
                , sborderC = System.Drawing.Color.FromArgb(Int32.Parse(up.getSegment("SelectedBorderColor").Get("SelectedBorderColor")))

                , hintFontColor = System.Drawing.Color.FromArgb(Int32.Parse(up.getSegment("HintFontColor").Get("HintFontColor")))
                , hintFontBackColor = System.Drawing.Color.FromArgb(Int32.Parse(up.getSegment("HintFontBackColor").Get("HintFontBackColor")));

            string fontName = up.getSegment("HintFontName").Get("HintFontName");
            float hintFontSize = float.Parse(up.getSegment("HintFontSize").Get("HintFontSize"));
            System.Drawing.FontStyle fs = (System.Drawing.FontStyle)int.Parse(up.getSegment("HintFontStyle").Get("HintFontStyle"));

            float borderWide = float.Parse(up.getSegment("BorderWide").Get("BorderWide"));
            float sBorderWide = float.Parse(up.getSegment("SelectedBorderWide").Get("SelectedBorderWide"));

            System.Drawing.Font f = new System.Drawing.Font(fontName, hintFontSize, fs);

            return new DrawState(backC, backCAlpha, sbackC, sbackCAlpha, borderC, borderCAlpha, sborderC, sborderCAlpha, borderWide, sBorderWide, f, hintFontColor, hintFontColorAlpha, hintFontBackColor, hintFontBackColorAlpha);
        }

        public override string ToString()
        {
            return "{" + String.Format("BackColor {0} BackColorAlpha {1} SelectedBackColor {2} SelectedBackColorAlpha {3} BorderColor {4} BorderColorAlpha {5} "
            + "SelectedBorderColor {6} SelectedBorderColorAlpha {7} BorderWide {8} SelectedBorderWide {9} HintFontName {10} HintFontSize {11} HintFontStyle {12} " + 
            "HintFontColor {13} HintFontColorAlpha {14} HintFontBackColor {15} HintFontBackColorAlpha {16}"
            , BackColor.ToArgb(), BackColorAlpha, SelectedBackColor.ToArgb(), SelectedBackColorAlpha, BorderColor.ToArgb(), BorderColorAlpha, SelectedBorderColor.ToArgb()
            , SelectedBorderColorAlpha, BorderWide, SelectedBorderWide, HintFont.Name, HintFont.Size.ToString(), (int)HintFont.Style, HintFontColor.ToArgb(), HintFontColorAlpha, HintFontBackColor.ToArgb(), HintFontBackColorAlpha) + "}";
        }
    }
}
