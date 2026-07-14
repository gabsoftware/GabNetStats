using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace GabNetStats
{
    internal static class StartupManager
    {
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName         = "GabNetStats";

        internal static bool TrySetStartup(bool enable, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                using (RegistryKey hStartKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true))
                {
                    if (hStartKey == null)
                    {
                        errorMessage = "Unable to open the Windows startup registry key.";
                        return false;
                    }

                    if (enable)
                    {
                        String strPath = Application.ExecutablePath;
                        strPath = "\"" + strPath + "\"";

                        hStartKey.SetValue(AppName, strPath);
                    }
                    else
                    {
                        hStartKey.DeleteValue(AppName, false);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
