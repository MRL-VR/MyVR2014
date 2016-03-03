using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MRL.Commons;
using MRL.IDE.Base;
using MRL.IDE.Robot;
using MRL.Utils;
using MRLRobot;

namespace Robot_Client
{

    class Program
    {

        #region "Variables"

        private static string staticAddress;

        private static int mRobotIndexInConfigFile;
        private static int mSpawnIndex;
        private static bool mSpawnInTestMode;

        #endregion

        #region "Private Methods"

        private static string getStartUpPath()
        {
            return Application.StartupPath;
        }

        private static void initCommons(USARParser ARGSparser)
        {
            string configFilename = ARGSparser.getString("Config");
            string robotINDX = ARGSparser.getString("robotIndex");
            string spawnINDX = ARGSparser.getString("spawnIndex");
            string spawnMode = ARGSparser.getString("TM");

            ProjectCommons.config = new USARConfig(configFilename);

            mSpawnIndex = int.Parse(spawnINDX);
            mRobotIndexInConfigFile = int.Parse(robotINDX);
            mSpawnInTestMode = bool.Parse(spawnMode);

            string robotName = ProjectCommons.config.botInfo[mRobotIndexInConfigFile].Name;

            if (ARGSparser.getSegment("SR") != null)
            {
                int[] iSRs = USARParser.parseInts(ARGSparser.getString("SR"), ",");
                List<int> spawningRobots = new List<int>(iSRs);

                int count = ProjectCommons.config.botInfo.Count;
                for (int i = 0; i < count; i++)
                {
                    if (spawningRobots.Contains(i))
                    {
                        ProjectCommons.config.botInfo[i].Spawned = true;
                        ProjectCommons.config.botInfo[i].MountIndex = i;
                    }
                }
            }

            string address = getStartUpPath();
            int len = address.LastIndexOf("MRLRobot");

            if (len != -1)
            {
                staticAddress = address.Substring(0, len);
                string logAddress = staticAddress + "Config\\Logs\\Robot_" + robotName + "_" +
                                    DateTime.Now.ToString().Replace("/", "-").Replace(":", ",") + ".log";

                //USARLog.logOut = new StreamWriter(logAddress);
            }


            Console.Title = robotName;
            setHeader("MRL - Robot Client " + mRobotIndexInConfigFile + " (" + robotName + ")");
            ProjectCommons.ConsoleMessage = new ProjectCommons._addConsoleMessage(writeMessage);
        }

        private static void mountRobot()
        {
            RobotInfo ri = ProjectCommons.config.botInfo[mRobotIndexInConfigFile];
            BaseRobot.Instance = ValidatedRobot.GetObject(ri, mSpawnIndex);
            if (BaseRobot.Instance != null)
                BaseRobot.Instance.Mount();
        }

        private static void setHeader(string header)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;

            int startIndex = (Console.WindowWidth - header.Length) / 2;
            string prefix = "";
            while (startIndex > 0)
            {
                prefix += " ";
                startIndex--;
            }
            Console.WriteLine(prefix + header);
        }

        private static void writeMessage(string message, ConsoleMessageType type)
        {
            ConsoleColor color = ConsoleColor.White;
            switch (type)
            {

                case ConsoleMessageType.Normal:
                    color = ConsoleColor.Gray;
                    break;
                case ConsoleMessageType.Exclamation:
                    color = ConsoleColor.Magenta;
                    break;
                case ConsoleMessageType.Error:
                    color = ConsoleColor.Red;
                    break;
                case ConsoleMessageType.Information:
                    color = ConsoleColor.Green;
                    break;
                default:
                    color = ConsoleColor.White;
                    break;
            }
            writeMessage(message, color);
        }

        private static void writeMessage(string message, ConsoleColor color)
        {
            ConsoleColor temp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = temp;
        }

        #endregion

        #region "Main Methods"

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                (new MRLRobot.Viz(true)).ShowDialog();
            }
            else
            {

                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.SetWindowPosition(0, 0);

                //reading & parsing arguments
                string values = "";
                foreach (string v in args) values += v + " ";
                values = values.Substring(0, values.Length - 1);
                //File.WriteAllText("RobotArgs.txt", values);

                USARParser ARGSparser = new USARParser(values, "#");

                initCommons(ARGSparser);
                ProjectCommons.writeConsoleMessage("Going to stablish connection ...", ConsoleMessageType.Normal);
                mountRobot();

                Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

                while (true)
                {
                    switch (Console.ReadLine().ToLower().Trim())
                    {
                        case "exit":
                            Application.Exit();
                            break;
                        case "close":
                            Application.Exit();
                            break;
                        case "viz":
                            Application.EnableVisualStyles();
                            (new MRLRobot.Viz()).ShowDialog();
                            break;
                        case "help":
                            Console.Write(Environment.NewLine +
                                "exit ------------------ exit" + Environment.NewLine +
                                "close ----------------- close" + Environment.NewLine +
                                "viz ------------------- visualizer" + Environment.NewLine
                                );
                            break;

                    }
                }
            }
        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (BaseRobot.Instance != null)
                BaseRobot.Instance.Unmount();
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message + " \n\n Source:\n" + e.Exception.Source);
        }

        #endregion

    }

}
