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
    public partial class FormMain : Form
    {
        //
        //  Constants
        //
        private const int    WM_QUERYENDSESSION                  = 0x0011;
        private const int    WM_ENDSESSION                       = 0x0016;
        private const string EXPLORER_EXE                        = "explorer.exe";
        private const string CLSID_MY_COMPUTER                   = "20D04FE0-3AEA-1069-A2D8-08002B30309D";
        private const string CLSID_CONTROL_PANEL                 = "21EC2020-3AEA-1069-A2DD-08002B30309D";
        private const string CLSID_NETWORK_CONNECTIONS           = "7007ACC7-3202-11D1-AAD2-00805FC1270E";
        private const string CLSID_CONTROL_PANEL_ALL_ITEMS       = "26EE0668-A00A-44D7-9371-BEB064C98683";
        private const string CLSID_NETWORK_AND_SHARING_CENTER    = "8E908FC9-BECC-40F6-915B-F4CA0E70D03D";
        private const string CLSID_WINDOWS_FIREWALL              = "4026492F-2F69-46B8-B9BF-5654FC07E423";
        private const string CLSID_HOMEGROUP                     = "67CA7650-96E6-4FDD-BB43-A8E774F73A57";
        private const string CLSID_MANAGE_WIRELESS_NETWORKS      = "1FA9085F-25A2-489B-85D4-86326EEDCD87";
        private const string CLSID_NETWORK_PLACES                = "208D2C60-3AEA-1069-A2D7-08002B30309D";
        private const string CLSID_NETWORK                       = "F02C1A0D-BE21-4350-88B0-7367FC96EF3C";
        private const string CLSID_NETWORK_MAP                   = "E7DE9B1A-7533-4556-9484-B26FB486475E";

        private TrayIconManager trayIconManager;
        internal NetworkInterfaceManager nicManager;
        private NetworkStatsWorker statsWorker;

        static FormStatsOverlay fBal;

        private volatile bool _nicMenuOpen;

        public FormMain()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
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

            statsWorker.InitializeSpeedSamples();
            statsWorker.ApplySettings();

            //registers the networkchange event handlers
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            NetworkChange.NetworkAddressChanged      += new NetworkAddressChangedEventHandler     (NetworkChange_NetworkAddressChanged     );

            contextMenuTray.Opening += (s, ev) => _nicMenuOpen = true;
            contextMenuTray.Closing += (s, ev) => _nicMenuOpen = false;

            statsWorker.Start();

            this.notifyIconPing.Visible = Settings.Default.AutoPingEnabled;

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
            FormAbout formAbout = new FormAbout();
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
            FormSettings formSettings = new FormSettings();
            try
            {
                formSettings.ShowDialog();
            }
            catch (InvalidOperationException) { }

            statsWorker.ApplySettings();

            FormStatsOverlay fb = (FormStatsOverlay)Application.OpenForms["FormStatsOverlay"];
            if (fb != null)
            {
                fb.BallonTimer.Interval = statsWorker.nDuration;
            }

            this.notifyIconPing.Visible = Settings.Default.AutoPingEnabled;
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
                    EXPLORER_EXE,
                    "/N,::{"  + CLSID_MY_COMPUTER         + "}\\::{"  + CLSID_CONTROL_PANEL        + "}\\::{"  + CLSID_NETWORK_CONNECTIONS + "}\\::" + nic.Id
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
                    EXPLORER_EXE,
                    "/N,::{"  + CLSID_MY_COMPUTER         + "}\\::{"  + CLSID_CONTROL_PANEL        + "}\\::{"  + CLSID_NETWORK_CONNECTIONS + "}"
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
                    EXPLORER_EXE,
                    "/N,::{"  + CLSID_CONTROL_PANEL_ALL_ITEMS  + "}\\0\\::{" + CLSID_NETWORK_AND_SHARING_CENTER + "}"
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
                    EXPLORER_EXE,
                    "/N,::{"  + CLSID_CONTROL_PANEL_ALL_ITEMS  + "}\\0\\::{" + CLSID_WINDOWS_FIREWALL + "}"
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
                Process.Start("Shell:::{"  + CLSID_MANAGE_WIRELESS_NETWORKS  + "}");
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
                    EXPLORER_EXE,
                    "/N,::{"  + CLSID_CONTROL_PANEL_ALL_ITEMS  + "}\\0\\::{" + CLSID_HOMEGROUP + "}"
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
                Process.Start("Shell:::{"  + CLSID_NETWORK_PLACES  + "}");
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
                Process.Start("Shell:::{"  + CLSID_NETWORK  + "}");
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
                Process.Start("Shell:::{"  + CLSID_NETWORK_MAP  + "}");
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
            //We want to avoid to run the following lines often because it temporarily raises the CPU usage to 2%, which is unacceptable for a background app.
            //This is why the procedure PopulateNICs is not periodically launched in a thread but by events and testing if the nb of adapters changes.

            //we do some little cleaning...
            ClearParentThreadSafe(parent);

            List<NetworkInterfaceManager.NicDisplayInfo> nics = nicManager.RefreshInterfaces();

            foreach (NetworkInterfaceManager.NicDisplayInfo info in nics)
            {
                string unit  = string.Empty;
                double speed = SpeedUtils.computeSpeed(info.Interface.Speed, ref unit, 2);

                //we generate the item related to the network interface
                ToolStripMenuItem itm = new ToolStripMenuItem(
                        info.Interface.Name +
                        " [" + info.Interface.Description + "]" +
                        " : " + speed + " " + unit + "/s" +
                        ", " + info.Ip
                        , info.Icon
                        , OnAdapterClick
                        , "itm_" + info.Interface.Id);
                itm.Tag = info.Interface;

                //we generate its subitem
                ToolStripMenuItem itm2   = new ToolStripMenuItem(Res.str_IncludeInStatistics);
                itm2.Name               = "itm_include_" + info.Interface.Id;
                itm2.CheckOnClick       = true;
                itm2.Checked            = info.Mac != null ? NetworkInterfaceManager.IsInterfaceEnabled(info.Mac) : false;
                itm2.CheckStateChanged += new EventHandler(OnAdapterCheckStateChanged);
                itm2.Tag                = info.Interface;

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
                fBal = new FormStatsOverlay();
            }
            fBal.EnsurePreferredLocation();
            if (!preload)
            {
               fBal.Show();
            }
        }

        private void advancedStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormStatsOverlay.frmAdv == null || !FormStatsOverlay.frmAdv.Created)
            {
                FormStatsOverlay.frmAdv = new FormNetworkDetails();
            }
            FormStatsOverlay.frmAdv.Show();
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
