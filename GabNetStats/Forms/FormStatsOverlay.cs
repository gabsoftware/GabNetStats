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

        private static int counter = 0;
        internal static FormNetworkDetails frmAdv;
        private bool suppressLocationPersistence;

        static FormStatsOverlay()
        {
            nfi.NumberDecimalDigits = 0; //we don't want decimals !
        }

        public FormStatsOverlay()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            chkAutoClose.Checked = Settings.Default.AutoCloseBalloon;
            nudAutoClose.Value = Settings.Default.AutoCloseBalloonAfter;
            UpdateTrackerCapacity();
            ApplyHistoryToTracker();
        }

        private void BallonTimer_Tick(object sender, EventArgs e)
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


            //gabtracker4
            gabTracker1.Feeds[0].Value = Math.Round(NetworkStatsWorker.lAvgSpeedReception / BYTES_PER_KIBIBYTE, 2);
            gabTracker1.Feeds[1].Value = Math.Round(NetworkStatsWorker.lAvgSpeedEmission / BYTES_PER_KIBIBYTE, 2);

            fSpeedReception = SpeedUtils.computeSpeed(NetworkStatsWorker.rawSpeedReception, ref sSpeedReception, 1);
            fSpeedEmission  = SpeedUtils.computeSpeed(NetworkStatsWorker.rawSpeedEmission , ref sSpeedEmission , 1);

            fAvgSpeedReception = SpeedUtils.computeSpeed(NetworkStatsWorker.lAvgSpeedReception, ref sAvgSpeedReception, 1);
            fAvgSpeedEmission  = SpeedUtils.computeSpeed(NetworkStatsWorker.lAvgSpeedEmission , ref sAvgSpeedEmission , 1);

            fReceived = SpeedUtils.computeSpeed(NetworkStatsWorker.bytesReceived, ref sReceived, 1);
            fSent     = SpeedUtils.computeSpeed(NetworkStatsWorker.bytesSent    , ref sSent    , 1);

            builder.AppendFormat("{0} : \t{1} {2}/s\n", str_RawReceptionSpeed, fSpeedReception.ToString(Math.Floor(fSpeedReception) != fSpeedReception ? CultureInfo.CurrentCulture.NumberFormat : nfi), sSpeedReception);
            builder.AppendFormat("{0} : \t{1} {2}/s\n", str_RawEmissionSpeed, fSpeedEmission.ToString(Math.Floor(fSpeedEmission) != fSpeedEmission ? CultureInfo.CurrentCulture.NumberFormat : nfi), sSpeedEmission);
            builder.AppendLine();
            builder.AppendFormat("{0} : \t{1} {2}/s\n", str_AvgReceptionSpeed, fAvgSpeedReception.ToString(Math.Floor(fAvgSpeedReception) != fAvgSpeedReception ? CultureInfo.CurrentCulture.NumberFormat : nfi), sAvgSpeedReception);
            builder.AppendFormat("{0} : \t{1} {2}/s\n", str_AvgEmissionSpeed, fAvgSpeedEmission.ToString(Math.Floor(fAvgSpeedEmission) != fAvgSpeedEmission ? CultureInfo.CurrentCulture.NumberFormat : nfi), sAvgSpeedEmission);
            builder.AppendLine();
            builder.AppendFormat("{0} : \t{1} {2} ({3} {4})\n", new Object[] { str_Received, NetworkStatsWorker.bytesReceived.ToString("n", nfi), str_Bytes, fReceived.ToString(Math.Floor(fReceived) != fReceived ? CultureInfo.CurrentCulture.NumberFormat : nfi), sReceived });
            builder.AppendFormat("{0} : \t{1} {2} ({3} {4})\n", new Object[] { str_Sent, NetworkStatsWorker.bytesSent.ToString("n", nfi), str_Bytes, fSent.ToString(Math.Floor(fSent) != fSent ? CultureInfo.CurrentCulture.NumberFormat : nfi), sSent });

            lblStatisticsData.Text = builder.ToString();

            if (Settings.Default.AutoCloseBalloon)
            {
                counter += BallonTimer.Interval;
                if (counter >= Settings.Default.AutoCloseBalloonAfter * 1000)
                {
                    counter = 0;
                    this.Hide();
                }
            }
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

        private void frmBalloon_FormClosing(object sender, FormClosingEventArgs e)
        {
            counter = 0;
            Settings.Default.Save();
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
                BallonTimer.Interval = Settings.Default.BlinkDuration;
                BallonTimer.Start();
            }
            else
            {
                counter = 0;
                BallonTimer.Stop();
            }
        }

        private void nudAutoClose_ValueChanged(object sender, EventArgs e)
        {
            counter = 0;
            Settings.Default.AutoCloseBalloonAfter = nudAutoClose.Value;
            Settings.Default.Save();
        }

        private void tbTrans_Scroll(object sender, EventArgs e)
        {
            Settings.Default.BalloonOpacitySlider = tbTrans.Value;
            Settings.Default.BalloonOpacity = (double)tbTrans.Value / 100;
        }

        private void tbTrans_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.Save();
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
            if (frmAdv == null || !frmAdv.Created)
            {
                frmAdv = new FormNetworkDetails();
            }
            frmAdv.Show();
        }

        private void chkAutoClose_CheckStateChanged(object sender, EventArgs e)
        {
            counter = 0;
            Settings.Default.AutoCloseBalloon = chkAutoClose.Checked;
            Settings.Default.Save();
        }

        private void frmBalloon_Resize(object sender, EventArgs e)
        {
            UpdateTrackerCapacity();
            ApplyHistoryToTracker();
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
            Settings.Default.Save();
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
