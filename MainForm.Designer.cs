namespace GabNetStats
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.contextMenuTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.advancedStatisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.manageWirelessNetworksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.homeGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkdomainworkgroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FirewallSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NetworkSharingCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NetworkConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NetworkAdaptersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIconActivity = new System.Windows.Forms.NotifyIcon(this.components);
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuTray.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuTray
            // 
            this.contextMenuTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.toolStripSeparator4,
            this.settingsToolStripMenuItem,
            this.toolStripSeparator3,
            this.advancedStatisticsToolStripMenuItem,
            this.toolStripSeparator2,
            this.manageWirelessNetworksToolStripMenuItem,
            this.homeGroupToolStripMenuItem,
            this.networkMapToolStripMenuItem,
            this.networkdomainworkgroupToolStripMenuItem,
            this.networkToolStripMenuItem,
            this.FirewallSettingsToolStripMenuItem,
            this.NetworkSharingCenterToolStripMenuItem,
            this.NetworkConnectionsToolStripMenuItem,
            this.NetworkAdaptersToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.contextMenuTray.Name = "contextMenuTray";
            resources.ApplyResources(this.contextMenuTray, "contextMenuTray");
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.OnAbout);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Image = global::GabNetStats.Properties.Resources.settings_icon_16x16;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.OnSettings);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // advancedStatisticsToolStripMenuItem
            // 
            this.advancedStatisticsToolStripMenuItem.Name = "advancedStatisticsToolStripMenuItem";
            resources.ApplyResources(this.advancedStatisticsToolStripMenuItem, "advancedStatisticsToolStripMenuItem");
            this.advancedStatisticsToolStripMenuItem.Click += new System.EventHandler(this.advancedStatisticsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // manageWirelessNetworksToolStripMenuItem
            // 
            this.manageWirelessNetworksToolStripMenuItem.Image = global::GabNetStats.Properties.Resources.wlanpref_101;
            this.manageWirelessNetworksToolStripMenuItem.Name = "manageWirelessNetworksToolStripMenuItem";
            resources.ApplyResources(this.manageWirelessNetworksToolStripMenuItem, "manageWirelessNetworksToolStripMenuItem");
            this.manageWirelessNetworksToolStripMenuItem.Click += new System.EventHandler(this.manageWirelessNetworksToolStripMenuItem_Click);
            // 
            // homeGroupToolStripMenuItem
            // 
            this.homeGroupToolStripMenuItem.Image = global::GabNetStats.Properties.Resources.imageres_1013;
            this.homeGroupToolStripMenuItem.Name = "homeGroupToolStripMenuItem";
            resources.ApplyResources(this.homeGroupToolStripMenuItem, "homeGroupToolStripMenuItem");
            this.homeGroupToolStripMenuItem.Click += new System.EventHandler(this.homeGroupToolStripMenuItem_Click);
            // 
            // networkMapToolStripMenuItem
            // 
            this.networkMapToolStripMenuItem.Image = global::GabNetStats.Properties.Resources.networkmap_1;
            this.networkMapToolStripMenuItem.Name = "networkMapToolStripMenuItem";
            resources.ApplyResources(this.networkMapToolStripMenuItem, "networkMapToolStripMenuItem");
            this.networkMapToolStripMenuItem.Click += new System.EventHandler(this.networkMapToolStripMenuItem_Click);
            // 
            // networkdomainworkgroupToolStripMenuItem
            // 
            this.networkdomainworkgroupToolStripMenuItem.Image = global::GabNetStats.Properties.Resources.shell32_18;
            this.networkdomainworkgroupToolStripMenuItem.Name = "networkdomainworkgroupToolStripMenuItem";
            resources.ApplyResources(this.networkdomainworkgroupToolStripMenuItem, "networkdomainworkgroupToolStripMenuItem");
            this.networkdomainworkgroupToolStripMenuItem.Click += new System.EventHandler(this.networkdomainworkgroupToolStripMenuItem_Click);
            // 
            // networkToolStripMenuItem
            // 
            this.networkToolStripMenuItem.Image = global::GabNetStats.Properties.Resources.shell32_18;
            this.networkToolStripMenuItem.Name = "networkToolStripMenuItem";
            resources.ApplyResources(this.networkToolStripMenuItem, "networkToolStripMenuItem");
            this.networkToolStripMenuItem.Click += new System.EventHandler(this.networkToolStripMenuItem_Click);
            // 
            // FirewallSettingsToolStripMenuItem
            // 
            this.FirewallSettingsToolStripMenuItem.Image = global::GabNetStats.Properties.Resources.FireWallControlPanel_16x16;
            this.FirewallSettingsToolStripMenuItem.Name = "FirewallSettingsToolStripMenuItem";
            resources.ApplyResources(this.FirewallSettingsToolStripMenuItem, "FirewallSettingsToolStripMenuItem");
            this.FirewallSettingsToolStripMenuItem.Click += new System.EventHandler(this.FirewallSettingsToolStripMenuItem_Click);
            // 
            // NetworkSharingCenterToolStripMenuItem
            // 
            this.NetworkSharingCenterToolStripMenuItem.Image = global::GabNetStats.Properties.Resources.netcenter_16x16;
            this.NetworkSharingCenterToolStripMenuItem.Name = "NetworkSharingCenterToolStripMenuItem";
            resources.ApplyResources(this.NetworkSharingCenterToolStripMenuItem, "NetworkSharingCenterToolStripMenuItem");
            this.NetworkSharingCenterToolStripMenuItem.Click += new System.EventHandler(this.NetworkSharingCenterToolStripMenuItem_Click);
            // 
            // NetworkConnectionsToolStripMenuItem
            // 
            this.NetworkConnectionsToolStripMenuItem.Image = global::GabNetStats.Properties.Resources.netshell_16x16;
            this.NetworkConnectionsToolStripMenuItem.Name = "NetworkConnectionsToolStripMenuItem";
            resources.ApplyResources(this.NetworkConnectionsToolStripMenuItem, "NetworkConnectionsToolStripMenuItem");
            this.NetworkConnectionsToolStripMenuItem.Click += new System.EventHandler(this.NetworkConnectionsToolStripMenuItem_Click);
            // 
            // NetworkAdaptersToolStripMenuItem
            // 
            this.NetworkAdaptersToolStripMenuItem.Image = global::GabNetStats.Properties.Resources.pnidui_2407_16x16;
            this.NetworkAdaptersToolStripMenuItem.Name = "NetworkAdaptersToolStripMenuItem";
            resources.ApplyResources(this.NetworkAdaptersToolStripMenuItem, "NetworkAdaptersToolStripMenuItem");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.OnExit);
            // 
            // notifyIconActivity
            // 
            this.notifyIconActivity.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            resources.ApplyResources(this.notifyIconActivity, "notifyIconActivity");
            this.notifyIconActivity.ContextMenuStrip = this.contextMenuTray;
            this.notifyIconActivity.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIconActivity_MouseClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Tag = "";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.OnLoad);
            this.contextMenuTray.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuTray;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem NetworkAdaptersToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIconActivity;
        private System.Windows.Forms.ToolStripMenuItem NetworkSharingCenterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NetworkConnectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FirewallSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem advancedStatisticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageWirelessNetworksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem homeGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkdomainworkgroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}

