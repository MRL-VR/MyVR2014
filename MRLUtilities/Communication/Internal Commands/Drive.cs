using System;
using System.Collections.Generic;
using MRL.Commons;
using MRL.Utils;

namespace MRL.Communication.Internal_Commands
{

    /// <summary>
    /// this class used for creating DRIVE command 
    /// that used for driving robots in simulation
    /// </summary>
    public static class USARDrive
    {

        //public static System.IO.StreamWriter log = new System.IO.StreamWriter("drive.txt");

        private static string toUSARCommand ( double l, double r )
        {
            string cmd = "DRIVE ";

            // Add appropriate options
            cmd += "{Left " + l + "} ";
            cmd += "{Right " + r + "} ";

            //log.WriteLine("UnNorm = Left," + l.ToString() + ",Right," + r.ToString() + ",StartTime," + Environment.TickCount);

            return cmd;
        }


        /// <summary>
        /// used in comstation
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string toArmCommand(List<double> value)
        {
            try
            {
                return "ARM " + toGeneralArmCommand(value);
            }
            catch (Exception e)
            {
                ProjectCommons.writeConsoleMessage("Err DW : " + e.Message, ConsoleMessageType.Error);
                return "";
            }
        }


        private static string toGeneralArmCommand(List<double> values)
        {
            return String.Format("{{deg true arm_fl {0} arm_fr {1} arm_rl {2} arm_rr {3}}}",
                                 values[0], values[1], values[2], values[3]);
        }

        /// <summary>
        /// used in comstation
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string toUSARCommand ( double value, DriveType type )
        {
            return "DRIVE " + toGeneralCommand(value, type);
        }

        private static string toGeneralCommand(double val, DriveType type)
        {
            switch (type)
            {
                case DriveType.MoveStop:
                    return "{move_s 0}";

                case DriveType.MoveInSide:
                    return "{move_rs " + val.ToString() + "}";

                case DriveType.Fly:
                    return "{move_up " + val.ToString() + "}";

                case DriveType.Straight:
                    return "{move_fw " + val.ToString() + "}";

                case DriveType.Rotate:
                    return "{move_rt " + val.ToString() + "}";

                case DriveType.Light:
                    bool valB = val == 0 ? false : true;
                    return "{light " + valB.ToString() + "}";

                case DriveType.MoveArm_All:
                    return "{deg True arm_all " + val.ToString() + "}";

                case DriveType.MoveArm_Front:
                    return "{deg True arm_front " + val.ToString() + "}";

                case DriveType.MoveArm_Rear:
                    return "{deg True arm_Rear " + val.ToString() + "}";

                case DriveType.MoveArm_fl:
                    return "{deg True arm_fl " + val.ToString() + "}";

                case DriveType.MoveArm_fr:
                    return "{deg True arm_fr " + val.ToString() + "}";

                case DriveType.MoveArm_rl:
                    return "{deg True arm_rl " + val.ToString() + "}";

                case DriveType.MoveArm_rr:
                    return "{deg True arm_rr " + val.ToString() + "}";
            }
            return "";
        }

        public static string toUSARCommand(double l, double r, bool norm)
        {
            string cmd = "DRIVE ";

            //if (l > 4) l = 4;
            //if (r > 4) r = 4;

            // Add appropriate options
            cmd += "{Left " + l.ToString("0.#####") + "} ";
            cmd += "{Right " + r.ToString("0.#####") + "} ";
            //cmd += "{Normalized " + norm.ToString() + "} ";

            ProjectCommons.ConsoleMessage(cmd, ConsoleMessageType.Exclamation);
            //log.WriteLine("Norm = Left," + l.ToString() + ",Right," + r.ToString() + ",StartTime," + Environment.TickCount);

            return cmd;
        }

        //public static string toUSARCommand ( bool fl )
        //{
        //    string cmd = "DRIVE ";

        //    // Add appropriate options
        //    cmd += "{Flip " + fl.ToString() + "} ";

        //    return cmd;
        //}

        //public static string toUSARCommand ( bool li, bool fl )
        //{
        //    string cmd = "DRIVE ";

        //    // Add appropriate options
        //    cmd += "{Light " + li.ToString() + "} ";
        //    cmd += "{Flip " + fl.ToString() + "} ";

        //    return cmd;
        //}

        //public static string toUSARCommand ( double l, double r, bool norm, bool li, bool fl )
        //{
        //    string cmd = "DRIVE ";

        //    // Add appropriate options
        //    cmd += "{Left " + l + "} ";
        //    cmd += "{Right " + r + "} ";
        //    cmd += "{Normalized " + norm.ToString() + "} ";
        //    cmd += "{Light " + li.ToString() + "} ";
        //    cmd += "{Flip " + fl.ToString() + "} ";

        //    //if (!norm)
        //    //    log.WriteLine("UnNorm = Left," + l.ToString() + ",Right," + r.ToString() + ",StartTime," + Environment.TickCount);
        //    //else
        //    //    log.WriteLine("Norm = Left," + l.ToString() + ",Right," + r.ToString() + ",StartTime," + Environment.TickCount);

        //    return cmd;
        //}


        public static string toUSARCommand(bool lightMode)
        {
            return "DRIVE {light " + lightMode.ToString() + "}";
        }
    }

}
