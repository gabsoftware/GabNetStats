using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using GabNetStats.Properties;

namespace GabNetStats
{
    internal class NetworkStatsWorker : IDisposable
    {
        //
        //  Constants
        //
        private const int THREAD_JOIN_TIMEOUT_MS      = 1000;
        private const int PING_TIMEOUT_MS             = 500;
        private const int INACTIVITY_COUNTER_THRESHOLD = 5;

        //
        //  Dependencies
        //
        private readonly TrayIconManager         _trayIconManager;
        private readonly NetworkInterfaceManager _nicManager;
        private readonly Action                  _populateNICsCallback;
        private readonly Func<bool>              _getNicMenuOpen;

        //
        //  Thread handles
        //
        private Thread netStatThread;
        private Thread hNICRefreshThread;
        private Thread hAutoPingThread;

        //
        //  Cancellation
        //
        private readonly CancellationTokenSource workerCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource autoPingCancellationTokenSource;

        //
        //  Settings
        //
        internal BandwidthVisualMode bandwidthVisualMode;

        internal enum BandwidthVisualMode
        {
            Default,
            Custom,
            Auto
        }

        internal const int BlinkDurationMinimum = 50;
        internal int nDuration = BlinkDurationMinimum;
        private bool disposed;

        //
        //  Speed samples
        //
        private const int avgSpeedNbItems = 50;
        private readonly object speedSamplesLock = new object();
        private readonly long[] receptionSamples = new long[avgSpeedNbItems];
        private readonly long[] emissionSamples  = new long[avgSpeedNbItems];
        private int receptionSampleCount = avgSpeedNbItems;
        private int emissionSampleCount  = avgSpeedNbItems;
        private int receptionSampleIndex;
        private int emissionSampleIndex;

        //
        //  Constructor
        //
        internal NetworkStatsWorker(
            TrayIconManager         trayIconManager,
            NetworkInterfaceManager nicManager,
            Action                  populateNICsCallback,
            Func<bool>              getNicMenuOpen)
        {
            _trayIconManager      = trayIconManager;
            _nicManager           = nicManager;
            _populateNICsCallback = populateNICsCallback;
            _getNicMenuOpen       = getNicMenuOpen;
        }

        //
        //  Lifecycle
        //
        internal void Start()
        {
            try
            {
                netStatThread = new Thread(new ParameterizedThreadStart(this.NetStatThread));
                netStatThread.IsBackground = true;
                netStatThread.Name = "netStatThread";
                netStatThread.Start(workerCancellationTokenSource.Token);
            }
            catch (ArgumentNullException) { }
            catch (ThreadStateException) { }
            catch (OutOfMemoryException) { }
            catch (InvalidOperationException) { }

            try
            {
                //used in case more than one interface is UP and this number is changing.
                hNICRefreshThread = new Thread(new ParameterizedThreadStart(this.NICRefreshThread));
                hNICRefreshThread.IsBackground = true;
                hNICRefreshThread.Name = "hNICRefreshThread";
                hNICRefreshThread.Start(workerCancellationTokenSource.Token);
            }
            catch (ArgumentNullException) { }
            catch (ThreadStateException) { }
            catch (OutOfMemoryException) { }
            catch (InvalidOperationException) { }
        }

        internal void CancelWorkers()
        {
            try { workerCancellationTokenSource.Cancel(); } catch { }
        }

        internal void Shutdown()
        {
            if (disposed)
            {
                return;
            }

            try
            {
                workerCancellationTokenSource.Cancel();
            }
            catch (ObjectDisposedException) { }

            StopAutoPingThread();

            TryJoinThread(netStatThread);
            TryJoinThread(hNICRefreshThread);

            workerCancellationTokenSource.Dispose();
            disposed = true;
        }

        public void Dispose()
        {
            Shutdown();
            GC.SuppressFinalize(this);
        }

        //
        //  Auto-ping thread management
        //
        internal void StartAutoPingThread()
        {
            if (hAutoPingThread != null && hAutoPingThread.IsAlive)
            {
                return;
            }

            autoPingCancellationTokenSource?.Dispose();
            autoPingCancellationTokenSource = new CancellationTokenSource();

            try
            {
                hAutoPingThread = new Thread(new ParameterizedThreadStart(this.AutoPingThread));
                hAutoPingThread.IsBackground = true;
                hAutoPingThread.Name = "hAutoPingThread";
                hAutoPingThread.Priority = ThreadPriority.Lowest;
                hAutoPingThread.Start(autoPingCancellationTokenSource.Token);
            }
            catch (ThreadStateException) { }
            catch (OutOfMemoryException) { }
            catch (InvalidOperationException) { }
            catch (ArgumentNullException) { }
            catch (ArgumentException) { }
        }

        internal void StopAutoPingThread()
        {
            if (autoPingCancellationTokenSource != null)
            {
                try
                {
                    autoPingCancellationTokenSource.Cancel();
                }
                catch (ObjectDisposedException) { }
            }

            TryJoinThread(hAutoPingThread);
            hAutoPingThread = null;

            if (autoPingCancellationTokenSource != null)
            {
                autoPingCancellationTokenSource.Dispose();
                autoPingCancellationTokenSource = null;
            }
        }

        //
        //  Settings refresh
        //
        internal void RefreshBlinkDurationFromSettings()
        {
            if (Settings.Default.BlinkDuration < BlinkDurationMinimum)
            {
                Settings.Default.BlinkDuration = BlinkDurationMinimum;
                Settings.Default.Save();
            }

            nDuration = Settings.Default.BlinkDuration;
        }

        internal void ApplySettings()
        {
            SettingsManager.ValidateSettings();
            RefreshBlinkDurationFromSettings();

            ApplyCustomBandwidthThresholds();
            bandwidthVisualMode = GetBandwidthVisualModeFromSettings();
            NetworkInterfaceManager.RefreshEnabledInterfacesCache();

            if (!Settings.Default.AutoPingEnabled)
            {
                StopAutoPingThread();
            }
            else
            {
                StartAutoPingThread();
            }

            _trayIconManager.ApplyIconSet();
        }

        internal void InitializeSpeedSamples()
        {
            lock (speedSamplesLock)
            {
                receptionSampleCount = avgSpeedNbItems;
                emissionSampleCount  = avgSpeedNbItems;
                receptionSampleIndex = 0;
                emissionSampleIndex  = 0;

                Array.Clear(receptionSamples, 0, receptionSamples.Length);
                Array.Clear(emissionSamples, 0, emissionSamples.Length);
            }
        }

        //
        //  Speed data store
        //
        internal readonly struct SpeedHistorySample
        {
            public SpeedHistorySample(double downloadKib, double uploadKib)
            {
                DownloadKib = downloadKib;
                UploadKib = uploadKib;
            }

            public double DownloadKib { get; }
            public double UploadKib { get; }
        }

        private const int MaxHistorySamples = 2048;
        private static readonly object historyLock = new object();
        private static readonly Queue<SpeedHistorySample> history = new Queue<SpeedHistorySample>(MaxHistorySamples);

        internal static long rawSpeedReception  { get; private set; }
        internal static long rawSpeedEmission   { get; private set; }
        internal static long lAvgSpeedReception { get; private set; }
        internal static long lAvgSpeedEmission  { get; private set; }
        internal static long bytesReceived      { get; private set; }
        internal static long bytesSent          { get; private set; }

        private static void StoreSpeedData(long rawRx, long rawTx, long avgRx, long avgTx, long rx, long tx)
        {
            rawSpeedReception  = rawRx;
            rawSpeedEmission   = rawTx;
            lAvgSpeedReception = avgRx;
            lAvgSpeedEmission  = avgTx;
            bytesReceived      = rx;
            bytesSent          = tx;
            StoreHistorySample(avgRx, avgTx);
        }

        private static void StoreHistorySample(long avgDownloadBytesPerSecond, long avgUploadBytesPerSecond)
        {
            double downloadKib = Math.Max(0d, avgDownloadBytesPerSecond / 1024d);
            double uploadKib = Math.Max(0d, avgUploadBytesPerSecond / 1024d);

            lock (historyLock)
            {
                history.Enqueue(new SpeedHistorySample(downloadKib, uploadKib));
                while (history.Count > MaxHistorySamples)
                {
                    history.Dequeue();
                }
            }
        }

        internal static List<SpeedHistorySample> GetHistorySnapshot(int maxSamples)
        {
            lock (historyLock)
            {
                if (maxSamples <= 0 || history.Count == 0)
                {
                    return new List<SpeedHistorySample>(0);
                }

                int count = Math.Min(maxSamples, history.Count);
                int skip = history.Count - count;
                List<SpeedHistorySample> snapshot = new List<SpeedHistorySample>(count);
                int index = 0;

                foreach (SpeedHistorySample sample in history)
                {
                    if (index++ >= skip)
                    {
                        snapshot.Add(sample);
                    }
                }

                return snapshot;
            }
        }

        //
        //  Static helpers
        //
        private static void StoreSample(long value, long[] buffer, ref int index, ref int count)
        {
            buffer[index] = value;
            index++;
            if (index >= buffer.Length)
            {
                index = 0;
            }

            if (count < buffer.Length)
            {
                count++;
            }
        }

        private static long ComputeAverageWithoutExtremes(long[] buffer, int count)
        {
            if (count == 0)
            {
                return 0;
            }

            long min = long.MaxValue;
            long max = long.MinValue;
            long total = 0;

            for (int i = 0; i < count; i++)
            {
                long value = buffer[i];
                if (value < min)
                {
                    min = value;
                }
                if (value > max)
                {
                    max = value;
                }
                total += value;
            }

            int divisor = (count > 2 ? count : 3) - 2;
            total -= min;

            if (count >= 2)
            {
                total -= max;
            }

            return total / divisor;
        }

        internal static long ComputeAutoBandwidthBytesPerSecond(IEnumerable<long> linkSpeedsBitsPerSecond)
        {
            long fastestBitsPerSecond = 0;

            if (linkSpeedsBitsPerSecond != null)
            {
                foreach (long linkSpeedBitsPerSecond in linkSpeedsBitsPerSecond)
                {
                    if (linkSpeedBitsPerSecond > fastestBitsPerSecond)
                    {
                        fastestBitsPerSecond = linkSpeedBitsPerSecond;
                    }
                }
            }

            return ComputeAutoBandwidthBytesPerSecond(fastestBitsPerSecond);
        }

        internal static long ComputeAutoBandwidthBytesPerSecond(long fastestBitsPerSecond)
        {
            if (fastestBitsPerSecond <= 0)
            {
                return SettingsManager.DEFAULT_BANDWIDTH_BPS;
            }

            long fastestBytesPerSecond = fastestBitsPerSecond / 8;
            return fastestBytesPerSecond > 0 ? fastestBytesPerSecond : SettingsManager.DEFAULT_BANDWIDTH_BPS;
        }

        private static BandwidthVisualMode GetBandwidthVisualModeFromSettings()
        {
            if (Settings.Default.BandwidthVisualsCustom)
            {
                return BandwidthVisualMode.Custom;
            }

            if (Settings.Default.BandwidthVisualsAuto)
            {
                return BandwidthVisualMode.Auto;
            }

            return BandwidthVisualMode.Default;
        }

        private void ApplyCustomBandwidthThresholds()
        {
            SetBandwidthThresholds(
                Settings.Default.BandwidthDownload * Settings.Default.BandwidthDownloadMultiplier / Settings.Default.BandwidthUnit,
                Settings.Default.BandwidthUpload   * Settings.Default.BandwidthUploadMultiplier   / Settings.Default.BandwidthUnit);
        }

        private void ApplyAutoBandwidthThresholds(long maxBandwidthBytesPerSecond)
        {
            SetBandwidthThresholds(maxBandwidthBytesPerSecond, maxBandwidthBytesPerSecond);
        }

        private void SetBandwidthThresholds(long downloadMaxBytesPerSecond, long uploadMaxBytesPerSecond)
        {
            _trayIconManager.bandwidthDownloadLvl5 = downloadMaxBytesPerSecond;
            _trayIconManager.bandwidthUploadLvl5   = uploadMaxBytesPerSecond;
            _trayIconManager.bandwidthDownloadLvl4 = _trayIconManager.bandwidthDownloadLvl5 * 4 / 5;
            _trayIconManager.bandwidthDownloadLvl3 = _trayIconManager.bandwidthDownloadLvl5 * 3 / 5;
            _trayIconManager.bandwidthDownloadLvl2 = _trayIconManager.bandwidthDownloadLvl5 * 2 / 5;
            _trayIconManager.bandwidthDownloadLvl1 = _trayIconManager.bandwidthDownloadLvl5     / 5;
            _trayIconManager.bandwidthUploadLvl4   = _trayIconManager.bandwidthUploadLvl5   * 4 / 5;
            _trayIconManager.bandwidthUploadLvl3   = _trayIconManager.bandwidthUploadLvl5   * 3 / 5;
            _trayIconManager.bandwidthUploadLvl2   = _trayIconManager.bandwidthUploadLvl5   * 2 / 5;
            _trayIconManager.bandwidthUploadLvl1   = _trayIconManager.bandwidthUploadLvl5       / 5;
        }

        private static void TryJoinThread(Thread thread)
        {
            if (thread == null)
            {
                return;
            }

            try
            {
                thread.Join(THREAD_JOIN_TIMEOUT_MS);
            }
            catch (ThreadStateException) { }
            catch (ThreadInterruptedException) { }
        }

        private static void WaitWithCancellation(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(millisecondsTimeout, Timeout.Infinite);

            if (millisecondsTimeout == 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return;
            }

            bool signaled = cancellationToken.WaitHandle.WaitOne(millisecondsTimeout);
            if (signaled)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        //
        //  Worker threads
        //
        /// <summary>
        /// Thread for auto-pinging a server
        /// </summary>
        private void AutoPingThread(object state)
        {
            CancellationToken cancellationToken = state is CancellationToken token ? token : CancellationToken.None;
            byte[] buffer = { 1 };
            PingReply reply = null;
            int pingTimeout = PING_TIMEOUT_MS;

            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string pingHost = Settings.Default.AutoPingHost;
                    int pingRate = (int)Settings.Default.AutoPingRate;

                    using (Ping ping = new Ping())
                    {
                        try
                        {
                            reply = ping.Send(pingHost, pingTimeout, buffer);
                        }
                        catch (PingException)
                        {
                            reply = null; // PingException does not provide a reply object
                        }
                        catch (Exception)
                        {
                            reply = null;
                        }
                    }

                    _trayIconManager.UpdateAutoPingIcon(reply);

                    try
                    {
                        WaitWithCancellation(pingRate, cancellationToken);
                    }
                    catch (ArgumentOutOfRangeException) { }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                if (!Program.IsWindowsShuttingDown)
                {
                    Program.ReportWorkerException(ex);
                }
            }
        }

        private void NICRefreshThread(object state)
        {
            CancellationToken cancellationToken = state is CancellationToken token ? token : CancellationToken.None;
            int nbv4, nbv6;

            try
            {

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    //we get some quick statistics about the number of network interfaces...
                    _nicManager.properties = IPGlobalProperties.GetIPGlobalProperties();
                    try
                    {
                        _nicManager.ipv4stat = _nicManager.properties.GetIPv4GlobalStatistics();
                        _nicManager.ipv6stat = _nicManager.properties.GetIPv6GlobalStatistics();
                    }
                    catch (NetworkInformationException)
                    {
                        continue;
                    }
                    catch( PlatformNotSupportedException )
                    {
                        continue;
                    }

                    nbv4         = _nicManager.ipv4stat.NumberOfInterfaces;
                    nbv6         = _nicManager.ipv6stat.NumberOfInterfaces;

                    //if number changed since last time AND we are not displaying the context menu then
                    if (nbv4 + nbv6 != _nicManager.nbNIC && !_getNicMenuOpen())
                    {
                        _nicManager.nbNIC = nbv4 + nbv6;
                        try { _populateNICsCallback(); }
                        catch (ObjectDisposedException) { }
                        catch (InvalidOperationException) { }
                    }

                    try
                    {
                        WaitWithCancellation(_nicManager.nNICRefresh, cancellationToken);
                    }
                    catch (ArgumentOutOfRangeException) { }
                }

            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                if (!Program.IsWindowsShuttingDown)
                {
                    Program.ReportWorkerException(ex);
                }
            }
        }

        private void NetStatThread(object state)
        {
            CancellationToken cancellationToken = state is CancellationToken token ? token : CancellationToken.None;
            int nCounter            = 0;
            long bytesReceived      = 0;
            long bytesSent          = 0;
            long oldbytesReceived   = 0;
            long oldbytesSent       = 0;
            long lAvgSpeedReception = 0;
            long lAvgSpeedEmission  = 0;
            long rawSpeedReception  = 0;
            long rawSpeedEmission   = 0;
            int enabledInterfaceCount = 0;
            int enabledInterfaceHash = 0;
            int previousEnabledInterfaceCount = -1;
            int previousEnabledInterfaceHash = 0;
            long fastestEnabledLinkSpeedBitsPerSecond = 0;

            IPInterfaceStatistics ipstats = null;
            Stopwatch sampleStopwatch = Stopwatch.StartNew();

            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    double elapsedMs = sampleStopwatch.Elapsed.TotalMilliseconds;
                    if (elapsedMs < 1)
                    {
                        elapsedMs = nDuration;
                    }
                    sampleStopwatch.Restart();
                    bytesReceived    = 0;
                    bytesSent        = 0;

                    if (NetworkInterfaceManager.connectionStatus == TrayIconManager.eState.disconnected)
                    {
                        _trayIconManager.SetActivityIcon(_trayIconManager.iconDisconnected);
                        rawSpeedReception            = 0;
                        rawSpeedEmission             = 0;
                        goto skip;
                    }
                    if (NetworkInterfaceManager.connectionStatus == TrayIconManager.eState.limited)
                    {
                        _trayIconManager.SetActivityIcon(_trayIconManager.iconLimited);
                        rawSpeedReception            = 0;
                        rawSpeedEmission             = 0;
                        goto skip;
                    }

                    lock (_nicManager.selectedInterfaces)
                    {
                        enabledInterfaceCount = 0;
                        fastestEnabledLinkSpeedBitsPerSecond = 0;
                        // Seed and multiplier are conventional small primes for a simple rolling hash.
                        enabledInterfaceHash = 17;

                        foreach (NetworkInterfaceManager.TrackedInterface tracked in _nicManager.selectedInterfaces)
                        {
                            if (!NetworkInterfaceManager.IsInterfaceEnabled(tracked.MacAddress))
                            {
                                continue;
                            }

                            enabledInterfaceCount++;
                            // Fold each enabled MAC into an order-sensitive fingerprint for baseline reset detection.
                            enabledInterfaceHash = enabledInterfaceHash * 31 + StringComparer.OrdinalIgnoreCase.GetHashCode(tracked.MacAddress ?? String.Empty);

                            try
                            {
                                long linkSpeedBitsPerSecond = tracked.Interface.Speed;
                                if (linkSpeedBitsPerSecond > fastestEnabledLinkSpeedBitsPerSecond)
                                {
                                    fastestEnabledLinkSpeedBitsPerSecond = linkSpeedBitsPerSecond;
                                }
                            }
                            catch (Exception)
                            {
                            }

                            try
                            {
                                ipstats = tracked.Interface.GetIPStatistics();
                                bytesReceived += ipstats.BytesReceived;
                                bytesSent += ipstats.BytesSent;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }

                    bool enabledInterfacesChanged =
                        enabledInterfaceCount != previousEnabledInterfaceCount ||
                        enabledInterfaceHash != previousEnabledInterfaceHash;

                    if (enabledInterfacesChanged)
                    {
                        previousEnabledInterfaceCount = enabledInterfaceCount;
                        previousEnabledInterfaceHash = enabledInterfaceHash;
                        oldbytesReceived = bytesReceived;
                        oldbytesSent = bytesSent;
                    }

                    if (bandwidthVisualMode == BandwidthVisualMode.Auto)
                    {
                        ApplyAutoBandwidthThresholds(ComputeAutoBandwidthBytesPerSecond(fastestEnabledLinkSpeedBitsPerSecond));
                    }

                    long deltaReceived = bytesReceived - oldbytesReceived;
                    long deltaSent = bytesSent - oldbytesSent;
                    bool receivedCounterReset = deltaReceived < 0;
                    bool sentCounterReset = deltaSent < 0;

                    if (receivedCounterReset)
                    {
                        oldbytesReceived = bytesReceived;
                        deltaReceived = 0;
                    }

                    if (sentCounterReset)
                    {
                        oldbytesSent = bytesSent;
                        deltaSent = 0;
                    }

                    bool hasDownload = deltaReceived > 0;
                    bool hasUpload = deltaSent > 0;

                    if (hasDownload)
                    {
                        rawSpeedReception = (long)(deltaReceived * 1000.0 / elapsedMs);
                        oldbytesReceived = bytesReceived;
                    }
                    else
                    {
                        rawSpeedReception = 0;
                    }

                    if (hasUpload)
                    {
                        rawSpeedEmission = (long)(deltaSent * 1000.0 / elapsedMs);
                        oldbytesSent = bytesSent;
                    }
                    else
                    {
                        rawSpeedEmission = 0;
                    }

                    if (hasDownload && hasUpload)
                    {
                        nCounter = 0;
                        if (bandwidthVisualMode != BandwidthVisualMode.Default)
                        {
                            int dl = TrayIconManager.GetSpeedLevel(rawSpeedReception, _trayIconManager.bandwidthDownloadLvl1, _trayIconManager.bandwidthDownloadLvl2, _trayIconManager.bandwidthDownloadLvl3, _trayIconManager.bandwidthDownloadLvl4);
                            int ul = TrayIconManager.GetSpeedLevel(rawSpeedEmission,  _trayIconManager.bandwidthUploadLvl1,   _trayIconManager.bandwidthUploadLvl2,  _trayIconManager.bandwidthUploadLvl3,  _trayIconManager.bandwidthUploadLvl4);
                            _trayIconManager.SetActivityIcon(_trayIconManager.iconsActive[dl, ul]);
                        }
                        else
                        {
                            _trayIconManager.SetActivityIcon(_trayIconManager.iconsActive[0, 0]);
                        }
                    }
                    else if (hasDownload && !hasUpload)
                    {
                        nCounter = 0;
                        int dl = bandwidthVisualMode != BandwidthVisualMode.Default ? TrayIconManager.GetSpeedLevel(rawSpeedReception, _trayIconManager.bandwidthDownloadLvl1, _trayIconManager.bandwidthDownloadLvl2, _trayIconManager.bandwidthDownloadLvl3, _trayIconManager.bandwidthDownloadLvl4) : 0;
                        _trayIconManager.SetActivityIcon(_trayIconManager.iconsReceive[dl]);
                    }
                    else if (!hasDownload && hasUpload)
                    {
                        nCounter = 0;
                        int ul = bandwidthVisualMode != BandwidthVisualMode.Default ? TrayIconManager.GetSpeedLevel(rawSpeedEmission, _trayIconManager.bandwidthUploadLvl1, _trayIconManager.bandwidthUploadLvl2, _trayIconManager.bandwidthUploadLvl3, _trayIconManager.bandwidthUploadLvl4) : 0;
                        _trayIconManager.SetActivityIcon(_trayIconManager.iconsSend[ul]);
                    }
                    else
                    {
                        nCounter++; //the counter adds a small persistance effect
                    }

                    if (nCounter == INACTIVITY_COUNTER_THRESHOLD)
                    {
                        nCounter                     = 0;
                        _trayIconManager.SetActivityIcon(_trayIconManager.iconInactive);
                        rawSpeedReception            = 0;
                        rawSpeedEmission             = 0;
                    }

                    skip:

                    lock (speedSamplesLock)
                    {
                        StoreSample(rawSpeedReception, receptionSamples, ref receptionSampleIndex, ref receptionSampleCount);
                        StoreSample(rawSpeedEmission, emissionSamples, ref emissionSampleIndex, ref emissionSampleCount);

                        lAvgSpeedReception = ComputeAverageWithoutExtremes(receptionSamples, receptionSampleCount);
                        lAvgSpeedEmission  = ComputeAverageWithoutExtremes(emissionSamples, emissionSampleCount);
                    }

                    StoreSpeedData(rawSpeedReception, rawSpeedEmission, lAvgSpeedReception, lAvgSpeedEmission, bytesReceived, bytesSent);

                    try
                    {
                        WaitWithCancellation(nDuration, cancellationToken);
                    }
                    catch (ArgumentOutOfRangeException) { }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                if (!Program.IsWindowsShuttingDown)
                {
                    Program.ReportWorkerException(ex);
                }
            }
        }
    }
}
