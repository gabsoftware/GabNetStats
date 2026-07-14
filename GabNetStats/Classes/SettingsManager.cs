using GabNetStats.Properties;
using System;
using System.Windows.Forms;

namespace GabNetStats
{
    internal static class SettingsManager
    {
        //
        //  Constants
        //
        internal const long DEFAULT_BANDWIDTH_BPS = 12500000;
        private const int SAVE_DEBOUNCE_MS = 750;

        private static readonly Timer saveTimer = new Timer { Interval = SAVE_DEBOUNCE_MS };

        static SettingsManager()
        {
            saveTimer.Tick += saveTimer_Tick;
        }

        internal static void ScheduleSave()
        {
            saveTimer.Stop();
            saveTimer.Start();
        }

        internal static void FlushPendingSave()
        {
            saveTimer.Stop();
            Settings.Default.Save();
        }

        private static void saveTimer_Tick(object sender, EventArgs e)
        {
            FlushPendingSave();
        }

        internal static void ValidateSettings()
        {
            if (Settings.Default.BlinkDuration < NetworkStatsWorker.BlinkDurationMinimum)
            {
                Settings.Default.BlinkDuration = NetworkStatsWorker.BlinkDurationMinimum;
            }

            if (Settings.Default.BandwidthDownloadMultiplier != (long)SpeedUtils.eBandwidthMultiplier.un
             && Settings.Default.BandwidthDownloadMultiplier != (long)SpeedUtils.eBandwidthMultiplier.K
             && Settings.Default.BandwidthDownloadMultiplier != (long)SpeedUtils.eBandwidthMultiplier.M
             && Settings.Default.BandwidthDownloadMultiplier != (long)SpeedUtils.eBandwidthMultiplier.G
             && Settings.Default.BandwidthDownloadMultiplier != (long)SpeedUtils.eBandwidthMultiplier.T)
            {
                Settings.Default.BandwidthDownloadMultiplier = (long)SpeedUtils.eBandwidthMultiplier.un;
            }

            if (Settings.Default.BandwidthUploadMultiplier != (long)SpeedUtils.eBandwidthMultiplier.un
             && Settings.Default.BandwidthUploadMultiplier != (long)SpeedUtils.eBandwidthMultiplier.K
             && Settings.Default.BandwidthUploadMultiplier != (long)SpeedUtils.eBandwidthMultiplier.M
             && Settings.Default.BandwidthUploadMultiplier != (long)SpeedUtils.eBandwidthMultiplier.G
             && Settings.Default.BandwidthUploadMultiplier != (long)SpeedUtils.eBandwidthMultiplier.T)
            {
                Settings.Default.BandwidthUploadMultiplier = (long)SpeedUtils.eBandwidthMultiplier.un;
            }

            if (Settings.Default.BandwidthUnit != (int)SpeedUtils.BandwidthUnit.Bit
             && Settings.Default.BandwidthUnit != (int)SpeedUtils.BandwidthUnit.Byte)
            {
                Settings.Default.BandwidthUnit = (int)SpeedUtils.BandwidthUnit.Byte;
            }

            if (Settings.Default.BandwidthDownload <= 0)
            {
                Settings.Default.BandwidthDownload = DEFAULT_BANDWIDTH_BPS;
            }

            if (Settings.Default.BandwidthUpload <= 0)
            {
                Settings.Default.BandwidthUpload = DEFAULT_BANDWIDTH_BPS;
            }

            NormalizeBandwidthVisualMode();
        }

        private static void NormalizeBandwidthVisualMode()
        {
            bool custom = Settings.Default.BandwidthVisualsCustom;
            bool auto = Settings.Default.BandwidthVisualsAuto;

            if (custom)
            {
                Settings.Default.BandwidthVisualsDefault = false;
                Settings.Default.BandwidthVisualsAuto = false;
                return;
            }

            if (auto)
            {
                Settings.Default.BandwidthVisualsDefault = false;
                Settings.Default.BandwidthVisualsCustom = false;
                return;
            }

            Settings.Default.BandwidthVisualsDefault = true;
            Settings.Default.BandwidthVisualsCustom = false;
            Settings.Default.BandwidthVisualsAuto = false;
        }
    }
}
