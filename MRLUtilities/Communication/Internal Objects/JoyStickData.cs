using System.Collections.Generic;
using System.IO;
using MRL.Utils;

namespace MRL.Communication.Internal_Objects
{
    public class JoyStickData:BaseInternalObject
    {
        #region Variabales

        public float CurrSpeed { get; set; }
        public float CurrRotationSpeed { set; get; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int XRot { get; set; }
        public int YRot { get; set; }
        public int ZRot { get; set; }
        public int[] slider { get; set; }
        public int[] pov { get; set; }
        public bool[] buttons { get; set; }

        #endregion

        #region Public Methods

        public List<int> PressedButtonsList
        {
            get
            {
                List<int> PressedButtons = new List<int>();

                for (int b = 0; b < buttons.Length; b++)
                    if (buttons[b])
                        PressedButtons.Add(b);

                return PressedButtons;
            }
        }

        public override byte MessageID
        {
            get { return (byte)InternalMessagesID.JoyStickData; ; }
        }

        public override byte[] Serialize()
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (BinaryWriter mWriter = new BinaryWriter(mStream))
                {
                    mWriter.Write(MessageID);

                    mWriter.Write(CurrSpeed);
                    mWriter.Write(CurrRotationSpeed);
                    
                    mWriter.Write(X);
                    mWriter.Write(Y);
                    mWriter.Write(Z);
                    
                    mWriter.Write(XRot);
                    mWriter.Write(YRot);
                    mWriter.Write(ZRot);

                    mWriter.Write(slider.Length);
                    for (int i = 0; i < slider.Length; i++)
                        mWriter.Write(slider[i]);

                    mWriter.Write(pov.Length);
                    for (int i = 0; i < pov.Length; i++)
                        mWriter.Write(pov[i]);

                    mWriter.Write(buttons.Length);
                    for (int i = 0; i < buttons.Length; i++)
                        mWriter.Write(buttons[i]);
                }
                return mStream.ToArray();
            }

        }

        public override void Deserialize(byte[] buffer)
        {
            using (MemoryStream mStream = new MemoryStream(buffer))
            {
                using (BinaryReader mReader = new BinaryReader(mStream))
                {
                    mReader.ReadByte();

                    CurrSpeed = mReader.ReadSingle();
                    CurrRotationSpeed = mReader.ReadSingle();

                    X = mReader.ReadInt32();
                    Y = mReader.ReadInt32();
                    Z = mReader.ReadInt32();
                    
                    XRot = mReader.ReadInt32();
                    YRot = mReader.ReadInt32();
                    ZRot = mReader.ReadInt32();

                    int sliderLenght = mReader.ReadInt32();
                    slider = new int[sliderLenght];
                    for (int i = 0; i < sliderLenght; i++)
                        slider[i] = mReader.ReadInt32();

                    int povLenght = mReader.ReadInt32();
                    pov = new int[povLenght];
                    for (int i = 0; i < povLenght; i++)
                        pov[i] = mReader.ReadInt32();

                    int buttonsLenght = mReader.ReadInt32();
                    buttons = new bool[buttonsLenght];
                    for (int i = 0; i < buttonsLenght; i++)
                        buttons[i] = mReader.ReadBoolean();
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            JoyStickData tmpObj = obj as JoyStickData;

            if (this.X != tmpObj.X)
                return false;
            if (this.Y != tmpObj.Y)
                return false;
            if (this.Z != tmpObj.Z)
                return false;

            if (this.XRot != tmpObj.XRot)
                return false;
            if (this.YRot != tmpObj.YRot)
                return false;
            if (this.ZRot != tmpObj.ZRot)
                return false;

            if (!compare(this.slider, tmpObj.slider))
                return false;

            if (!compare(this.pov, tmpObj.pov))
                return false;

            if (!compare(this.buttons, tmpObj.buttons))
                return false;

            return true;
        }

        #endregion

        #region Private Methods

        private bool compare(int[] arr1, int[] arr2)
        {
            if (arr1.Length == arr2.Length)
            {
                for (int i = 0; i < arr1.Length; i++)
                    if (arr1[i] != arr2[i])
                        return false;
            }
            else
                return false;
            return true;
        }

        private bool compare(bool[] arr1, bool[] arr2)
        {
            if (arr1.Length == arr2.Length)
            {
                for (int i = 0; i < arr1.Length; i++)
                    if (arr1[i] != arr2[i])
                        return false;
            }
            else
                return false;
            return true;
        }

        #endregion
    }

}
