using System.Collections.Generic;
using MRL.Commons;
using MRL.Communication.Internal_Objects;

namespace MRL.Command.Joystick
{
    public class AirRobotCmd : CmdGenerator
    {
        #region Public Methods

        public override string GetDriveCommnad(JoyStickData joyStickData)
        {
            float MaxSpeed = ProjectCommons.config.MAX_ROBOT_SPEED;
            float MinSpeed = ProjectCommons.config.MIN_ROBOT_SPEED;

            //The rotation for Air Robot is more than another type of robots
            float MaxRotSpeed = ProjectCommons.config.MAX_ROTATION_ROBOT_SPEED * 40;
            float MinRotSpeed = ProjectCommons.config.MIN_ROTATION_ROBOT_SPEED * 40;

            int X = joyStickData.X;
            int Y = joyStickData.Y;
            int Z = joyStickData.Z;

            List<int> PressedButtons = joyStickData.PressedButtonsList;

            if (X < 20 && X > -20) X = 0;
            if (Y < 20 && Y > -20) Y = 0;

            float newX = (float)X * MaxSpeed / 100.0f;
            float newY = (float)Y * MaxSpeed / 100.0f;
            float newZ = (float)Y * MaxSpeed / 100.0f;
            
            newX *= 3;
            newY *= 3;
            newZ *= 3;

            float H = 0.0f;
            float R = 0.0f;

            if (PressedButtons.Contains(1))
                H = MinSpeed;
            if (PressedButtons.Contains(2))
                H = MaxSpeed;
            if (PressedButtons.Contains(3))
                R = MinRotSpeed;
            if (PressedButtons.Contains(4))
                R = MaxRotSpeed;

            string DRIVE = "Drive {AltitudeVelocity " + H + "} {LinearVelocity " + -newY + "} {LateralVelocity " + newX + "} {RotationalVelocity " + R + "}";

            return DRIVE;
        }

        #endregion
    }
}
