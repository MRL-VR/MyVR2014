using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MRL.Communication.Internal_Objects;
using MRL.Utils;
using SlimDX;
using SlimDX.DirectInput;

namespace MRL.Commons
{

    public class PilotJoyStickController
    {
        #region Variables

        private Joystick joystick;
        private JoystickState state = new JoystickState();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private JoyStickData currJoyStickData;

        public event Action<JoyStickData> PilotJoyStickData_Received;

        #endregion

        #region Public Methods

        public void Start()
        {
            try
            {
                Task.Factory.StartNew(() => TryToConnect());
            }
            catch (Exception e)
            {
                ProjectCommons.writeConsoleMessage("Pilot JoyStick Manager Crashed!", Utils.ConsoleMessageType.Error);
                USARLog.println("PilotJoyStickManager Crashed >> " + e.ToString(), this.ToString());
            }

        }

        private bool CaptureDevice(IntPtr handle)
        {
            try
            {
                ReleaseDevice(false);

                DirectInput dinput = new DirectInput();

                // search for devices
                foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
                {
                    // create the device
                    try
                    {
                        joystick = new Joystick(dinput, device.InstanceGuid);
                        //joystick.SetCooperativeLevel(handle, CooperativeLevel.Exclusive | CooperativeLevel.Foreground);
                        break;
                    }
                    catch (DirectInputException)
                    {
                        return false;
                    }
                }

                if (joystick == null)
                    return false;

                foreach (DeviceObjectInstance deviceObject in joystick.GetObjects())
                {
                    if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                        joystick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-100, 100);

                }

                joystick.Acquire();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ReleaseDevice(bool byForce)
        {
            if (byForce)
                cts.Cancel();

            if (joystick != null)
            {
                joystick.Unacquire();
                joystick.Dispose();
            }
            joystick = null;
        }

        #endregion

        #region Private Methods

        private void TryToConnect()
        {
            try
            {
                if (cts != null)
                    cts.Cancel();

                cts = new CancellationTokenSource();
                while (!cts.Token.IsCancellationRequested)
                {
                    bool state = CaptureDevice(IntPtr.Zero);
                    if (state)
                    {
                        Task.Factory.StartNew(() => ControlPressedKeys());
                        ProjectCommons.writeConsoleMessage("Pilot Jotstick Connected", ConsoleMessageType.Exclamation);
                        break;
                    }

                    Thread.Sleep(2000);
                }
            }
            catch (Exception e)
            {
                ProjectCommons.writeConsoleMessage("Exception in PilotJoyStickController->TryConnect()", ConsoleMessageType.Error);
                USARLog.println("Exception in TryConnect >> " + e, "PilotJoyStickController");
            }
        }

        private void ControlPressedKeys()
        {
            try
            {
                cts = new CancellationTokenSource();
                while (!cts.Token.IsCancellationRequested)
                {
                    if (joystick == null)
                    {
                        cts.Cancel();
                        continue;
                    }

                    if (joystick.Acquire().IsFailure)
                    {
                        cts.Cancel();
                        continue;
                    }

                    if (joystick.Poll().IsFailure)
                    {
                        cts.Cancel();
                        continue;
                    }

                    state = joystick.GetCurrentState();

                    if (Result.Last.IsFailure)
                    {
                        cts.Cancel();
                        continue;
                    }

                    int xNew = int.Parse(state.X.ToString(CultureInfo.CurrentCulture));
                    int yNew = int.Parse(state.Y.ToString(CultureInfo.CurrentCulture));
                    int zNew = int.Parse(state.Z.ToString(CultureInfo.CurrentCulture));

                    int xRotNew = int.Parse(state.RotationX.ToString(CultureInfo.CurrentCulture));
                    int yRotNew = int.Parse(state.RotationY.ToString(CultureInfo.CurrentCulture));
                    int zRotNew = int.Parse(state.RotationZ.ToString(CultureInfo.CurrentCulture));

                    int[] slideNew = state.GetSliders();
                    int[] povNew = state.GetPointOfViewControllers();
                    bool[] buttonsNew = state.GetButtons();
                    JoyStickData newPJD = new JoyStickData()
                    {
                        X = xNew,
                        Y = yNew,
                        Z = zNew,
                        XRot = xRotNew,
                        YRot = yRotNew,
                        ZRot = zRotNew,
                        slider = slideNew,
                        buttons = buttonsNew,
                        pov = povNew
                    };

                    if (!newPJD.Equals(currJoyStickData))
                    {
                        currJoyStickData = newPJD;
                        if (PilotJoyStickData_Received != null)
                            PilotJoyStickData_Received(currJoyStickData);
                    }

                    Thread.Sleep(100);
                }
            }
            catch
            {
                try
                {
                    ProjectCommons.writeConsoleMessage("Pilot JoyStick Ejected!", Utils.ConsoleMessageType.Error);
                    Task.Factory.StartNew(() => TryToConnect());
                }
                catch (Exception e)
                {
                    ProjectCommons.writeConsoleMessage("Pilot JoyStick Manager Crashed!", Utils.ConsoleMessageType.Error);
                    USARLog.println("PilotJoyStickManager Crashed >> " + e.ToString(), this.ToString());
                }

            }
        }

        #endregion
    }
}
