namespace GabNetStats
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            contextMenuTray = new System.Windows.Forms.ContextMenuStrip(components);
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            advancedStatisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            manageWirelessNetworksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            homeGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            networkMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            networkdomainworkgroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            networkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            FirewallSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            FirewallAllowedAppsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            NetworkSharingCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            NetworkAndInternetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            NetworkConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            NetworkConnectionsAlternateViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            NetworkAdaptersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            notifyIconActivity = new System.Windows.Forms.NotifyIcon(components);
            notifyIconPing = new System.Windows.Forms.NotifyIcon(components);
            contextMenuTray.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuTray
            // 
            resources.ApplyResources(contextMenuTray, "contextMenuTray");
            contextMenuTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { aboutToolStripMenuItem, toolStripSeparator4, settingsToolStripMenuItem, toolStripSeparator3, advancedStatisticsToolStripMenuItem, toolStripSeparator2, manageWirelessNetworksToolStripMenuItem, homeGroupToolStripMenuItem, networkMapToolStripMenuItem, networkdomainworkgroupToolStripMenuItem, networkToolStripMenuItem, FirewallSettingsToolStripMenuItem, FirewallAllowedAppsToolStripMenuItem, NetworkSharingCenterToolStripMenuItem, NetworkAndInternetToolStripMenuItem, NetworkConnectionsToolStripMenuItem, NetworkConnectionsAlternateViewToolStripMenuItem, NetworkAdaptersToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            contextMenuTray.Name = "contextMenuTray";
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(aboutToolStripMenuItem, "aboutToolStripMenuItem");
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Click += OnAbout;
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(toolStripSeparator4, "toolStripSeparator4");
            toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // settingsToolStripMenuItem
            // 
            resources.ApplyResources(settingsToolStripMenuItem, "settingsToolStripMenuItem");
            settingsToolStripMenuItem.Image = Properties.Resources.settings_icon_16x16;
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Click += OnSettings;
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
            toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // advancedStatisticsToolStripMenuItem
            // 
            resources.ApplyResources(advancedStatisticsToolStripMenuItem, "advancedStatisticsToolStripMenuItem");
            advancedStatisticsToolStripMenuItem.Name = "advancedStatisticsToolStripMenuItem";
            advancedStatisticsToolStripMenuItem.Click += advancedStatisticsToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // manageWirelessNetworksToolStripMenuItem
            // 
            resources.ApplyResources(manageWirelessNetworksToolStripMenuItem, "manageWirelessNetworksToolStripMenuItem");
            manageWirelessNetworksToolStripMenuItem.Image = Properties.Resources.wlanpref_101;
            manageWirelessNetworksToolStripMenuItem.Name = "manageWirelessNetworksToolStripMenuItem";
            manageWirelessNetworksToolStripMenuItem.Click += manageWirelessNetworksToolStripMenuItem_Click;
            // 
            // homeGroupToolStripMenuItem
            // 
            resources.ApplyResources(homeGroupToolStripMenuItem, "homeGroupToolStripMenuItem");
            homeGroupToolStripMenuItem.Image = Properties.Resources.imageres_1013;
            homeGroupToolStripMenuItem.Name = "homeGroupToolStripMenuItem";
            homeGroupToolStripMenuItem.Click += homeGroupToolStripMenuItem_Click;
            // 
            // networkMapToolStripMenuItem
            // 
            resources.ApplyResources(networkMapToolStripMenuItem, "networkMapToolStripMenuItem");
            networkMapToolStripMenuItem.Image = Properties.Resources.networkmap_1;
            networkMapToolStripMenuItem.Name = "networkMapToolStripMenuItem";
            networkMapToolStripMenuItem.Click += networkMapToolStripMenuItem_Click;
            // 
            // networkdomainworkgroupToolStripMenuItem
            // 
            resources.ApplyResources(networkdomainworkgroupToolStripMenuItem, "networkdomainworkgroupToolStripMenuItem");
            networkdomainworkgroupToolStripMenuItem.Image = Properties.Resources.shell32_18;
            networkdomainworkgroupToolStripMenuItem.Name = "networkdomainworkgroupToolStripMenuItem";
            networkdomainworkgroupToolStripMenuItem.Click += networkdomainworkgroupToolStripMenuItem_Click;
            // 
            // networkToolStripMenuItem
            // 
            resources.ApplyResources(networkToolStripMenuItem, "networkToolStripMenuItem");
            networkToolStripMenuItem.Image = Properties.Resources.shell32_18;
            networkToolStripMenuItem.Name = "networkToolStripMenuItem";
            networkToolStripMenuItem.Click += networkToolStripMenuItem_Click;
            // 
            // FirewallSettingsToolStripMenuItem
            // 
            resources.ApplyResources(FirewallSettingsToolStripMenuItem, "FirewallSettingsToolStripMenuItem");
            FirewallSettingsToolStripMenuItem.Image = Properties.Resources.FireWallControlPanel_16x16;
            FirewallSettingsToolStripMenuItem.Name = "FirewallSettingsToolStripMenuItem";
            FirewallSettingsToolStripMenuItem.Click += FirewallSettingsToolStripMenuItem_Click;
            // 
            // FirewallAllowedAppsToolStripMenuItem
            // 
            resources.ApplyResources(FirewallAllowedAppsToolStripMenuItem, "FirewallAllowedAppsToolStripMenuItem");
            FirewallAllowedAppsToolStripMenuItem.Image = Properties.Resources.FireWallControlPanel_16x16;
            FirewallAllowedAppsToolStripMenuItem.Name = "FirewallAllowedAppsToolStripMenuItem";
            FirewallAllowedAppsToolStripMenuItem.Click += FirewallAllowedAppsToolStripMenuItem_Click;
            // 
            // NetworkSharingCenterToolStripMenuItem
            // 
            resources.ApplyResources(NetworkSharingCenterToolStripMenuItem, "NetworkSharingCenterToolStripMenuItem");
            NetworkSharingCenterToolStripMenuItem.Image = Properties.Resources.netcenter_16x16;
            NetworkSharingCenterToolStripMenuItem.Name = "NetworkSharingCenterToolStripMenuItem";
            NetworkSharingCenterToolStripMenuItem.Click += NetworkSharingCenterToolStripMenuItem_Click;
            // 
            // NetworkAndInternetToolStripMenuItem
            // 
            resources.ApplyResources(NetworkAndInternetToolStripMenuItem, "NetworkAndInternetToolStripMenuItem");
            NetworkAndInternetToolStripMenuItem.Image = Properties.Resources.shell32_18;
            NetworkAndInternetToolStripMenuItem.Name = "NetworkAndInternetToolStripMenuItem";
            NetworkAndInternetToolStripMenuItem.Click += NetworkAndInternetToolStripMenuItem_Click;
            // 
            // NetworkConnectionsToolStripMenuItem
            // 
            resources.ApplyResources(NetworkConnectionsToolStripMenuItem, "NetworkConnectionsToolStripMenuItem");
            NetworkConnectionsToolStripMenuItem.Image = Properties.Resources.netshell_16x16;
            NetworkConnectionsToolStripMenuItem.Name = "NetworkConnectionsToolStripMenuItem";
            NetworkConnectionsToolStripMenuItem.Click += NetworkConnectionsToolStripMenuItem_Click;
            // 
            // NetworkConnectionsAlternateViewToolStripMenuItem
            // 
            resources.ApplyResources(NetworkConnectionsAlternateViewToolStripMenuItem, "NetworkConnectionsAlternateViewToolStripMenuItem");
            NetworkConnectionsAlternateViewToolStripMenuItem.Image = Properties.Resources.netshell_16x16;
            NetworkConnectionsAlternateViewToolStripMenuItem.Name = "NetworkConnectionsAlternateViewToolStripMenuItem";
            NetworkConnectionsAlternateViewToolStripMenuItem.Click += NetworkConnectionsAlternateViewToolStripMenuItem_Click;
            // 
            // NetworkAdaptersToolStripMenuItem
            // 
            resources.ApplyResources(NetworkAdaptersToolStripMenuItem, "NetworkAdaptersToolStripMenuItem");
            NetworkAdaptersToolStripMenuItem.Image = Properties.Resources.pnidui_2407_16x16;
            NetworkAdaptersToolStripMenuItem.Name = "NetworkAdaptersToolStripMenuItem";
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // exitToolStripMenuItem
            // 
            resources.ApplyResources(exitToolStripMenuItem, "exitToolStripMenuItem");
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Click += OnExit;
            // 
            // notifyIconActivity
            // 
            notifyIconActivity.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            resources.ApplyResources(notifyIconActivity, "notifyIconActivity");
            notifyIconActivity.ContextMenuStrip = contextMenuTray;
            notifyIconActivity.MouseClick += notifyIconActivity_MouseClick;
            // 
            // notifyIconPing
            // 
            notifyIconPing.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            resources.ApplyResources(notifyIconPing, "notifyIconPing");
            notifyIconPing.ContextMenuStrip = contextMenuTray;
            notifyIconPing.MouseClick += notifyIconPing_MouseClick;
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FormMain";
            ShowIcon = false;
            ShowInTaskbar = false;
            Tag = "";
            WindowState = System.Windows.Forms.FormWindowState.Minimized;
            FormClosing += MainForm_FormClosing;
            Load += OnLoad;
            contextMenuTray.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuTray;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        internal System.Windows.Forms.ToolStripMenuItem NetworkAdaptersToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIconActivity;
        private System.Windows.Forms.ToolStripMenuItem NetworkSharingCenterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NetworkAndInternetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NetworkConnectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NetworkConnectionsAlternateViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FirewallSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FirewallAllowedAppsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem advancedStatisticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageWirelessNetworksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem homeGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkdomainworkgroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.NotifyIcon notifyIconPing;
    }
}

