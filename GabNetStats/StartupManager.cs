using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace GabNetStats
{
    internal static class StartupManager
    {
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName         = "GabNetStats";

        internal static void SetStartup(bool enable)
        {
            RegistryKey hStartKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true);
            if (hStartKey != null)
            {
                try
                {
                    if (enable)
                    {
                        String strPath = Application.ExecutablePath;
                        strPath = "\"" + strPath + "\"";

                        hStartKey.SetValue(AppName, strPath);
                    }
                    else
                    {
                        hStartKey.DeleteValue(AppName);
                    }
                }
                catch
                {
                    //Catching exceptions is for communists
                }

                hStartKey.Close();
            }
        }
    }
}
