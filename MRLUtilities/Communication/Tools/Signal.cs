using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRL.Communication.Tools
{
    public enum SignalType { DIRECT, ROUTED }
    public class Signal
    {
        public SignalType Type { set; get; }
        public bool Status { set; get; }
        public int Value { set; get; }
    }
}
