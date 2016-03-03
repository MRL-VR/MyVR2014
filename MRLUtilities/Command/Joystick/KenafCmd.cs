using System;
using System.Collections.Generic;
using MRL.Commons;
using MRL.Communication.Internal_Objects;

namespace MRL.Command.Joystick
{
    public class KenafCmd : CmdGenerator
    {
        public override string GetDriveCommnad(JoyStickData joyStickData)
        {
            float MaxSpeed = ProjectCommons.config.MAX_ROBOT_SPEED;
            float MinSpeed = ProjectCommons.config.MIN_ROBOT_SPEED;

            //The rotation for Air Robot is more than another type of robots
            float MaxRotSpeed = ProjectCommons.config.MAX_ROTATION_ROBOT_SPEED;
            float MinRotSpeed = ProjectCommons.config.MIN_ROTATION_ROBOT_SPEED;

            int X = joyStickData.X;
            int Y = joyStickData.Y;
            int Z = joyStickData.Z;

            List<int> PressedButtons = joyStickData.PressedButtonsList;

            if (X < 20 && X > -20) X = 0;
            if (Y < 20 && Y > -20) Y = 0;

            float leftW = CalcLeftWheel(X, -Y) * MaxSpeed / 100.0f;
            float rightW = CalcRightWheel(X, -Y) * MaxSpeed / 100.0f;


            if (PressedButtons.Contains(1))
            {
                leftW = MinSpeed;
                rightW = MinSpeed;
            }

            if (PressedButtons.Contains(2))
            {
                leftW = MaxSpeed;
                rightW = MaxSpeed;
            }

            if (PressedButtons.Contains(3))
            {
                leftW = MinRotSpeed;
                rightW = MaxRotSpeed;
            }

            if (PressedButtons.Contains(4))
            {
                leftW = MaxRotSpeed;
                rightW = MinRotSpeed;
            }

            leftW /= 1f;
            rightW /= 1f;
            string DRIVE = "DRIVE {Left " + leftW + "} {Right " + rightW + "} {Normalized False}";
            return DRIVE;
        }

        #region Private Methods

        private float CalcLeftWheel(float x, float y)
        {
            float xx = Math.Abs(x);
            float yy = Math.Abs(y);
            if (x < 10 && x > -10)
                x = 0;
            if (y < 10 && y > -10)
                y = 0;
            if (y == 0 || x == 0)
                return (y + x);
            else
                if (x < 0)
                    if (y > 0)
                        y = Math.Abs(yy - (1.0f * xx));
                    else if (y < 0)
                        y = -(Math.Abs(yy - (1.0f * xx)));
            return y;
        }

        private float CalcRightWheel(float x, float y)
        {
            float xx = Math.Abs(x);
            float yy = Math.Abs(y);
            if (x < 10 && x > -10)
                x = 0;
            if (y < 10 && y > -10)
                y = 0;
            if (x == 0 || y == 0)
                return (y - x);
            else
                if (x > 0)
                    if (y > 0)
                        y = Math.Abs(yy - (1.0f * xx));
                    else if (y < 0)
                        y = -(Math.Abs(yy - (1.0f * xx)));
            return y;
        }

        #endregion

    }
}
