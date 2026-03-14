using GabNetStats.Properties;

namespace GabNetStats
{
    internal static class SettingsManager
    {
        internal static void ValidateSettings()
        {
            if (Settings.Default.BlinkDuration < NetworkStatsWorker.BlinkDurationMinimum)
            {
                Settings.Default.BlinkDuration = NetworkStatsWorker.BlinkDurationMinimum;
            }

            if (Settings.Default.BandwidthDownloadMultiplier != (long)TrayIconManager.eBandwidthMultiplier.un
             && Settings.Default.BandwidthDownloadMultiplier != (long)TrayIconManager.eBandwidthMultiplier.K
             && Settings.Default.BandwidthDownloadMultiplier != (long)TrayIconManager.eBandwidthMultiplier.M
             && Settings.Default.BandwidthDownloadMultiplier != (long)TrayIconManager.eBandwidthMultiplier.G
             && Settings.Default.BandwidthDownloadMultiplier != (long)TrayIconManager.eBandwidthMultiplier.T)
            {
                Settings.Default.BandwidthDownloadMultiplier = (long)TrayIconManager.eBandwidthMultiplier.un;
            }

            if (Settings.Default.BandwidthUploadMultiplier != (long)TrayIconManager.eBandwidthMultiplier.un
             && Settings.Default.BandwidthUploadMultiplier != (long)TrayIconManager.eBandwidthMultiplier.K
             && Settings.Default.BandwidthUploadMultiplier != (long)TrayIconManager.eBandwidthMultiplier.M
             && Settings.Default.BandwidthUploadMultiplier != (long)TrayIconManager.eBandwidthMultiplier.G
             && Settings.Default.BandwidthUploadMultiplier != (long)TrayIconManager.eBandwidthMultiplier.T)
            {
                Settings.Default.BandwidthUploadMultiplier = (long)TrayIconManager.eBandwidthMultiplier.un;
            }

            if (Settings.Default.BandwidthUnit != (int)TrayIconManager.eBandwithUnit.bit
             && Settings.Default.BandwidthUnit != (int)TrayIconManager.eBandwithUnit.Byte)
            {
                Settings.Default.BandwidthUnit = (int)TrayIconManager.eBandwithUnit.Byte;
            }

            if (Settings.Default.BandwidthDownload <= 0)
            {
                Settings.Default.BandwidthDownload = 12500000;
            }

            if (Settings.Default.BandwidthUpload <= 0)
            {
                Settings.Default.BandwidthUpload = 12500000;
            }
        }
    }
}
