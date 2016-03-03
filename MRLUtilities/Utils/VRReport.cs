using System;
using System.Diagnostics;
using System.IO;

namespace MRL.Utils
{
    public static class VRReport
    {
        private const string dateFormat = "MMddyyyy_hhmmss";
        private const string dbgFormat = "hh:mm:ss";
        private static StreamWriter logOut = null;
        private static object LockObject = new object();
        private static Stopwatch sw = Stopwatch.StartNew();
        public static event Action<string, string> NewReport;

        public static void Report(string str, string className)
        {
            MakeLogOut();
            LOG(string.Format("{0}  {1} - {2}", NOW(dbgFormat), className, str));
            if (NewReport != null)
                NewReport(className, str);
        }

        private static void LOG(string wrr)
        {
            lock (LockObject)
            {
                if (logOut != null)
                {
                    wrr = sw.ElapsedMilliseconds + "\t" + wrr;
                    sw.Restart();
                    logOut.WriteLine(wrr);
                    logOut.Flush();
                }
            }
        }

        private static void MakeLogOut()
        {
            lock (LockObject)
            {
                if (logOut == null)
                {
                    try
                    {
                        if (!Directory.Exists("Reports"))
                            Directory.CreateDirectory("Reports");
                        logOut = new StreamWriter("Reports\\report_" + NOW(dateFormat) + ".log");
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
        }

        public static void Close()
        {
            lock (LockObject)
            {
                if (logOut != null)
                {
                    logOut.Close();
                    logOut = null;
                }
            }
        }

        private static string NOW(string format)
        {
            return DateTime.Now.ToString(format);
        }
    }
}
