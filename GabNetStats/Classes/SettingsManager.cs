using GabNetStats.Properties;

namespace GabNetStats
{
    internal static class SettingsManager
    {
        //
        //  Constants
        //
        internal const long DEFAULT_BANDWIDTH_BPS = 12500000;

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

            if (Settings.Default.BandwidthUnit != (int)SpeedUtils.eBandwithUnit.bit
             && Settings.Default.BandwidthUnit != (int)SpeedUtils.eBandwithUnit.Byte)
            {
                Settings.Default.BandwidthUnit = (int)SpeedUtils.eBandwithUnit.Byte;
            }

            if (Settings.Default.BandwidthDownload <= 0)
            {
                Settings.Default.BandwidthDownload = DEFAULT_BANDWIDTH_BPS;
            }

            if (Settings.Default.BandwidthUpload <= 0)
            {
                Settings.Default.BandwidthUpload = DEFAULT_BANDWIDTH_BPS;
            }
        }
    }
}
