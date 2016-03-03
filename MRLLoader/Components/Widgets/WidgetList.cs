using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MRL.Commons;
using MRL.Components.Tools.Widgets;
using MRL.Utils;

namespace MRL.Components.Tools
{
    public class WidgetList : List<WidgetBase>
    {
        public new void Add(WidgetBase widget)
        {
            base.Add(widget);
        }
        List<WidgetTypes> lastOrder = new List<WidgetTypes>();

        public void ReOrder()
        {
            Array array = Enum.GetValues(typeof(WidgetTypes));
            List<WidgetTypes> wb = new List<WidgetTypes>();
            for (int i = 0; i < array.Length; i++)
            { wb.Add((WidgetTypes)array.GetValue(i)); }
            ReOrder(wb);
        }
        public void ReOrder(List<WidgetTypes> types)
        {
            foreach (WidgetTypes w in types)
                ProjectCommons.writeConsoleMessage(w.ToString(), ConsoleMessageType.Exclamation);
            List<WidgetBase> ordered = new List<WidgetBase>();
            lastOrder.Clear();
            lastOrder.AddRange(types);
            int count = types.Count;
            for (int i = 0; i < count; i++)
            {
                int countj = this.Count;
                for (int j = 0; j < countj; j++)
                {
                    if (types[i] == this[j].Type)
                    {
                        ordered.Add(this[j]);
                    }
                }
            }
            lock (((ICollection)this))
            {
                this.Clear();
                this.AddRange(ordered);
            }
        }
        public void ReOrderWithLastOrder()
        {
            if (lastOrder.Count == 0)
            { return; }
            ReOrder(lastOrder.ToList());
        }

    }
}
