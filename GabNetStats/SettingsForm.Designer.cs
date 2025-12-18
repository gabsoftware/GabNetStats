using GabNetStats.Properties;

namespace GabNetStats
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            Properties.Settings settings2 = new Properties.Settings();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            radioCustomSpeed = new System.Windows.Forms.RadioButton();
            txtDownload = new System.Windows.Forms.TextBox();
            txtUpload = new System.Windows.Forms.TextBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label6 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            txtIconSet = new System.Windows.Forms.TextBox();
            grpBandwidthPreferences = new System.Windows.Forms.GroupBox();
            cbUpload = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            cbDownload = new System.Windows.Forms.ComboBox();
            lblUpload = new System.Windows.Forms.Label();
            lblDownload = new System.Windows.Forms.Label();
            rbBytes = new System.Windows.Forms.RadioButton();
            rbBits = new System.Windows.Forms.RadioButton();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            radioDefault = new System.Windows.Forms.RadioButton();
            textBoxDuration = new System.Windows.Forms.TextBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            chkSettingsAutoPingNotification = new System.Windows.Forms.CheckBox();
            numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            label8 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            txtSettingsAutoPingHost = new System.Windows.Forms.TextBox();
            chkSettingsAutoPingEnabled = new System.Windows.Forms.CheckBox();
            checkBoxStartup = new System.Windows.Forms.CheckBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            chkShowDisconnectedInterfaces = new System.Windows.Forms.CheckBox();
            groupBox1.SuspendLayout();
            grpBandwidthPreferences.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // buttonOK
            // 
            resources.ApplyResources(buttonOK, "buttonOK");
            buttonOK.Name = "buttonOK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += OnOK;
            // 
            // buttonCancel
            // 
            resources.ApplyResources(buttonCancel, "buttonCancel");
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Name = "buttonCancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // radioCustomSpeed
            // 
            resources.ApplyResources(radioCustomSpeed, "radioCustomSpeed");
            radioCustomSpeed.Name = "radioCustomSpeed";
            radioCustomSpeed.UseVisualStyleBackColor = true;
            radioCustomSpeed.CheckedChanged += radioCustomSpeed_CheckedChanged;
            // 
            // txtDownload
            // 
            resources.ApplyResources(txtDownload, "txtDownload");
            txtDownload.Name = "txtDownload";
            // 
            // txtUpload
            // 
            resources.ApplyResources(txtUpload, "txtUpload");
            txtUpload.Name = "txtUpload";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(txtIconSet);
            groupBox1.Controls.Add(grpBandwidthPreferences);
            groupBox1.Controls.Add(radioDefault);
            groupBox1.Controls.Add(radioCustomSpeed);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            // 
            // txtIconSet
            // 
            settings2.AutoCloseBalloon = true;
            settings2.AutoCloseBalloonAfter = new decimal(new int[] { 5, 0, 0, 0 });
            settings2.AutoPingEnabled = false;
            settings2.AutoPingHost = "google.com";
            settings2.AutoPingNotif = true;
            settings2.AutoPingRate = new decimal(new int[] { 5000, 0, 0, 0 });
            settings2.BalloonOpacity = 1D;
            settings2.BalloonOpacitySlider = 100;
            settings2.BandwidthDownload = 12500000L;
            settings2.BandwidthDownloadMultiplier = 1L;
            settings2.BandwidthUnit = 1;
            settings2.BandwidthUpload = 12500000L;
            settings2.BandwidthUploadMultiplier = 1L;
            settings2.BandwidthVisualsCustom = false;
            settings2.BandwidthVisualsDefault = true;
            settings2.BlinkDuration = 200;
            settings2.EnabledInterfaceMACList = "TOSET";
            settings2.IconSet = "xp";
            settings2.KnownInterfaceMACList = "";
            settings2.LoadOnStartup = false;
            settings2.SettingsKey = "";
            txtIconSet.DataBindings.Add(new System.Windows.Forms.Binding("Text", settings2, "IconSet", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(txtIconSet, "txtIconSet");
            txtIconSet.Name = "txtIconSet";
            // 
            // grpBandwidthPreferences
            // 
            grpBandwidthPreferences.Controls.Add(cbUpload);
            grpBandwidthPreferences.Controls.Add(label4);
            grpBandwidthPreferences.Controls.Add(cbDownload);
            grpBandwidthPreferences.Controls.Add(lblUpload);
            grpBandwidthPreferences.Controls.Add(lblDownload);
            grpBandwidthPreferences.Controls.Add(rbBytes);
            grpBandwidthPreferences.Controls.Add(rbBits);
            grpBandwidthPreferences.Controls.Add(label2);
            grpBandwidthPreferences.Controls.Add(txtDownload);
            grpBandwidthPreferences.Controls.Add(txtUpload);
            grpBandwidthPreferences.Controls.Add(label3);
            resources.ApplyResources(grpBandwidthPreferences, "grpBandwidthPreferences");
            grpBandwidthPreferences.Name = "grpBandwidthPreferences";
            grpBandwidthPreferences.TabStop = false;
            // 
            // cbUpload
            // 
            cbUpload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbUpload.FormattingEnabled = true;
            resources.ApplyResources(cbUpload, "cbUpload");
            cbUpload.Name = "cbUpload";
            cbUpload.SelectedIndexChanged += cbUpload_SelectedIndexChanged;
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            // 
            // cbDownload
            // 
            cbDownload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbDownload.FormattingEnabled = true;
            resources.ApplyResources(cbDownload, "cbDownload");
            cbDownload.Name = "cbDownload";
            cbDownload.SelectedIndexChanged += cbDownload_SelectedIndexChanged;
            // 
            // lblUpload
            // 
            resources.ApplyResources(lblUpload, "lblUpload");
            lblUpload.Name = "lblUpload";
            // 
            // lblDownload
            // 
            resources.ApplyResources(lblDownload, "lblDownload");
            lblDownload.Name = "lblDownload";
            // 
            // rbBytes
            // 
            resources.ApplyResources(rbBytes, "rbBytes");
            rbBytes.Checked = true;
            rbBytes.Name = "rbBytes";
            rbBytes.TabStop = true;
            rbBytes.UseVisualStyleBackColor = true;
            // 
            // rbBits
            // 
            resources.ApplyResources(rbBits, "rbBits");
            rbBits.Name = "rbBits";
            rbBits.UseVisualStyleBackColor = true;
            rbBits.CheckedChanged += rbBits_CheckedChanged;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // radioDefault
            // 
            resources.ApplyResources(radioDefault, "radioDefault");
            radioDefault.Checked = true;
            radioDefault.Name = "radioDefault";
            radioDefault.TabStop = true;
            radioDefault.UseVisualStyleBackColor = true;
            radioDefault.CheckedChanged += radioDefault_CheckedChanged;
            // 
            // textBoxDuration
            // 
            resources.ApplyResources(textBoxDuration, "textBoxDuration");
            textBoxDuration.Name = "textBoxDuration";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(chkSettingsAutoPingNotification);
            groupBox2.Controls.Add(numericUpDown1);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(txtSettingsAutoPingHost);
            groupBox2.Controls.Add(chkSettingsAutoPingEnabled);
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // chkSettingsAutoPingNotification
            // 
            resources.ApplyResources(chkSettingsAutoPingNotification, "chkSettingsAutoPingNotification");
            chkSettingsAutoPingNotification.Checked = true;
            chkSettingsAutoPingNotification.CheckState = System.Windows.Forms.CheckState.Checked;
            chkSettingsAutoPingNotification.DataBindings.Add(new System.Windows.Forms.Binding("Checked", settings2, "AutoPingNotif", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            chkSettingsAutoPingNotification.Name = "chkSettingsAutoPingNotification";
            chkSettingsAutoPingNotification.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            numericUpDown1.DataBindings.Add(new System.Windows.Forms.Binding("Value", settings2, "AutoPingRate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            numericUpDown1.Increment = new decimal(new int[] { 500, 0, 0, 0 });
            resources.ApplyResources(numericUpDown1, "numericUpDown1");
            numericUpDown1.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 500, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Value = new decimal(new int[] { 5000, 0, 0, 0 });
            // 
            // label8
            // 
            resources.ApplyResources(label8, "label8");
            label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.Name = "label7";
            // 
            // txtSettingsAutoPingHost
            // 
            txtSettingsAutoPingHost.DataBindings.Add(new System.Windows.Forms.Binding("Text", settings2, "AutoPingHost", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(txtSettingsAutoPingHost, "txtSettingsAutoPingHost");
            txtSettingsAutoPingHost.Name = "txtSettingsAutoPingHost";
            // 
            // chkSettingsAutoPingEnabled
            // 
            resources.ApplyResources(chkSettingsAutoPingEnabled, "chkSettingsAutoPingEnabled");
            chkSettingsAutoPingEnabled.DataBindings.Add(new System.Windows.Forms.Binding("Checked", settings2, "AutoPingEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            chkSettingsAutoPingEnabled.Name = "chkSettingsAutoPingEnabled";
            chkSettingsAutoPingEnabled.UseVisualStyleBackColor = true;
            // 
            // checkBoxStartup
            // 
            resources.ApplyResources(checkBoxStartup, "checkBoxStartup");
            checkBoxStartup.DataBindings.Add(new System.Windows.Forms.Binding("Checked", settings2, "LoadOnStartup", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            checkBoxStartup.Name = "checkBoxStartup";
            checkBoxStartup.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(chkShowDisconnectedInterfaces);
            resources.ApplyResources(groupBox3, "groupBox3");
            groupBox3.Name = "groupBox3";
            groupBox3.TabStop = false;
            // 
            // chkShowDisconnectedInterfaces
            // 
            resources.ApplyResources(chkShowDisconnectedInterfaces, "chkShowDisconnectedInterfaces");
            chkShowDisconnectedInterfaces.DataBindings.Add(new System.Windows.Forms.Binding("Checked", settings2, "ShowDisconnectedInterfaces", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            chkShowDisconnectedInterfaces.CheckedChanged += chkShowDisconnectedInterfaces_CheckedChanged;
            chkShowDisconnectedInterfaces.Name = "chkShowDisconnectedInterfaces";
            chkShowDisconnectedInterfaces.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            AcceptButton = buttonOK;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(checkBoxStartup);
            Controls.Add(textBoxDuration);
            Controls.Add(label1);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Load += OnLoad;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            grpBandwidthPreferences.ResumeLayout(false);
            grpBandwidthPreferences.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDuration;
        private System.Windows.Forms.CheckBox checkBoxStartup;
        private System.Windows.Forms.RadioButton radioCustomSpeed;
        private System.Windows.Forms.TextBox txtDownload;
        private System.Windows.Forms.TextBox txtUpload;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblUpload;
        private System.Windows.Forms.Label lblDownload;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioDefault;
        private System.Windows.Forms.GroupBox grpBandwidthPreferences;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbDownload;
        private System.Windows.Forms.RadioButton rbBytes;
        private System.Windows.Forms.RadioButton rbBits;
        private System.Windows.Forms.ComboBox cbUpload;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtIconSet;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtSettingsAutoPingHost;
        private System.Windows.Forms.CheckBox chkSettingsAutoPingEnabled;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.CheckBox chkSettingsAutoPingNotification;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkShowDisconnectedInterfaces;
    }
}
