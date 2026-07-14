using GabNetStats.Properties;

namespace GabNetStats
{
    partial class FormSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            toolTipSettings = new System.Windows.Forms.ToolTip(components);
            radioAutoBandwidth = new System.Windows.Forms.RadioButton();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            radioCustomSpeed = new System.Windows.Forms.RadioButton();
            txtDownload = new System.Windows.Forms.TextBox();
            txtUpload = new System.Windows.Forms.TextBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            btnRefreshIconSets = new System.Windows.Forms.Button();
            label6 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            cboIconSet = new System.Windows.Forms.ComboBox();
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
            lblLanguage = new System.Windows.Forms.Label();
            cbLanguage = new System.Windows.Forms.ComboBox();
            chkShowHiddenInterfaces = new System.Windows.Forms.CheckBox();
            chkShowDisconnectedInterfaces = new System.Windows.Forms.CheckBox();
            groupBox1.SuspendLayout();
            grpBandwidthPreferences.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // radioAutoBandwidth
            // 
            resources.ApplyResources(radioAutoBandwidth, "radioAutoBandwidth");
            radioAutoBandwidth.Name = "radioAutoBandwidth";
            toolTipSettings.SetToolTip(radioAutoBandwidth, resources.GetString("radioAutoBandwidth.ToolTip"));
            radioAutoBandwidth.UseVisualStyleBackColor = true;
            radioAutoBandwidth.CheckedChanged += radioAutoBandwidth_CheckedChanged;
            // 
            // buttonOK
            // 
            resources.ApplyResources(buttonOK, "buttonOK");
            buttonOK.Name = "buttonOK";
            toolTipSettings.SetToolTip(buttonOK, resources.GetString("buttonOK.ToolTip"));
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += OnOK;
            // 
            // buttonCancel
            // 
            resources.ApplyResources(buttonCancel, "buttonCancel");
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Name = "buttonCancel";
            toolTipSettings.SetToolTip(buttonCancel, resources.GetString("buttonCancel.ToolTip"));
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            toolTipSettings.SetToolTip(label1, resources.GetString("label1.ToolTip"));
            // 
            // radioCustomSpeed
            // 
            resources.ApplyResources(radioCustomSpeed, "radioCustomSpeed");
            radioCustomSpeed.Name = "radioCustomSpeed";
            toolTipSettings.SetToolTip(radioCustomSpeed, resources.GetString("radioCustomSpeed.ToolTip"));
            radioCustomSpeed.UseVisualStyleBackColor = true;
            radioCustomSpeed.CheckedChanged += radioCustomSpeed_CheckedChanged;
            // 
            // txtDownload
            // 
            resources.ApplyResources(txtDownload, "txtDownload");
            txtDownload.Name = "txtDownload";
            toolTipSettings.SetToolTip(txtDownload, resources.GetString("txtDownload.ToolTip"));
            // 
            // txtUpload
            // 
            resources.ApplyResources(txtUpload, "txtUpload");
            txtUpload.Name = "txtUpload";
            toolTipSettings.SetToolTip(txtUpload, resources.GetString("txtUpload.ToolTip"));
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(radioAutoBandwidth);
            groupBox1.Controls.Add(btnRefreshIconSets);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(cboIconSet);
            groupBox1.Controls.Add(grpBandwidthPreferences);
            groupBox1.Controls.Add(radioDefault);
            groupBox1.Controls.Add(radioCustomSpeed);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            toolTipSettings.SetToolTip(groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // btnRefreshIconSets
            // 
            resources.ApplyResources(btnRefreshIconSets, "btnRefreshIconSets");
            btnRefreshIconSets.Name = "btnRefreshIconSets";
            toolTipSettings.SetToolTip(btnRefreshIconSets, resources.GetString("btnRefreshIconSets.ToolTip"));
            btnRefreshIconSets.UseCompatibleTextRendering = true;
            btnRefreshIconSets.UseVisualStyleBackColor = true;
            btnRefreshIconSets.Click += btnRefreshIconSets_Click;
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            toolTipSettings.SetToolTip(label6, resources.GetString("label6.ToolTip"));
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            toolTipSettings.SetToolTip(label5, resources.GetString("label5.ToolTip"));
            // 
            // cboIconSet
            // 
            resources.ApplyResources(cboIconSet, "cboIconSet");
            cboIconSet.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cboIconSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboIconSet.FormattingEnabled = true;
            cboIconSet.Name = "cboIconSet";
            toolTipSettings.SetToolTip(cboIconSet, resources.GetString("cboIconSet.ToolTip"));
            cboIconSet.DrawItem += cboIconSet_DrawItem;
            // 
            // grpBandwidthPreferences
            // 
            resources.ApplyResources(grpBandwidthPreferences, "grpBandwidthPreferences");
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
            grpBandwidthPreferences.Name = "grpBandwidthPreferences";
            grpBandwidthPreferences.TabStop = false;
            toolTipSettings.SetToolTip(grpBandwidthPreferences, resources.GetString("grpBandwidthPreferences.ToolTip"));
            // 
            // cbUpload
            // 
            resources.ApplyResources(cbUpload, "cbUpload");
            cbUpload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbUpload.FormattingEnabled = true;
            cbUpload.Name = "cbUpload";
            toolTipSettings.SetToolTip(cbUpload, resources.GetString("cbUpload.ToolTip"));
            cbUpload.SelectedIndexChanged += cbUpload_SelectedIndexChanged;
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            toolTipSettings.SetToolTip(label4, resources.GetString("label4.ToolTip"));
            // 
            // cbDownload
            // 
            resources.ApplyResources(cbDownload, "cbDownload");
            cbDownload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbDownload.FormattingEnabled = true;
            cbDownload.Name = "cbDownload";
            toolTipSettings.SetToolTip(cbDownload, resources.GetString("cbDownload.ToolTip"));
            cbDownload.SelectedIndexChanged += cbDownload_SelectedIndexChanged;
            // 
            // lblUpload
            // 
            resources.ApplyResources(lblUpload, "lblUpload");
            lblUpload.Name = "lblUpload";
            toolTipSettings.SetToolTip(lblUpload, resources.GetString("lblUpload.ToolTip"));
            // 
            // lblDownload
            // 
            resources.ApplyResources(lblDownload, "lblDownload");
            lblDownload.Name = "lblDownload";
            toolTipSettings.SetToolTip(lblDownload, resources.GetString("lblDownload.ToolTip"));
            // 
            // rbBytes
            // 
            resources.ApplyResources(rbBytes, "rbBytes");
            rbBytes.Checked = true;
            rbBytes.Name = "rbBytes";
            rbBytes.TabStop = true;
            toolTipSettings.SetToolTip(rbBytes, resources.GetString("rbBytes.ToolTip"));
            rbBytes.UseVisualStyleBackColor = true;
            // 
            // rbBits
            // 
            resources.ApplyResources(rbBits, "rbBits");
            rbBits.Name = "rbBits";
            toolTipSettings.SetToolTip(rbBits, resources.GetString("rbBits.ToolTip"));
            rbBits.UseVisualStyleBackColor = true;
            rbBits.CheckedChanged += rbBits_CheckedChanged;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            toolTipSettings.SetToolTip(label2, resources.GetString("label2.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            toolTipSettings.SetToolTip(label3, resources.GetString("label3.ToolTip"));
            // 
            // radioDefault
            // 
            resources.ApplyResources(radioDefault, "radioDefault");
            radioDefault.Checked = true;
            radioDefault.Name = "radioDefault";
            radioDefault.TabStop = true;
            toolTipSettings.SetToolTip(radioDefault, resources.GetString("radioDefault.ToolTip"));
            radioDefault.UseVisualStyleBackColor = true;
            radioDefault.CheckedChanged += radioDefault_CheckedChanged;
            // 
            // textBoxDuration
            // 
            resources.ApplyResources(textBoxDuration, "textBoxDuration");
            textBoxDuration.Name = "textBoxDuration";
            toolTipSettings.SetToolTip(textBoxDuration, resources.GetString("textBoxDuration.ToolTip"));
            // 
            // groupBox2
            // 
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Controls.Add(chkSettingsAutoPingNotification);
            groupBox2.Controls.Add(numericUpDown1);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(txtSettingsAutoPingHost);
            groupBox2.Controls.Add(chkSettingsAutoPingEnabled);
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            toolTipSettings.SetToolTip(groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // chkSettingsAutoPingNotification
            // 
            resources.ApplyResources(chkSettingsAutoPingNotification, "chkSettingsAutoPingNotification");
            chkSettingsAutoPingNotification.Name = "chkSettingsAutoPingNotification";
            toolTipSettings.SetToolTip(chkSettingsAutoPingNotification, resources.GetString("chkSettingsAutoPingNotification.ToolTip"));
            chkSettingsAutoPingNotification.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            resources.ApplyResources(numericUpDown1, "numericUpDown1");
            numericUpDown1.Increment = new decimal(new int[] { 500, 0, 0, 0 });
            numericUpDown1.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 500, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            toolTipSettings.SetToolTip(numericUpDown1, resources.GetString("numericUpDown1.ToolTip"));
            numericUpDown1.Value = new decimal(new int[] { 500, 0, 0, 0 });
            // 
            // label8
            // 
            resources.ApplyResources(label8, "label8");
            label8.Name = "label8";
            toolTipSettings.SetToolTip(label8, resources.GetString("label8.ToolTip"));
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.Name = "label7";
            toolTipSettings.SetToolTip(label7, resources.GetString("label7.ToolTip"));
            // 
            // txtSettingsAutoPingHost
            // 
            resources.ApplyResources(txtSettingsAutoPingHost, "txtSettingsAutoPingHost");
            txtSettingsAutoPingHost.Name = "txtSettingsAutoPingHost";
            toolTipSettings.SetToolTip(txtSettingsAutoPingHost, resources.GetString("txtSettingsAutoPingHost.ToolTip"));
            // 
            // chkSettingsAutoPingEnabled
            // 
            resources.ApplyResources(chkSettingsAutoPingEnabled, "chkSettingsAutoPingEnabled");
            chkSettingsAutoPingEnabled.Name = "chkSettingsAutoPingEnabled";
            toolTipSettings.SetToolTip(chkSettingsAutoPingEnabled, resources.GetString("chkSettingsAutoPingEnabled.ToolTip"));
            chkSettingsAutoPingEnabled.UseVisualStyleBackColor = true;
            // 
            // checkBoxStartup
            // 
            resources.ApplyResources(checkBoxStartup, "checkBoxStartup");
            checkBoxStartup.Name = "checkBoxStartup";
            toolTipSettings.SetToolTip(checkBoxStartup, resources.GetString("checkBoxStartup.ToolTip"));
            checkBoxStartup.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            resources.ApplyResources(groupBox3, "groupBox3");
            groupBox3.Controls.Add(lblLanguage);
            groupBox3.Controls.Add(cbLanguage);
            groupBox3.Controls.Add(chkShowHiddenInterfaces);
            groupBox3.Controls.Add(chkShowDisconnectedInterfaces);
            groupBox3.Name = "groupBox3";
            groupBox3.TabStop = false;
            toolTipSettings.SetToolTip(groupBox3, resources.GetString("groupBox3.ToolTip"));
            // 
            // lblLanguage
            // 
            resources.ApplyResources(lblLanguage, "lblLanguage");
            lblLanguage.Name = "lblLanguage";
            toolTipSettings.SetToolTip(lblLanguage, resources.GetString("lblLanguage.ToolTip"));
            // 
            // cbLanguage
            // 
            resources.ApplyResources(cbLanguage, "cbLanguage");
            cbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbLanguage.FormattingEnabled = true;
            cbLanguage.Name = "cbLanguage";
            toolTipSettings.SetToolTip(cbLanguage, resources.GetString("cbLanguage.ToolTip"));
            cbLanguage.SelectedIndexChanged += cbLanguage_SelectedIndexChanged;
            // 
            // chkShowHiddenInterfaces
            // 
            resources.ApplyResources(chkShowHiddenInterfaces, "chkShowHiddenInterfaces");
            chkShowHiddenInterfaces.Name = "chkShowHiddenInterfaces";
            toolTipSettings.SetToolTip(chkShowHiddenInterfaces, resources.GetString("chkShowHiddenInterfaces.ToolTip"));
            chkShowHiddenInterfaces.UseVisualStyleBackColor = true;
            chkShowHiddenInterfaces.CheckedChanged += chkShowHiddenInterfaces_CheckedChanged;
            // 
            // chkShowDisconnectedInterfaces
            // 
            resources.ApplyResources(chkShowDisconnectedInterfaces, "chkShowDisconnectedInterfaces");
            chkShowDisconnectedInterfaces.Name = "chkShowDisconnectedInterfaces";
            toolTipSettings.SetToolTip(chkShowDisconnectedInterfaces, resources.GetString("chkShowDisconnectedInterfaces.ToolTip"));
            chkShowDisconnectedInterfaces.UseVisualStyleBackColor = true;
            chkShowDisconnectedInterfaces.CheckedChanged += chkShowDisconnectedInterfaces_CheckedChanged;
            // 
            // FormSettings
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
            Name = "FormSettings";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            toolTipSettings.SetToolTip(this, resources.GetString("$this.ToolTip"));
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
        private System.Windows.Forms.ComboBox cboIconSet;
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
        private System.Windows.Forms.Button btnRefreshIconSets;
        private System.Windows.Forms.ComboBox cbLanguage;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.CheckBox chkShowHiddenInterfaces;
        private System.Windows.Forms.RadioButton radioAutoBandwidth;
        private System.Windows.Forms.ToolTip toolTipSettings;
    }
}
