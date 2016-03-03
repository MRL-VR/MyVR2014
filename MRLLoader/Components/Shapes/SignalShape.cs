using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Components.Tools.Shapes;
using MRL.Utils;
using MRL.CustomMath;

namespace MRL.Components.Tools.Shapes
{
    public class SignalShape : ShapeBase
    {
        private RobotInfo _robotInfo;
        private SignalLine signalInfo;


        public SignalShape()
        {

        }

        public RobotInfo RobotInfo
        {
            get { return _robotInfo; }
            set { _robotInfo = value; }
        }

        public SignalLine SignalInfo
        {
            get { return signalInfo; }
            set { signalInfo = value; }
        }

        public override string getHint()
        {
            return "SignalShape";
        }
    }
}
