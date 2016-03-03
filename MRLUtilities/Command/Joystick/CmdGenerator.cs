using MRL.Communication.Internal_Objects;

namespace MRL.Command.Joystick
{
    public abstract class CmdGenerator
    {
        public abstract string GetDriveCommnad(JoyStickData joyStickData);
    }
}
