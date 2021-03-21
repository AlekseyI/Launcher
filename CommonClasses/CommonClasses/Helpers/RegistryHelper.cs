using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace CommonClasses.Helpers
{
    public class RegistryHelper
    {
        private RegistryKey[] _baseKeys = new RegistryKey[5] { Registry.LocalMachine, Registry.Users, Registry.CurrentUser, Registry.CurrentConfig, Registry.ClassesRoot };

        private RegistryHive[] _baseHive = new RegistryHive[5] { RegistryHive.LocalMachine, RegistryHive.Users, RegistryHive.CurrentUser, RegistryHive.CurrentConfig, RegistryHive.ClassesRoot };

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

        public static bool Is64Bit()
        {
            bool retVal;
            IsWow64Process(Process.GetCurrentProcess().Handle, out retVal);
            return retVal;
        }

        public RegistryHelper()
        {
            for (int i = 0; i < _baseKeys.Length; i++)
            {
                if (Is64Bit())
                {
                    _baseKeys[i] = RegistryKey.OpenBaseKey(_baseHive[i], RegistryView.Registry64);
                }
                else
                {
                    _baseKeys[i] = RegistryKey.OpenBaseKey(_baseHive[i], RegistryView.Registry32);
                }

            }
        }


        public void DeleteKey(string path, string delKey = null)
        {
            
            foreach (var baseKey in _baseKeys)
            {
                if (path.Contains(baseKey.Name))
                {                    
                    var newPath = new StringBuilder(path);
                    newPath = newPath.Remove(0, baseKey.Name.Length + 1);
                    if (delKey != null)
                    {
                        var key = baseKey.OpenSubKey(newPath.ToString(), true);
                        if (key != null)
                        {
                            key.DeleteValue(delKey, false);
                            key.Close();
                        }
                        
                    }
                    else
                    {
                        var patSub = @"\w+";
                        var matches = Regex.Matches(path, patSub);
                        string lastSub = null;
                        foreach (Match m in matches)
                        {
                            lastSub = m.Value;
                        }
                        

                        if (lastSub != newPath.ToString())
                        {
                            newPath = newPath.Remove(newPath.Length - lastSub.Length - 1, lastSub.Length + 1);
                            var key = baseKey.OpenSubKey(newPath.ToString(), true);
                            if (key != null)
                            {
                                key.DeleteSubKeyTree(lastSub, false);
                                key.Close();
                            }
                            
                        }
                        else
                        {
                            var key = baseKey;
                            if (baseKey.Name != lastSub)
                            {
                                key.DeleteSubKeyTree(lastSub, false);                               
                            }
                            key.Close();
                        }
                     

                    }
                   

                    break;
                }
            }
        }


    }
}
