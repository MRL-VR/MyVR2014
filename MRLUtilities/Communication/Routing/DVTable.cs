using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace MRL.Communication.Tools
{
    public class DVTable
    {

        public ConcurrentBag<DVLink> items = new ConcurrentBag<DVLink>();

        public DVLink this[string destRobot]
        {
            get
            {
                foreach (var item in items)
                {
                    if (item.destNode.Equals(destRobot))
                        return item;
                }
                return null;
            }
        }
    }
}
