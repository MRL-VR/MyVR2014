using MRL.Utils;

namespace MRL.Command.Drive
{
    public class AirBotDrive : AbstractDrive
    {
        public AirBotDrive()
        {
            _type = "AirBot";
        }
        public override string Type
        {
            get { return _type; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <exception cref="Move Robot:"/>
        /// <exception cref="       move_fw value --> this command send robot foreward with 'value' speed."/>
        /// <exception cref="       move_bw value --> this command send robot backward with 'value' speed."/>
        /// <exception cref="       move_ls value --> this command send robot left with 'value' speed."/>
        /// <exception cref="       move_rs value --> this command send robot right with 'value' speed."/>
        /// <exception cref="       move_fl value --> this command send robot foreward-left with 'value' speed."/>
        /// <exception cref="       move_fr value --> this command send robot foreward-right with 'value' speed."/>
        /// <exception cref="       move_rl value --> this command send robot backward-left with 'value' speed."/>
        /// <exception cref="       move_rr value --> this command send robot backward-right with 'value' speed."/>
        /// <exception cref="       move_up value --> this command send robot up with 'value' speed."/>
        /// <exception cref="       move_down value --> this command send robot down with 'value' speed."/>
        /// <exception cref="       norm value --> if you set this true, you shuld give all 'value' between -100 and 100."/>
        /// <returns></returns>
        public override string GetCommand(string cmd)
        {
            string retValue_Drive = "Drive ";

            //pase command -> Move Robot
            USARParser parse = new USARParser(cmd);

            string val = null;

            val = parse.getString("move_s"); //Stop
            if (val != null) { retValue_Drive += "{LinearVelocity 0} {LateralVelocity 0} {AltitudeVelocity 0} {RotationalVelocity 0}"; }

            val = parse.getString("move_fw"); //Foreward
            if (val != null) { retValue_Drive += "{LinearVelocity " + val + "} {LateralVelocity 0} {AltitudeVelocity 0} {RotationalVelocity 0}"; }

            val = parse.getString("move_bw"); //Backward
            if (val != null) { retValue_Drive += "{LinearVelocity " + (-double.Parse(val)).ToString() + "} {LateralVelocity 0} {AltitudeVelocity 0} {RotationalVelocity 0}"; }

            val = parse.getString("move_ls"); //Left Side
            if (val != null) { retValue_Drive += "{LinearVelocity 0} {LateralVelocity " + (-double.Parse(val)).ToString() + "} {AltitudeVelocity 0} {RotationalVelocity 0}"; }

            val = parse.getString("move_rs"); //Right Side
            if (val != null) { retValue_Drive += "{LinearVelocity 0} {LateralVelocity " + val + "} {AltitudeVelocity 0} {RotationalVelocity 0}"; }

            val = parse.getString("move_fl"); //Front Left
            if (val != null) { retValue_Drive += "{LateralVelocity " + (-double.Parse(val)).ToString() + "} " + "{LinearVelocity " + val + "} {AltitudeVelocity 0} {RotationalVelocity 0}"; }

            val = parse.getString("move_fr"); //Front Right
            if (val != null) { retValue_Drive += "{LateralVelocity " + val + "} " + "{LinearVelocity " + val + "} {AltitudeVelocity 0} {RotationalVelocity 0}"; }

            val = parse.getString("move_rl"); //Rear Left
            if (val != null) { retValue_Drive += "{LateralVelocity " + (-double.Parse(val)).ToString() + "} " + "{LinearVelocity " + (-double.Parse(val)).ToString() + "} {AltitudeVelocity 0} {RotationalVelocity 0}"; }

            val = parse.getString("move_rr"); //Rear Right
            if (val != null) { retValue_Drive += "{LateralVelocity " + val + "} " + "{LinearVelocity " + (-double.Parse(val)).ToString() + "} {AltitudeVelocity 0} {RotationalVelocity 0}"; }

            val = parse.getString("move_up"); //Move Up
            if (val != null) { retValue_Drive += "{LinearVelocity 0} {LateralVelocity 0} {AltitudeVelocity " + val + "} {RotationalVelocity 0}"; }

            val = parse.getString("move_down"); //Move Down
            if (val != null) { retValue_Drive += "{LinearVelocity 0} {LateralVelocity 0} {AltitudeVelocity " + (-double.Parse(val)).ToString() + "} {RotationalVelocity 0}"; }

            val = parse.getString("move_rt"); //Rotate Right
            if (val != null) { retValue_Drive += "{AltitudeVelocity 0.00} {LinearVelocity 0.00} {LateralVelocity 0.00} {RotationalVelocity " + decimal.Parse(val) * 20 + "} "; }

            val = parse.getString("norm"); //Normalized
            if (val == null) val = "False";
            if (val != null) { retValue_Drive += "{Normalized " + val + "} "; }

            if (retValue_Drive.Length > 7)
                return retValue_Drive;
            return null;
        }

        //public override string GetCommand(string cmd)
        //{
        //    string retValue_Drive = "Drive ";

        //    //pase command -> Move Robot
        //    USARParser parse = new USARParser(cmd);

        //    string val = null;

        //    val = parse.getString("move_s"); //Stop
        //    if (val != null) { retValue_Drive += "{LinearVelocity 0} {LateralVelocity 0} {AltitudeVelocity 0} {RotationalVelocity 0}"; }

        //    val = parse.getString("move_fw"); //Foreward
        //    if (val != null) { retValue_Drive += "{LinearVelocity " + val + "} "; }

        //    val = parse.getString("move_bw"); //Backward
        //    if (val != null) { retValue_Drive += "{LinearVelocity " + (-double.Parse(val)).ToString() + "} "; }

        //    val = parse.getString("move_ls"); //Left Side
        //    if (val != null) { retValue_Drive += "{LateralVelocity " + (-double.Parse(val)).ToString() + "} "; }

        //    val = parse.getString("move_rs"); //Right Side
        //    if (val != null) { retValue_Drive += "{LateralVelocity " + val + "} "; }

        //    val = parse.getString("move_fl"); //Front Left
        //    if (val != null) { retValue_Drive += "{LateralVelocity " + (-double.Parse(val)).ToString() + "} " + "{LinearVelocity " + val + "} "; }

        //    val = parse.getString("move_fr"); //Front Right
        //    if (val != null) { retValue_Drive += "{LateralVelocity " + val + "} " + "{LinearVelocity " + val + "} "; }

        //    val = parse.getString("move_rl"); //Rear Left
        //    if (val != null) { retValue_Drive += "{LateralVelocity " + (-double.Parse(val)).ToString() + "} " + "{LinearVelocity " + (-double.Parse(val)).ToString() + "} "; }

        //    val = parse.getString("move_rr"); //Rear Right
        //    if (val != null) { retValue_Drive += "{LateralVelocity " + val + "} " + "{LinearVelocity " + (-double.Parse(val)).ToString() + "} "; }

        //    val = parse.getString("move_up"); //Move Up
        //    if (val != null) { retValue_Drive += "{AltitudeVelocity " + val + "} "; }

        //    val = parse.getString("move_down"); //Move Down
        //    if (val != null) { retValue_Drive += "{AltitudeVelocity " + (-double.Parse(val)).ToString() + "} "; }

        //    val = parse.getString("move_rt"); //Rotate Right
        //    if (val != null) { retValue_Drive += "{AltitudeVelocity 0.00} {LinearVelocity 0.00} {LateralVelocity 0.00} {RotationalVelocity " + decimal.Parse(val) * 20 + "} "; }

        //    val = parse.getString("norm"); //Normalized
        //    if (val == null) val = "False";
        //    if (val != null) { retValue_Drive += "{Normalized " + val + "} "; }

        //    if (retValue_Drive.Length > 7)
        //        return retValue_Drive;
        //    return null;
        //}


        public override string GetStopCommand()
        {
            return "Drive {AltitudeVelocity 0} {LinearVelocity 0} {LateralVelocity 0} {RotationalVelocity 0}";
        }
    }
}
