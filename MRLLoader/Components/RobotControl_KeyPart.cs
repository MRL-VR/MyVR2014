using MRL.Communication.Internal_Commands;
using MRL.Communication.Internal_Objects;
using MRL.Utils;
using SlimDX.DirectInput;
using MRL.Commons;
using MRL.ComStation;

namespace MRL.Components
{
    public partial class RobotControl
    {
        private static double keyTransSpeed = 0.5;
        private static double deltaTransSpeed = 0.4;
        private static double keyRotSpeed = 0.2;
        private static double deltaRotSpeed = 0.1;
        private static double keyArmDeg_Seed = 10;
        private Key prevKey = Key.Escape;

        public void ProcessRobotKey(Key key)
        {
            // return in switch if you don't want to keep "prevKey"
            switch (key)
            {
                case Key.L:
                    PressedKeys pk = new PressedKeys();
                    pk.keysList.Add(Key.L);
                    SendString(pk);
                    //RobotLamp = !RobotLamp;
                    //SendString(new Drive(USARDrive.toUSARCommand(RobotLamp)));
                    return;
                case Key.R:
                    if (keyTransSpeed + deltaTransSpeed <= ProjectCommons.config.MAX_ROBOT_SPEED)
                    {
                        keyTransSpeed += deltaTransSpeed;

                        if (prevKey == Key.UpArrow)
                            SendDriveCommand(keyTransSpeed, DriveType.Straight);
                        else if (prevKey == Key.DownArrow)
                            SendDriveCommand(-keyTransSpeed, DriveType.Straight);
                    }
                    return;

                case Key.F:
                    if (keyTransSpeed - deltaTransSpeed >= deltaTransSpeed)
                    {
                        keyTransSpeed -= deltaTransSpeed;

                        if (prevKey == Key.UpArrow)
                            SendDriveCommand(keyTransSpeed, DriveType.Straight);
                        else if (prevKey == Key.DownArrow)
                            SendDriveCommand(-keyTransSpeed, DriveType.Straight);
                    }
                    return;

                case Key.T:
                    if (keyRotSpeed + deltaRotSpeed <= ProjectCommons.config.MAX_ROTATION_ROBOT_SPEED)
                    {
                        keyRotSpeed += deltaRotSpeed;

                        if (prevKey == Key.LeftArrow)
                            SendDriveCommand(-keyRotSpeed, DriveType.Rotate);
                        else if (prevKey == Key.RightArrow)
                            SendDriveCommand(keyRotSpeed, DriveType.Rotate);
                    }
                    return;

                case Key.G:
                    if (keyRotSpeed - deltaRotSpeed >= deltaRotSpeed)
                    {
                        keyRotSpeed -= deltaRotSpeed;

                        if (prevKey == Key.LeftArrow)
                            SendDriveCommand(-keyRotSpeed, DriveType.Rotate);
                        else if (prevKey == Key.RightArrow)
                            SendDriveCommand(keyRotSpeed, DriveType.Rotate);
                    }
                    return;

                case Key.I:
                    BaseStation.Instance.ActiveImageServer();
                    break;

                case Key.P:
                    BaseStation.Instance.DeactiveImageServer();
                    break;

                case Key.UpArrow:
                    SendDriveCommand(keyTransSpeed, DriveType.Straight);
                    break;

                case Key.DownArrow:
                    SendDriveCommand(-keyTransSpeed, DriveType.Straight);
                    break;

                case Key.LeftArrow:
                    SendDriveCommand(-keyRotSpeed, DriveType.Rotate);
                    break;

                case Key.RightArrow:
                    SendDriveCommand(keyRotSpeed, DriveType.Rotate);
                    break;

                case Key.PageUp:
                    SendDriveCommand(keyRotSpeed, DriveType.Fly);
                    break;

                case Key.PageDown:
                    SendDriveCommand(-keyRotSpeed, DriveType.Fly);
                    break;

                case Key.Space:
                    SendString(new AutonomousChange() { Tag = "Auto" });
                    break;

                case Key.Escape:
                    PressedKeys pke = new PressedKeys();
                    pke.keysList.Add(Key.Escape);
                    SendString(pke);

                    SendDriveCommand(0, DriveType.MoveStop);
                    break;

                case Key.Q:
                    BaseStation.Instance.CaptureCameraImage();
                    return;

                case Key.Y:
                    if (!RobotType.Equals("Kenaf")) return;
                    ArmDeg_Reduce(keyArmDeg_Seed);
                    SendArmCommand(getArmValues());
                    return;

                case Key.H:
                    if (!RobotType.Equals("Kenaf")) return;
                    ArmDeg_Add(keyArmDeg_Seed);
                    SendArmCommand(getArmValues());
                    return;

                case Key.F1:
                    if (!RobotType.Equals("Kenaf")) return;
                    ArmChk_fl.Checked = !ArmChk_fl.Checked;
                    return;

                case Key.F2:
                    if (!RobotType.Equals("Kenaf")) return;
                    ArmChk_fr.Checked = !ArmChk_fr.Checked;
                    return;

                case Key.F3:
                    if (!RobotType.Equals("Kenaf")) return;
                    ArmChk_rl.Checked = !ArmChk_rl.Checked;
                    return;

                case Key.F4:
                    if (!RobotType.Equals("Kenaf")) return;
                    ArmChk_rr.Checked = !ArmChk_rr.Checked;
                    return;
            }
            prevKey = key;
        }
    }
}
