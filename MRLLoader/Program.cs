using System;
using System.Windows.Forms;

namespace MRL.Components
{

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.Run(new frmMain());
//            Application.Run(new FilterSettingForm());
        }

        static void Application_ThreadException ( object sender, System.Threading.ThreadExceptionEventArgs e )
        {
            MessageBox.Show(e.Exception.Message + " \n\n Source:\n" + e.Exception.Source);
        }
    }

}
