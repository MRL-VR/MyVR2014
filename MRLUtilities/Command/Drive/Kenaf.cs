using System;

using MRL.Utils;

namespace MRL.Command.Drive
{
    public class KenafDrive : AbstractDrive
    {
        public KenafDrive()
        {
            _type = "Kenaf";
        }
        public override string Type
        {
            get { return _type; }
        }
        /// <summary>
        /// {move_lw, move_rw, move_fw, move_bw, move_lt,move_rt, light, norm}
        /// </summary>
        /// <param name="cmd"></param>
        /// <exception cref="Move Robot:"/>
        /// <exception cref="       move_lw value --> this command give 'value' speed to the robot left wheel."/>
        /// <exception cref="       move_rw value --> this command give 'value' speed to the robot right wheel."/>
        /// <exception cref="       move_fw value --> this command give 'value' speed to both wheels of the robot."/>
        /// <exception cref="       move_bw value --> this command give '-value' speed to both wheels of the robot."/>
        /// <exception cref="       move_lt value --> this command give '-value' speed to the left wheel and 'value' speed to the right wheel. by the way robot will turn left."/>
        /// <exception cref="       move_rt value --> this command give 'value' speed to the left wheel and '-value' speed to the right wheel. by the way robot will turn right."/>
        /// <exception cref="Robot Arms:"/>
        /// <exception cref="       deg value --> if you set this true, all value shuld give in degree."/>
        /// <exception cref="       arm_front value --> this command give 'value' angle to the arms in front of the robot in degree or radian."/>
        /// <exception cref="       arm_rear value -->  this command give 'value' angle to the arms in rear of the robot in degree or radian."/>
        /// <exception cref="       arm_all value -->  this command give 'value' angle to all arm of the robot in degree or radian."/>
        /// <exception cref="       arm_fl value -->  this command give 'value' angle to the front-left arm of the robot in degree or radian."/>
        /// <exception cref="       arm_fr value -->  this command give 'value' angle to the front-right arm of the robot in degree or radian."/>
        /// <exception cref="       arm_rl value -->  this command give 'value' angle to the rear-left arm of the robot in degree or radian."/>
        /// <exception cref="       arm_rr value -->  this command give 'value' angle to the rear-right arm of the robot in degree or radian."/>
        /// <exception cref="       norm value --> if you set this true, you shuld give all 'value' between -100 and 100."/>
        /// <returns></returns>
        public override string GetCommand(string cmd)
        {
            string retValue_Drive = "Drive ";
            string retValue_Arm = "MultiDrive ";

            //pase command -> Move Robot
            USARParser parse = new USARParser(cmd);

            string val = null;
            bool isSet = false;

            val = parse.getString("move_s"); //Left Wheel
            if (val != null) { retValue_Drive += "{Left 0} {Right 0} "; isSet = true; }

            val = parse.getString("move_lw");
            if (val != null) { retValue_Drive += "{Left " + val + "} "; isSet = true; }

            val = parse.getString("move_rw");
            if (val != null) { retValue_Drive += "{Right " + val + "} "; isSet = true; }

            val = parse.getString("move_fw");
            if (val != null) { retValue_Drive += "{Left " + val + "} " + "{Right " + val + "} "; isSet = true; }

            val = parse.getString("move_bw");
            if (val != null) { retValue_Drive += "{Left " + (-double.Parse(val)).ToString() + "} " + "{Right " + (-double.Parse(val)).ToString() + "} "; isSet = true; }

            val = parse.getString("move_lt");
            if (val != null) { retValue_Drive += "{Left " + (-double.Parse(val)).ToString() + "} " + "{Right " + val + "} "; isSet = true; }

            val = parse.getString("move_rt");
            if (val != null) { retValue_Drive += "{Left " + val + "} " + "{Right " + (-double.Parse(val)).ToString() + "} "; isSet = true; }

            val = parse.getString("norm");
            if (val == null) val = "False";
            if (val != null) { retValue_Drive += "{Normalized " + val + "} "; }

            if (isSet)
                return retValue_Drive;

            // parse command -> Robot arms

            bool isDeg = true;//is degree

            try { isDeg = Convert.ToBoolean(parse.getString("deg")); }
            catch { }

            val = parse.getString("arm_front");
            if (val != null)
            {
                if (isDeg) val = MathHelper.DegToRad(float.Parse(val)).ToString();
                retValue_Arm += "{FLFlipper " + val + "} {FRFlipper " + val + "} ";
                isSet = true;
            }

            val = parse.getString("arm_rear");
            if (val != null)
            {
                if (isDeg) val = MathHelper.DegToRad(float.Parse(val)).ToString();
                retValue_Arm += "{RLFlipper " + val + "} {RRFlipper " + val + "} ";
                isSet = true;
            }

            val = parse.getString("arm_all");
            if (val != null)
            {
                if (isDeg) val = MathHelper.DegToRad(float.Parse(val)).ToString();
                retValue_Arm += "{FLFlipper " + val + "} {FRFlipper " + val + "} {RLFlipper  " + val + "} {RRFlipper  " + val + "} ";
                isSet = true;
            }

            val = parse.getString("arm_fl");
            if (val != null)
            {
                if (isDeg) val = MathHelper.DegToRad(float.Parse(val)).ToString();
                retValue_Arm += "{FLFlipper " + val + "} ";
                isSet = true;
            }

            val = parse.getString("arm_fr");
            if (val != null)
            {
                if (isDeg) val = MathHelper.DegToRad(float.Parse(val)).ToString();
                retValue_Arm += "{FRFlipper " + val + "} ";
                isSet = true;
            }

            val = parse.getString("arm_rl");
            if (val != null)
            {
                if (isDeg) val = MathHelper.DegToRad(float.Parse(val)).ToString();
                retValue_Arm += "{RLFlipper " + val + "} ";
                isSet = true;
            }

            val = parse.getString("arm_rr");
            if (val != null)
            {
                if (isDeg) val = MathHelper.DegToRad(float.Parse(val)).ToString();
                retValue_Arm += "{RRFlipper " + val + "} ";
                isSet = true;
            }

            val = parse.getString("norm");
            if (val == null) val = "False";
            if (val != null) { retValue_Arm += "{Normalized " + val + "} "; }

            if (isSet)
                return retValue_Arm;

            return null;
        }

        public override string GetStopCommand()
        {
            return "DRIVE {Left 0} {Right 0} {Normalized False}";
        }
    }
}
