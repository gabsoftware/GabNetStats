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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.radioCustomSpeed = new System.Windows.Forms.RadioButton();
            this.txtDownload = new System.Windows.Forms.TextBox();
            this.txtUpload = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grpBandwidthPreferences = new System.Windows.Forms.GroupBox();
            this.cbUpload = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbDownload = new System.Windows.Forms.ComboBox();
            this.lblUpload = new System.Windows.Forms.Label();
            this.lblDownload = new System.Windows.Forms.Label();
            this.rbBytes = new System.Windows.Forms.RadioButton();
            this.rbBits = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.radioDefault = new System.Windows.Forms.RadioButton();
            this.textBoxDuration = new System.Windows.Forms.TextBox();
            this.checkBoxStartup = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.grpBandwidthPreferences.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.OnOK);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // radioCustomSpeed
            // 
            resources.ApplyResources(this.radioCustomSpeed, "radioCustomSpeed");
            this.radioCustomSpeed.Name = "radioCustomSpeed";
            this.radioCustomSpeed.UseVisualStyleBackColor = true;
            this.radioCustomSpeed.CheckedChanged += new System.EventHandler(this.radioCustomSpeed_CheckedChanged);
            // 
            // txtDownload
            // 
            resources.ApplyResources(this.txtDownload, "txtDownload");
            this.txtDownload.Name = "txtDownload";
            // 
            // txtUpload
            // 
            resources.ApplyResources(this.txtUpload, "txtUpload");
            this.txtUpload.Name = "txtUpload";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.grpBandwidthPreferences);
            this.groupBox1.Controls.Add(this.radioDefault);
            this.groupBox1.Controls.Add(this.radioCustomSpeed);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // grpBandwidthPreferences
            // 
            resources.ApplyResources(this.grpBandwidthPreferences, "grpBandwidthPreferences");
            this.grpBandwidthPreferences.Controls.Add(this.cbUpload);
            this.grpBandwidthPreferences.Controls.Add(this.label4);
            this.grpBandwidthPreferences.Controls.Add(this.cbDownload);
            this.grpBandwidthPreferences.Controls.Add(this.lblUpload);
            this.grpBandwidthPreferences.Controls.Add(this.lblDownload);
            this.grpBandwidthPreferences.Controls.Add(this.rbBytes);
            this.grpBandwidthPreferences.Controls.Add(this.rbBits);
            this.grpBandwidthPreferences.Controls.Add(this.label2);
            this.grpBandwidthPreferences.Controls.Add(this.txtDownload);
            this.grpBandwidthPreferences.Controls.Add(this.txtUpload);
            this.grpBandwidthPreferences.Controls.Add(this.label3);
            this.grpBandwidthPreferences.Name = "grpBandwidthPreferences";
            this.grpBandwidthPreferences.TabStop = false;
            // 
            // cbUpload
            // 
            resources.ApplyResources(this.cbUpload, "cbUpload");
            this.cbUpload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUpload.FormattingEnabled = true;
            this.cbUpload.Name = "cbUpload";
            this.cbUpload.SelectedIndexChanged += new System.EventHandler(this.cbUpload_SelectedIndexChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // cbDownload
            // 
            resources.ApplyResources(this.cbDownload, "cbDownload");
            this.cbDownload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDownload.FormattingEnabled = true;
            this.cbDownload.Name = "cbDownload";
            this.cbDownload.SelectedIndexChanged += new System.EventHandler(this.cbDownload_SelectedIndexChanged);
            // 
            // lblUpload
            // 
            resources.ApplyResources(this.lblUpload, "lblUpload");
            this.lblUpload.Name = "lblUpload";
            // 
            // lblDownload
            // 
            resources.ApplyResources(this.lblDownload, "lblDownload");
            this.lblDownload.Name = "lblDownload";
            // 
            // rbBytes
            // 
            resources.ApplyResources(this.rbBytes, "rbBytes");
            this.rbBytes.Checked = true;
            this.rbBytes.Name = "rbBytes";
            this.rbBytes.TabStop = true;
            this.rbBytes.UseVisualStyleBackColor = true;
            // 
            // rbBits
            // 
            resources.ApplyResources(this.rbBits, "rbBits");
            this.rbBits.Name = "rbBits";
            this.rbBits.UseVisualStyleBackColor = true;
            this.rbBits.CheckedChanged += new System.EventHandler(this.rbBits_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // radioDefault
            // 
            resources.ApplyResources(this.radioDefault, "radioDefault");
            this.radioDefault.Checked = true;
            this.radioDefault.Name = "radioDefault";
            this.radioDefault.TabStop = true;
            this.radioDefault.UseVisualStyleBackColor = true;
            this.radioDefault.CheckedChanged += new System.EventHandler(this.radioDefault_CheckedChanged);
            // 
            // textBoxDuration
            // 
            resources.ApplyResources(this.textBoxDuration, "textBoxDuration");
            this.textBoxDuration.Name = "textBoxDuration";
            // 
            // checkBoxStartup
            // 
            resources.ApplyResources(this.checkBoxStartup, "checkBoxStartup");
            this.checkBoxStartup.Checked = global::GabNetStats.Properties.Settings.Default.LoadOnStartup;
            this.checkBoxStartup.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GabNetStats.Properties.Settings.Default, "LoadOnStartup", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxStartup.Name = "checkBoxStartup";
            this.checkBoxStartup.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBoxStartup);
            this.Controls.Add(this.textBoxDuration);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.OnLoad);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpBandwidthPreferences.ResumeLayout(false);
            this.grpBandwidthPreferences.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}