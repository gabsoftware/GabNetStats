//
// From the idea of Igor Tolmachev, IT Samples
//        http://www.itsamples.com
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Collections;
using GabNetStats.Properties;
using System.Globalization;
using System.IO;

namespace GabNetStats
{
    public partial class MainForm : Form
    {
        private Thread hNetStatThread2   = null;
        private Thread hNICRefreshThread = null;
        private Thread hAutoPingThread   = null;

        private readonly CancellationTokenSource workerCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource autoPingCancellationTokenSource;

        private bool bSetIconContinue    = true;
        private bool customBandwidth     = false;

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


        static eState connectionStatus;

        private const int BlinkDurationMinimum = 200;
        static int nDuration   = BlinkDurationMinimum;
        static int nNICRefresh = 10000; //time interval for refreshing the NIC list (10s by default)
        static int nbNIC       = 0;
        static long bandwidthDownloadLvl1;
        static long bandwidthDownloadLvl2;
        static long bandwidthDownloadLvl3;
        static long bandwidthDownloadLvl4;
        static long bandwidthDownloadLvl5;
        static long bandwidthUploadLvl1;
        static long bandwidthUploadLvl2;
        static long bandwidthUploadLvl3;
        static long bandwidthUploadLvl4;
        static long bandwidthUploadLvl5;
        private const int avgSpeedNbItems = 50;
        private static readonly object speedSamplesLock = new object();
        private static readonly long[] receptionSamples = new long[avgSpeedNbItems];
        private static readonly long[] emissionSamples  = new long[avgSpeedNbItems];
        private static int receptionSampleCount = avgSpeedNbItems;
        private static int emissionSampleCount  = avgSpeedNbItems;
        private static int receptionSampleIndex = 0;
        private static int emissionSampleIndex  = 0;

        static NetworkInterface[] interfaces;
        static ArrayList          selectedInterfaces = new ArrayList();
        static IPGlobalProperties properties;
        static IPGlobalStatistics ipv4stat;
        static IPGlobalStatistics ipv6stat;

        static Icon iconActive_blue_blue     = Properties.Resources.active_blue_blue;
        static Icon iconActive_blue_green    = Properties.Resources.active_blue_green;
        static Icon iconActive_blue_yellow   = Properties.Resources.active_blue_yellow;
        static Icon iconActive_blue_orange   = Properties.Resources.active_blue_orange;
        static Icon iconActive_blue_red      = Properties.Resources.active_blue_red;
        static Icon iconActive_green_blue    = Properties.Resources.active_green_blue;
        static Icon iconActive_green_green   = Properties.Resources.active_green_green;
        static Icon iconActive_green_yellow  = Properties.Resources.active_green_yellow;
        static Icon iconActive_green_orange  = Properties.Resources.active_green_orange;
        static Icon iconActive_green_red     = Properties.Resources.active_green_red;
        static Icon iconActive_yellow_blue   = Properties.Resources.active_yellow_blue;
        static Icon iconActive_yellow_green  = Properties.Resources.active_yellow_green;
        static Icon iconActive_yellow_yellow = Properties.Resources.active_yellow_yellow;
        static Icon iconActive_yellow_orange = Properties.Resources.active_yellow_orange;
        static Icon iconActive_yellow_red    = Properties.Resources.active_yellow_red;
        static Icon iconActive_orange_blue   = Properties.Resources.active_orange_blue;
        static Icon iconActive_orange_green  = Properties.Resources.active_orange_green;
        static Icon iconActive_orange_yellow = Properties.Resources.active_orange_yellow;
        static Icon iconActive_orange_orange = Properties.Resources.active_orange_orange;
        static Icon iconActive_orange_red    = Properties.Resources.active_orange_red;
        static Icon iconActive_red_blue      = Properties.Resources.active_red_blue;
        static Icon iconActive_red_green     = Properties.Resources.active_red_green;
        static Icon iconActive_red_yellow    = Properties.Resources.active_red_yellow;
        static Icon iconActive_red_orange    = Properties.Resources.active_red_orange;
        static Icon iconActive_red_red       = Properties.Resources.active_red_red;

        static Icon iconInactive             = Properties.Resources.inactive;
        static Icon currentActivityIcon      = Properties.Resources.inactive;

        static Icon iconSend_blue            = Properties.Resources.send_blue;
        static Icon iconSend_green           = Properties.Resources.send_green;
        static Icon iconSend_yellow          = Properties.Resources.send_yellow;
        static Icon iconSend_orange          = Properties.Resources.send_orange;
        static Icon iconSend_red             = Properties.Resources.send_red;

        static Icon iconReceive_blue         = Properties.Resources.receive_blue;
        static Icon iconReceive_green        = Properties.Resources.receive_green;
        static Icon iconReceive_yellow       = Properties.Resources.receive_yellow;
        static Icon iconReceive_orange       = Properties.Resources.receive_orange;
        static Icon iconReceive_red          = Properties.Resources.receive_red;

        static Icon iconDisconnected         = Properties.Resources.netshell_195;
        static Icon iconLimited              = Properties.Resources.netshell_IDI_CFI_TRAY_WIRED_WARNING;

        static Icon iconCircle_green  = Properties.Resources.circle_green;
        static Icon iconCircle_red    = Properties.Resources.circle_red;
        static Icon iconCircle_grey   = Properties.Resources.circle_grey;
        static Icon iconCircle_orange = Properties.Resources.circle_orange;
        private static string appliedIconSet = "xp";

        static frmBalloon fBal;

        static MainForm()
        {
            try
            {
                interfaces = NetworkInterface.GetAllNetworkInterfaces();
            }
            catch (NetworkInformationException)
            {

            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void applyIconSet()
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
                currentActivityIcon = this.notifyIconActivity.Icon;
                return;
            }

            string path = Path.Combine(Application.StartupPath, "icons", desiredSet);
            if (!Directory.Exists(path))
            {
                return;
            }

            LoadIconSetFromDirectory(path);
            appliedIconSet = desiredSet;
            currentActivityIcon = this.notifyIconActivity.Icon;
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

        private static void LoadDefaultIconSet()
        {
            iconActive_blue_blue     = Properties.Resources.active_blue_blue;
            iconActive_blue_green    = Properties.Resources.active_blue_green;
            iconActive_blue_yellow   = Properties.Resources.active_blue_yellow;
            iconActive_blue_orange   = Properties.Resources.active_blue_orange;
            iconActive_blue_red      = Properties.Resources.active_blue_red;
            iconActive_green_blue    = Properties.Resources.active_green_blue;
            iconActive_green_green   = Properties.Resources.active_green_green;
            iconActive_green_yellow  = Properties.Resources.active_green_yellow;
            iconActive_green_orange  = Properties.Resources.active_green_orange;
            iconActive_green_red     = Properties.Resources.active_green_red;
            iconActive_yellow_blue   = Properties.Resources.active_yellow_blue;
            iconActive_yellow_green  = Properties.Resources.active_yellow_green;
            iconActive_yellow_yellow = Properties.Resources.active_yellow_yellow;
            iconActive_yellow_orange = Properties.Resources.active_yellow_orange;
            iconActive_yellow_red    = Properties.Resources.active_yellow_red;
            iconActive_orange_blue   = Properties.Resources.active_orange_blue;
            iconActive_orange_green  = Properties.Resources.active_orange_green;
            iconActive_orange_yellow = Properties.Resources.active_orange_yellow;
            iconActive_orange_orange = Properties.Resources.active_orange_orange;
            iconActive_orange_red    = Properties.Resources.active_orange_red;
            iconActive_red_blue      = Properties.Resources.active_red_blue;
            iconActive_red_green     = Properties.Resources.active_red_green;
            iconActive_red_yellow    = Properties.Resources.active_red_yellow;
            iconActive_red_orange    = Properties.Resources.active_red_orange;
            iconActive_red_red       = Properties.Resources.active_red_red;

            iconInactive             = Properties.Resources.inactive;

            iconSend_blue            = Properties.Resources.send_blue;
            iconSend_green           = Properties.Resources.send_green;
            iconSend_yellow          = Properties.Resources.send_yellow;
            iconSend_orange          = Properties.Resources.send_orange;
            iconSend_red             = Properties.Resources.send_red;

            iconReceive_blue         = Properties.Resources.receive_blue;
            iconReceive_green        = Properties.Resources.receive_green;
            iconReceive_yellow       = Properties.Resources.receive_yellow;
            iconReceive_orange       = Properties.Resources.receive_orange;
            iconReceive_red          = Properties.Resources.receive_red;

            iconCircle_green         = Properties.Resources.circle_green;
            iconCircle_red           = Properties.Resources.circle_red;
            iconCircle_grey          = Properties.Resources.circle_grey;
            iconCircle_orange        = Properties.Resources.circle_orange;
        }

        private static void LoadIconSetFromDirectory(string basePath)
        {
            iconActive_blue_blue     = LoadIconFromFile(Path.Combine(basePath, "active_blue_blue.ico"), iconActive_blue_blue);
            iconActive_blue_green    = LoadIconFromFile(Path.Combine(basePath, "active_blue_green.ico"), iconActive_blue_green);
            iconActive_blue_yellow   = LoadIconFromFile(Path.Combine(basePath, "active_blue_yellow.ico"), iconActive_blue_yellow);
            iconActive_blue_orange   = LoadIconFromFile(Path.Combine(basePath, "active_blue_orange.ico"), iconActive_blue_orange);
            iconActive_blue_red      = LoadIconFromFile(Path.Combine(basePath, "active_blue_red.ico"), iconActive_blue_red);
            iconActive_green_blue    = LoadIconFromFile(Path.Combine(basePath, "active_green_blue.ico"), iconActive_green_blue);
            iconActive_green_green   = LoadIconFromFile(Path.Combine(basePath, "active_green_green.ico"), iconActive_green_green);
            iconActive_green_yellow  = LoadIconFromFile(Path.Combine(basePath, "active_green_yellow.ico"), iconActive_green_yellow);
            iconActive_green_orange  = LoadIconFromFile(Path.Combine(basePath, "active_green_orange.ico"), iconActive_green_orange);
            iconActive_green_red     = LoadIconFromFile(Path.Combine(basePath, "active_green_red.ico"), iconActive_green_red);
            iconActive_yellow_blue   = LoadIconFromFile(Path.Combine(basePath, "active_yellow_blue.ico"), iconActive_yellow_blue);
            iconActive_yellow_green  = LoadIconFromFile(Path.Combine(basePath, "active_yellow_green.ico"), iconActive_yellow_green);
            iconActive_yellow_yellow = LoadIconFromFile(Path.Combine(basePath, "active_yellow_yellow.ico"), iconActive_yellow_yellow);
            iconActive_yellow_orange = LoadIconFromFile(Path.Combine(basePath, "active_yellow_orange.ico"), iconActive_yellow_orange);
            iconActive_yellow_red    = LoadIconFromFile(Path.Combine(basePath, "active_yellow_red.ico"), iconActive_yellow_red);
            iconActive_orange_blue   = LoadIconFromFile(Path.Combine(basePath, "active_orange_blue.ico"), iconActive_orange_blue);
            iconActive_orange_green  = LoadIconFromFile(Path.Combine(basePath, "active_orange_green.ico"), iconActive_orange_green);
            iconActive_orange_yellow = LoadIconFromFile(Path.Combine(basePath, "active_orange_yellow.ico"), iconActive_orange_yellow);
            iconActive_orange_orange = LoadIconFromFile(Path.Combine(basePath, "active_orange_orange.ico"), iconActive_orange_orange);
            iconActive_orange_red    = LoadIconFromFile(Path.Combine(basePath, "active_orange_red.ico"), iconActive_orange_red);
            iconActive_red_blue      = LoadIconFromFile(Path.Combine(basePath, "active_red_blue.ico"), iconActive_red_blue);
            iconActive_red_green     = LoadIconFromFile(Path.Combine(basePath, "active_red_green.ico"), iconActive_red_green);
            iconActive_red_yellow    = LoadIconFromFile(Path.Combine(basePath, "active_red_yellow.ico"), iconActive_red_yellow);
            iconActive_red_orange    = LoadIconFromFile(Path.Combine(basePath, "active_red_orange.ico"), iconActive_red_orange);
            iconActive_red_red       = LoadIconFromFile(Path.Combine(basePath, "active_red_red.ico"), iconActive_red_red);

            iconInactive             = LoadIconFromFile(Path.Combine(basePath, "inactive.ico"), iconInactive);

            iconSend_blue            = LoadIconFromFile(Path.Combine(basePath, "send_blue.ico"), iconSend_blue);
            iconSend_green           = LoadIconFromFile(Path.Combine(basePath, "send_green.ico"), iconSend_green);
            iconSend_yellow          = LoadIconFromFile(Path.Combine(basePath, "send_yellow.ico"), iconSend_yellow);
            iconSend_orange          = LoadIconFromFile(Path.Combine(basePath, "send_orange.ico"), iconSend_orange);
            iconSend_red             = LoadIconFromFile(Path.Combine(basePath, "send_red.ico"), iconSend_red);

            iconReceive_blue         = LoadIconFromFile(Path.Combine(basePath, "receive_blue.ico"), iconReceive_blue);
            iconReceive_green        = LoadIconFromFile(Path.Combine(basePath, "receive_green.ico"), iconReceive_green);
            iconReceive_yellow       = LoadIconFromFile(Path.Combine(basePath, "receive_yellow.ico"), iconReceive_yellow);
            iconReceive_orange       = LoadIconFromFile(Path.Combine(basePath, "receive_orange.ico"), iconReceive_orange);
            iconReceive_red          = LoadIconFromFile(Path.Combine(basePath, "receive_red.ico"), iconReceive_red);

            iconCircle_green         = LoadIconFromFile(Path.Combine(basePath, "circle_green.ico" ), iconCircle_green);
            iconCircle_red           = LoadIconFromFile(Path.Combine(basePath, "circle_red.ico"   ), iconCircle_red);
            iconCircle_grey          = LoadIconFromFile(Path.Combine(basePath, "circle_grey.ico"  ), iconCircle_grey);
            iconCircle_orange        = LoadIconFromFile(Path.Combine(basePath, "circle_orange.ico"), iconCircle_orange);
        }

        //occurs when an adapter IP changed.
        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("Network address changed !");
            this.PopulateNICs(this.NetworkAdaptersToolStripMenuItem);
        }

        //occurs when changed to at least one adapter got the "UP" status, or when all the adapters are different from the "UP" status.
        //Warning : It means that if one adapter is UP, another one changed its status to UP, the event is not fired !
        void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            //MessageBox.Show("Network availability changed !");
            this.PopulateNICs(this.NetworkAdaptersToolStripMenuItem);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            // apply icon set if necessary
            if( Settings.Default.IconSet != "xp" )
            {
                this.applyIconSet();
            }

            this.InitializeSpeedSamples();

            this.RefreshBlinkDurationFromSettings();

            if (Settings.Default.BandwidthUnit != 1 && Settings.Default.BandwidthUnit != 8)
            {
                Settings.Default.BandwidthUnit = (int)eBandwithUnit.Byte;
            }
            if (Settings.Default.BandwidthDownloadMultiplier == 0)
            {
                Settings.Default.BandwidthDownloadMultiplier = (long)eBandwidthMultiplier.un;
            }
            if (Settings.Default.BandwidthUploadMultiplier   == 0)
            {
                Settings.Default.BandwidthUploadMultiplier   = (long)eBandwidthMultiplier.un;
            }

            bandwidthDownloadLvl5 = Settings.Default.BandwidthDownload * Settings.Default.BandwidthDownloadMultiplier / Settings.Default.BandwidthUnit;
            bandwidthUploadLvl5   = Settings.Default.BandwidthUpload   * Settings.Default.BandwidthUploadMultiplier   / Settings.Default.BandwidthUnit;
            bandwidthDownloadLvl4 = bandwidthDownloadLvl5 * 4 / 5;
            bandwidthDownloadLvl3 = bandwidthDownloadLvl5 * 3 / 5;
            bandwidthDownloadLvl2 = bandwidthDownloadLvl5 * 2 / 5;
            bandwidthDownloadLvl1 = bandwidthDownloadLvl5     / 5;
            bandwidthUploadLvl4   = bandwidthUploadLvl5   * 4 / 5;
            bandwidthUploadLvl3   = bandwidthUploadLvl5   * 3 / 5;
            bandwidthUploadLvl2   = bandwidthUploadLvl5   * 2 / 5;
            bandwidthUploadLvl1   = bandwidthUploadLvl5       / 5;

            customBandwidth = Settings.Default.BandwidthVisualsCustom == true;

            //registers the networkchange event handlers
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            NetworkChange.NetworkAddressChanged      += new NetworkAddressChangedEventHandler     (NetworkChange_NetworkAddressChanged     );

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

            this.notifyIconPing.Visible = Settings.Default.AutoPingEnabled;
            if (Settings.Default.AutoPingEnabled)
            {
                this.StartAutoPingThread();
            }
            else
            {
                this.StopAutoPingThread();
            }

            try
            {
                foreach (ProcessThread t in Process.GetCurrentProcess().Threads)
                {
                    t.PriorityLevel = ThreadPriorityLevel.Lowest; //we don't want our application to eat too much processor time !
                }
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Idle; //same here !
            }
            catch (PlatformNotSupportedException) { }
            catch (System.ComponentModel.Win32Exception) { }
            catch (NotSupportedException) { }
            catch (InvalidOperationException) { }
            catch (System.ComponentModel.InvalidEnumArgumentException) { }
            catch (SystemException) { }

            this.Hide();
            
            showBalloon(true);
        }

        private void RefreshBlinkDurationFromSettings()
        {
            if (Settings.Default.BlinkDuration < BlinkDurationMinimum)
            {
                Settings.Default.BlinkDuration = BlinkDurationMinimum;
                Settings.Default.Save();
            }

            nDuration = Settings.Default.BlinkDuration;
        }

        private void InitializeSpeedSamples()
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

        private void OnAbout(object sender, EventArgs e)
        {
            AboutForm formAbout = new AboutForm();
            try
            {
                formAbout.ShowDialog();
            }
            catch (InvalidOperationException) { }
        }

        private void OnSettings(object sender, EventArgs e)
        {
            this.showSettings();
        }

        internal void showSettings()
        {
            SettingsForm formSettings = new SettingsForm();
            try
            {
                formSettings.ShowDialog();
            }
            catch (InvalidOperationException) { }
            
            this.RefreshBlinkDurationFromSettings();

            frmBalloon fb = (frmBalloon)Application.OpenForms["frmBalloon"];
            if (fb != null)
            {
                fb.BallonTimer.Interval = nDuration;
            }

            if (Settings.Default.BandwidthUnit != 1 && Settings.Default.BandwidthUnit != 8)
            {
                Settings.Default.BandwidthUnit = (int)eBandwithUnit.Byte;
            }
            if (Settings.Default.BandwidthDownloadMultiplier == 0)
            {
                Settings.Default.BandwidthDownloadMultiplier = (long)eBandwidthMultiplier.un;
            }
            if (Settings.Default.BandwidthUploadMultiplier == 0)
            {
                Settings.Default.BandwidthUploadMultiplier = (long)eBandwidthMultiplier.un;
            }

            bandwidthDownloadLvl5 = Settings.Default.BandwidthDownload * Settings.Default.BandwidthDownloadMultiplier / Settings.Default.BandwidthUnit;
            bandwidthUploadLvl5   = Settings.Default.BandwidthUpload   * Settings.Default.BandwidthUploadMultiplier   / Settings.Default.BandwidthUnit;
            bandwidthDownloadLvl4 = bandwidthDownloadLvl5 * 4 / 5;
            bandwidthDownloadLvl3 = bandwidthDownloadLvl5 * 3 / 5;
            bandwidthDownloadLvl2 = bandwidthDownloadLvl5 * 2 / 5;
            bandwidthDownloadLvl1 = bandwidthDownloadLvl5     / 5;
            bandwidthUploadLvl4   = bandwidthUploadLvl5   * 4 / 5;
            bandwidthUploadLvl3   = bandwidthUploadLvl5   * 3 / 5;
            bandwidthUploadLvl2   = bandwidthUploadLvl5   * 2 / 5;
            bandwidthUploadLvl1   = bandwidthUploadLvl5       / 5;

            customBandwidth = Settings.Default.BandwidthVisualsCustom == true;

            this.notifyIconPing.Visible = Settings.Default.AutoPingEnabled;
            if (!Settings.Default.AutoPingEnabled)
            {
                this.StopAutoPingThread();
            }
            else
            {
                this.StartAutoPingThread();
            }

            this.applyIconSet();
        }

        private void OnExit(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            finally
            {
                Application.Exit();
            }
            
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                workerCancellationTokenSource.Cancel();
            }
            catch (ObjectDisposedException) { }

            this.StopAutoPingThread();

            TryJoinThread(hNetStatThread2);
            TryJoinThread(hNICRefreshThread);

            workerCancellationTokenSource.Dispose();
        }

        private void StartAutoPingThread()
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

        private void StopAutoPingThread()
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

        /// <summary>
        /// Add the specified MAC address to the list of the known MAC addresses
        /// </summary>
        /// <param name="mac">A MAC address</param>
        /// <returns>True if the MAC address was already known, false otherwise</returns>
        private bool AddToKnownInterface(string mac, bool saveSettings = true)
        {
            bool contains = false;
            try
            {
                contains = Settings.Default.KnownInterfaceMACList.Contains(mac);
            }
            catch (ArgumentNullException) { }
            if (!contains)
            {
                Settings.Default.KnownInterfaceMACList += (Settings.Default.KnownInterfaceMACList == string.Empty ? String.Empty : ";") + mac;
                if (saveSettings)
                {
                    Settings.Default.Save();
                }
            }
            return contains;
        }

        /// <summary>
        /// Enable the statistics for the interface designed by the specified MAC address
        /// </summary>
        /// <param name="mac">A MAC address</param>
        /// <param name="enable">If set to True (default value), the interface will be enabled for statistics</param>
        private bool EnableStatisticsForInterface(string mac, bool enable = true, bool saveSettings = true)
        {
            string tmp = Settings.Default.EnabledInterfaceMACList;
            bool contains = false;
            try
            {
                contains = tmp.Contains(mac);
            }
            catch (ArgumentNullException) { }
            
            bool modified = false;
            bool empty = false;

            if (mac == String.Empty)
            {
                return false;
            }

            if (tmp == "TOSET")
            {
                tmp = String.Empty;
            }
            empty = tmp == String.Empty;

            if (contains)
            {
                if (!enable)
                {
                    try
                    {
                        tmp = tmp.Replace(";" + mac, ""); //mac is second to last value
                        tmp = tmp.Replace(mac + ";", ""); //mac is first value
                        tmp = tmp.Replace(mac, ""); //mac was the only value
                        modified = true;
                    }
                    catch (ArgumentNullException) { }
                    catch (ArgumentException) { }
                }
            }
            else
            {
                if (enable)
                {
                    tmp      = tmp + (empty ? String.Empty : ";") + mac;
                    modified = true;
                }
            }
            if (modified)
            {
                Settings.Default.EnabledInterfaceMACList = tmp;
                if (saveSettings)
                {
                    Settings.Default.Save();
                }

                if (tmp == String.Empty)
                {
                    try
                    {
                        MessageBox.Show(Res.str_WarningNoInterfaceSelected, Res.str_WarningNoInterfaceSelectedCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (System.ComponentModel.InvalidEnumArgumentException) { }
                    catch (InvalidOperationException) { }
                }
            }
            return modified;
        }

        private void OnAdapterClick(object sender, EventArgs e)
        {
            //we get the adapter item that was clicked
            ToolStripMenuItem itm = (ToolStripMenuItem)sender;

            //we get the NetworkInterface object linked to it
            NetworkInterface nic = (NetworkInterface)itm.Tag;

            //we open the adapter properties

            try
            {
                Process.Start(
                    "explorer.exe",
                    "/N,::{20D04FE0-3AEA-1069-A2D8-08002B30309D}\\::{21EC2020-3AEA-1069-A2DD-08002B30309D}\\::{7007ACC7-3202-11D1-AAD2-00805FC1270E}\\::" + nic.Id
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(Res.str_ErrorCrash + Environment.NewLine + ex.Message );
            }
        }

        private void OnAdapterCheckStateChanged(object sender, EventArgs e)
        {
            //we get the adapter item that was clicked
            ToolStripMenuItem itm = (ToolStripMenuItem)sender;

            //we get the NetworkInterface object linked to it
            NetworkInterface nic = (NetworkInterface)itm.Tag;

            //we enable or disable statistics for the NetworkInterface 
            EnableStatisticsForInterface(nic.GetPhysicalAddress().ToString(), itm.Checked);
        }

        private void NetworkConnectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens the Network Connections applet
            try
            {
                Process.Start(
                    "explorer.exe",
                    "/N,::{20D04FE0-3AEA-1069-A2D8-08002B30309D}\\::{21EC2020-3AEA-1069-A2DD-08002B30309D}\\::{7007ACC7-3202-11D1-AAD2-00805FC1270E}"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(Res.str_ErrorCrash + Environment.NewLine + ex.Message);
            }
        }

        private void NetworkSharingCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens the Network Sharing Center applet
            try
            {
                Process.Start(
                    "explorer.exe",
                    "/N,::{26EE0668-A00A-44D7-9371-BEB064C98683}\\0\\::{8E908FC9-BECC-40F6-915B-F4CA0E70D03D}"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(Res.str_ErrorCrash + Environment.NewLine + ex.Message);
            }
        }

        private void FirewallSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens the Firewall Settings applet
            try
            {
                Process.Start(
                    "explorer.exe",
                    "/N,::{26EE0668-A00A-44D7-9371-BEB064C98683}\\0\\::{4026492F-2F69-46B8-B9BF-5654FC07E423}"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(Res.str_ErrorCrash + Environment.NewLine + ex.Message);
            }
        }

        private void manageWirelessNetworksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens the Manage wireless networks applet
            try
            {
                Process.Start("Shell:::{1FA9085F-25A2-489B-85D4-86326EEDCD87}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(Res.str_ErrorCrash + Environment.NewLine + ex.Message);
            }            
        }

        private void homeGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens the Home Group Settings applet
            try
            {
                Process.Start(
                    "explorer.exe",
                    "/N,::{26EE0668-A00A-44D7-9371-BEB064C98683}\\0\\::{67CA7650-96E6-4FDD-BB43-A8E774F73A57}"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(Res.str_ErrorCrash + Environment.NewLine + ex.Message);
            }
        }

        private void networkdomainworkgroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens the Domain/workgroup applet
            try
            {
                Process.Start("Shell:::{208D2C60-3AEA-1069-A2D7-08002B30309D}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(Res.str_ErrorCrash + Environment.NewLine + ex.Message);
            }
            
        }

        private void networkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens the network applet
            try
            {
                Process.Start("Shell:::{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(Res.str_ErrorCrash + Environment.NewLine + ex.Message);
            }            
        }

        private void networkMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens the Domain/workgroup applet
            try
            {
                Process.Start("Shell:::{E7DE9B1A-7533-4556-9484-B26FB486475E}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(Res.str_NotAvailableWinver + Environment.NewLine + ex.Message);
            }
        }

        private void PopulateNICs(ToolStripMenuItem parent)
        {
            int nUp = 0;
            string ip = "";
            string unit = string.Empty;
            double speed;
            Bitmap icon = Resources.netshell_1612_16x16.ToBitmap();
            bool valid = false;
            bool isFirstTime = Settings.Default.EnabledInterfaceMACList == "TOSET";
            string mac = string.Empty;
            ToolStripMenuItem itm;
            ToolStripMenuItem itm2;
            IPInterfaceProperties ipproperties;
            bool shouldSaveSettings = false;

            //we do some little cleaning...
            ClearParentThreadSafe(parent);

            //We want to avoid to run the following lines often because it temporarily raises the CPU usage to 2%, which is unacceptable for a background app.
            //This is why the procedure PopulateNICs is not periodically launched in a thread but by events and testing if the nb of adapters changes.

            try
            {
                Monitor.Enter(selectedInterfaces);
            }
            catch (ArgumentNullException) { }

            try
            {
                interfaces = NetworkInterface.GetAllNetworkInterfaces();
            }
            catch (NetworkInformationException) { interfaces = Array.Empty<NetworkInterface>(); }

            try
            {
                selectedInterfaces.Clear();
            }
            catch (NotSupportedException) { selectedInterfaces = new ArrayList(); }
            
            foreach (NetworkInterface netInterface in interfaces)
            {
                //test if the interface is active
                if (netInterface.OperationalStatus != OperationalStatus.Up)
                {
                    ip = Res.str_NotConnected;
                }
                else
                {
                    ipproperties = netInterface.GetIPProperties();
                    if (ipproperties.UnicastAddresses.Count > 0)
                    {
                        if (netInterface.NetworkInterfaceType.ToString().ToLower(CultureInfo.InvariantCulture).Contains("ethernet") ||
                            netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                            netInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel )
                        {
                            nUp++;
                        }
                        try
                        {
                            ip = ipproperties.UnicastAddresses[ipproperties.UnicastAddresses.Count - 1].Address.ToString();
                        }
                        catch (IndexOutOfRangeException) { }
                        catch (System.Net.Sockets.SocketException) { }
                    }
                    else
                    {
                        ip = Res.str_NoIpAvailable;
                    }
                }

                // Only get ethernet adapters
                if (netInterface.NetworkInterfaceType.ToString().ToLower(CultureInfo.InvariantCulture).Contains("ethernet"))
                {
                    valid = true;
                    icon = (netInterface.Description + netInterface.Name).ToLower(CultureInfo.InvariantCulture).Contains("bluetooth") ? Properties.Resources.netshell_1613_16x16.ToBitmap() : Properties.Resources.deskadp_16x16.ToBitmap();
                }
                // Only get 802.11 adapters
                else if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    valid = true;
                    icon = Properties.Resources.netshell_1612_16x16.ToBitmap();
                }
                // Only get tunnels
                else if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                {
                    valid = true;
                    icon = Properties.Resources.network_pipe_16x16.ToBitmap();
                }
                else
                {
                    valid = false;
                }

                if (valid) //we want to use this interface in our list
                {
                    //retrieve the mac address of the interface
                    mac = netInterface.GetPhysicalAddress().ToString();
                    
                    // if we never selected an interface before, enable it for statistics by default.
                    if (isFirstTime)
                    {
                        if (EnableStatisticsForInterface(mac, true, false))
                        {
                            shouldSaveSettings = true;
                        }
                    }

                    // if this is the first time we encounter this interface, enable it for statistics by default.
                    if (!AddToKnownInterface(mac, false))
                    {
                        shouldSaveSettings = true;
                        if (EnableStatisticsForInterface(mac, true, false))
                        {
                            shouldSaveSettings = true;
                        }
                    }

                    selectedInterfaces.Add(netInterface);
                    speed = computeSpeed(netInterface.Speed, ref unit, 2);

                    //we generate the item related to the network interface
                    itm = new ToolStripMenuItem(netInterface.Name +
                            " [" + netInterface.Description + "]" +
                            " : " + speed + " " + unit + "/s" +
                            ", " + ip
                            , icon
                            , OnAdapterClick
                            , "itm_" + netInterface.Id);                    
                    itm.Tag = netInterface;

                    //we generate its subitem
                    itm2                    = new ToolStripMenuItem(Res.str_IncludeInStatistics);
                    itm2.Name               = "itm_include_" + netInterface.Id;
                    itm2.CheckOnClick       = true;
                    itm2.Checked            = mac != null ? Settings.Default.EnabledInterfaceMACList.Contains(mac) : false;
                    itm2.CheckStateChanged += new EventHandler(OnAdapterCheckStateChanged);
                    itm2.Tag                = netInterface;

                    //we add the subitem to the item
                    try
                    {
                        itm.DropDownItems.Add(itm2);
                    }
                    catch (ArgumentNullException) { }

                    //add item in parent menu item
                    AddItemThreadSafe(itm, parent);
                }
            }

            try
            {
                Array.Clear(interfaces, 0, interfaces.Length);
            }
            catch (OverflowException) { }
            catch (ArgumentNullException) { }
            catch (IndexOutOfRangeException) { }

            try
            {
                Monitor.Exit(selectedInterfaces);
            }
            catch (ArgumentNullException) { }
            catch (SynchronizationLockException) { }


            if (nUp == 0)
            {
                MainForm.connectionStatus = eState.disconnected;
            }
            else
            {
                MainForm.connectionStatus = eState.up;
            }

            if (shouldSaveSettings)
            {
                Settings.Default.Save();
            }
        }

        // clears the children toolstripmenuitems of parent, thread-safely
        delegate void ClearParentThreadSafeCallback(ToolStripMenuItem parent);
        private void ClearParentThreadSafe(ToolStripMenuItem parent)
        {
            if (this.InvokeRequired)
            {
                ClearParentThreadSafeCallback d = new ClearParentThreadSafeCallback(ClearParentThreadSafe);
                this.Invoke(d, new object[] { parent });
            }
            else
            {
                try
                {
                    parent.DropDownItems.Clear();
                }
                catch (NotSupportedException) { }
            }
        }

        // adds a child toolstripmenuitem to parent, thread-safely
        delegate void AddItemThreadSafeCallback(ToolStripMenuItem itm, ToolStripMenuItem parent);
        private void AddItemThreadSafe(ToolStripMenuItem itm, ToolStripMenuItem parent)
        {
            if (this.InvokeRequired)
            {
                AddItemThreadSafeCallback d = new AddItemThreadSafeCallback(AddItemThreadSafe);
                this.Invoke(d, new object[] { itm, parent });
            }
            else
            {
                try
                {
                    parent.DropDownItems.Add(itm);
                }
                catch (ArgumentNullException) { }
            }
        }

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

                    previous = this.notifyIconPing.Icon;
                    UpdateAutoPingIcon(reply);

                    if (Settings.Default.AutoPingNotif && !previous.Equals(this.notifyIconPing.Icon))
                    {
                        this.notifyIconPing.ShowBalloonTip(1000);
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
                MessageBox.Show(
                    Res.str_ErrorCrash +
                    "\n\n" + "Thread : " +
                    Thread.CurrentThread.Name +
                    "\n\n" +
                    ex.ToString(), "GabNetStats", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Application.Restart();
            }
        }

        private void UpdateAutoPingIcon(PingReply reply)
        {
            if (reply == null || reply.Status != IPStatus.Success)
            {
                if (this.notifyIconPing.Icon.Equals(iconCircle_green))
                {
                    this.notifyIconPing.Icon = iconCircle_orange;
                    this.notifyIconPing.Text = this.notifyIconPing.BalloonTipText = "Connection issue?";
                    this.notifyIconPing.BalloonTipText += "\nThe host \"" + Settings.Default.AutoPingHost + "\" seems to be unreachable.";
                    this.notifyIconPing.BalloonTipIcon = ToolTipIcon.Warning;
                }
                else if (this.notifyIconPing.Icon.Equals(iconCircle_orange))
                {
                    this.notifyIconPing.Icon = iconCircle_red;
                    this.notifyIconPing.Text = this.notifyIconPing.BalloonTipText = "Connection issue!";
                    this.notifyIconPing.BalloonTipText += "\nThe host \"" + Settings.Default.AutoPingHost + "\" could not be reached.";
                    this.notifyIconPing.BalloonTipIcon = ToolTipIcon.Error;
                }
                else if (!this.notifyIconPing.Icon.Equals(iconCircle_red))
                {
                    this.notifyIconPing.Icon = iconCircle_orange;
                    this.notifyIconPing.Text = this.notifyIconPing.BalloonTipText = "Connection issue?";
                    this.notifyIconPing.BalloonTipIcon = ToolTipIcon.Warning;
                }
            }
            else
            {
                this.notifyIconPing.Icon = iconCircle_green;
                this.notifyIconPing.Text = this.notifyIconPing.BalloonTipText = "Connection OK";
                this.notifyIconPing.BalloonTipIcon = ToolTipIcon.Info;
            }
        }

        private void SetActivityIcon(Icon icon)
        {
            if (icon == null)
            {
                return;
            }

            if (!ReferenceEquals(currentActivityIcon, icon))
            {
                currentActivityIcon = icon;
                this.notifyIconActivity.Icon = icon;
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
                    properties = IPGlobalProperties.GetIPGlobalProperties();
                    try
                    {
                        ipv4stat = properties.GetIPv4GlobalStatistics();
                        ipv6stat = properties.GetIPv6GlobalStatistics();
                    }
                    catch (NetworkInformationException)
                    {
                        continue;
                    }
                    catch( PlatformNotSupportedException )
                    {
                        continue;
                    }
                   
                    nbv4         = ipv4stat.NumberOfInterfaces;
                    nbv6         = ipv6stat.NumberOfInterfaces;

                    //if number changed since last time AND we are not displaying the context menu then
                    if (nbv4 + nbv6 != nbNIC && this.NetworkAdaptersToolStripMenuItem.Visible == false)
                    {
                        nbNIC = nbv4 + nbv6;
                        this.PopulateNICs(this.NetworkAdaptersToolStripMenuItem);
                    }

                    try
                    {
                        WaitWithCancellation(cancellationToken, nNICRefresh);
                    }
                    catch (ArgumentOutOfRangeException) { }
                }

            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
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

            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (bSetIconContinue)
                    {
                        bSetIconContinue = false;
                        bytesReceived    = 0;
                        bytesSent        = 0;

                        if (connectionStatus == eState.disconnected)
                        {
                            this.SetActivityIcon(iconDisconnected);
                            rawSpeedReception            = 0;
                            rawSpeedEmission             = 0;
                            goto skip;
                        }
                        if (connectionStatus == eState.limited)
                        {
                            this.SetActivityIcon(iconLimited);
                            rawSpeedReception            = 0;
                            rawSpeedEmission             = 0;
                            goto skip;
                        }

                        try
                        {
                            Monitor.Enter(selectedInterfaces);
                        }
                        catch (ArgumentNullException)
                        {
                            continue;
                        }
                        
                        
                        foreach (NetworkInterface netInterface in selectedInterfaces)
                        {
                            try
                            {
                                if (Settings.Default.EnabledInterfaceMACList.Contains(netInterface.GetPhysicalAddress().ToString()))
                                {
                                    ipstats = netInterface.GetIPStatistics();
                                    bytesReceived += ipstats.BytesReceived;
                                    bytesSent += ipstats.BytesSent;
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }

                        try
                        {
                            Monitor.Exit(selectedInterfaces);
                        }
                        catch (ArgumentNullException)
                        {
                            continue;
                        }
                        catch( SynchronizationLockException )
                        {
                            continue;
                        }
                        

                        if (bytesReceived != oldbytesReceived && bytesSent != oldbytesSent)
                        {
                            try
                            {
                                rawSpeedReception = Math.Abs((1000 / nDuration) * (bytesReceived - oldbytesReceived));
                                rawSpeedEmission = Math.Abs((1000 / nDuration) * (bytesSent - oldbytesSent));
                            }
                            catch (Exception)
                            {
                                rawSpeedReception = 0;
                                rawSpeedEmission = 0;
                            }
                            
                            oldbytesReceived  = bytesReceived;
                            oldbytesSent      = bytesSent;
                            nCounter          = 0;

                            if (customBandwidth)
                            {
                                if (rawSpeedReception >= bandwidthDownloadLvl4 && rawSpeedEmission >= bandwidthUploadLvl4)
                                {
                                    this.SetActivityIcon(iconActive_red_red);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl4 && rawSpeedEmission >= bandwidthUploadLvl3)
                                {
                                    this.SetActivityIcon(iconActive_orange_red);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl4 && rawSpeedEmission >= bandwidthUploadLvl2)
                                {
                                    this.SetActivityIcon(iconActive_yellow_red);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl4 && rawSpeedEmission >= bandwidthUploadLvl1)
                                {
                                    this.SetActivityIcon(iconActive_green_red);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl4 && rawSpeedEmission < bandwidthUploadLvl1)
                                {
                                    this.SetActivityIcon(iconActive_blue_red);
                                }

                                else if (rawSpeedReception >= bandwidthDownloadLvl3 && rawSpeedEmission >= bandwidthUploadLvl4)
                                {
                                    this.SetActivityIcon(iconActive_red_orange);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl3 && rawSpeedEmission >= bandwidthUploadLvl3)
                                {
                                    this.SetActivityIcon(iconActive_orange_orange);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl3 && rawSpeedEmission >= bandwidthUploadLvl2)
                                {
                                    this.SetActivityIcon(iconActive_yellow_orange);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl3 && rawSpeedEmission >= bandwidthUploadLvl1)
                                {
                                    this.SetActivityIcon(iconActive_green_orange);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl3 && rawSpeedEmission < bandwidthUploadLvl1)
                                {
                                    this.SetActivityIcon(iconActive_blue_orange);
                                }

                                else if (rawSpeedReception >= bandwidthDownloadLvl2 && rawSpeedEmission >= bandwidthUploadLvl4)
                                {
                                    this.SetActivityIcon(iconActive_red_yellow);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl2 && rawSpeedEmission >= bandwidthUploadLvl3)
                                {
                                    this.SetActivityIcon(iconActive_orange_yellow);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl2 && rawSpeedEmission >= bandwidthUploadLvl2)
                                {
                                    this.SetActivityIcon(iconActive_yellow_yellow);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl2 && rawSpeedEmission >= bandwidthUploadLvl1)
                                {
                                    this.SetActivityIcon(iconActive_green_yellow);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl2 && rawSpeedEmission < bandwidthUploadLvl1)
                                {
                                    this.SetActivityIcon(iconActive_blue_yellow);
                                }

                                else if (rawSpeedReception >= bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl4)
                                {
                                    this.SetActivityIcon(iconActive_red_green);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl3)
                                {
                                    this.SetActivityIcon(iconActive_orange_green);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl2)
                                {
                                    this.SetActivityIcon(iconActive_yellow_green);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl1)
                                {
                                    this.SetActivityIcon(iconActive_green_green);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl1 && rawSpeedEmission < bandwidthUploadLvl1)
                                {
                                    this.SetActivityIcon(iconActive_blue_green);
                                }

                                else if (rawSpeedReception < bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl4)
                                {
                                    this.SetActivityIcon(iconActive_red_blue);
                                }
                                else if (rawSpeedReception < bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl3)
                                {
                                    this.SetActivityIcon(iconActive_orange_blue);
                                }
                                else if (rawSpeedReception < bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl2)
                                {
                                    this.SetActivityIcon(iconActive_yellow_blue);
                                }
                                else if (rawSpeedReception < bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl1)
                                {
                                    this.SetActivityIcon(iconActive_green_blue);
                                }
                                else if (rawSpeedReception < bandwidthDownloadLvl1 && rawSpeedEmission < bandwidthUploadLvl1)
                                {
                                    this.SetActivityIcon(iconActive_blue_blue);
                                }
                            }
                            else
                            {
                                this.SetActivityIcon(iconActive_blue_blue);
                            }

                        }
                        else if (bytesReceived != oldbytesReceived && bytesSent == oldbytesSent)
                        {
                            try
                            {
                                rawSpeedReception = Math.Abs((1000 / nDuration) * (bytesReceived - oldbytesReceived));
                            }
                            catch (Exception)
                            {
                                rawSpeedReception = 0;
                            }
                            
                            rawSpeedEmission  = 0;
                            oldbytesReceived  = bytesReceived;
                            nCounter          = 0;

                            if (customBandwidth)
                            {
                                if (rawSpeedReception >= bandwidthDownloadLvl4)
                                {
                                    this.SetActivityIcon(iconReceive_red);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl3)
                                {
                                    this.SetActivityIcon(iconReceive_orange);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl2)
                                {
                                    this.SetActivityIcon(iconReceive_yellow);
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl1)
                                {
                                    this.SetActivityIcon(iconReceive_green);
                                }
                                else if (rawSpeedReception < bandwidthDownloadLvl1)
                                {
                                    this.SetActivityIcon(iconReceive_blue);
                                }
                            }
                            else
                            {
                                this.SetActivityIcon(iconReceive_blue);
                            }
                        }
                        else if (bytesReceived == oldbytesReceived && bytesSent != oldbytesSent)
                        {

                            rawSpeedReception = 0;
                            try
                            {
                                rawSpeedEmission = Math.Abs((1000 / nDuration) * (bytesSent - oldbytesSent));
                            }
                            catch (Exception)
                            {
                                rawSpeedEmission = 0;
                            }
                            
                            oldbytesSent      = bytesSent;
                            nCounter          = 0;

                            if (customBandwidth)
                            {
                                if (rawSpeedEmission >= bandwidthUploadLvl4)
                                {
                                    this.SetActivityIcon(iconSend_red);
                                }
                                else if (rawSpeedEmission >= bandwidthUploadLvl3)
                                {
                                    this.SetActivityIcon(iconSend_orange);
                                }
                                else if (rawSpeedEmission >= bandwidthUploadLvl2)
                                {
                                    this.SetActivityIcon(iconSend_yellow);
                                }
                                else if (rawSpeedEmission >= bandwidthUploadLvl1)
                                {
                                    this.SetActivityIcon(iconSend_green);
                                }
                                else if (rawSpeedEmission < bandwidthUploadLvl1)
                                {
                                    this.SetActivityIcon(iconSend_blue);
                                }
                            }
                            else
                            {
                                this.SetActivityIcon(iconSend_blue);
                            }
                        }
                        else
                        {
                            nCounter++; //the counter adds a small persistance effect
                        }

                        if (nCounter == 5)
                        {
                            nCounter                     = 0;
                            this.SetActivityIcon(iconInactive);
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

                        bSetIconContinue = true;
                    }

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
                MessageBox.Show(
                    Res.str_ErrorCrash +
                    "\n\n" + "Thread : " +
                    Thread.CurrentThread.Name +
                    "\n\n" +
                    ex.ToString(), "GabNetStats", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Application.Restart();
            }
        }

        internal static double computeSpeed(long rawSpeed, ref string speedUnit)
        {
            return computeSpeed(rawSpeed, ref speedUnit, 1);
        }

        internal static double computeSpeed(long rawSpeed, ref string speedUnit, int typeunit)
        {
            double res = 0;
            try
            {
                switch (typeunit)
                {
                    case 1:
                        if (rawSpeed >= 1073741824) //1073741824 = 2 ^ 30
                        {
                            speedUnit = SpeedUnitsByte.GiB;
                            res = Math.Round(rawSpeed / (double)1073741824, 2);
                        }
                        else if (rawSpeed >= 1048576) //1048576 = 2 ^ 20
                        {
                            speedUnit = SpeedUnitsByte.MiB;
                            res = Math.Round(rawSpeed / (double)1048576, 2);
                        }
                        else if (rawSpeed >= 1024) //1024 = 2 ^ 10
                        {
                            speedUnit = SpeedUnitsByte.KiB;
                            res = Math.Round(rawSpeed / (double)1024, 2);
                        }
                        else
                        {
                            speedUnit = SpeedUnitsByte.Bytes;
                            res = rawSpeed;
                        }
                        break;

                    case 2:
                        if (rawSpeed >= 1000000000)
                        {
                            speedUnit = SpeedUnitsBit.Gbit;
                            res = Math.Round(rawSpeed / (double)1000000000, 2);
                        }
                        else if (rawSpeed >= 1000000)
                        {
                            speedUnit = SpeedUnitsBit.Mbit;
                            res = Math.Round(rawSpeed / (double)1000000, 2);
                        }
                        else if (rawSpeed >= 1000)
                        {
                            speedUnit = SpeedUnitsBit.Kbit;
                            res = Math.Round(rawSpeed / (double)1000, 2);
                        }
                        else
                        {
                            speedUnit = SpeedUnitsBit.bit;
                            res = rawSpeed;
                        }
                        break;
                    default:
                        goto case 1;
                }
            }
            catch (Exception)
            {
                res = 0;
            }
            return res;
        }

        private void notifyIconActivity_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                showBalloon(false);
            }
        }

        private void showBalloon(bool preload = false)
        {
            if (fBal == null || !fBal.Created)
            {
                fBal = new frmBalloon();
            }
            if (!preload)
            {
               fBal.Show();
            }
            fBal.SetDesktopLocation(Screen.PrimaryScreen.WorkingArea.Width - fBal.Width - SystemInformation.FixedFrameBorderSize.Width, Screen.PrimaryScreen.WorkingArea.Height - fBal.Height - SystemInformation.FixedFrameBorderSize.Height);
        }

        private void advancedStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmBalloon.frmAdv == null || !frmBalloon.frmAdv.Created)
            {
                frmBalloon.frmAdv = new frmAdvanced();
            }
            frmBalloon.frmAdv.Show();
        }

        private void notifyIconPing_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                showBalloon(false);
            }
        }
    }
}
