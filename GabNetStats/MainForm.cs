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

        private bool customBandwidth     = false;

        static TrayIconManager.eState connectionStatus;

        private TrayIconManager trayIconManager;

        internal const int BlinkDurationMinimum = 50;
        static int nDuration   = BlinkDurationMinimum;
        static int nNICRefresh = 10000; //time interval for refreshing the NIC list (10s by default)
        static int nbNIC       = 0;
        private const int avgSpeedNbItems = 50;
        private static readonly object speedSamplesLock = new object();
        private static readonly long[] receptionSamples = new long[avgSpeedNbItems];
        private static readonly long[] emissionSamples  = new long[avgSpeedNbItems];
        private static int receptionSampleCount = avgSpeedNbItems;
        private static int emissionSampleCount  = avgSpeedNbItems;
        private static int receptionSampleIndex = 0;
        private static int emissionSampleIndex  = 0;
        private static HashSet<string> enabledInterfaceMacs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        internal sealed class TrackedInterface
        {
            public TrackedInterface(NetworkInterface networkInterface, string macAddress)
            {
                Interface = networkInterface;
                MacAddress = macAddress;
            }

            public NetworkInterface Interface { get; }
            public string MacAddress { get; }
            public bool IsEnabled => IsInterfaceEnabled(MacAddress);
        }

        static List<TrackedInterface> selectedInterfaces = new List<TrackedInterface>();
        static IPGlobalProperties properties;
        static IPGlobalStatistics ipv4stat;
        static IPGlobalStatistics ipv6stat;
        private static readonly string[] hiddenInterfaceKeywords = new[]
        {
            "virtual",
            "hyper-v",
            "vmware",
            "loopback",
            "kernel debug",
            "container",
            "wi-fi direct",
            "bluetooth device",
            "qos",
            "wfp",
            "wan miniport",
            "filter"
        };

        static frmBalloon fBal;

        private volatile bool _nicMenuOpen;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_QUERYENDSESSION = 0x0011;
            const int WM_ENDSESSION = 0x0016;

            if (m.Msg == WM_QUERYENDSESSION || m.Msg == WM_ENDSESSION)
            {
                Program.IsWindowsShuttingDown = true;
                // Signal threads to stop and save state cleanly
                try { workerCancellationTokenSource.Cancel(); } catch { }
                Settings.Default.Save();
                m.Result = (IntPtr)1; // Tell Windows: OK to shut down
                return;
            }
            base.WndProc(ref m);
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
            trayIconManager = new TrayIconManager(this.notifyIconActivity, this.notifyIconPing);

            // apply icon set if necessary
            if( Settings.Default.IconSet != "xp" )
            {
                trayIconManager.applyIconSet();
            }

            this.InitializeSpeedSamples();

            this.RefreshBlinkDurationFromSettings();

            if (Settings.Default.BandwidthUnit != 1 && Settings.Default.BandwidthUnit != 8)
            {
                Settings.Default.BandwidthUnit = (int)TrayIconManager.eBandwithUnit.Byte;
            }
            if (Settings.Default.BandwidthDownloadMultiplier == 0)
            {
                Settings.Default.BandwidthDownloadMultiplier = (long)TrayIconManager.eBandwidthMultiplier.un;
            }
            if (Settings.Default.BandwidthUploadMultiplier   == 0)
            {
                Settings.Default.BandwidthUploadMultiplier   = (long)TrayIconManager.eBandwidthMultiplier.un;
            }

            trayIconManager.bandwidthDownloadLvl5 = Settings.Default.BandwidthDownload * Settings.Default.BandwidthDownloadMultiplier / Settings.Default.BandwidthUnit;
            trayIconManager.bandwidthUploadLvl5   = Settings.Default.BandwidthUpload   * Settings.Default.BandwidthUploadMultiplier   / Settings.Default.BandwidthUnit;
            trayIconManager.bandwidthDownloadLvl4 = trayIconManager.bandwidthDownloadLvl5 * 4 / 5;
            trayIconManager.bandwidthDownloadLvl3 = trayIconManager.bandwidthDownloadLvl5 * 3 / 5;
            trayIconManager.bandwidthDownloadLvl2 = trayIconManager.bandwidthDownloadLvl5 * 2 / 5;
            trayIconManager.bandwidthDownloadLvl1 = trayIconManager.bandwidthDownloadLvl5     / 5;
            trayIconManager.bandwidthUploadLvl4   = trayIconManager.bandwidthUploadLvl5   * 4 / 5;
            trayIconManager.bandwidthUploadLvl3   = trayIconManager.bandwidthUploadLvl5   * 3 / 5;
            trayIconManager.bandwidthUploadLvl2   = trayIconManager.bandwidthUploadLvl5   * 2 / 5;
            trayIconManager.bandwidthUploadLvl1   = trayIconManager.bandwidthUploadLvl5       / 5;

            customBandwidth = Settings.Default.BandwidthVisualsCustom == true;
            RefreshEnabledInterfacesCache();

            //registers the networkchange event handlers
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            NetworkChange.NetworkAddressChanged      += new NetworkAddressChangedEventHandler     (NetworkChange_NetworkAddressChanged     );

            contextMenuTray.Opening += (s, ev) => _nicMenuOpen = true;
            contextMenuTray.Closing += (s, ev) => _nicMenuOpen = false;

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

        private static void RefreshEnabledInterfacesCache()
        {
            HashSet<string> newSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string enabledList = Settings.Default.EnabledInterfaceMACList;

            if (!String.IsNullOrWhiteSpace(enabledList) && !String.Equals(enabledList, "TOSET", StringComparison.OrdinalIgnoreCase))
            {
                string[] entries = enabledList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string entry in entries)
                {
                    newSet.Add(entry);
                }
            }

            Volatile.Write(ref enabledInterfaceMacs, newSet);
        }

        internal static bool IsInterfaceEnabled(string mac)
        {
            if (String.IsNullOrEmpty(mac))
            {
                return false;
            }

            HashSet<string> snapshot = Volatile.Read(ref enabledInterfaceMacs);
            return snapshot.Contains(mac);
        }

        private static bool ShouldDisplayInterface(NetworkInterface netInterface)
        {
            if (netInterface == null)
            {
                return false;
            }

            bool showDisconnected = Settings.Default.ShowDisconnectedInterfaces;
            if (!showDisconnected && netInterface.OperationalStatus != OperationalStatus.Up)
            {
                return false;
            }

            if (!HasValidPhysicalAddress(netInterface))
            {
                return false;
            }

            switch (netInterface.NetworkInterfaceType)
            {
                case NetworkInterfaceType.Ethernet:
                case NetworkInterfaceType.Ethernet3Megabit:
                case NetworkInterfaceType.FastEthernetFx:
                case NetworkInterfaceType.FastEthernetT:
                case NetworkInterfaceType.GigabitEthernet:
                case NetworkInterfaceType.Wireless80211:
                case NetworkInterfaceType.Tunnel:
                    break;
                default:
                    return false;
            }

            string description = netInterface.Description ?? String.Empty;
            string name = netInterface.Name ?? String.Empty;
            foreach (string keyword in hiddenInterfaceKeywords)
            {
                if (description.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static Bitmap GetInterfaceIcon(NetworkInterface netInterface)
        {
            string combined = ((netInterface.Description ?? String.Empty) + (netInterface.Name ?? String.Empty));
            if (combined.IndexOf("bluetooth", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Properties.Resources.netshell_1613_16x16.ToBitmap();
            }

            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
            {
                return Properties.Resources.netshell_1612_16x16.ToBitmap();
            }

            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
            {
                return Properties.Resources.network_pipe_16x16.ToBitmap();
            }

            return Properties.Resources.deskadp_16x16.ToBitmap();
        }

        private static bool HasValidPhysicalAddress(NetworkInterface netInterface)
        {
            try
            {
                byte[] addressBytes = netInterface.GetPhysicalAddress().GetAddressBytes();
                if (addressBytes == null || addressBytes.Length == 0)
                {
                    return false;
                }

                bool allZero = true;
                foreach (byte b in addressBytes)
                {
                    if (b != 0)
                    {
                        allZero = false;
                        break;
                    }
                }

                return !allZero;
            }
            catch (NetworkInformationException)
            {
                return false;
            }
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
                Settings.Default.BandwidthUnit = (int)TrayIconManager.eBandwithUnit.Byte;
            }
            if (Settings.Default.BandwidthDownloadMultiplier == 0)
            {
                Settings.Default.BandwidthDownloadMultiplier = (long)TrayIconManager.eBandwidthMultiplier.un;
            }
            if (Settings.Default.BandwidthUploadMultiplier == 0)
            {
                Settings.Default.BandwidthUploadMultiplier = (long)TrayIconManager.eBandwidthMultiplier.un;
            }

            trayIconManager.bandwidthDownloadLvl5 = Settings.Default.BandwidthDownload * Settings.Default.BandwidthDownloadMultiplier / Settings.Default.BandwidthUnit;
            trayIconManager.bandwidthUploadLvl5   = Settings.Default.BandwidthUpload   * Settings.Default.BandwidthUploadMultiplier   / Settings.Default.BandwidthUnit;
            trayIconManager.bandwidthDownloadLvl4 = trayIconManager.bandwidthDownloadLvl5 * 4 / 5;
            trayIconManager.bandwidthDownloadLvl3 = trayIconManager.bandwidthDownloadLvl5 * 3 / 5;
            trayIconManager.bandwidthDownloadLvl2 = trayIconManager.bandwidthDownloadLvl5 * 2 / 5;
            trayIconManager.bandwidthDownloadLvl1 = trayIconManager.bandwidthDownloadLvl5     / 5;
            trayIconManager.bandwidthUploadLvl4   = trayIconManager.bandwidthUploadLvl5   * 4 / 5;
            trayIconManager.bandwidthUploadLvl3   = trayIconManager.bandwidthUploadLvl5   * 3 / 5;
            trayIconManager.bandwidthUploadLvl2   = trayIconManager.bandwidthUploadLvl5   * 2 / 5;
            trayIconManager.bandwidthUploadLvl1   = trayIconManager.bandwidthUploadLvl5       / 5;

            customBandwidth = Settings.Default.BandwidthVisualsCustom == true;
            RefreshEnabledInterfacesCache();

            this.notifyIconPing.Visible = Settings.Default.AutoPingEnabled;
            if (!Settings.Default.AutoPingEnabled)
            {
                this.StopAutoPingThread();
            }
            else
            {
                this.StartAutoPingThread();
            }

            trayIconManager.applyIconSet();
        }

        internal IReadOnlyList<TrackedInterface> GetDisplayableInterfacesSnapshot()
        {
            lock (selectedInterfaces)
            {
                if (selectedInterfaces.Count == 0)
                {
                    return Array.Empty<TrackedInterface>();
                }

                TrackedInterface[] snapshot = new TrackedInterface[selectedInterfaces.Count];
                for (int i = 0; i < selectedInterfaces.Count; i++)
                {
                    TrackedInterface tracked = selectedInterfaces[i];
                    snapshot[i] = new TrackedInterface(tracked.Interface, tracked.MacAddress);
                }

                return snapshot;
            }
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
            NetworkChange.NetworkAvailabilityChanged -= NetworkChange_NetworkAvailabilityChanged;
            NetworkChange.NetworkAddressChanged      -= NetworkChange_NetworkAddressChanged;

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
        private bool EnableStatisticsForInterface(string mac, bool enable = true, bool saveSettings = true, bool refreshCache = true)
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
                if (refreshCache)
                {
                    RefreshEnabledInterfacesCache();
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

        internal bool SetInterfaceEnabledState(string mac, bool enable, bool refreshMenus = true)
        {
            bool changed = EnableStatisticsForInterface(mac, enable);
            if (changed && refreshMenus)
            {
                this.PopulateNICs(this.NetworkAdaptersToolStripMenuItem);
            }

            return changed;
        }

        internal void PopulateNICs(ToolStripMenuItem parent)
        {
            int nUp = 0;
            string ip = "";
            string unit = string.Empty;
            double speed;
            Bitmap icon = Resources.netshell_1612_16x16.ToBitmap();
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

            NetworkInterface[] nicSnapshot;
            try
            {
                nicSnapshot = NetworkInterface.GetAllNetworkInterfaces();
            }
            catch (NetworkInformationException)
            {
                nicSnapshot = Array.Empty<NetworkInterface>();
            }

            lock (selectedInterfaces)
            {
                selectedInterfaces.Clear();

                foreach (NetworkInterface netInterface in nicSnapshot)
                {
                    if (!ShouldDisplayInterface(netInterface))
                    {
                        continue;
                    }

                    //test if the interface is active
                    if (netInterface.OperationalStatus != OperationalStatus.Up)
                    {
                        ip = Res.str_NotConnected;
                    }
                    else
                    {
                        try
                        {
                            ipproperties = netInterface.GetIPProperties();
                        }
                        catch (NetworkInformationException)
                        {
                            ip = Res.str_NoIpAvailable;
                            continue;
                        }
                        catch (PlatformNotSupportedException)
                        {
                            ip = Res.str_NoIpAvailable;
                            continue;
                        }
                        if (ipproperties.UnicastAddresses.Count > 0)
                        {
                            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                                netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit ||
                                netInterface.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx ||
                                netInterface.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT ||
                                netInterface.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet ||
                                netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                                netInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
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

                    icon = GetInterfaceIcon(netInterface);

                    //retrieve the mac address of the interface
                    mac = netInterface.GetPhysicalAddress().ToString();
                    
                    // if we never selected an interface before, enable it for statistics by default.
                    if (isFirstTime)
                    {
                        if (EnableStatisticsForInterface(mac, true, false, false))
                        {
                            shouldSaveSettings = true;
                        }
                    }

                    // if this is the first time we encounter this interface, enable it for statistics by default.
                    if (!AddToKnownInterface(mac, false))
                    {
                        shouldSaveSettings = true;
                        if (EnableStatisticsForInterface(mac, true, false, false))
                        {
                            shouldSaveSettings = true;
                        }
                    }

                    selectedInterfaces.Add(new TrackedInterface(netInterface, mac));
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
                    itm2.Checked            = mac != null ? IsInterfaceEnabled(mac) : false;
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


            if (nUp == 0)
            {
                MainForm.connectionStatus = TrayIconManager.eState.disconnected;
            }
            else
            {
                MainForm.connectionStatus = TrayIconManager.eState.up;
            }

            if (shouldSaveSettings)
            {
                Settings.Default.Save();
                RefreshEnabledInterfacesCache();
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
                    trayIconManager.UpdateAutoPingIcon(reply);

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
                    if (nbv4 + nbv6 != nbNIC && !_nicMenuOpen)
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

                    if (connectionStatus == TrayIconManager.eState.disconnected)
                    {
                        trayIconManager.SetActivityIcon(trayIconManager.iconDisconnected);
                        rawSpeedReception            = 0;
                        rawSpeedEmission             = 0;
                        goto skip;
                    }
                    if (connectionStatus == TrayIconManager.eState.limited)
                    {
                        trayIconManager.SetActivityIcon(trayIconManager.iconLimited);
                        rawSpeedReception            = 0;
                        rawSpeedEmission             = 0;
                        goto skip;
                    }

                    lock (selectedInterfaces)
                    {
                        foreach (TrackedInterface tracked in selectedInterfaces)
                        {
                            if (!IsInterfaceEnabled(tracked.MacAddress))
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
                            int dl = TrayIconManager.GetSpeedLevel(rawSpeedReception, trayIconManager.bandwidthDownloadLvl1, trayIconManager.bandwidthDownloadLvl2, trayIconManager.bandwidthDownloadLvl3, trayIconManager.bandwidthDownloadLvl4);
                            int ul = TrayIconManager.GetSpeedLevel(rawSpeedEmission,  trayIconManager.bandwidthUploadLvl1,   trayIconManager.bandwidthUploadLvl2,  trayIconManager.bandwidthUploadLvl3,  trayIconManager.bandwidthUploadLvl4);
                            trayIconManager.SetActivityIcon(trayIconManager.iconsActive[dl, ul]);
                        }
                        else
                        {
                            trayIconManager.SetActivityIcon(trayIconManager.iconsActive[0, 0]);
                        }
                    }
                    else if (hasDownload && !hasUpload)
                    {
                        nCounter = 0;
                        int dl = customBandwidth ? TrayIconManager.GetSpeedLevel(rawSpeedReception, trayIconManager.bandwidthDownloadLvl1, trayIconManager.bandwidthDownloadLvl2, trayIconManager.bandwidthDownloadLvl3, trayIconManager.bandwidthDownloadLvl4) : 0;
                        trayIconManager.SetActivityIcon(trayIconManager.iconsReceive[dl]);
                    }
                    else if (!hasDownload && hasUpload)
                    {
                        nCounter = 0;
                        int ul = customBandwidth ? TrayIconManager.GetSpeedLevel(rawSpeedEmission, trayIconManager.bandwidthUploadLvl1, trayIconManager.bandwidthUploadLvl2, trayIconManager.bandwidthUploadLvl3, trayIconManager.bandwidthUploadLvl4) : 0;
                        trayIconManager.SetActivityIcon(trayIconManager.iconsSend[ul]);
                    }
                    else
                    {
                        nCounter++; //the counter adds a small persistance effect
                    }

                    if (nCounter == 5)
                    {
                        nCounter                     = 0;
                        trayIconManager.SetActivityIcon(trayIconManager.iconInactive);
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

        internal static double computeSpeed(long rawSpeed, ref string speedUnit)
        {
            return computeSpeed(rawSpeed, ref speedUnit, 1);
        }

        internal static double computeSpeed(long rawSpeed, ref string speedUnit, int typeunit)
        {
            double res = 0;
            switch (typeunit)
            {
                case 1:
                    if (rawSpeed >= 1073741824) //1073741824 = 2 ^ 30
                    {
                        speedUnit = TrayIconManager.SpeedUnitsByte.GiB;
                        res = Math.Round(rawSpeed / (double)1073741824, 2);
                    }
                    else if (rawSpeed >= 1048576) //1048576 = 2 ^ 20
                    {
                        speedUnit = TrayIconManager.SpeedUnitsByte.MiB;
                        res = Math.Round(rawSpeed / (double)1048576, 2);
                    }
                    else if (rawSpeed >= 1024) //1024 = 2 ^ 10
                    {
                        speedUnit = TrayIconManager.SpeedUnitsByte.KiB;
                        res = Math.Round(rawSpeed / (double)1024, 2);
                    }
                    else
                    {
                        speedUnit = TrayIconManager.SpeedUnitsByte.Bytes;
                        res = rawSpeed;
                    }
                    break;

                case 2:
                    if (rawSpeed >= 1000000000)
                    {
                        speedUnit = TrayIconManager.SpeedUnitsBit.Gbit;
                        res = Math.Round(rawSpeed / (double)1000000000, 2);
                    }
                    else if (rawSpeed >= 1000000)
                    {
                        speedUnit = TrayIconManager.SpeedUnitsBit.Mbit;
                        res = Math.Round(rawSpeed / (double)1000000, 2);
                    }
                    else if (rawSpeed >= 1000)
                    {
                        speedUnit = TrayIconManager.SpeedUnitsBit.Kbit;
                        res = Math.Round(rawSpeed / (double)1000, 2);
                    }
                    else
                    {
                        speedUnit = TrayIconManager.SpeedUnitsBit.bit;
                        res = rawSpeed;
                    }
                    break;
                default:
                    goto case 1;
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
            fBal.EnsurePreferredLocation();
            if (!preload)
            {
               fBal.Show();
            }
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
