// After an original idea of Igor Tolmachev, IT Samples
//        http://www.itsamples.com
//
// Copyright (c) 2010

using System;
using System.Windows.Forms;
using GabNetStats.Properties;
using System.Globalization;

namespace GabNetStats
{
    public partial class SettingsForm : Form
    {
        private int nDuration = NetworkStatsWorker.BlinkDurationMinimum;
        private long nDownload = 12500000;
        private long nUpload = 12500000;
        
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

        public SettingsForm()
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
                    new BandwidthItem(Res.str_bit, (long)TrayIconManager.eBandwidthMultiplier.un),
                    new BandwidthItem(Res.str_Kbit, (long)TrayIconManager.eBandwidthMultiplier.K),
                    new BandwidthItem(Res.str_Mbit, (long)TrayIconManager.eBandwidthMultiplier.M),
                    new BandwidthItem(Res.str_Gbit, (long)TrayIconManager.eBandwidthMultiplier.G),
                    new BandwidthItem(Res.str_Tbit, (long)TrayIconManager.eBandwidthMultiplier.T)
                });

                cbUpload.Items.AddRange(new object[]
                { 
                    new BandwidthItem(Res.str_bit, (long)TrayIconManager.eBandwidthMultiplier.un),
                    new BandwidthItem(Res.str_Kbit, (long)TrayIconManager.eBandwidthMultiplier.K),
                    new BandwidthItem(Res.str_Mbit, (long)TrayIconManager.eBandwidthMultiplier.M),
                    new BandwidthItem(Res.str_Gbit, (long)TrayIconManager.eBandwidthMultiplier.G),
                    new BandwidthItem(Res.str_Tbit, (long)TrayIconManager.eBandwidthMultiplier.T)
                });
            }
            else
            {
                cbDownload.Items.AddRange(new object[]
                { 
                    new BandwidthItem(Res.str_Bytes, (long)TrayIconManager.eBandwidthMultiplier.un),
                    new BandwidthItem(Res.str_KiB, (long)TrayIconManager.eBandwidthMultiplier.K),
                    new BandwidthItem(Res.str_MiB, (long)TrayIconManager.eBandwidthMultiplier.M),
                    new BandwidthItem(Res.str_GiB, (long)TrayIconManager.eBandwidthMultiplier.G),
                    new BandwidthItem(Res.str_TiB, (long)TrayIconManager.eBandwidthMultiplier.T)
                });

                cbUpload.Items.AddRange(new object[]
                { 
                    new BandwidthItem(Res.str_Bytes, (long)TrayIconManager.eBandwidthMultiplier.un),
                    new BandwidthItem(Res.str_KiB, (long)TrayIconManager.eBandwidthMultiplier.K),
                    new BandwidthItem(Res.str_MiB, (long)TrayIconManager.eBandwidthMultiplier.M),
                    new BandwidthItem(Res.str_GiB, (long)TrayIconManager.eBandwidthMultiplier.G),
                    new BandwidthItem(Res.str_TiB, (long)TrayIconManager.eBandwidthMultiplier.T)
                });
            }
            cbDownload.ValueMember = "multiplier";
            cbDownload.DisplayMember = "name";
            cbDownload.SelectedIndex = (bdm==(long)TrayIconManager.eBandwidthMultiplier.un ? 0 : 
                                       (bdm==(long)TrayIconManager.eBandwidthMultiplier.K ? 1 : 
                                       (bdm==(long)TrayIconManager.eBandwidthMultiplier.M ? 2 : 
                                       (bdm==(long)TrayIconManager.eBandwidthMultiplier.G ? 3 :
                                       (bdm==(long)TrayIconManager.eBandwidthMultiplier.T ? 4 : 0)))));

            cbUpload.ValueMember = "multiplier";
            cbUpload.DisplayMember = "name";
            cbUpload.SelectedIndex =   (bum == (long)TrayIconManager.eBandwidthMultiplier.un ? 0 :
                                       (bum == (long)TrayIconManager.eBandwidthMultiplier.K ? 1 :
                                       (bum == (long)TrayIconManager.eBandwidthMultiplier.M ? 2 :
                                       (bum == (long)TrayIconManager.eBandwidthMultiplier.G ? 3 :
                                       (bum == (long)TrayIconManager.eBandwidthMultiplier.T ? 4 : 0)))));
        }

        private void OnLoad(object sender, EventArgs e)
        {
            Settings.Default.Reload();
            SettingsManager.ValidateSettings();

            textBoxDuration.Text = Settings.Default.BlinkDuration.ToString(CultureInfo.InvariantCulture);
            txtDownload.Text     = Settings.Default.BandwidthDownload.ToString(CultureInfo.InvariantCulture);
            txtUpload.Text       = Settings.Default.BandwidthUpload.ToString(CultureInfo.InvariantCulture);

            rbBits.Checked = Settings.Default.BandwidthUnit == (int)TrayIconManager.eBandwithUnit.bit;

            updateCombos();

            radioDefault.Checked = Settings.Default.BandwidthVisualsDefault;
            radioCustomSpeed.Checked = Settings.Default.BandwidthVisualsCustom;

            //label2.Enabled = radioCustomSpeed.Checked;
            //label3.Enabled = radioCustomSpeed.Checked;
            //lblDownload.Enabled = radioCustomSpeed.Checked;
            //lblUpload.Enabled = radioCustomSpeed.Checked;
            //txtDownload.Enabled = radioCustomSpeed.Checked;
            //txtUpload.Enabled = radioCustomSpeed.Checked;

            grpBandwidthPreferences.Enabled = radioCustomSpeed.Checked;
            chkShowDisconnectedInterfaces.Checked = Settings.Default.ShowDisconnectedInterfaces;
            checkBoxStartup.Checked = Settings.Default.LoadOnStartup;
            settingsInitialized = true;
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
                    nDownload = Convert.ToInt32(strtmp, CultureInfo.InvariantCulture);
                    if (nDownload <= 0)
                    {
                        nDownload = 12500000;
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
                    nUpload = Convert.ToInt32(strtmp, CultureInfo.InvariantCulture);
                    if (nUpload <= 0)
                    {
                        nUpload = 12500000;
                    }

                    Settings.Default.BandwidthUpload = nUpload;
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }
            }




            StartupManager.SetStartup(checkBoxStartup.Checked);

            Settings.Default.IconSet = txtIconSet.Text;

            Settings.Default.BandwidthVisualsDefault = radioDefault.Checked;
            Settings.Default.BandwidthVisualsCustom = radioCustomSpeed.Checked;
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

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Settings.Default.Reload();
        }

        private void radioDefault_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.BandwidthVisualsDefault = radioDefault.Checked;
        }

        private void rbBits_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBits.Checked)
            {
                Settings.Default.BandwidthUnit = (int)TrayIconManager.eBandwithUnit.bit;
            }
            else
            {
                Settings.Default.BandwidthUnit = (int)TrayIconManager.eBandwithUnit.Byte;
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
            MainForm main = (MainForm)Application.OpenForms["MainForm"];
            if (main != null)
            {
                main.PopulateNICs(main.NetworkAdaptersToolStripMenuItem);
            }
        }
    }
}
