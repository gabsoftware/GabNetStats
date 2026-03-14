using System;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using GabNetStats.Properties;

namespace GabNetStats
{
    internal class TrayIconManager
    {
        private readonly NotifyIcon _notifyIconActivity;
        private readonly NotifyIcon _notifyIconPing;

        internal enum eState
        {
            disconnected = 0,
            limited      = 1,
            up           = 2
        }

        internal enum eBandwithUnit : int
        {
            bit  = 8,
            Byte = 1
        }
        internal enum eBandwidthMultiplier : long
        {
            un = 1,               // 2^0
            K  = 1024,            // 2^10
            M  = 1048576,         // 2^20
            G  = 1073741824,      // 2^30
            T  = 1099511627776    // 2^40
        }

        internal static class SpeedUnitsByte
        {
            public static string Bytes = Res.str_Bytes;
            public static string KiB   = Res.str_KiB;
            public static string MiB   = Res.str_MiB;
            public static string GiB   = Res.str_GiB;
            public static string TiB   = Res.str_TiB;
        }
        internal static class SpeedUnitsBit
        {
            public static string bit  = Res.str_bit;
            public static string Kbit = Res.str_Kbit;
            public static string Mbit = Res.str_Mbit;
            public static string Gbit = Res.str_Gbit;
            public static string Tbit = Res.str_Tbit;
        }

        // 0=blue, 1=green, 2=yellow, 3=orange, 4=red
        private static readonly string[] colorNames = { "blue", "green", "yellow", "orange", "red" };

        // iconsActive[downloadLevel, uploadLevel]
        internal Icon[,] iconsActive = {
            { Properties.Resources.active_blue_blue,   Properties.Resources.active_blue_green,   Properties.Resources.active_blue_yellow,   Properties.Resources.active_blue_orange,   Properties.Resources.active_blue_red   },
            { Properties.Resources.active_green_blue,  Properties.Resources.active_green_green,  Properties.Resources.active_green_yellow,  Properties.Resources.active_green_orange,  Properties.Resources.active_green_red  },
            { Properties.Resources.active_yellow_blue, Properties.Resources.active_yellow_green, Properties.Resources.active_yellow_yellow, Properties.Resources.active_yellow_orange, Properties.Resources.active_yellow_red },
            { Properties.Resources.active_orange_blue, Properties.Resources.active_orange_green, Properties.Resources.active_orange_yellow, Properties.Resources.active_orange_orange, Properties.Resources.active_orange_red },
            { Properties.Resources.active_red_blue,    Properties.Resources.active_red_green,    Properties.Resources.active_red_yellow,    Properties.Resources.active_red_orange,    Properties.Resources.active_red_red    },
        };

        internal Icon iconInactive        = Properties.Resources.inactive;
        internal Icon currentActivityIcon = Properties.Resources.inactive;

        internal Icon[] iconsSend    = { Properties.Resources.send_blue,    Properties.Resources.send_green,    Properties.Resources.send_yellow,    Properties.Resources.send_orange,    Properties.Resources.send_red    };
        internal Icon[] iconsReceive = { Properties.Resources.receive_blue, Properties.Resources.receive_green, Properties.Resources.receive_yellow, Properties.Resources.receive_orange, Properties.Resources.receive_red };

        internal Icon iconDisconnected         = Properties.Resources.netshell_195;
        internal Icon iconLimited              = Properties.Resources.netshell_IDI_CFI_TRAY_WIRED_WARNING;

        internal Icon iconCircle_green  = Properties.Resources.circle_green;
        internal Icon iconCircle_red    = Properties.Resources.circle_red;
        internal Icon iconCircle_grey   = Properties.Resources.circle_grey;
        internal Icon iconCircle_orange = Properties.Resources.circle_orange;
        private string appliedIconSet = "xp";

        internal long bandwidthDownloadLvl1;
        internal long bandwidthDownloadLvl2;
        internal long bandwidthDownloadLvl3;
        internal long bandwidthDownloadLvl4;
        internal long bandwidthDownloadLvl5;
        internal long bandwidthUploadLvl1;
        internal long bandwidthUploadLvl2;
        internal long bandwidthUploadLvl3;
        internal long bandwidthUploadLvl4;
        internal long bandwidthUploadLvl5;

        internal TrayIconManager(NotifyIcon notifyIconActivity, NotifyIcon notifyIconPing)
        {
            _notifyIconActivity = notifyIconActivity;
            _notifyIconPing     = notifyIconPing;
        }

        internal void applyIconSet()
        {
            string desiredSet = Settings.Default.IconSet;
            if (String.IsNullOrEmpty(desiredSet))
            {
                desiredSet = "xp";
            }

            if (String.Compare(desiredSet, appliedIconSet, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return;
            }

            if (String.Compare(desiredSet, "xp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                LoadDefaultIconSet();
                appliedIconSet = "xp";
                currentActivityIcon = _notifyIconActivity.Icon;
                return;
            }

            string path = Path.Combine(Application.StartupPath, "icons", desiredSet);
            if (!Directory.Exists(path))
            {
                return;
            }

            LoadIconSetFromDirectory(path);
            appliedIconSet = desiredSet;
            currentActivityIcon = _notifyIconActivity.Icon;
        }

        internal static int GetSpeedLevel(long speed, long lvl1, long lvl2, long lvl3, long lvl4)
        {
            if (speed >= lvl4) return 4;
            if (speed >= lvl3) return 3;
            if (speed >= lvl2) return 2;
            if (speed >= lvl1) return 1;
            return 0;
        }

        private static Icon LoadIconFromFile(string filePath, Icon fallback)
        {
            try
            {
                return new Icon(filePath);
            }
            catch (IOException)
            {
                return fallback;
            }
            catch (UnauthorizedAccessException)
            {
                return fallback;
            }
            catch (ArgumentException)
            {
                return fallback;
            }
        }

        private void LoadDefaultIconSet()
        {
            var r = Properties.Resources.ResourceManager;
            for (int dl = 0; dl < 5; dl++)
            {
                for (int ul = 0; ul < 5; ul++)
                {
                    iconsActive[dl, ul] = (Icon)r.GetObject($"active_{colorNames[dl]}_{colorNames[ul]}");
                }
            }

            for (int i = 0; i < 5; i++)
            {
                iconsSend[i]    = (Icon)r.GetObject($"send_{colorNames[i]}");
                iconsReceive[i] = (Icon)r.GetObject($"receive_{colorNames[i]}");
            }

            iconInactive     = Properties.Resources.inactive;

            iconCircle_green  = Properties.Resources.circle_green;
            iconCircle_red    = Properties.Resources.circle_red;
            iconCircle_grey   = Properties.Resources.circle_grey;
            iconCircle_orange = Properties.Resources.circle_orange;
        }

        private void LoadIconSetFromDirectory(string basePath)
        {
            for (int dl = 0; dl < 5; dl++)
            {
                for (int ul = 0; ul < 5; ul++)
                {
                    string name = $"active_{colorNames[dl]}_{colorNames[ul]}.ico";
                    iconsActive[dl, ul] = LoadIconFromFile(Path.Combine(basePath, name), iconsActive[dl, ul]);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                iconsSend[i]    = LoadIconFromFile(Path.Combine(basePath, $"send_{colorNames[i]}.ico"),    iconsSend[i]);
                iconsReceive[i] = LoadIconFromFile(Path.Combine(basePath, $"receive_{colorNames[i]}.ico"), iconsReceive[i]);
            }

            iconInactive  = LoadIconFromFile(Path.Combine(basePath, "inactive.ico"), iconInactive);

            iconCircle_green  = LoadIconFromFile(Path.Combine(basePath, "circle_green.ico" ), iconCircle_green);
            iconCircle_red    = LoadIconFromFile(Path.Combine(basePath, "circle_red.ico"   ), iconCircle_red);
            iconCircle_grey   = LoadIconFromFile(Path.Combine(basePath, "circle_grey.ico"  ), iconCircle_grey);
            iconCircle_orange = LoadIconFromFile(Path.Combine(basePath, "circle_orange.ico"), iconCircle_orange);
        }

        internal void UpdateAutoPingIcon(PingReply reply)
        {
            if (reply == null || reply.Status != IPStatus.Success)
            {
                if (_notifyIconPing.Icon.Equals(iconCircle_green))
                {
                    _notifyIconPing.Icon = iconCircle_orange;
                    _notifyIconPing.Text = _notifyIconPing.BalloonTipText = "Connection issue?";
                    _notifyIconPing.BalloonTipText += "\nThe host \"" + Settings.Default.AutoPingHost + "\" seems to be unreachable.";
                    _notifyIconPing.BalloonTipIcon = ToolTipIcon.Warning;
                }
                else if (_notifyIconPing.Icon.Equals(iconCircle_orange))
                {
                    _notifyIconPing.Icon = iconCircle_red;
                    _notifyIconPing.Text = _notifyIconPing.BalloonTipText = "Connection issue!";
                    _notifyIconPing.BalloonTipText += "\nThe host \"" + Settings.Default.AutoPingHost + "\" could not be reached.";
                    _notifyIconPing.BalloonTipIcon = ToolTipIcon.Error;
                }
                else if (!_notifyIconPing.Icon.Equals(iconCircle_red))
                {
                    _notifyIconPing.Icon = iconCircle_orange;
                    _notifyIconPing.Text = _notifyIconPing.BalloonTipText = "Connection issue?";
                    _notifyIconPing.BalloonTipIcon = ToolTipIcon.Warning;
                }
            }
            else
            {
                _notifyIconPing.Icon = iconCircle_green;
                _notifyIconPing.Text = _notifyIconPing.BalloonTipText = "Connection OK";
                _notifyIconPing.BalloonTipIcon = ToolTipIcon.Info;
            }
        }

        internal void SetActivityIcon(Icon icon)
        {
            if (icon == null)
            {
                return;
            }

            if (!ReferenceEquals(currentActivityIcon, icon))
            {
                currentActivityIcon = icon;
                _notifyIconActivity.Icon = icon;
            }
        }
    }
}
