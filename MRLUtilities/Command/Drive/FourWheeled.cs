
using MRL.Utils;

namespace MRL.Command.Drive
{
    public class FourWheeledDrive : AbstractDrive
    {
        public FourWheeledDrive()
        {
            _type = "FourWheeled";
        }
        public override string Type
        {
            get { return _type; }
        }

        /// <summary>
        /// {move_lw, move_rw, move_fw, move_bw, move_lt,move_rt, light, norm}
        /// </summary>
        /// <param name="cmd"></param>
        /// <exception cref="move_lw value --> this command give 'value' speed to the robot left wheel."/>
        /// <exception cref="move_rw value --> this command give 'value' speed to the robot right wheel."/>
        /// <exception cref="move_fw value --> this command give 'value' speed to both wheels of the robot."/>
        /// <exception cref="move_bw value --> this command give '-value' speed to both wheels of the robot."/>
        /// <exception cref="move_lt value --> this command give '-value' speed to the left wheel and 'value' speed to the right wheel. by the way robot will turn left."/>
        /// <exception cref="move_rt value --> this command give 'value' speed to the left wheel and '-value' speed to the right wheel. by the way robot will turn right."/>
        /// <exception cref="light value --> this command give 'value' state to the robot light."/>
        /// <exception cref="norm value --> if you set this true, you shuld give all 'value' between -100 and 100."/>
        /// <returns></returns>
        public override string GetCommand(string cmd)
        {
            string retValue = "Drive ";

            //pase commande
            USARParser parse = new USARParser(cmd);

            string val = null;

            val = parse.getString("Left"); //Left Wheel
            if (val != null) { retValue += "{Left " + val + "} "; }

            val = parse.getString("Right"); //Left Wheel
            if (val != null) { retValue += "{Right " + val + "} "; }

            val = parse.getString("move_s"); //Left Wheel
            if (val != null) { retValue += "{Left 0} {Right 0} "; }

            val = parse.getString("move_lw"); //Left Wheel
            if (val != null) { retValue += "{Left " + val + "} "; }

            val = parse.getString("move_rw"); //Right Wheel
            if (val != null) { retValue += "{Right " + val + "} "; }

            val = parse.getString("move_fw"); //Foreward (Left Right)
            if (val != null) { retValue += "{Left " + val + "} " + "{Right " + val + "} "; }

            val = parse.getString("move_bw"); //Backward (Left Right)
            if (val != null) { retValue += "{Left " + (-double.Parse(val)).ToString() + "} " + "{Right " + (-double.Parse(val)).ToString() + "} "; }

            val = parse.getString("move_lt"); //Left Turn
            if (val != null) { retValue += "{Left " + (-double.Parse(val)).ToString() + "} " + "{Right " + val + "} "; }

            val = parse.getString("move_rt"); //Right Turn
            if (val != null) { retValue += "{Left " + val + "} " + "{Right " + (-double.Parse(val)).ToString() + "} "; }

            val = parse.getString("light"); //Light
            if (val != null) { retValue += "{Light " + val + "} "; }

            val = parse.getString("norm");
            if (val == null) val = "False";
            if (val != null) { retValue += "{Normalized " + val + "} "; }


            if (retValue.Length > 8)
                return retValue;
            return null;
        }

        public override string GetStopCommand()
        {
            return "DRIVE {Left 0} {Right 0} {Normalized False}";
        }
    }
}
