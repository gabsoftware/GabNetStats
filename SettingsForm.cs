// After an original idea of Igor Tolmachev, IT Samples
//        http://www.itsamples.com
//
// Copyright (c) 2010

using System;
using System.Windows.Forms;
using Microsoft.Win32;
using GabNetStats.Properties;
using System.Globalization;

namespace GabNetStats
{
    public partial class SettingsForm : Form
    {
        private int nDuration = 100;
        private long nDownload = 12500000;
        private long nUpload = 12500000;
        private long nDefaultMultiplier = (long)MainForm.eBandwidthMultiplier.un;
        private int nDefaultUnit = (int)MainForm.eBandwithUnit.Byte;
        
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
                    new BandwidthItem(Res.str_bit, (long)MainForm.eBandwidthMultiplier.un),
                    new BandwidthItem(Res.str_Kbit, (long)MainForm.eBandwidthMultiplier.K),
                    new BandwidthItem(Res.str_Mbit, (long)MainForm.eBandwidthMultiplier.M),
                    new BandwidthItem(Res.str_Gbit, (long)MainForm.eBandwidthMultiplier.G),
                    new BandwidthItem(Res.str_Tbit, (long)MainForm.eBandwidthMultiplier.T)
                });

                cbUpload.Items.AddRange(new object[]
                { 
                    new BandwidthItem(Res.str_bit, (long)MainForm.eBandwidthMultiplier.un),
                    new BandwidthItem(Res.str_Kbit, (long)MainForm.eBandwidthMultiplier.K),
                    new BandwidthItem(Res.str_Mbit, (long)MainForm.eBandwidthMultiplier.M),
                    new BandwidthItem(Res.str_Gbit, (long)MainForm.eBandwidthMultiplier.G),
                    new BandwidthItem(Res.str_Tbit, (long)MainForm.eBandwidthMultiplier.T)
                });
            }
            else
            {
                cbDownload.Items.AddRange(new object[]
                { 
                    new BandwidthItem(Res.str_Bytes, (long)MainForm.eBandwidthMultiplier.un),
                    new BandwidthItem(Res.str_KiB, (long)MainForm.eBandwidthMultiplier.K),
                    new BandwidthItem(Res.str_MiB, (long)MainForm.eBandwidthMultiplier.M),
                    new BandwidthItem(Res.str_GiB, (long)MainForm.eBandwidthMultiplier.G),
                    new BandwidthItem(Res.str_TiB, (long)MainForm.eBandwidthMultiplier.T)
                });

                cbUpload.Items.AddRange(new object[]
                { 
                    new BandwidthItem(Res.str_Bytes, (long)MainForm.eBandwidthMultiplier.un),
                    new BandwidthItem(Res.str_KiB, (long)MainForm.eBandwidthMultiplier.K),
                    new BandwidthItem(Res.str_MiB, (long)MainForm.eBandwidthMultiplier.M),
                    new BandwidthItem(Res.str_GiB, (long)MainForm.eBandwidthMultiplier.G),
                    new BandwidthItem(Res.str_TiB, (long)MainForm.eBandwidthMultiplier.T)
                });
            }
            cbDownload.ValueMember = "multiplier";
            cbDownload.DisplayMember = "name";
            cbDownload.SelectedIndex = (bdm==(long)MainForm.eBandwidthMultiplier.un ? 0 : 
                                       (bdm==(long)MainForm.eBandwidthMultiplier.K ? 1 : 
                                       (bdm==(long)MainForm.eBandwidthMultiplier.M ? 2 : 
                                       (bdm==(long)MainForm.eBandwidthMultiplier.G ? 3 :
                                       (bdm==(long)MainForm.eBandwidthMultiplier.T ? 4 : 0)))));

            cbUpload.ValueMember = "multiplier";
            cbUpload.DisplayMember = "name";
            cbUpload.SelectedIndex =   (bum == (long)MainForm.eBandwidthMultiplier.un ? 0 :
                                       (bum == (long)MainForm.eBandwidthMultiplier.K ? 1 :
                                       (bum == (long)MainForm.eBandwidthMultiplier.M ? 2 :
                                       (bum == (long)MainForm.eBandwidthMultiplier.G ? 3 :
                                       (bum == (long)MainForm.eBandwidthMultiplier.T ? 4 : 0)))));
        }

        private void OnLoad(object sender, EventArgs e)
        {
            if (Settings.Default.BlinkDuration > 10)
            {
                textBoxDuration.Text = Settings.Default.BlinkDuration.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                Settings.Default.BlinkDuration = nDuration;
                textBoxDuration.Text = Settings.Default.BlinkDuration.ToString(CultureInfo.InvariantCulture);
            }

            if (Settings.Default.BandwidthDownloadMultiplier != (long)MainForm.eBandwidthMultiplier.un
             && Settings.Default.BandwidthDownloadMultiplier != (long)MainForm.eBandwidthMultiplier.K
             && Settings.Default.BandwidthDownloadMultiplier != (long)MainForm.eBandwidthMultiplier.M
             && Settings.Default.BandwidthDownloadMultiplier != (long)MainForm.eBandwidthMultiplier.G
             && Settings.Default.BandwidthDownloadMultiplier != (long)MainForm.eBandwidthMultiplier.T)
            {
                Settings.Default.BandwidthDownloadMultiplier = nDefaultMultiplier;
            }

            if (Settings.Default.BandwidthUploadMultiplier != (long)MainForm.eBandwidthMultiplier.un
             && Settings.Default.BandwidthUploadMultiplier != (long)MainForm.eBandwidthMultiplier.K
             && Settings.Default.BandwidthUploadMultiplier != (long)MainForm.eBandwidthMultiplier.M
             && Settings.Default.BandwidthUploadMultiplier != (long)MainForm.eBandwidthMultiplier.G
             && Settings.Default.BandwidthUploadMultiplier != (long)MainForm.eBandwidthMultiplier.T)
            {
                Settings.Default.BandwidthUploadMultiplier = nDefaultMultiplier;
            }

            if (Settings.Default.BandwidthUnit != (int)MainForm.eBandwithUnit.bit && Settings.Default.BandwidthUnit != (int)MainForm.eBandwithUnit.Byte)
            {
                Settings.Default.BandwidthUnit = nDefaultUnit;
            }

            if (Settings.Default.BandwidthDownload > 0)
            {
                txtDownload.Text = Settings.Default.BandwidthDownload.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                Settings.Default.BandwidthDownload = nDownload;
                txtDownload.Text = Settings.Default.BandwidthDownload.ToString(CultureInfo.InvariantCulture);
            }

            if (Settings.Default.BandwidthUpload > 0)
            {
                txtUpload.Text = Settings.Default.BandwidthUpload.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                Settings.Default.BandwidthUpload = nUpload;
                txtUpload.Text = Settings.Default.BandwidthUpload.ToString(CultureInfo.InvariantCulture);
            }

            rbBits.Checked = Settings.Default.BandwidthUnit == (int)MainForm.eBandwithUnit.bit;

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
        }

        private void OnOK(object sender, EventArgs e)
        {
           
            String strtmp;
            
            strtmp = textBoxDuration.Text.Trim();
            if (!String.IsNullOrEmpty(strtmp))
            {
                try
                {
                    nDuration = Convert.ToInt32(strtmp, CultureInfo.InvariantCulture);
                    if (nDuration < 10)
                    {
                        nDuration = 10;
                    }

                    Settings.Default.BlinkDuration = nDuration;
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }
            }

            if (nDuration < 50)
            {
                MessageBox.Show(Res.str_WarningContent, Res.str_WarningTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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




            RegistryKey hStartKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (hStartKey != null)
            {
                try
                {
                    if (checkBoxStartup.Checked)
                    {
                        String strPath = Application.ExecutablePath;
                        strPath = "\"" + strPath + "\"";

                        hStartKey.SetValue("GabNetStats", strPath);
                    }
                    else
                    {
                        hStartKey.DeleteValue("GabNetStats");
                    }
                }
                catch
                {
                    //Catching exceptions is for communists
                }

                hStartKey.Close();
            }


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
                Settings.Default.BandwidthUnit = (int)MainForm.eBandwithUnit.bit;
            }
            else
            {
                Settings.Default.BandwidthUnit = (int)MainForm.eBandwithUnit.Byte;
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
    }
}