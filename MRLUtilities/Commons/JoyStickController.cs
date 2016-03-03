using System;
using System.Drawing;
using System.Threading;
using SlimDX.DirectInput;

namespace MRL.Commons
{

    public class JoyStickController : IDisposable
    {

        #region "-----Variables----"

        public delegate void keyPressed(JoystickState state);
        public event keyPressed KeyPressed;
        public int MinRange = -100000, MaxRange = 100000;
        public PointF NormalState = new PointF();
        public float StateLength = 0;
        public float StateDirection = 0;
        private AutoResetEvent _AreReceiveKey;
        private Thread _threadReceiveKey;
        private Joystick _joyStick;
        private bool isConnected;

        #endregion

        #region "-----Constructor----"

        public JoyStickController()
        {
            _AreReceiveKey = new AutoResetEvent(false);
            _threadReceiveKey = new Thread(new ThreadStart(beginToAcquire));
            _threadReceiveKey.IsBackground = true;
        }

        #endregion

        #region "-----Private Methode------"

        private void beginToAcquire()
        {
            while (true)
            {
                _AreReceiveKey.WaitOne();
                if (!isConnected)
                    break;
                JoystickState state = _joyStick.GetCurrentState();
                float x = state.X, y = state.Y;
                NormalState.X = x / MaxRange;
                NormalState.Y = y / MaxRange;
                StateLength = (float)Math.Pow(Math.Pow(x, 2) + Math.Pow(y, 2), 0.5);
                StateLength /= (float)Math.Pow(2, 0.5);
                StateDirection = (float)Math.Atan2(x, y);
                StateDirection /= (float)Math.PI;
                KeyPressed(state);
            }
        }

        #endregion

        #region "-----Public Methode-----"

        public bool Connect()
        {
            isConnected = false;
            using (DirectInput dinput = new DirectInput())
                foreach (DeviceInstance instance in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
                {
                    _joyStick =new Joystick(dinput, instance.InstanceGuid);
                    new Joystick(dinput, instance.InstanceGuid);
                    break;
                }
            if (_joyStick == null)
                return false;
//            _joyStick.SetDataFormat(DeviceDataFormat.Joystick);

            //foreach (DeviceObjectInstance deviceObj in _joyStick.Objects)
            //{
            //    if ((0 != (deviceObj.ObjectId & (int)DeviceObjectTypeFlags.Axis)))
            //    {
            //        _joyStick.Properties.SetRange(ParameterHow.ById, deviceObj.ObjectId, new InputRange(MinRange, MaxRange));
            //    }
            //}
            isConnected = true;
            return true;
        }

        public void Dispose()
        {
            if (!isConnected) return;

            KeyPressed = null;
            _AreReceiveKey = null;
            _AreReceiveKey.Set();
            _AreReceiveKey = null;
        }

        public void Start()
        {
            if (!isConnected)
                return;
            _joyStick.SetNotification(_AreReceiveKey);
            _joyStick.Acquire();
            _threadReceiveKey.Start();
        }

        #endregion

        #region "-----Property----"

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose();
        }

        #endregion

    }

}
