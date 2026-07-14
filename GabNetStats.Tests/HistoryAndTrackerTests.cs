using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GabNetStats.Tests;

[TestClass]
public sealed class HistoryAndTrackerTests
{
    [TestMethod]
    public void SpeedHistoryKeepsOnlyRequestedSnapshotCount()
    {
        ClearSpeedHistory();
        MethodInfo storeHistorySample = typeof(NetworkStatsWorker).GetMethod(
            "StoreHistorySample",
            BindingFlags.NonPublic | BindingFlags.Static)!;

        storeHistorySample.Invoke(null, new object[] { 1024L, 2048L });
        storeHistorySample.Invoke(null, new object[] { 2048L, 3072L });
        storeHistorySample.Invoke(null, new object[] { 4096L, 5120L });

        List<NetworkStatsWorker.SpeedHistorySample> snapshot = NetworkStatsWorker.GetHistorySnapshot(2);

        Assert.AreEqual(2, snapshot.Count);
        Assert.AreEqual(2d, snapshot[0].DownloadKib);
        Assert.AreEqual(3d, snapshot[0].UploadKib);
        Assert.AreEqual(4d, snapshot[1].DownloadKib);
        Assert.AreEqual(5d, snapshot[1].UploadKib);
    }

    [TestMethod]
    public void PendingGraphAverageConsumesUndisplayedWorkerSamples()
    {
        ClearSpeedHistory();
        NetworkStatsWorker.InitializeSpeedSamples();

        MethodInfo storeSpeedData = typeof(NetworkStatsWorker).GetMethod(
            "StoreSpeedData",
            BindingFlags.NonPublic | BindingFlags.Static)!;

        storeSpeedData.Invoke(null, new object[] { 0L, 0L, 1024L, 2048L, 0L, 0L });
        storeSpeedData.Invoke(null, new object[] { 0L, 0L, 3072L, 4096L, 0L, 0L });

        NetworkStatsWorker.SpeedHistorySample sample = NetworkStatsWorker.ConsumePendingGraphAverage();
        NetworkStatsWorker.SpeedHistorySample fallback = NetworkStatsWorker.ConsumePendingGraphAverage();

        Assert.AreEqual(2d, sample.DownloadKib);
        Assert.AreEqual(3d, sample.UploadKib);
        Assert.AreEqual(3d, fallback.DownloadKib);
        Assert.AreEqual(4d, fallback.UploadKib);
    }

    [TestMethod]
    public void GabTrackerFeedKeepsNewestValuesUpToCapacity()
    {
        using var tracker = new GabTracker.GabTracker
        {
            AutoStart       = false,
            MaxDataInMemory = 3
        };

        var feed = new GabTracker.GabTrackerFeed();
        tracker.Feeds.Add(feed);

        for (int i = 1; i <= 5; i++)
        {
            feed.Value = i;
            tracker.AddCurrentValues();
        }

        CollectionAssert.AreEqual(new[] { 3d, 4d, 5d }, feed.Data.ToArray());
    }

    [TestMethod]
    public void GabTrackerAutoStartCanBeDisabled()
    {
        using var tracker = new GabTracker.GabTracker();

        tracker.AutoStart = false;

        FieldInfo timerField = typeof(GabTracker.GabTracker).GetField(
            "internalTimer",
            BindingFlags.NonPublic | BindingFlags.Instance)!;
        var timer = (System.Windows.Forms.Timer)timerField.GetValue(tracker)!;

        Assert.IsFalse(tracker.AutoStart);
        Assert.IsFalse(timer.Enabled);
    }

    [TestMethod]
    public void AdaptiveAverageSmoothsSmallChanges()
    {
        long average = NetworkStatsWorker.ComputeAdaptiveAverage(1_000, 1_050);

        Assert.IsTrue(average > 1_000);
        Assert.IsTrue(average < 1_050);
    }

    [TestMethod]
    public void AdaptiveAverageFollowsLargeChangesQuickly()
    {
        long risingAverage = NetworkStatsWorker.ComputeAdaptiveAverage(1_000, 10_000);
        long fallingAverage = NetworkStatsWorker.ComputeAdaptiveAverage(10_000, 0);

        Assert.IsTrue(risingAverage > 2_000);
        Assert.IsTrue(risingAverage < 3_500);
        Assert.IsTrue(fallingAverage > 6_000);
        Assert.IsTrue(fallingAverage < 8_000);
        Assert.IsTrue(10_000 - fallingAverage > risingAverage - 1_000);
    }

    private static void ClearSpeedHistory()
    {
        FieldInfo historyField = typeof(NetworkStatsWorker).GetField(
            "history",
            BindingFlags.NonPublic | BindingFlags.Static)!;
        var history = (Queue<NetworkStatsWorker.SpeedHistorySample>)historyField.GetValue(null)!;
        history.Clear();
    }
}
