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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBalloon));
            GabTracker.GabTrackerFeed gabTrackerFeed1 = new GabTracker.GabTrackerFeed();
            GabTracker.GabTrackerFeed gabTrackerFeed2 = new GabTracker.GabTrackerFeed();
            this.BallonTimer = new System.Windows.Forms.Timer(this.components);
            this.lblStatisticsData = new System.Windows.Forms.Label();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.lblStatisticsTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.tbTrans = new System.Windows.Forms.TrackBar();
            this.nudAutoClose = new System.Windows.Forms.NumericUpDown();
            this.chkAutoClose = new System.Windows.Forms.CheckBox();
            this.gabTracker1 = new GabTracker.GabTracker();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbTrans)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAutoClose)).BeginInit();
            this.SuspendLayout();
            // 
            // BallonTimer
            // 
            this.BallonTimer.Interval = 1000;
            this.BallonTimer.Tick += new System.EventHandler(this.BallonTimer_Tick);
            // 
            // lblStatisticsData
            // 
            resources.ApplyResources(this.lblStatisticsData, "lblStatisticsData");
            this.lblStatisticsData.Name = "lblStatisticsData";
            // 
            // lblSeconds
            // 
            resources.ApplyResources(this.lblSeconds, "lblSeconds");
            this.lblSeconds.Name = "lblSeconds";
            // 
            // lblStatisticsTitle
            // 
            resources.ApplyResources(this.lblStatisticsTitle, "lblStatisticsTitle");
            this.lblStatisticsTitle.Name = "lblStatisticsTitle";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.btnAdvanced);
            this.panel1.Controls.Add(this.lblStatisticsTitle);
            this.panel1.Name = "panel1";
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnAdvanced
            // 
            resources.ApplyResources(this.btnAdvanced, "btnAdvanced");
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
            // 
            // tbTrans
            // 
            resources.ApplyResources(this.tbTrans, "tbTrans");
            this.tbTrans.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GabNetStats.Properties.Settings.Default, "BalloonOpacitySlider", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbTrans.LargeChange = 10;
            this.tbTrans.Maximum = 100;
            this.tbTrans.Minimum = 10;
            this.tbTrans.Name = "tbTrans";
            this.tbTrans.TickFrequency = 10;
            this.tbTrans.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTrans.Value = global::GabNetStats.Properties.Settings.Default.BalloonOpacitySlider;
            this.tbTrans.Scroll += new System.EventHandler(this.tbTrans_Scroll);
            this.tbTrans.ValueChanged += new System.EventHandler(this.tbTrans_ValueChanged);
            // 
            // nudAutoClose
            // 
            resources.ApplyResources(this.nudAutoClose, "nudAutoClose");
            this.nudAutoClose.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GabNetStats.Properties.Settings.Default, "AutoCloseBalloonAfter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nudAutoClose.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudAutoClose.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAutoClose.Name = "nudAutoClose";
            this.nudAutoClose.Value = global::GabNetStats.Properties.Settings.Default.AutoCloseBalloonAfter;
            this.nudAutoClose.ValueChanged += new System.EventHandler(this.nudAutoClose_ValueChanged);
            // 
            // chkAutoClose
            // 
            resources.ApplyResources(this.chkAutoClose, "chkAutoClose");
            this.chkAutoClose.Checked = global::GabNetStats.Properties.Settings.Default.AutoCloseBalloon;
            this.chkAutoClose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoClose.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GabNetStats.Properties.Settings.Default, "AutoCloseBalloon", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkAutoClose.Name = "chkAutoClose";
            this.chkAutoClose.UseVisualStyleBackColor = true;
            this.chkAutoClose.CheckStateChanged += new System.EventHandler(this.chkAutoClose_CheckStateChanged);
            // 
            // gabTracker1
            // 
            resources.ApplyResources(this.gabTracker1, "gabTracker1");
            this.gabTracker1.AutoMax = true;
            this.gabTracker1.AutoMaxPercentage = 110D;
            gabTrackerFeed1.Coefficient = 1D;
            gabTrackerFeed1.FillAlpha = ((byte)(100));
            gabTrackerFeed1.FillThickness = 10F;
            gabTrackerFeed1.FillUnder = true;
            gabTrackerFeed1.Legend = "Download";
            gabTrackerFeed1.LineColor = System.Drawing.Color.DodgerBlue;
            gabTrackerFeed1.LineThickness = 2F;
            gabTrackerFeed1.Unit = "KB/s";
            gabTrackerFeed1.Value = 40D;
            gabTrackerFeed2.Coefficient = 1D;
            gabTrackerFeed2.FillAlpha = ((byte)(100));
            gabTrackerFeed2.FillThickness = 10F;
            gabTrackerFeed2.FillUnder = true;
            gabTrackerFeed2.Legend = "Upload";
            gabTrackerFeed2.LineColor = System.Drawing.Color.Red;
            gabTrackerFeed2.LineThickness = 2F;
            gabTrackerFeed2.Unit = "KB/s";
            gabTrackerFeed2.Value = 50D;
            this.gabTracker1.Feeds.Add(gabTrackerFeed1);
            this.gabTracker1.Feeds.Add(gabTrackerFeed2);
            this.gabTracker1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(75)))), ((int)(((byte)(0)))));
            this.gabTracker1.GridThickerColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(75)))), ((int)(((byte)(0)))));
            this.gabTracker1.GridThickerThickness = 1F;
            this.gabTracker1.GridThickness = 1F;
            this.gabTracker1.LegendOpacity = ((byte)(75));
            this.gabTracker1.MaxDataInMemory = 32;
            this.gabTracker1.Maximum = 56;
            this.gabTracker1.Name = "gabTracker1";
            this.gabTracker1.RefreshRate = 500;
            // 
            // frmBalloon
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.Controls.Add(this.gabTracker1);
            this.Controls.Add(this.tbTrans);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblSeconds);
            this.Controls.Add(this.nudAutoClose);
            this.Controls.Add(this.chkAutoClose);
            this.Controls.Add(this.lblStatisticsData);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Opacity", global::GabNetStats.Properties.Settings.Default, "BalloonOpacity", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBalloon";
            this.Opacity = global::GabNetStats.Properties.Settings.Default.BalloonOpacity;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmBalloon_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.frmBalloon_VisibleChanged);
            this.Resize += new System.EventHandler(this.frmBalloon_Resize);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbTrans)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAutoClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer BallonTimer;
        private System.Windows.Forms.Label lblStatisticsData;
        private System.Windows.Forms.CheckBox chkAutoClose;
        private System.Windows.Forms.NumericUpDown nudAutoClose;
        private System.Windows.Forms.Label lblSeconds;
        private System.Windows.Forms.Label lblStatisticsTitle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar tbTrans;
        private System.Windows.Forms.Button btnAdvanced;
        private GabTracker.GabTracker gabTracker1;
    }
}