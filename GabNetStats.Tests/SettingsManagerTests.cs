using GabNetStats.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GabNetStats.Tests;

[TestClass]
public sealed class SettingsManagerTests
{
    [TestMethod]
    public void ValidateSettingsRepairsInvalidValues()
    {
        int originalBlinkDuration = Settings.Default.BlinkDuration;
        long originalDownloadMultiplier = Settings.Default.BandwidthDownloadMultiplier;
        long originalUploadMultiplier = Settings.Default.BandwidthUploadMultiplier;
        int originalBandwidthUnit = Settings.Default.BandwidthUnit;
        long originalDownload = Settings.Default.BandwidthDownload;
        long originalUpload = Settings.Default.BandwidthUpload;

        try
        {
            Settings.Default.BlinkDuration = NetworkStatsWorker.BlinkDurationMinimum - 1;
            Settings.Default.BandwidthDownloadMultiplier = -1;
            Settings.Default.BandwidthUploadMultiplier = -1;
            Settings.Default.BandwidthUnit = -1;
            Settings.Default.BandwidthDownload = 0;
            Settings.Default.BandwidthUpload = 0;

            SettingsManager.ValidateSettings();

            Assert.AreEqual(NetworkStatsWorker.BlinkDurationMinimum, Settings.Default.BlinkDuration);
            Assert.AreEqual((long)SpeedUtils.eBandwidthMultiplier.un, Settings.Default.BandwidthDownloadMultiplier);
            Assert.AreEqual((long)SpeedUtils.eBandwidthMultiplier.un, Settings.Default.BandwidthUploadMultiplier);
            Assert.AreEqual((int)SpeedUtils.BandwidthUnit.Byte, Settings.Default.BandwidthUnit);
            Assert.AreEqual(SettingsManager.DEFAULT_BANDWIDTH_BPS, Settings.Default.BandwidthDownload);
            Assert.AreEqual(SettingsManager.DEFAULT_BANDWIDTH_BPS, Settings.Default.BandwidthUpload);
        }
        finally
        {
            Settings.Default.BlinkDuration = originalBlinkDuration;
            Settings.Default.BandwidthDownloadMultiplier = originalDownloadMultiplier;
            Settings.Default.BandwidthUploadMultiplier = originalUploadMultiplier;
            Settings.Default.BandwidthUnit = originalBandwidthUnit;
            Settings.Default.BandwidthDownload = originalDownload;
            Settings.Default.BandwidthUpload = originalUpload;
        }
    }
}
