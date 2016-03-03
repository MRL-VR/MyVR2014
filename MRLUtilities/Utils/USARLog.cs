using System;
using System.Diagnostics;
using System.IO;
using MRL.Commons;

namespace MRL.Utils
{
    public static class USARLog
    {
        private const string dateFormat = "MMddyyyy_hhmmss";
        private const string dbgFormat = "hh:mm:ss";
        private static StreamWriter logOut = null;
        private static object LockObject = new object();
        private static Stopwatch sw = Stopwatch.StartNew();

        public static void println(string str, string className)
        {
            MakeLogOut();
            LOG(string.Format("{0}  {1} - {2}", NOW(dbgFormat), className, str));
        }

        public static void println(string str, int dbgLevel, string className)
        {
            MakeLogOut();
            LOG(string.Format("{0}  {1} {2} - {3}", NOW(dbgFormat), className, dbgLevel, str));
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
                if (ProjectCommons.UserLog && logOut == null)
                {
                    try
                    {
                        if (!Directory.Exists("LOG"))
                            Directory.CreateDirectory("LOG");
                        logOut = new StreamWriter("LOG\\mrl_usar_" + NOW(dateFormat) + ".log");
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