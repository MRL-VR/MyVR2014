using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRL.Communication.Tools
{
    public class DVLink
    {
        public volatile string interfaceNode;
        public volatile string destNode;
        public volatile bool cost;
        public volatile int hob;
        public double signalStrength;
        public int SignalPercentage
        {
            get
            {
                if (double.IsNaN(signalStrength))
                    return 0;

                int newSignal = (int)signalStrength + 100;
                int final = (newSignal * 100 / 85);
                final = (final < 0) ? 0 : final;
                final = (final > 100) ? 100 : final;
                return final;
            }
        }
        public DVLink(string interfaceNode, string destNode, bool cost, int hob, double signalStrength)
        {
            this.interfaceNode = interfaceNode;
            this.destNode = destNode;
            this.cost = cost;
            this.hob = hob;
            this.signalStrength = signalStrength;
        }
    }
}
