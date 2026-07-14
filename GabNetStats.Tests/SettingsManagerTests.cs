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
        bool originalDefaultVisuals = Settings.Default.BandwidthVisualsDefault;
        bool originalCustomVisuals = Settings.Default.BandwidthVisualsCustom;
        bool originalAutoVisuals = Settings.Default.BandwidthVisualsAuto;

        try
        {
            Settings.Default.BlinkDuration = NetworkStatsWorker.BlinkDurationMinimum - 1;
            Settings.Default.BandwidthDownloadMultiplier = -1;
            Settings.Default.BandwidthUploadMultiplier = -1;
            Settings.Default.BandwidthUnit = -1;
            Settings.Default.BandwidthDownload = 0;
            Settings.Default.BandwidthUpload = 0;
            Settings.Default.BandwidthVisualsDefault = false;
            Settings.Default.BandwidthVisualsCustom = false;
            Settings.Default.BandwidthVisualsAuto = false;

            SettingsManager.ValidateSettings();

            Assert.AreEqual(NetworkStatsWorker.BlinkDurationMinimum, Settings.Default.BlinkDuration);
            Assert.AreEqual((long)SpeedUtils.eBandwidthMultiplier.un, Settings.Default.BandwidthDownloadMultiplier);
            Assert.AreEqual((long)SpeedUtils.eBandwidthMultiplier.un, Settings.Default.BandwidthUploadMultiplier);
            Assert.AreEqual((int)SpeedUtils.BandwidthUnit.Byte, Settings.Default.BandwidthUnit);
            Assert.AreEqual(SettingsManager.DEFAULT_BANDWIDTH_BPS, Settings.Default.BandwidthDownload);
            Assert.AreEqual(SettingsManager.DEFAULT_BANDWIDTH_BPS, Settings.Default.BandwidthUpload);
            Assert.IsTrue(Settings.Default.BandwidthVisualsDefault);
            Assert.IsFalse(Settings.Default.BandwidthVisualsCustom);
            Assert.IsFalse(Settings.Default.BandwidthVisualsAuto);
        }
        finally
        {
            Settings.Default.BlinkDuration = originalBlinkDuration;
            Settings.Default.BandwidthDownloadMultiplier = originalDownloadMultiplier;
            Settings.Default.BandwidthUploadMultiplier = originalUploadMultiplier;
            Settings.Default.BandwidthUnit = originalBandwidthUnit;
            Settings.Default.BandwidthDownload = originalDownload;
            Settings.Default.BandwidthUpload = originalUpload;
            Settings.Default.BandwidthVisualsDefault = originalDefaultVisuals;
            Settings.Default.BandwidthVisualsCustom = originalCustomVisuals;
            Settings.Default.BandwidthVisualsAuto = originalAutoVisuals;
        }
    }

    [TestMethod]
    public void ValidateSettingsKeepsOnlyOneBandwidthVisualMode()
    {
        bool originalDefaultVisuals = Settings.Default.BandwidthVisualsDefault;
        bool originalCustomVisuals = Settings.Default.BandwidthVisualsCustom;
        bool originalAutoVisuals = Settings.Default.BandwidthVisualsAuto;

        try
        {
            Settings.Default.BandwidthVisualsDefault = true;
            Settings.Default.BandwidthVisualsCustom = true;
            Settings.Default.BandwidthVisualsAuto = true;

            SettingsManager.ValidateSettings();

            Assert.IsFalse(Settings.Default.BandwidthVisualsDefault);
            Assert.IsTrue(Settings.Default.BandwidthVisualsCustom);
            Assert.IsFalse(Settings.Default.BandwidthVisualsAuto);

            Settings.Default.BandwidthVisualsDefault = true;
            Settings.Default.BandwidthVisualsCustom = false;
            Settings.Default.BandwidthVisualsAuto = true;

            SettingsManager.ValidateSettings();

            Assert.IsFalse(Settings.Default.BandwidthVisualsDefault);
            Assert.IsFalse(Settings.Default.BandwidthVisualsCustom);
            Assert.IsTrue(Settings.Default.BandwidthVisualsAuto);
        }
        finally
        {
            Settings.Default.BandwidthVisualsDefault = originalDefaultVisuals;
            Settings.Default.BandwidthVisualsCustom = originalCustomVisuals;
            Settings.Default.BandwidthVisualsAuto = originalAutoVisuals;
        }
    }

    [TestMethod]
    public void ComputeAutoBandwidthUsesFastestPositiveLinkSpeed()
    {
        Assert.AreEqual(
            SettingsManager.DEFAULT_BANDWIDTH_BPS,
            NetworkStatsWorker.ComputeAutoBandwidthBytesPerSecond(Array.Empty<long>()));

        Assert.AreEqual(
            SettingsManager.DEFAULT_BANDWIDTH_BPS,
            NetworkStatsWorker.ComputeAutoBandwidthBytesPerSecond(new long[] { 0, -1 }));

        Assert.AreEqual(
            12_500_000,
            NetworkStatsWorker.ComputeAutoBandwidthBytesPerSecond(new long[] { 100_000_000 }));

        Assert.AreEqual(
            125_000_000,
            NetworkStatsWorker.ComputeAutoBandwidthBytesPerSecond(new long[] { 100_000_000, 1_000_000_000, 250_000_000 }));

        Assert.AreEqual(
            31_250_000,
            NetworkStatsWorker.ComputeAutoBandwidthBytesPerSecond(new long[] { 100_000_000, 250_000_000 }));
    }
}
