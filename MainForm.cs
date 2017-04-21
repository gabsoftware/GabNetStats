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

namespace GabNetStats
{
    public partial class MainForm : Form
    {
        private Thread hNetStatThread2   = null;
        private Thread hNICRefreshThread = null;

        private bool bSetIconContinue    = true;
        private bool bWorkContinue       = true;
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

        static int nDuration   = 50;
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


        static Queue<long> queueReception = new Queue<long>(avgSpeedNbItems);
        static Queue<long> queueEmission  = new Queue<long>(avgSpeedNbItems);

        static NetworkInterface[] interfaces         = NetworkInterface.GetAllNetworkInterfaces();
        static ArrayList          selectedInterfaces = new ArrayList();
        static IPGlobalProperties properties;
        static IPGlobalStatistics ipstat;

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

        static frmBalloon fBal;

        public MainForm()
        {
            InitializeComponent();
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
            // fill empty data
            while (queueReception.Count < avgSpeedNbItems)
            {
                queueReception.Enqueue(0);
            }
            while (queueEmission.Count < avgSpeedNbItems)
            {
                queueEmission.Enqueue(0);
            }

            nDuration = Settings.Default.BlinkDuration;

            if (Settings.Default.BandwidthUnit == 0)
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
            bandwidthDownloadLvl4 = (bandwidthDownloadLvl5 * 4) / 5;
            bandwidthDownloadLvl3 = (bandwidthDownloadLvl5 * 3) / 5;
            bandwidthDownloadLvl2 = (bandwidthDownloadLvl5 * 2) / 5;
            bandwidthDownloadLvl1 = (bandwidthDownloadLvl5 * 1) / 5;
            bandwidthUploadLvl4   = (bandwidthUploadLvl5   * 4) / 5;
            bandwidthUploadLvl3   = (bandwidthUploadLvl5   * 3) / 5;
            bandwidthUploadLvl2   = (bandwidthUploadLvl5   * 2) / 5;
            bandwidthUploadLvl1   = (bandwidthUploadLvl5   * 1) / 5;

            customBandwidth = Settings.Default.BandwidthVisualsCustom == true;

            //registers the networkchange event handlers
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            NetworkChange.NetworkAddressChanged      += new NetworkAddressChangedEventHandler     (NetworkChange_NetworkAddressChanged     );

            hNetStatThread2              = new Thread(new ThreadStart(this.NetStatThread));
            hNetStatThread2.IsBackground = true;
            hNetStatThread2.Name         = "hNetStatThread2";
            hNetStatThread2.Start();

            //used in case more than one interface is UP and this number is changing.
            hNICRefreshThread              = new Thread(new ThreadStart(this.NICRefreshThread));
            hNICRefreshThread.IsBackground = true;
            hNICRefreshThread.Name         = "hNICRefreshThread";
            hNICRefreshThread.Start();

            foreach (System.Diagnostics.ProcessThread t in System.Diagnostics.Process.GetCurrentProcess().Threads)
            {
                t.PriorityLevel = ThreadPriorityLevel.Lowest; //we don't want our application to eat too much processor time !
            }
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.Idle; //same here !

            this.Hide();

            showBalloon(true);
        }

        private void OnAbout(object sender, EventArgs e)
        {
            AboutForm formAbout = new AboutForm();
            formAbout.ShowDialog();
        }

        private void OnSettings(object sender, EventArgs e)
        {
            SettingsForm formSettings = new SettingsForm();
            formSettings.ShowDialog();
            nDuration = Settings.Default.BlinkDuration;

            if (Settings.Default.BandwidthUnit == 0)
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
            bandwidthDownloadLvl4 = (bandwidthDownloadLvl5 * 4) / 5;
            bandwidthDownloadLvl3 = (bandwidthDownloadLvl5 * 3) / 5;
            bandwidthDownloadLvl2 = (bandwidthDownloadLvl5 * 2) / 5;
            bandwidthDownloadLvl1 = (bandwidthDownloadLvl5 * 1) / 5;
            bandwidthUploadLvl4   = (bandwidthUploadLvl5   * 4) / 5;
            bandwidthUploadLvl3   = (bandwidthUploadLvl5   * 3) / 5;
            bandwidthUploadLvl2   = (bandwidthUploadLvl5   * 2) / 5;
            bandwidthUploadLvl1   = (bandwidthUploadLvl5   * 1) / 5;

            customBandwidth = Settings.Default.BandwidthVisualsCustom == true;
        }

        private void OnExit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bWorkContinue = false;
            Thread.Sleep(2 * nDuration);

            if (hNetStatThread2 != null)
            {
                if (hNetStatThread2.IsAlive)
                {
                    hNetStatThread2.Abort();
                }
            }

            if (hNICRefreshThread != null)
            {
                if (hNICRefreshThread.IsAlive)
                {
                    hNICRefreshThread.Abort();
                }
            }
        }

        /// <summary>
        /// Add the specified MAC address to the list of the known MAC addresses
        /// </summary>
        /// <param name="mac">A MAC address</param>
        /// <returns>True if the MAC address was already known, false otherwise</returns>
        private bool AddToKnownInterface(string mac)
        {
            bool contains = Settings.Default.KnownInterfaceMACList.Contains(mac);
            if (!contains)
            {
                Settings.Default.KnownInterfaceMACList += (Settings.Default.KnownInterfaceMACList == string.Empty ? String.Empty : ";") + mac;
                Settings.Default.Save();
            }
            return contains;
        }

        /// <summary>
        /// Enable the statistics for the interface designed by the specified MAC address
        /// </summary>
        /// <param name="mac">A MAC address</param>
        /// <param name="enable">If set to True (default value), the interface will be enabled for statistics</param>
        private void EnableStatisticsForInterface(string mac, bool enable = true)
        {
            string tmp = Settings.Default.EnabledInterfaceMACList;
            bool contains = tmp.Contains(mac);
            bool modified = false;
            bool empty = false;

            if (mac == String.Empty)
            {
                return;
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
                    tmp      = tmp.Replace(";" + mac, ""); //mac is second to last value
                    tmp      = tmp.Replace(mac + ";", ""); //mac is first value
                    tmp      = tmp.Replace(mac      , ""); //mac was the only value
                    modified = true;
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
                Settings.Default.Save();

                if (tmp == String.Empty)
                {
                    MessageBox.Show(Res.str_WarningNoInterfaceSelected, Res.str_WarningNoInterfaceSelectedCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
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
            catch (Exception)
            {
                MessageBox.Show(Res.str_ErrorCrash);
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
            catch (Exception)
            {
                MessageBox.Show(Res.str_ErrorCrash);
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
            catch (Exception)
            {
                MessageBox.Show(Res.str_ErrorCrash);
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
            catch (Exception)
            {
                MessageBox.Show(Res.str_ErrorCrash);
            }
        }

        private void manageWirelessNetworksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens the Manage wireless networks applet
            try
            {
                Process.Start("Shell:::{1FA9085F-25A2-489B-85D4-86326EEDCD87}");
            }
            catch (Exception)
            {
                MessageBox.Show(Res.str_ErrorCrash);
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
            catch (Exception)
            {
                MessageBox.Show(Res.str_ErrorCrash);
            }
        }

        private void networkdomainworkgroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens the Domain/workgroup applet
            try
            {
                Process.Start("Shell:::{208D2C60-3AEA-1069-A2D7-08002B30309D}");
            }
            catch (Exception)
            {
                MessageBox.Show(Res.str_ErrorCrash);
            }
            
        }

        private void networkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens the network applet
            try
            {
                Process.Start("Shell:::{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}");
            }
            catch (Exception)
            {
                MessageBox.Show(Res.str_ErrorCrash);
            }            
        }

        private void networkMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens the Domain/workgroup applet
            try
            {
                Process.Start("Shell:::{E7DE9B1A-7533-4556-9484-B26FB486475E}");
            }
            catch (Exception)
            {
                MessageBox.Show(Res.str_NotAvailableWinver);
            }
        }

        private void PopulateNICs(ToolStripMenuItem parent)
        {
            int nUp = 0;
            string ip;
            string unit = string.Empty;
            double speed;
            Bitmap icon = Properties.Resources.netshell_1612_16x16.ToBitmap();
            bool valid = false;
            bool isFirstTime = Settings.Default.EnabledInterfaceMACList == "TOSET";
            string mac = string.Empty;
            ToolStripMenuItem itm;
            ToolStripMenuItem itm2;

            //we do some little cleaning...
            ClearParentThreadSafe(parent);

            //We want to avoid to run the following lines often because it temporarily raises the CPU usage to 2%, which is unacceptable for a background app.
            //This is why the procedure PopulateNICs is not periodically launched in a thread but by events and testing if the nb of adapters changes.

            Monitor.Enter(selectedInterfaces);

            interfaces = NetworkInterface.GetAllNetworkInterfaces();
            selectedInterfaces.Clear();

            
            foreach (NetworkInterface netInterface in interfaces)
            {

                //test if the interface is active
                if (netInterface.OperationalStatus != OperationalStatus.Up)
                {
                    ip = Res.str_NotConnected;
                }
                else
                {
                    if (netInterface.GetIPProperties().UnicastAddresses.Count > 0)
                    {
                        if (netInterface.NetworkInterfaceType.ToString().ToLower(CultureInfo.InvariantCulture).Contains("ethernet") ||
                            netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                        {
                            nUp++;
                        }
                        ip = netInterface.GetIPProperties().UnicastAddresses[netInterface.GetIPProperties().UnicastAddresses.Count - 1].Address.ToString();
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
                        EnableStatisticsForInterface(mac, true);
                    }

                    // if this is the first time we encounter this interface, enable it for statistics by default.
                    if (!AddToKnownInterface(mac))
                    {
                        EnableStatisticsForInterface(mac, true);
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
                    itm2.Checked            = Settings.Default.EnabledInterfaceMACList.Contains(mac);
                    itm2.CheckStateChanged += new EventHandler(OnAdapterCheckStateChanged);
                    itm2.Tag                = netInterface;
                    
                    //we add the subitem to the item
                    itm.DropDownItems.Add(itm2);

                    //add item in parent menu item
                    AddItemThreadSafe(itm, parent);
                }
            }
            
            Array.Clear(interfaces, 0, interfaces.Length);

            Monitor.Exit(selectedInterfaces);

            if (nUp == 0)
            {
                MainForm.connectionStatus = eState.disconnected;
            }
            else
            {
                MainForm.connectionStatus = eState.up;
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
                parent.DropDownItems.Clear();
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
                parent.DropDownItems.Add(itm);
            }
        }


        private void NICRefreshThread()
        {
            int nb;

            try
            {

                while (bWorkContinue)
                {
                    //we get some quick statistics about the number of network interfaces...
                    properties = IPGlobalProperties.GetIPGlobalProperties();
                    ipstat     = properties.GetIPv4GlobalStatistics();
                    nb         = ipstat.NumberOfInterfaces;

                    //if number changed since last time AND we are not displaying the context menu then
                    if (nb != nbNIC && this.NetworkAdaptersToolStripMenuItem.Visible == false)
                    {
                        nbNIC = nb;
                        this.PopulateNICs(this.NetworkAdaptersToolStripMenuItem);
                    }

                    Thread.Sleep(nNICRefresh);
                }

            }
            catch (Exception ex)
            {

                if (ex.GetType() != typeof(ThreadAbortException))
                {
                    MessageBox.Show(
                        Res.str_ErrorCrash +
                        "\n\n" + "Thread : " +
                        Thread.CurrentThread.Name +
                        "\n\n" +
                        ex.ToString(), "GabNetStats", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
                Application.Restart();
            }

        }

        private void NetStatThread()
        {
            int nCounter            = 0;
            long bytesReceived      = 0;
            long bytesSent          = 0;
            long oldbytesReceived   = 0;
            long oldbytesSent       = 0;
            long lAvgSpeedReception = 0;
            long lAvgSpeedEmission  = 0;
            long rawSpeedReception  = 0;
            long rawSpeedEmission   = 0;
            long[] tRawSpeed;

            IPv4InterfaceStatistics ipv4stats = null;

            try
            {
                while (bWorkContinue)
                {
                    if (bSetIconContinue)
                    {
                        bSetIconContinue = false;
                        bytesReceived    = 0;
                        bytesSent        = 0;

                        if (connectionStatus == eState.disconnected)
                        {
                            this.notifyIconActivity.Icon = iconDisconnected;
                            rawSpeedReception            = 0;
                            rawSpeedEmission             = 0;
                            goto skip;
                        }
                        if (connectionStatus == eState.limited)
                        {
                            this.notifyIconActivity.Icon = iconLimited;
                            rawSpeedReception            = 0;
                            rawSpeedEmission             = 0;
                            goto skip;
                        }

                        Monitor.Enter(selectedInterfaces);
                        
                        foreach (NetworkInterface netInterface in selectedInterfaces)
                        {
                            if (Settings.Default.EnabledInterfaceMACList.Contains(netInterface.GetPhysicalAddress().ToString()))
                            {
                                ipv4stats      = netInterface.GetIPv4Statistics();
                                bytesReceived += ipv4stats.BytesReceived;
                                bytesSent     += ipv4stats.BytesSent;
                            }
                        }

                        Monitor.Exit(selectedInterfaces);

                        if (bytesReceived != oldbytesReceived && bytesSent != oldbytesSent)
                        {
                            rawSpeedReception = Math.Abs((1000 / nDuration) * (bytesReceived - oldbytesReceived));
                            rawSpeedEmission  = Math.Abs((1000 / nDuration) * (bytesSent - oldbytesSent));
                            oldbytesReceived  = bytesReceived;
                            oldbytesSent      = bytesSent;
                            nCounter          = 0;

                            if (customBandwidth)
                            {
                                if (rawSpeedReception >= bandwidthDownloadLvl4 && rawSpeedEmission >= bandwidthUploadLvl4)
                                {
                                    this.notifyIconActivity.Icon = iconActive_red_red;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl4 && rawSpeedEmission >= bandwidthUploadLvl3)
                                {
                                    this.notifyIconActivity.Icon = iconActive_orange_red;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl4 && rawSpeedEmission >= bandwidthUploadLvl2)
                                {
                                    this.notifyIconActivity.Icon = iconActive_yellow_red;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl4 && rawSpeedEmission >= bandwidthUploadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconActive_green_red;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl4 && rawSpeedEmission < bandwidthUploadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconActive_blue_red;
                                }

                                else if (rawSpeedReception >= bandwidthDownloadLvl3 && rawSpeedEmission >= bandwidthUploadLvl4)
                                {
                                    this.notifyIconActivity.Icon = iconActive_red_orange;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl3 && rawSpeedEmission >= bandwidthUploadLvl3)
                                {
                                    this.notifyIconActivity.Icon = iconActive_orange_orange;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl3 && rawSpeedEmission >= bandwidthUploadLvl2)
                                {
                                    this.notifyIconActivity.Icon = iconActive_yellow_orange;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl3 && rawSpeedEmission >= bandwidthUploadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconActive_green_orange;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl3 && rawSpeedEmission < bandwidthUploadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconActive_blue_orange;
                                }

                                else if (rawSpeedReception >= bandwidthDownloadLvl2 && rawSpeedEmission >= bandwidthUploadLvl4)
                                {
                                    this.notifyIconActivity.Icon = iconActive_red_yellow;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl2 && rawSpeedEmission >= bandwidthUploadLvl3)
                                {
                                    this.notifyIconActivity.Icon = iconActive_orange_yellow;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl2 && rawSpeedEmission >= bandwidthUploadLvl2)
                                {
                                    this.notifyIconActivity.Icon = iconActive_yellow_yellow;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl2 && rawSpeedEmission >= bandwidthUploadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconActive_green_yellow;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl2 && rawSpeedEmission < bandwidthUploadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconActive_blue_yellow;
                                }

                                else if (rawSpeedReception >= bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl4)
                                {
                                    this.notifyIconActivity.Icon = iconActive_red_green;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl3)
                                {
                                    this.notifyIconActivity.Icon = iconActive_orange_green;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl2)
                                {
                                    this.notifyIconActivity.Icon = iconActive_yellow_green;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconActive_green_green;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl1 && rawSpeedEmission < bandwidthUploadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconActive_blue_green;
                                }

                                else if (rawSpeedReception < bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl4)
                                {
                                    this.notifyIconActivity.Icon = iconActive_red_blue;
                                }
                                else if (rawSpeedReception < bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl3)
                                {
                                    this.notifyIconActivity.Icon = iconActive_orange_blue;
                                }
                                else if (rawSpeedReception < bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl2)
                                {
                                    this.notifyIconActivity.Icon = iconActive_yellow_blue;
                                }
                                else if (rawSpeedReception < bandwidthDownloadLvl1 && rawSpeedEmission >= bandwidthUploadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconActive_green_blue;
                                }
                                else if (rawSpeedReception < bandwidthDownloadLvl1 && rawSpeedEmission < bandwidthUploadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconActive_blue_blue;
                                }
                            }
                            else
                            {
                                this.notifyIconActivity.Icon = iconActive_blue_blue;
                            }

                        }
                        else if (bytesReceived != oldbytesReceived && bytesSent == oldbytesSent)
                        {

                            rawSpeedReception = Math.Abs((1000 / nDuration) * (bytesReceived - oldbytesReceived));
                            rawSpeedEmission  = 0;
                            oldbytesReceived  = bytesReceived;
                            nCounter          = 0;

                            if (customBandwidth)
                            {
                                if (rawSpeedReception >= bandwidthDownloadLvl4)
                                {
                                    this.notifyIconActivity.Icon = iconReceive_red;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl3)
                                {
                                    this.notifyIconActivity.Icon = iconReceive_orange;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl2)
                                {
                                    this.notifyIconActivity.Icon = iconReceive_yellow;
                                }
                                else if (rawSpeedReception >= bandwidthDownloadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconReceive_green;
                                }
                                else if (rawSpeedReception < bandwidthDownloadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconReceive_blue;
                                }
                            }
                            else
                            {
                                this.notifyIconActivity.Icon = iconReceive_blue;
                            }
                        }
                        else if (bytesReceived == oldbytesReceived && bytesSent != oldbytesSent)
                        {

                            rawSpeedReception = 0;
                            rawSpeedEmission  = Math.Abs((1000 / nDuration) * (bytesSent - oldbytesSent));
                            oldbytesSent      = bytesSent;
                            nCounter          = 0;

                            if (customBandwidth)
                            {
                                if (rawSpeedEmission >= bandwidthUploadLvl4)
                                {
                                    this.notifyIconActivity.Icon = iconSend_red;
                                }
                                else if (rawSpeedEmission >= bandwidthUploadLvl3)
                                {
                                    this.notifyIconActivity.Icon = iconSend_orange;
                                }
                                else if (rawSpeedEmission >= bandwidthUploadLvl2)
                                {
                                    this.notifyIconActivity.Icon = iconSend_yellow;
                                }
                                else if (rawSpeedEmission >= bandwidthUploadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconSend_green;
                                }
                                else if (rawSpeedEmission < bandwidthUploadLvl1)
                                {
                                    this.notifyIconActivity.Icon = iconSend_blue;
                                }
                            }
                            else
                            {
                                this.notifyIconActivity.Icon = iconSend_blue;
                            }
                        }
                        else
                        {
                            nCounter++; //the counter adds a small persistance effect
                        }

                        if (nCounter == 5)
                        {
                            nCounter                     = 0;
                            this.notifyIconActivity.Icon = iconInactive;
                            rawSpeedReception            = 0;
                            rawSpeedEmission             = 0;
                        }

                    skip:

                        Monitor.Enter(queueReception);
                        Monitor.Enter(queueEmission);

                        //computing speed units
                        queueReception.Enqueue(rawSpeedReception);
                        queueEmission.Enqueue(rawSpeedEmission);
                        queueReception.Dequeue();
                        queueEmission.Dequeue();

                        tRawSpeed = queueReception.ToArray();
                        lAvgSpeedReception = 0;
                        for (int i = 0; i < queueReception.Count; i++)
                        {
                            lAvgSpeedReception += tRawSpeed[i];
                        }
                        Array.Clear(tRawSpeed, 0, tRawSpeed.Length);
                        lAvgSpeedReception = lAvgSpeedReception / queueReception.Count;

                        tRawSpeed = queueEmission.ToArray();
                        lAvgSpeedEmission = 0;
                        for (int i = 0; i < queueEmission.Count; i++)
                        {
                            lAvgSpeedEmission += tRawSpeed[i];
                        }
                        Array.Clear(tRawSpeed, 0, tRawSpeed.Length);
                        lAvgSpeedEmission = lAvgSpeedEmission / queueEmission.Count;

                        Monitor.Exit(queueReception);
                        Monitor.Exit(queueEmission);

                        frmBalloon.UpdateInfos(rawSpeedReception, rawSpeedEmission, lAvgSpeedReception, lAvgSpeedEmission, bytesReceived, bytesSent);

                        bSetIconContinue = true;
                    }

                    Thread.Sleep(nDuration);
                }

            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ThreadAbortException))
                {
                    MessageBox.Show(
                        Res.str_ErrorCrash +
                        "\n\n" + "Thread : " +
                        Thread.CurrentThread.Name +
                        "\n\n" +
                        ex.ToString(), "GabNetStats", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
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

    }
}