using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.CustomMath;
using MRL.Commons;

namespace MRL.Utils
{
    public class SignalLine
    {
        public Pose2D RealHead { set; get; }
        public Pose2D RealTail { set; get; }

        public Pose2D ConvasHead
        {
            get
            {
                return ProjectCommons.Drawing2D.ChangeRealToCanvas(RealHead);
            }
        }

        public Pose2D ConvasTail
        {
            get
            {
                return ProjectCommons.Drawing2D.ChangeRealToCanvas(RealTail);
            }
        }

        public int SignalByPercent { set; get; }
    }
}
