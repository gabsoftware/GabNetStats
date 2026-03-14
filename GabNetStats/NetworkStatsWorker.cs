using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using GabNetStats.Properties;

namespace GabNetStats
{
    internal class NetworkStatsWorker
    {
        //
        //  Dependencies
        //
        private readonly TrayIconManager         _trayIconManager;
        private readonly NetworkInterfaceManager _nicManager;
        private readonly NotifyIcon              _notifyIconPing;
        private readonly Action                  _populateNICsCallback;
        private readonly Func<bool>              _getNicMenuOpen;

        //
        //  Thread handles
        //
        private Thread hNetStatThread2   = null;
        private Thread hNICRefreshThread = null;
        private Thread hAutoPingThread   = null;

        //
        //  Cancellation
        //
        private readonly CancellationTokenSource workerCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource autoPingCancellationTokenSource;

        //
        //  Settings
        //
        internal bool customBandwidth     = false;

        internal const int BlinkDurationMinimum = 50;
        internal int nDuration = BlinkDurationMinimum;

        //
        //  Speed samples
        //
        private const int avgSpeedNbItems = 50;
        private readonly object speedSamplesLock = new object();
        private readonly long[] receptionSamples = new long[avgSpeedNbItems];
        private readonly long[] emissionSamples  = new long[avgSpeedNbItems];
        private int receptionSampleCount = avgSpeedNbItems;
        private int emissionSampleCount  = avgSpeedNbItems;
        private int receptionSampleIndex = 0;
        private int emissionSampleIndex  = 0;

        //
        //  Constructor
        //
        internal NetworkStatsWorker(
            TrayIconManager         trayIconManager,
            NetworkInterfaceManager nicManager,
            NotifyIcon              notifyIconPing,
            Action                  populateNICsCallback,
            Func<bool>              getNicMenuOpen)
        {
            _trayIconManager      = trayIconManager;
            _nicManager           = nicManager;
            _notifyIconPing       = notifyIconPing;
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
                hNetStatThread2 = new Thread(new ParameterizedThreadStart(this.NetStatThread));
                hNetStatThread2.IsBackground = true;
                hNetStatThread2.Name = "hNetStatThread2";
                hNetStatThread2.Start(workerCancellationTokenSource.Token);
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
            try
            {
                workerCancellationTokenSource.Cancel();
            }
            catch (ObjectDisposedException) { }

            StopAutoPingThread();

            TryJoinThread(hNetStatThread2);
            TryJoinThread(hNICRefreshThread);

            workerCancellationTokenSource.Dispose();
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

        private static void TryJoinThread(Thread thread)
        {
            if (thread == null)
            {
                return;
            }

            try
            {
                thread.Join(1000);
            }
            catch (ThreadStateException) { }
            catch (ThreadInterruptedException) { }
        }

        private static void WaitWithCancellation(CancellationToken cancellationToken, int millisecondsTimeout)
        {
            if (millisecondsTimeout < Timeout.Infinite)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }

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
            Icon previous = null;
            string pingHost = Settings.Default.AutoPingHost;
            int pingTimeout = 500;

            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

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

                    previous = _notifyIconPing.Icon;
                    _trayIconManager.UpdateAutoPingIcon(reply);

                    if (Settings.Default.AutoPingNotif && !previous.Equals(_notifyIconPing.Icon))
                    {
                        _notifyIconPing.ShowBalloonTip(1000);
                    }

                    try
                    {
                        WaitWithCancellation(cancellationToken, (int)Settings.Default.AutoPingRate);
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
                    MessageBox.Show(
                        Res.str_ErrorCrash +
                        "\n\n" + "Thread : " +
                        Thread.CurrentThread.Name +
                        "\n\n" +
                        ex.ToString(), "GabNetStats", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Application.Restart();
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
                        _populateNICsCallback();
                    }

                    try
                    {
                        WaitWithCancellation(cancellationToken, _nicManager.nNICRefresh);
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
                    MessageBox.Show(
                        Res.str_ErrorCrash +
                        "\n\n" + "Thread : " +
                        Thread.CurrentThread.Name +
                        "\n\n" +
                        ex.ToString(), "GabNetStats", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Application.Restart();
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
                        foreach (NetworkInterfaceManager.TrackedInterface tracked in _nicManager.selectedInterfaces)
                        {
                            if (!NetworkInterfaceManager.IsInterfaceEnabled(tracked.MacAddress))
                            {
                                continue;
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


                    long deltaReceived = bytesReceived - oldbytesReceived;
                    long deltaSent = bytesSent - oldbytesSent;
                    bool hasDownload = deltaReceived != 0;
                    bool hasUpload = deltaSent != 0;

                    if (hasDownload)
                    {
                        rawSpeedReception = (long)Math.Abs(deltaReceived * 1000.0 / elapsedMs);
                        oldbytesReceived = bytesReceived;
                    }
                    else
                    {
                        rawSpeedReception = 0;
                    }

                    if (hasUpload)
                    {
                        rawSpeedEmission = (long)Math.Abs(deltaSent * 1000.0 / elapsedMs);
                        oldbytesSent = bytesSent;
                    }
                    else
                    {
                        rawSpeedEmission = 0;
                    }

                    if (hasDownload && hasUpload)
                    {
                        nCounter = 0;
                        if (customBandwidth)
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
                        int dl = customBandwidth ? TrayIconManager.GetSpeedLevel(rawSpeedReception, _trayIconManager.bandwidthDownloadLvl1, _trayIconManager.bandwidthDownloadLvl2, _trayIconManager.bandwidthDownloadLvl3, _trayIconManager.bandwidthDownloadLvl4) : 0;
                        _trayIconManager.SetActivityIcon(_trayIconManager.iconsReceive[dl]);
                    }
                    else if (!hasDownload && hasUpload)
                    {
                        nCounter = 0;
                        int ul = customBandwidth ? TrayIconManager.GetSpeedLevel(rawSpeedEmission, _trayIconManager.bandwidthUploadLvl1, _trayIconManager.bandwidthUploadLvl2, _trayIconManager.bandwidthUploadLvl3, _trayIconManager.bandwidthUploadLvl4) : 0;
                        _trayIconManager.SetActivityIcon(_trayIconManager.iconsSend[ul]);
                    }
                    else
                    {
                        nCounter++; //the counter adds a small persistance effect
                    }

                    if (nCounter == 5)
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

                    frmBalloon.UpdateInfos(rawSpeedReception, rawSpeedEmission, lAvgSpeedReception, lAvgSpeedEmission, bytesReceived, bytesSent);

                    try
                    {
                        WaitWithCancellation(cancellationToken, nDuration);
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
                    MessageBox.Show(
                        Res.str_ErrorCrash +
                        "\n\n" + "Thread : " +
                        Thread.CurrentThread.Name +
                        "\n\n" +
                        ex.ToString(), "GabNetStats", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Application.Restart();
                }
            }
        }
    }
}
