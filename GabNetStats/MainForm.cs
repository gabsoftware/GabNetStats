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
        private TrayIconManager trayIconManager;
        internal NetworkInterfaceManager nicManager;
        private NetworkStatsWorker statsWorker;

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
                statsWorker?.CancelWorkers();
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
            nicManager      = new NetworkInterfaceManager();
            statsWorker     = new NetworkStatsWorker(
                trayIconManager,
                nicManager,
                this.notifyIconPing,
                () => this.PopulateNICs(this.NetworkAdaptersToolStripMenuItem),
                () => _nicMenuOpen);

            // apply icon set if necessary
            if( Settings.Default.IconSet != "xp" )
            {
                trayIconManager.applyIconSet();
            }

            statsWorker.InitializeSpeedSamples();

            statsWorker.RefreshBlinkDurationFromSettings();

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

            statsWorker.customBandwidth = Settings.Default.BandwidthVisualsCustom == true;
            NetworkInterfaceManager.RefreshEnabledInterfacesCache();

            //registers the networkchange event handlers
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            NetworkChange.NetworkAddressChanged      += new NetworkAddressChangedEventHandler     (NetworkChange_NetworkAddressChanged     );

            contextMenuTray.Opening += (s, ev) => _nicMenuOpen = true;
            contextMenuTray.Closing += (s, ev) => _nicMenuOpen = false;

            statsWorker.Start();

            this.notifyIconPing.Visible = Settings.Default.AutoPingEnabled;
            if (Settings.Default.AutoPingEnabled)
            {
                statsWorker.StartAutoPingThread();
            }
            else
            {
                statsWorker.StopAutoPingThread();
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
            
            statsWorker.RefreshBlinkDurationFromSettings();

            frmBalloon fb = (frmBalloon)Application.OpenForms["frmBalloon"];
            if (fb != null)
            {
                fb.BallonTimer.Interval = statsWorker.nDuration;
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

            statsWorker.customBandwidth = Settings.Default.BandwidthVisualsCustom == true;
            NetworkInterfaceManager.RefreshEnabledInterfacesCache();

            this.notifyIconPing.Visible = Settings.Default.AutoPingEnabled;
            if (!Settings.Default.AutoPingEnabled)
            {
                statsWorker.StopAutoPingThread();
            }
            else
            {
                statsWorker.StartAutoPingThread();
            }

            trayIconManager.applyIconSet();
        }

        internal IReadOnlyList<NetworkInterfaceManager.TrackedInterface> GetDisplayableInterfacesSnapshot()
        {
            return nicManager.GetDisplayableInterfacesSnapshot();
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

            statsWorker.Shutdown();
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
            NetworkInterfaceManager.EnableStatisticsForInterface(nic.GetPhysicalAddress().ToString(), itm.Checked);
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
            bool changed = NetworkInterfaceManager.EnableStatisticsForInterface(mac, enable);
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

            lock (nicManager.selectedInterfaces)
            {
                nicManager.selectedInterfaces.Clear();

                foreach (NetworkInterface netInterface in nicSnapshot)
                {
                    if (!NetworkInterfaceManager.ShouldDisplayInterface(netInterface))
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

                    icon = NetworkInterfaceManager.GetInterfaceIcon(netInterface);

                    //retrieve the mac address of the interface
                    mac = netInterface.GetPhysicalAddress().ToString();

                    // if we never selected an interface before, enable it for statistics by default.
                    if (isFirstTime)
                    {
                        if (NetworkInterfaceManager.EnableStatisticsForInterface(mac, true, false, false))
                        {
                            shouldSaveSettings = true;
                        }
                    }

                    // if this is the first time we encounter this interface, enable it for statistics by default.
                    if (!NetworkInterfaceManager.AddToKnownInterface(mac, false))
                    {
                        shouldSaveSettings = true;
                        if (NetworkInterfaceManager.EnableStatisticsForInterface(mac, true, false, false))
                        {
                            shouldSaveSettings = true;
                        }
                    }

                    nicManager.selectedInterfaces.Add(new NetworkInterfaceManager.TrackedInterface(netInterface, mac));
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
                    itm2.Checked            = mac != null ? NetworkInterfaceManager.IsInterfaceEnabled(mac) : false;
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
                NetworkInterfaceManager.connectionStatus = TrayIconManager.eState.disconnected;
            }
            else
            {
                NetworkInterfaceManager.connectionStatus = TrayIconManager.eState.up;
            }

            if (shouldSaveSettings)
            {
                Settings.Default.Save();
                NetworkInterfaceManager.RefreshEnabledInterfacesCache();
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
