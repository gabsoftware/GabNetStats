using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using GabNetStats.Properties;

namespace GabNetStats
{
    public partial class FormStatsOverlay : Form
    {
        //
        //  Constants
        //
        private const double BYTES_PER_KIBIBYTE      = 1024D;
        private const int    TRACKER_PIXELS_PER_SAMPLE = 10;

        private static string str_RawReceptionSpeed = Res.str_RawReceptionSpeed;
        private static string str_RawEmissionSpeed = Res.str_RawEmissionSpeed;
        private static string str_AvgReceptionSpeed = Res.str_AvgReceptionSpeed;
        private static string str_AvgEmissionSpeed = Res.str_AvgEmissionSpeed;
        private static string str_Received = Res.str_Received;
        private static string str_Sent = Res.str_Sent;
        private static string str_Bytes = Res.str_Bytes;

        private static NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();

        private int counter;
        private bool suppressLocationPersistence;
        private bool suppressSizePersistence;
        private bool suppressSettingsPersistence;

        static FormStatsOverlay()
        {
            nfi.NumberDecimalDigits = 0; //we don't want decimals !
        }

        public FormStatsOverlay()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            try
            {
                suppressSettingsPersistence = true;
                chkAutoClose.Checked = Settings.Default.AutoCloseBalloon;
                nudAutoClose.Value = Settings.Default.AutoCloseBalloonAfter;
                tbTrans.Value = LoadOpacitySliderValue();
                ApplyOpacityFromSlider();
                ApplySizeWithoutPersistence(LoadPreferredSize(this.Size));
            }
            finally
            {
                suppressSettingsPersistence = false;
            }
            gabTracker1.CurrentValuesNeeded += gabTracker1_CurrentValuesNeeded;
            UpdateTrackerCapacity();
            ApplyHistoryToTracker();
        }

        private void BalloonTimer_Tick(object sender, EventArgs e)
        {
            string sSpeedReception = String.Empty;
            string sSpeedEmission = String.Empty;
            string sAvgSpeedReception = String.Empty;
            string sAvgSpeedEmission = String.Empty;
            string sReceived = String.Empty;
            string sSent = String.Empty;

            double fSpeedReception = 0;
            double fSpeedEmission = 0;
            double fAvgSpeedReception = 0;
            double fAvgSpeedEmission = 0;
            double fReceived = 0;
            double fSent = 0;

            StringBuilder builder = new StringBuilder();

            fSpeedReception = SpeedUtils.ComputeSpeed(NetworkStatsWorker.rawSpeedReception, ref sSpeedReception, 1);
            fSpeedEmission  = SpeedUtils.ComputeSpeed(NetworkStatsWorker.rawSpeedEmission , ref sSpeedEmission , 1);

            fAvgSpeedReception = SpeedUtils.ComputeSpeed(NetworkStatsWorker.lAvgSpeedReception, ref sAvgSpeedReception, 1);
            fAvgSpeedEmission  = SpeedUtils.ComputeSpeed(NetworkStatsWorker.lAvgSpeedEmission , ref sAvgSpeedEmission , 1);

            fReceived = SpeedUtils.ComputeSpeed(NetworkStatsWorker.bytesReceived, ref sReceived, 1);
            fSent     = SpeedUtils.ComputeSpeed(NetworkStatsWorker.bytesSent    , ref sSent    , 1);

            builder.AppendFormat(CultureInfo.CurrentCulture, "{0} : \t{1} {2}/s\n", str_RawReceptionSpeed, fSpeedReception.ToString(Math.Floor(fSpeedReception) != fSpeedReception ? CultureInfo.CurrentCulture.NumberFormat : nfi), sSpeedReception);
            builder.AppendFormat(CultureInfo.CurrentCulture, "{0} : \t{1} {2}/s\n", str_RawEmissionSpeed, fSpeedEmission.ToString(Math.Floor(fSpeedEmission) != fSpeedEmission ? CultureInfo.CurrentCulture.NumberFormat : nfi), sSpeedEmission);
            builder.AppendLine();
            builder.AppendFormat(CultureInfo.CurrentCulture, "{0} : \t{1} {2}/s\n", str_AvgReceptionSpeed, fAvgSpeedReception.ToString(Math.Floor(fAvgSpeedReception) != fAvgSpeedReception ? CultureInfo.CurrentCulture.NumberFormat : nfi), sAvgSpeedReception);
            builder.AppendFormat(CultureInfo.CurrentCulture, "{0} : \t{1} {2}/s\n", str_AvgEmissionSpeed, fAvgSpeedEmission.ToString(Math.Floor(fAvgSpeedEmission) != fAvgSpeedEmission ? CultureInfo.CurrentCulture.NumberFormat : nfi), sAvgSpeedEmission);
            builder.AppendLine();
            builder.AppendFormat(CultureInfo.CurrentCulture, "{0} : \t{1} {2} ({3} {4})\n", new Object[] { str_Received, NetworkStatsWorker.bytesReceived.ToString("n", nfi), str_Bytes, fReceived.ToString(Math.Floor(fReceived) != fReceived ? CultureInfo.CurrentCulture.NumberFormat : nfi), sReceived });
            builder.AppendFormat(CultureInfo.CurrentCulture, "{0} : \t{1} {2} ({3} {4})\n", new Object[] { str_Sent, NetworkStatsWorker.bytesSent.ToString("n", nfi), str_Bytes, fSent.ToString(Math.Floor(fSent) != fSent ? CultureInfo.CurrentCulture.NumberFormat : nfi), sSent });

            lblStatisticsData.Text = builder.ToString();

            if (Settings.Default.AutoCloseBalloon)
            {
                counter += BalloonTimer.Interval;
                if (counter >= Settings.Default.AutoCloseBalloonAfter * 1000)
                {
                    counter = 0;
                    this.Hide();
                }
            }
        }

        private void gabTracker1_CurrentValuesNeeded(object sender, EventArgs e)
        {
            if (gabTracker1 == null || gabTracker1.Feeds.Count < 2)
            {
                return;
            }

            NetworkStatsWorker.SpeedHistorySample sample = NetworkStatsWorker.ConsumePendingGraphAverage();
            gabTracker1.Feeds[0].Value = Math.Round(sample.DownloadKib, 2);
            gabTracker1.Feeds[1].Value = Math.Round(sample.UploadKib, 2);
        }

        internal void EnsurePreferredLocation()
        {
            Point preferred = LoadPreferredLocation(this.Size);
            ApplyLocationWithoutPersistence(preferred);
        }

        private static Point LoadPreferredLocation(Size formSize)
        {
            int savedX = Settings.Default.BalloonLocationX;
            int savedY = Settings.Default.BalloonLocationY;
            if (savedX >= 0 && savedY >= 0)
            {
                Point saved = new Point(savedX, savedY);
                if (IsLocationVisible(saved, formSize))
                {
                    return saved;
                }
            }

            return GetDefaultLocation(formSize);
        }

        private static Size LoadPreferredSize(Size defaultSize)
        {
            int savedWidth = Settings.Default.BalloonWidth;
            int savedHeight = Settings.Default.BalloonHeight;
            if (savedWidth > 0 && savedHeight > 0)
            {
                return new Size(savedWidth, savedHeight);
            }

            return defaultSize;
        }

        private static Point GetDefaultLocation(Size formSize)
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            int x = workingArea.Right - formSize.Width - SystemInformation.FixedFrameBorderSize.Width;
            int y = workingArea.Bottom - formSize.Height - SystemInformation.FixedFrameBorderSize.Height;
            x = Math.Max(workingArea.Left, x);
            y = Math.Max(workingArea.Top, y);
            return new Point(x, y);
        }

        private static bool IsLocationVisible(Point location, Size formSize)
        {
            Rectangle windowBounds = new Rectangle(location, formSize);
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(windowBounds))
                {
                    return true;
                }
            }
            return false;
        }

        private void ApplyLocationWithoutPersistence(Point location)
        {
            suppressLocationPersistence = true;
            try
            {
                this.Location = location;
            }
            finally
            {
                suppressLocationPersistence = false;
            }
        }

        private void ApplySizeWithoutPersistence(Size size)
        {
            suppressSizePersistence = true;
            try
            {
                this.Size = size;
            }
            finally
            {
                suppressSizePersistence = false;
            }
        }

        private void frmBalloon_FormClosing(object sender, FormClosingEventArgs e)
        {
            counter = 0;
            SettingsManager.FlushPendingSave();
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            // For WindowsShutDown, ApplicationExitCall, etc., let it close
        }

        private void frmBalloon_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                counter = 0;
                BalloonTimer.Interval = Settings.Default.BlinkDuration;
                BalloonTimer.Start();
            }
            else
            {
                counter = 0;
                BalloonTimer.Stop();
            }
        }

        private void nudAutoClose_ValueChanged(object sender, EventArgs e)
        {
            counter = 0;
            Settings.Default.AutoCloseBalloonAfter = nudAutoClose.Value;
            if (!suppressSettingsPersistence)
            {
                SettingsManager.ScheduleSave();
            }
        }

        private void tbTrans_Scroll(object sender, EventArgs e)
        {
            UpdateOpacityFromSlider();
        }

        private void tbTrans_ValueChanged(object sender, EventArgs e)
        {
            UpdateOpacityFromSlider();
            if (!suppressSettingsPersistence)
            {
                SettingsManager.ScheduleSave();
            }
        }

        private void UpdateOpacityFromSlider()
        {
            Settings.Default.BalloonOpacitySlider = tbTrans.Value;
            Settings.Default.BalloonOpacity = (double)tbTrans.Value / 100;
            ApplyOpacityFromSlider();
        }

        private void ApplyOpacityFromSlider()
        {
            this.Opacity = (double)tbTrans.Value / 100;
        }

        private int ClampOpacitySliderValue(int value)
        {
            return Math.Max(tbTrans.Minimum, Math.Min(tbTrans.Maximum, value));
        }

        private int LoadOpacitySliderValue()
        {
            if (Settings.Default.BalloonOpacitySlider == tbTrans.Maximum
             && Settings.Default.BalloonOpacity < 1)
            {
                return ClampOpacitySliderValue((int)Math.Round(Settings.Default.BalloonOpacity * 100));
            }

            return ClampOpacitySliderValue(Settings.Default.BalloonOpacitySlider);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
                Color.Black, 0, ButtonBorderStyle.None,
                Color.Black, 0, ButtonBorderStyle.None,
                Color.Black, 0, ButtonBorderStyle.None,
                Color.Black, 1, ButtonBorderStyle.Dashed);
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            FormMain mf = (FormMain)Application.OpenForms["FormMain"];
            mf?.ShowNetworkDetails();
        }

        internal void DisableAutoClose()
        {
            chkAutoClose.Checked = false;
        }

        private void chkAutoClose_CheckStateChanged(object sender, EventArgs e)
        {
            counter = 0;
            Settings.Default.AutoCloseBalloon = chkAutoClose.Checked;
            if (!suppressSettingsPersistence)
            {
                SettingsManager.ScheduleSave();
            }
        }

        private void frmBalloon_Resize(object sender, EventArgs e)
        {
            UpdateTrackerCapacity();
            ApplyHistoryToTracker();

            if (suppressSizePersistence)
            {
                return;
            }

            Settings.Default.BalloonWidth = this.Width;
            Settings.Default.BalloonHeight = this.Height;
            SettingsManager.ScheduleSave();
        }

        private void btn_settings_Click(object sender, EventArgs e)
        {
            FormMain mf = (FormMain)Application.OpenForms["FormMain"];
            mf?.showSettings();
        }

        private void frmBalloon_LocationChanged(object sender, EventArgs e)
        {
            if (suppressLocationPersistence)
            {
                return;
            }

            Settings.Default.BalloonLocationX = this.Left;
            Settings.Default.BalloonLocationY = this.Top;
            SettingsManager.ScheduleSave();
        }

        private void UpdateTrackerCapacity()
        {
            if (gabTracker1 == null)
            {
                return;
            }

            int desiredCapacity = Math.Max(1, this.Width / TRACKER_PIXELS_PER_SAMPLE + 1);
            gabTracker1.MaxDataInMemory = desiredCapacity;
        }

        private void ApplyHistoryToTracker()
        {
            if (gabTracker1 == null || gabTracker1.Feeds.Count < 2)
            {
                return;
            }

            List<NetworkStatsWorker.SpeedHistorySample> snapshot = NetworkStatsWorker.GetHistorySnapshot(gabTracker1.MaxDataInMemory);
            if (snapshot.Count == 0)
            {
                return;
            }

            Queue<double> downloadData = gabTracker1.Feeds[0].Data;
            Queue<double> uploadData = gabTracker1.Feeds[1].Data;
            downloadData.Clear();
            uploadData.Clear();

            foreach (NetworkStatsWorker.SpeedHistorySample sample in snapshot)
            {
                downloadData.Enqueue(sample.DownloadKib);
                uploadData.Enqueue(sample.UploadKib);
            }

            gabTracker1.Invalidate();
        }
    }
}
