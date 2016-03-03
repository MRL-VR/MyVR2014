using System;
using System.Windows.Forms;
using MRL.Utils;

namespace MRL.Loader.Components
{
    public partial class ReportControl : ListView
    {
        public ReportControl()
        {
            InitializeComponent();

            VRReport.NewReport += new Action<string, string>(VRReport_NewReport);
            //Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        Thread.Sleep(1000);
            //        VRReport.Report("Class", "Text");
            //    }
            //});
        }

        void VRReport_NewReport(string arg1, string arg2)
        {
            this.Invoke(new Action(() =>
            {
                ListViewItem i = new ListViewItem() { Text = arg1, Selected = true };
                i.SubItems.Add(arg2);
                Items.Add(i);
                TopItem = i;
            }));
        }
    }
}
