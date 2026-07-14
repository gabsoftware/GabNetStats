// After an original idea of Igor Tolmachev, IT Samples
//        http://www.itsamples.com
//
// Copyright (c) 2010

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GabNetStats.Properties;
using System.Globalization;

namespace GabNetStats
{
    public partial class FormSettings : Form
    {
        private int nDuration = NetworkStatsWorker.BlinkDurationMinimum;
        private long nDownload = SettingsManager.DEFAULT_BANDWIDTH_BPS;
        private long nUpload = SettingsManager.DEFAULT_BANDWIDTH_BPS;

        /*private int _nStartUp = 0;
        protected int nStartup
        {
            get
            {
                return _nStartUp;
            }
            set
            {
                _nStartUp = value;
            }
        }*/


        private bool settingsInitialized;

        private readonly Dictionary<string, Icon> _iconSetPreviews = new Dictionary<string, Icon>();

        public FormSettings()
        {
            InitializeComponent();
        }

        private class BandwidthItem
        {
            public BandwidthItem(string sname, long lmultiplier)
            {
                name = sname;
                multiplier = lmultiplier;
            }
            public string name { get; set; }
            public long multiplier { get; set; }
        }

        private class LanguageItem
        {
            public LanguageItem(string sname, string scultureName)
            {
                name = sname;
                cultureName = scultureName;
            }
            public string name { get; set; }
            public string cultureName { get; set; }
        }

        private void PopulateLanguageCombo()
        {
            cbLanguage.Items.Clear();

            foreach (CultureInfo culture in LanguageManager.GetAvailableLanguages())
            {
                bool isDefault = string.Equals(culture.Name, LanguageManager.DEFAULT_LANGUAGE, StringComparison.OrdinalIgnoreCase);
                string displayName = isDefault ? Res.str_LanguageDefault : culture.NativeName;
                cbLanguage.Items.Add(new LanguageItem(displayName, culture.Name));
            }

            cbLanguage.ValueMember = "cultureName";
            cbLanguage.DisplayMember = "name";

            string current = Settings.Default.Language;
            if (string.IsNullOrEmpty(current))
            {
                current = LanguageManager.GetDefaultLanguage();
            }

            int idx = -1;
            for (int i = 0; i < cbLanguage.Items.Count; i++)
            {
                if (string.Equals(((LanguageItem)cbLanguage.Items[i]).cultureName, current, StringComparison.OrdinalIgnoreCase))
                {
                    idx = i;
                    break;
                }
            }
            cbLanguage.SelectedIndex = idx >= 0 ? idx : 0;
        }

        private void updateCombos()
        {
            cbDownload.Items.Clear();
            cbUpload.Items.Clear();
            long bdm = Settings.Default.BandwidthDownloadMultiplier;
            long bum = Settings.Default.BandwidthUploadMultiplier;

            if (rbBits.Checked)
            {
                cbDownload.Items.AddRange(new object[]
                {
                    new BandwidthItem(Res.str_bit, (long)SpeedUtils.eBandwidthMultiplier.un),
                    new BandwidthItem(Res.str_Kbit, (long)SpeedUtils.eBandwidthMultiplier.K),
                    new BandwidthItem(Res.str_Mbit, (long)SpeedUtils.eBandwidthMultiplier.M),
                    new BandwidthItem(Res.str_Gbit, (long)SpeedUtils.eBandwidthMultiplier.G),
                    new BandwidthItem(Res.str_Tbit, (long)SpeedUtils.eBandwidthMultiplier.T)
                });

                cbUpload.Items.AddRange(new object[]
                {
                    new BandwidthItem(Res.str_bit, (long)SpeedUtils.eBandwidthMultiplier.un),
                    new BandwidthItem(Res.str_Kbit, (long)SpeedUtils.eBandwidthMultiplier.K),
                    new BandwidthItem(Res.str_Mbit, (long)SpeedUtils.eBandwidthMultiplier.M),
                    new BandwidthItem(Res.str_Gbit, (long)SpeedUtils.eBandwidthMultiplier.G),
                    new BandwidthItem(Res.str_Tbit, (long)SpeedUtils.eBandwidthMultiplier.T)
                });
            }
            else
            {
                cbDownload.Items.AddRange(new object[]
                {
                    new BandwidthItem(Res.str_Bytes, (long)SpeedUtils.eBandwidthMultiplier.un),
                    new BandwidthItem(Res.str_KiB, (long)SpeedUtils.eBandwidthMultiplier.K),
                    new BandwidthItem(Res.str_MiB, (long)SpeedUtils.eBandwidthMultiplier.M),
                    new BandwidthItem(Res.str_GiB, (long)SpeedUtils.eBandwidthMultiplier.G),
                    new BandwidthItem(Res.str_TiB, (long)SpeedUtils.eBandwidthMultiplier.T)
                });

                cbUpload.Items.AddRange(new object[]
                {
                    new BandwidthItem(Res.str_Bytes, (long)SpeedUtils.eBandwidthMultiplier.un),
                    new BandwidthItem(Res.str_KiB, (long)SpeedUtils.eBandwidthMultiplier.K),
                    new BandwidthItem(Res.str_MiB, (long)SpeedUtils.eBandwidthMultiplier.M),
                    new BandwidthItem(Res.str_GiB, (long)SpeedUtils.eBandwidthMultiplier.G),
                    new BandwidthItem(Res.str_TiB, (long)SpeedUtils.eBandwidthMultiplier.T)
                });
            }
            cbDownload.ValueMember = "multiplier";
            cbDownload.DisplayMember = "name";
            cbDownload.SelectedIndex = (bdm == (long)SpeedUtils.eBandwidthMultiplier.un ? 0 :
                                       (bdm == (long)SpeedUtils.eBandwidthMultiplier.K ? 1 :
                                       (bdm == (long)SpeedUtils.eBandwidthMultiplier.M ? 2 :
                                       (bdm == (long)SpeedUtils.eBandwidthMultiplier.G ? 3 :
                                       (bdm == (long)SpeedUtils.eBandwidthMultiplier.T ? 4 : 0)))));

            cbUpload.ValueMember = "multiplier";
            cbUpload.DisplayMember = "name";
            cbUpload.SelectedIndex = (bum == (long)SpeedUtils.eBandwidthMultiplier.un ? 0 :
                                       (bum == (long)SpeedUtils.eBandwidthMultiplier.K ? 1 :
                                       (bum == (long)SpeedUtils.eBandwidthMultiplier.M ? 2 :
                                       (bum == (long)SpeedUtils.eBandwidthMultiplier.G ? 3 :
                                       (bum == (long)SpeedUtils.eBandwidthMultiplier.T ? 4 : 0)))));
        }

        private void OnLoad(object sender, EventArgs e)
        {
            FormClosed += (s, a) =>
            {
                foreach (Icon icon in _iconSetPreviews.Values)
                    icon?.Dispose();
                _iconSetPreviews.Clear();
            };

            Settings.Default.Reload();
            SettingsManager.ValidateSettings();

            chkSettingsAutoPingNotification.DataBindings.Add(new Binding("Checked",  Settings.Default, "AutoPingNotif",   true, DataSourceUpdateMode.OnPropertyChanged));
            numericUpDown1.DataBindings.Add              (new Binding("Value",   Settings.Default, "AutoPingRate",    true, DataSourceUpdateMode.OnPropertyChanged));
            txtSettingsAutoPingHost.DataBindings.Add     (new Binding("Text",    Settings.Default, "AutoPingHost",    true, DataSourceUpdateMode.OnPropertyChanged));
            chkSettingsAutoPingEnabled.DataBindings.Add  (new Binding("Checked", Settings.Default, "AutoPingEnabled", true, DataSourceUpdateMode.OnPropertyChanged));

            textBoxDuration.Text = Settings.Default.BlinkDuration.ToString(CultureInfo.InvariantCulture);
            txtDownload.Text = Settings.Default.BandwidthDownload.ToString(CultureInfo.InvariantCulture);
            txtUpload.Text = Settings.Default.BandwidthUpload.ToString(CultureInfo.InvariantCulture);

            rbBits.Checked = Settings.Default.BandwidthUnit == (int)SpeedUtils.BandwidthUnit.Bit;

            updateCombos();

            radioDefault.Checked = Settings.Default.BandwidthVisualsDefault;
            radioCustomSpeed.Checked = Settings.Default.BandwidthVisualsCustom;
            radioAutoBandwidth.Checked = Settings.Default.BandwidthVisualsAuto;

            //label2.Enabled = radioCustomSpeed.Checked;
            //label3.Enabled = radioCustomSpeed.Checked;
            //lblDownload.Enabled = radioCustomSpeed.Checked;
            //lblUpload.Enabled = radioCustomSpeed.Checked;
            //txtDownload.Enabled = radioCustomSpeed.Checked;
            //txtUpload.Enabled = radioCustomSpeed.Checked;

            grpBandwidthPreferences.Enabled = radioCustomSpeed.Checked;
            chkShowDisconnectedInterfaces.Checked = Settings.Default.ShowDisconnectedInterfaces;
            chkShowHiddenInterfaces.Checked = Settings.Default.ShowHiddenInterfaces;
            checkBoxStartup.Checked = Settings.Default.LoadOnStartup;
            btnRefreshIconSets.Height = cboIconSet.Height + 2;
            PopulateIconSetsCombo();
            PopulateLanguageCombo();
            settingsInitialized = true;
        }

        private Icon GetIconSetPreview(string setName)
        {
            if (_iconSetPreviews.TryGetValue(setName, out Icon cached))
                return cached;

            Icon icon;
            if (string.Equals(setName, TrayIconManager.DEFAULT_ICON_SET, StringComparison.OrdinalIgnoreCase))
            {
                icon = Properties.Resources.send_blue;
            }
            else
            {
                string path = Path.Combine(Application.StartupPath, "icons", setName, "send_blue.ico");
                icon = File.Exists(path) ? new Icon(path) : Properties.Resources.send_blue;
            }

            _iconSetPreviews[setName] = icon;
            return icon;
        }

        private void cboIconSet_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();

            string setName = cboIconSet.Items[e.Index].ToString();
            Icon preview = GetIconSetPreview(setName);

            int iconSize = e.Bounds.Height - 4;
            int iconX    = e.Bounds.Right - iconSize - 4;

            TextRenderer.DrawText(
                e.Graphics,
                setName,
                e.Font,
                new Rectangle(e.Bounds.Left + 4, e.Bounds.Top, iconX - e.Bounds.Left - 8, e.Bounds.Height),
                e.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            if (preview != null)
            {
                e.Graphics.DrawIcon(preview, new Rectangle(iconX, e.Bounds.Top + 2, iconSize, iconSize));
            }

            e.DrawFocusRectangle();
        }

        private void PopulateIconSetsCombo()
        {
            foreach (Icon icon in _iconSetPreviews.Values)
                icon?.Dispose();
            _iconSetPreviews.Clear();

            string current = cboIconSet.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(current))
            {
                current = Settings.Default.IconSet;
            }
            if (string.IsNullOrEmpty(current))
            {
                current = TrayIconManager.DEFAULT_ICON_SET;
            }

            cboIconSet.BeginUpdate();
            cboIconSet.Items.Clear();
            foreach (string set in TrayIconManager.GetAvailableIconSets())
            {
                cboIconSet.Items.Add(set);
            }
            // If the saved setting isn't in the validated list, add it anyway so it isn't silently lost.
            if (!string.IsNullOrEmpty(current) && cboIconSet.FindStringExact(current) < 0)
            {
                cboIconSet.Items.Add(current);
            }
            cboIconSet.EndUpdate();

            int idx = cboIconSet.FindStringExact(current);
            cboIconSet.SelectedIndex = idx >= 0 ? idx : 0;
        }

        private void OnOK(object sender, EventArgs e)
        {

            String strtmp;

            strtmp = textBoxDuration.Text.Trim();
            if (!String.IsNullOrEmpty(strtmp))
            {
                try
                {
                    int requestedDuration = Convert.ToInt32(strtmp, CultureInfo.InvariantCulture);
                    bool durationClamped = requestedDuration < NetworkStatsWorker.BlinkDurationMinimum;

                    nDuration = durationClamped ? NetworkStatsWorker.BlinkDurationMinimum : requestedDuration;
                    Settings.Default.BlinkDuration = nDuration;

                    if (durationClamped)
                    {
                        MessageBox.Show(Res.str_WarningContent, Res.str_WarningTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }
            }

            strtmp = txtDownload.Text.Trim();
            if (!String.IsNullOrEmpty(strtmp))
            {
                try
                {
                    nDownload = Convert.ToInt64(strtmp, CultureInfo.InvariantCulture);
                    if (nDownload <= 0)
                    {
                        nDownload = SettingsManager.DEFAULT_BANDWIDTH_BPS;
                    }

                    Settings.Default.BandwidthDownload = nDownload;
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }
            }

            strtmp = txtUpload.Text.Trim();
            if (!String.IsNullOrEmpty(strtmp))
            {
                try
                {
                    nUpload = Convert.ToInt64(strtmp, CultureInfo.InvariantCulture);
                    if (nUpload <= 0)
                    {
                        nUpload = SettingsManager.DEFAULT_BANDWIDTH_BPS;
                    }

                    Settings.Default.BandwidthUpload = nUpload;
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }
            }
            bool startupChanged = checkBoxStartup.Checked != Settings.Default.LoadOnStartup;
            if (startupChanged && !StartupManager.TrySetStartup(checkBoxStartup.Checked, out string startupErrorMessage))
            {
                checkBoxStartup.Checked = Settings.Default.LoadOnStartup;
                MessageBox.Show("Unable to update startup setting." + Environment.NewLine + startupErrorMessage, "GabNetStats", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Settings.Default.IconSet = cboIconSet.SelectedItem?.ToString() ?? TrayIconManager.DEFAULT_ICON_SET;

            Settings.Default.BandwidthVisualsDefault = radioDefault.Checked;
            Settings.Default.BandwidthVisualsCustom = radioCustomSpeed.Checked;
            Settings.Default.BandwidthVisualsAuto = radioAutoBandwidth.Checked;
            Settings.Default.LoadOnStartup = checkBoxStartup.Checked;

            Settings.Default.Save();

            this.Close();
        }

        private void radioCustomSpeed_CheckedChanged(object sender, EventArgs e)
        {
            //label2.Enabled = radioCustomSpeed.Checked;
            //label3.Enabled = radioCustomSpeed.Checked;
            //lblDownload.Enabled = radioCustomSpeed.Checked;
            //lblUpload.Enabled = radioCustomSpeed.Checked;
            //txtDownload.Enabled = radioCustomSpeed.Checked;
            //txtUpload.Enabled = radioCustomSpeed.Checked;

            grpBandwidthPreferences.Enabled = radioCustomSpeed.Checked;
            Settings.Default.BandwidthVisualsCustom = radioCustomSpeed.Checked;
        }

        private void radioAutoBandwidth_CheckedChanged(object sender, EventArgs e)
        {
            grpBandwidthPreferences.Enabled = radioCustomSpeed.Checked;
            Settings.Default.BandwidthVisualsAuto = radioAutoBandwidth.Checked;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Settings.Default.Reload();
        }

        private void radioDefault_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.BandwidthVisualsDefault = radioDefault.Checked;
            grpBandwidthPreferences.Enabled = radioCustomSpeed.Checked;
        }

        private void rbBits_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBits.Checked)
            {
                Settings.Default.BandwidthUnit = (int)SpeedUtils.BandwidthUnit.Bit;
            }
            else
            {
                Settings.Default.BandwidthUnit = (int)SpeedUtils.BandwidthUnit.Byte;
            }
            updateCombos();
        }

        private void cbDownload_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.BandwidthDownloadMultiplier = ((BandwidthItem)cbDownload.SelectedItem).multiplier;
        }

        private void cbUpload_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.BandwidthUploadMultiplier = ((BandwidthItem)cbUpload.SelectedItem).multiplier;
        }

        private void chkShowDisconnectedInterfaces_CheckedChanged(object sender, EventArgs e)
        {
            if (!settingsInitialized)
            {
                return;
            }

            Settings.Default.ShowDisconnectedInterfaces = chkShowDisconnectedInterfaces.Checked;
            FormMain main = (FormMain)Application.OpenForms["FormMain"];
            if (main != null)
            {
                main.PopulateNICs(main.NetworkAdaptersToolStripMenuItem);
            }
        }

        private void chkShowHiddenInterfaces_CheckedChanged(object sender, EventArgs e)
        {
            if (!settingsInitialized)
            {
                return;
            }

            Settings.Default.ShowHiddenInterfaces = chkShowHiddenInterfaces.Checked;
            FormMain main = (FormMain)Application.OpenForms["FormMain"];
            if (main != null)
            {
                main.PopulateNICs(main.NetworkAdaptersToolStripMenuItem);
            }
        }

        private void btnRefreshIconSets_Click(object sender, EventArgs e)
        {
            PopulateIconSetsCombo();
        }

        private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!settingsInitialized)
            {
                return;
            }

            string selectedCulture = ((LanguageItem)cbLanguage.SelectedItem).cultureName;
            if (string.Equals(selectedCulture, Settings.Default.Language ?? LanguageManager.DEFAULT_LANGUAGE, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Settings.Default.Language = selectedCulture;
            Settings.Default.Save();

            DialogResult result = MessageBox.Show(
                Res.str_RestartRequiredContent,
                Res.str_RestartRequiredTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Restart();
            }
        }
    }
}
