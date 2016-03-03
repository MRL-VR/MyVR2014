using System;
using Microsoft.Win32;

namespace MRL.Commons
{

    public class RegisteryController
    {
        public enum DataType { Desimal = 0, Binary = 1, String = 2, DWord = 3, }
        public enum Root
        {
            /// <summary>
            /// HKEY_CLASSES_ROOT
            /// </summary>
            ClassesRoot = 0,

            /// <summary>
            /// HKEY_CURRENT_USER
            /// </summary>
            CurrentUser = 1,

            /// <summary>
            /// HKEY_LOCAL_MACHINE
            /// </summary>
            LocalMachine = 2,

            /// <summary>
            /// HKEY_CURRENT_CONFIG
            /// </summary>
            CurrentConfig = 3,

            /// <summary>
            /// HKEY_USERS
            /// </summary>
            Users = 4,
        }

        private byte[] StrToByte(string str)
        {
            string[] preResult = str.Split(' ');
            byte[] result = { byte.Parse(preResult[0]), byte.Parse(preResult[1]), byte.Parse(preResult[2]), byte.Parse(preResult[3]) };
            return result;
        }
        
        /// <summary>
        /// this class set the vatlue for registry
        /// </summary>
        /// <param name="root">inset one of the Registry key</param>
        /// <param name="Address">type your destination adderess</param>
        /// <param name="Name">type the field name</param>
        /// <param name="Value">type the field value</param>
        /// <param name="Type">type the type of field</param>
        /// <returns></returns>
        public Boolean SetRegValue(Root root, string Address, string Name, string Value, DataType Type)
        {
            RegistryKey reg;
            reg = Registry.Users.CreateSubKey("");
            if (root == Root.ClassesRoot) { reg = Registry.ClassesRoot.CreateSubKey(Address); }
            else if (root == Root.CurrentUser) { reg = Registry.CurrentUser.CreateSubKey(Address); }
            else if (root == Root.LocalMachine) { reg = Registry.LocalMachine.CreateSubKey(Address); }
            else if (root == Root.CurrentConfig) { reg = Registry.CurrentConfig.CreateSubKey(Address); }
            else if (root == Root.Users) { reg = Registry.Users.CreateSubKey(Address); }

            if (Type == DataType.String) { reg.SetValue(Name, Value); }
            else if (Type == DataType.DWord) { reg.SetValue(Name, int.Parse(Value)); }
            else if (Type == DataType.Binary) { reg.SetValue(Name, StrToByte(Value)); }
            return true;
        }
        public void RenameRegFolder(Root root, string Address, string NewName)
        {

        }
        public void DelRegValue(Root root, string Address, string Name)
        {
            RegistryKey reg;
            reg = Registry.Users.CreateSubKey("");
            if (root == Root.ClassesRoot) { reg = Registry.ClassesRoot.CreateSubKey(Address); }
            else if (root == Root.CurrentUser) { reg = Registry.CurrentUser.CreateSubKey(Address); }
            else if (root == Root.LocalMachine) { reg = Registry.LocalMachine.CreateSubKey(Address); }
            else if (root == Root.CurrentConfig) { reg = Registry.CurrentConfig.CreateSubKey(Address); }
            else if (root == Root.Users) { reg = Registry.Users.CreateSubKey(Address); }

            Registry.CurrentUser.SetValue("", "");
            reg.DeleteValue(Name);
        }
        public string getRegValue(Root root, string address, string name)
        {
            RegistryKey reg;
            reg = Registry.Users.CreateSubKey("");
            if (root == Root.ClassesRoot) { reg = Registry.ClassesRoot.CreateSubKey(address); }
            else if (root == Root.CurrentUser) { reg = Registry.CurrentUser.CreateSubKey(address); }
            else if (root == Root.LocalMachine) { reg = Registry.LocalMachine.CreateSubKey(address); }
            else if (root == Root.CurrentConfig) { reg = Registry.CurrentConfig.CreateSubKey(address); }
            else if (root == Root.Users) { reg = Registry.Users.CreateSubKey(address); }

            if (reg == null) return "";
            try
            {
                return reg.GetValue(name).ToString();
            }
            catch (Exception ex) { return ""; }
        }
    }

}
