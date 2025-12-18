namespace GabNetStats
{
    partial class frmBalloon
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBalloon));
            Properties.Settings settings1 = new Properties.Settings();
            GabTracker.GabTrackerFeed gabTrackerFeed1 = new GabTracker.GabTrackerFeed();
            GabTracker.GabTrackerFeed gabTrackerFeed2 = new GabTracker.GabTrackerFeed();
            BallonTimer = new System.Windows.Forms.Timer(components);
            lblStatisticsData = new System.Windows.Forms.Label();
            lblSeconds = new System.Windows.Forms.Label();
            lblStatisticsTitle = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            btn_settings = new System.Windows.Forms.Button();
            btnAdvanced = new System.Windows.Forms.Button();
            tbTrans = new System.Windows.Forms.TrackBar();
            nudAutoClose = new System.Windows.Forms.NumericUpDown();
            chkAutoClose = new System.Windows.Forms.CheckBox();
            gabTracker1 = new GabTracker.GabTracker();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tbTrans).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAutoClose).BeginInit();
            SuspendLayout();
            // 
            // BallonTimer
            // 
            BallonTimer.Interval = 1000;
            BallonTimer.Tick += BallonTimer_Tick;
            // 
            // lblStatisticsData
            // 
            resources.ApplyResources(lblStatisticsData, "lblStatisticsData");
            lblStatisticsData.Name = "lblStatisticsData";
            // 
            // lblSeconds
            // 
            resources.ApplyResources(lblSeconds, "lblSeconds");
            lblSeconds.Name = "lblSeconds";
            // 
            // lblStatisticsTitle
            // 
            resources.ApplyResources(lblStatisticsTitle, "lblStatisticsTitle");
            lblStatisticsTitle.Name = "lblStatisticsTitle";
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.Color.White;
            panel1.Controls.Add(btn_settings);
            panel1.Controls.Add(btnAdvanced);
            panel1.Controls.Add(lblStatisticsTitle);
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            panel1.Paint += panel1_Paint;
            // 
            // btn_settings
            // 
            resources.ApplyResources(btn_settings, "btn_settings");
            btn_settings.Image = Properties.Resources.settings_icon_16x16;
            btn_settings.Name = "btn_settings";
            btn_settings.UseVisualStyleBackColor = true;
            btn_settings.Click += btn_settings_Click;
            // 
            // btnAdvanced
            // 
            resources.ApplyResources(btnAdvanced, "btnAdvanced");
            btnAdvanced.Name = "btnAdvanced";
            btnAdvanced.UseVisualStyleBackColor = true;
            btnAdvanced.Click += btnAdvanced_Click;
            // 
            // tbTrans
            // 
            resources.ApplyResources(tbTrans, "tbTrans");
            tbTrans.BackColor = System.Drawing.SystemColors.Info;
            settings1.AdvancedSelectedInterfaceMac = "";
            settings1.AutoCloseBalloon = true;
            settings1.AutoCloseBalloonAfter = new decimal(new int[] { 5, 0, 0, 0 });
            settings1.AutoPingEnabled = false;
            settings1.AutoPingHost = "google.com";
            settings1.AutoPingNotif = true;
            settings1.AutoPingRate = new decimal(new int[] { 5000, 0, 0, 0 });
            settings1.BalloonLocationX = -1;
            settings1.BalloonLocationY = -1;
            settings1.BalloonOpacity = 1D;
            settings1.BalloonOpacitySlider = 100;
            settings1.BandwidthDownload = 12500000L;
            settings1.BandwidthDownloadMultiplier = 1L;
            settings1.BandwidthUnit = 1;
            settings1.BandwidthUpload = 12500000L;
            settings1.BandwidthUploadMultiplier = 1L;
            settings1.BandwidthVisualsCustom = false;
            settings1.BandwidthVisualsDefault = true;
            settings1.BlinkDuration = 200;
            settings1.EnabledInterfaceMACList = "TOSET";
            settings1.IconSet = "xp";
            settings1.KnownInterfaceMACList = "";
            settings1.LoadOnStartup = false;
            settings1.SettingsKey = "";
            settings1.ShowDisconnectedInterfaces = false;
            tbTrans.DataBindings.Add(new System.Windows.Forms.Binding("Value", settings1, "BalloonOpacitySlider", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            tbTrans.LargeChange = 10;
            tbTrans.Maximum = 100;
            tbTrans.Minimum = 10;
            tbTrans.Name = "tbTrans";
            tbTrans.TickFrequency = 10;
            tbTrans.TickStyle = System.Windows.Forms.TickStyle.None;
            tbTrans.Value = 100;
            tbTrans.Scroll += tbTrans_Scroll;
            tbTrans.ValueChanged += tbTrans_ValueChanged;
            // 
            // nudAutoClose
            // 
            resources.ApplyResources(nudAutoClose, "nudAutoClose");
            nudAutoClose.DataBindings.Add(new System.Windows.Forms.Binding("Value", settings1, "AutoCloseBalloonAfter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            nudAutoClose.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            nudAutoClose.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudAutoClose.Name = "nudAutoClose";
            nudAutoClose.Value = new decimal(new int[] { 5, 0, 0, 0 });
            nudAutoClose.ValueChanged += nudAutoClose_ValueChanged;
            // 
            // chkAutoClose
            // 
            resources.ApplyResources(chkAutoClose, "chkAutoClose");
            chkAutoClose.Checked = true;
            chkAutoClose.CheckState = System.Windows.Forms.CheckState.Checked;
            chkAutoClose.DataBindings.Add(new System.Windows.Forms.Binding("Checked", settings1, "AutoCloseBalloon", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            chkAutoClose.Name = "chkAutoClose";
            chkAutoClose.UseVisualStyleBackColor = true;
            chkAutoClose.CheckStateChanged += chkAutoClose_CheckStateChanged;
            // 
            // gabTracker1
            // 
            resources.ApplyResources(gabTracker1, "gabTracker1");
            gabTracker1.AutoMax = true;
            gabTracker1.AutoMaxPercentage = 110D;
            gabTrackerFeed1.Coefficient = 1D;
            gabTrackerFeed1.FillAlpha = 100;
            gabTrackerFeed1.FillThickness = 10F;
            gabTrackerFeed1.FillUnder = true;
            gabTrackerFeed1.Legend = "Download";
            gabTrackerFeed1.LineColor = System.Drawing.Color.DodgerBlue;
            gabTrackerFeed1.LineThickness = 2F;
            gabTrackerFeed1.Unit = "KB/s";
            gabTrackerFeed1.Value = 40D;
            gabTrackerFeed2.Coefficient = 1D;
            gabTrackerFeed2.FillAlpha = 100;
            gabTrackerFeed2.FillThickness = 10F;
            gabTrackerFeed2.FillUnder = true;
            gabTrackerFeed2.Legend = "Upload";
            gabTrackerFeed2.LineColor = System.Drawing.Color.Red;
            gabTrackerFeed2.LineThickness = 2F;
            gabTrackerFeed2.Unit = "KB/s";
            gabTrackerFeed2.Value = 50D;
            gabTracker1.Feeds.Add(gabTrackerFeed1);
            gabTracker1.Feeds.Add(gabTrackerFeed2);
            gabTracker1.GridThickerThickness = 1F;
            gabTracker1.GridThickness = 1F;
            gabTracker1.LegendOpacity = 75;
            gabTracker1.MaxDataInMemory = 32;
            gabTracker1.Maximum = 56;
            gabTracker1.Name = "gabTracker1";
            gabTracker1.RefreshRate = 500;
            // 
            // frmBalloon
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Info;
            Controls.Add(gabTracker1);
            Controls.Add(tbTrans);
            Controls.Add(panel1);
            Controls.Add(lblSeconds);
            Controls.Add(nudAutoClose);
            Controls.Add(chkAutoClose);
            Controls.Add(lblStatisticsData);
            DataBindings.Add(new System.Windows.Forms.Binding("Opacity", settings1, "BalloonOpacity", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmBalloon";
            ShowIcon = false;
            ShowInTaskbar = false;
            TopMost = true;
            FormClosing += frmBalloon_FormClosing;
            Load += frmBalloon_Load;
            LocationChanged += frmBalloon_LocationChanged;
            VisibleChanged += frmBalloon_VisibleChanged;
            Resize += frmBalloon_Resize;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tbTrans).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAutoClose).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblStatisticsData;
        private System.Windows.Forms.CheckBox chkAutoClose;
        private System.Windows.Forms.NumericUpDown nudAutoClose;
        private System.Windows.Forms.Label lblSeconds;
        private System.Windows.Forms.Label lblStatisticsTitle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar tbTrans;
        private System.Windows.Forms.Button btnAdvanced;
        private GabTracker.GabTracker gabTracker1;
        private System.Windows.Forms.Button btn_settings;
        internal System.Windows.Forms.Timer BallonTimer;
    }
}
