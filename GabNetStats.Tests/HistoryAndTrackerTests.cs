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
    public void GabTrackerFeedKeepsNewestValuesUpToCapacity()
    {
        using var tracker = new GabTracker.GabTracker();
        tracker.Stop();
        tracker.MaxDataInMemory = 3;

        var feed = new GabTracker.GabTrackerFeed();
        tracker.Feeds.Add(feed);

        MethodInfo tick = typeof(GabTracker.GabTracker).GetMethod(
            "internalTimer_Tick",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        for (int i = 1; i <= 5; i++)
        {
            feed.Value = i;
            tick.Invoke(tracker, new object?[] { tracker, EventArgs.Empty });
        }

        CollectionAssert.AreEqual(new[] { 3d, 4d, 5d }, feed.Data.ToArray());
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
