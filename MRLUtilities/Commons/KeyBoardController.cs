using System;
using System.Threading;
using MRL.Utils;
using SlimDX;
using SlimDX.DirectInput;

namespace MRL.Commons
{
    public class KeyBoardController : IDisposable
    {
        #region "Variables"

        private Keyboard keyboard;
        private volatile bool isConnected = false;

        public delegate void keyPressed(KeyboardState states);
        public event keyPressed KeyPressed;

        #endregion

        #region "Public Methods"

        public void Connect(IntPtr windowsHandle)
        {
            // make sure that DirectInput has been initialized
            DirectInput dinput = new DirectInput();

            // build up cooperative flag
            //CooperativeLevel cooperativeLevel;

            //cooperativeLevel = CooperativeLevel.Exclusive;
            //cooperativeLevel = CooperativeLevel.Nonexclusive;
            //cooperativeLevel |= CooperativeLevel.Foreground;
            //cooperativeLevel |= CooperativeLevel.Background;
            //cooperativeLevel |= CooperativeLevel.NoWinKey;

            // create the device
            try
            {
                keyboard = new Keyboard(dinput);
            //    keyboard.SetCooperativeLevel(windowsHandle, cooperativeLevel);
            }
            catch (DirectInputException e)
            {
                ProjectCommons.writeConsoleMessage("Connection fialed to keyboard : " + e.Message, Utils.ConsoleMessageType.Error);
                keyboard = null;
                return;
            }


            keyboard.Acquire();
            isConnected = true;
            ThreadPool.QueueUserWorkItem(ControlPressedKeys);
        }

        public void Disconnect()
        {
            isConnected = false;
            if (keyboard != null)
            {
                keyboard.Unacquire();
                keyboard.Dispose();
            }
            keyboard = null;
        }

        public void Dispose()
        {
            if (!isConnected) return;

            isConnected = false;
            KeyPressed = null;
        }


        #endregion

        #region "Private Methods"
        private void ControlPressedKeys(object o)
        {
            try
            {
            while (isConnected)
            {
                Thread.Sleep(83);
                ReadBufferedData();
            }
            }
            catch (Exception e)
            {
                ProjectCommons.writeConsoleMessage("Keyboard Crashed", Utils.ConsoleMessageType.Error);
                USARLog.println("Keyboard Crashed >> " + e.ToString(), this.ToString());
            }
        }

        private void ReadBufferedData()
        {

            if (keyboard.Acquire().IsFailure)
                return;

            if (keyboard.Poll().IsFailure)
                return;

            KeyboardState state = keyboard.GetCurrentState();

            if (Result.Last.IsFailure)
                return;
            KeyPressed(state);
        }

        #endregion

        //public delegate void keyPressed(KeyboardState state);
        //public event keyPressed KeyPressed;

        //private AutoResetEvent _AreReceiveKey;
        //private Thread _threadReceiveKey;
        //private Device _keyBoard;
        //private volatile bool isConnected = false;

        //#endregion

        //#region "-----Constructor----"

        //public KeyBoardController()
        //{
        //    _AreReceiveKey = new AutoResetEvent(false);
        //    _threadReceiveKey = new Thread(new ThreadStart(beginToAcquire));
        //    _threadReceiveKey.IsBackground = true;
        //}

        //#endregion

        //#region "-----Public Methode----"
        //public bool Connect()
        //{
        //    isConnected = false;
        //    try
        //    {
        //        _keyBoard = new Device(SystemGuid.Keyboard);

        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    isConnected = true;

        //    return true;
        //}
        //public bool Start()
        //{
        //    if (!isConnected)
        //        return false;
        //    _keyBoard.SetEventNotification(_AreReceiveKey);
        //    _threadReceiveKey.Start();
        //    _keyBoard.Acquire();
        //    return true;
        //}
        //public void Stop()
        //{
        //    _keyBoard.Unacquire();
        //    Dispose();
        //}
        //public void Dispose()
        //{
        //    if (!isConnected) return;

        //    isConnected = false;
        //    KeyPressed = null;
        //    _AreReceiveKey.Set();
        //}
        //#endregion

        //#region "-----private Methode----"

        //private void beginToAcquire()
        //{
        //    //if (!isConnected) return;
        //    while (true)
        //    {
        //        _AreReceiveKey.WaitOne();
        //        if (!isConnected) return;
        //        KeyboardState state = _keyBoard.GetCurrentKeyboardState();
        //        KeyPressed(state);
        //    }
        //}

        //#endregion
    }
}
