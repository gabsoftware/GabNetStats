using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using GabNetStats.Properties;

namespace GabNetStats
{
    public partial class frmBalloon : Form
    {

        private static long rawSpeedReception {get; set;}
        private static long rawSpeedEmission { get; set; }
        private static long lAvgSpeedReception { get; set; }
        private static long lAvgSpeedEmission { get; set; }
        private static long bytesReceived { get; set; }
        private static long bytesSent { get; set; }


        private static string str_RawReceptionSpeed = Res.str_RawReceptionSpeed;
        private static string str_RawEmissionSpeed = Res.str_RawEmissionSpeed;
        private static string str_AvgReceptionSpeed = Res.str_AvgReceptionSpeed;
        private static string str_AvgEmissionSpeed = Res.str_AvgEmissionSpeed;
        private static string str_Received = Res.str_Received;
        private static string str_Sent = Res.str_Sent;
        private static string str_Bytes = Res.str_Bytes;

        private static NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();

        private static int counter = 0;
        internal static frmAdvanced frmAdv;
        private bool suppressLocationPersistence;

        static frmBalloon()
        {
            rawSpeedReception = 0;
            rawSpeedEmission = 0;
            lAvgSpeedReception = 0;
            lAvgSpeedEmission = 0;
            bytesReceived = 0;
            bytesSent = 0;
            nfi.NumberDecimalDigits = 0; //we don't want decimals !
        }

        public frmBalloon()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
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
            gabTracker1.Feeds[0].Value = Math.Round(lAvgSpeedReception / 1024D, 2);
            gabTracker1.Feeds[1].Value = Math.Round(lAvgSpeedEmission / 1024D, 2);

            fSpeedReception = MainForm.computeSpeed(rawSpeedReception, ref sSpeedReception, 1);
            fSpeedEmission  = MainForm.computeSpeed(rawSpeedEmission , ref sSpeedEmission , 1);

            fAvgSpeedReception = MainForm.computeSpeed(lAvgSpeedReception, ref sAvgSpeedReception, 1);
            fAvgSpeedEmission  = MainForm.computeSpeed(lAvgSpeedEmission , ref sAvgSpeedEmission , 1);

            fReceived = MainForm.computeSpeed(bytesReceived, ref sReceived, 1);
            fSent     = MainForm.computeSpeed(bytesSent    , ref sSent    , 1);

            builder.Remove(0, builder.Length);
            builder.AppendFormat("{0} : \t{1} {2}/s\n", str_RawReceptionSpeed, fSpeedReception.ToString(Math.Floor(fSpeedReception) != fSpeedReception ? CultureInfo.CurrentCulture.NumberFormat : nfi), sSpeedReception);
            builder.AppendFormat("{0} : \t{1} {2}/s\n", str_RawEmissionSpeed, fSpeedEmission.ToString(Math.Floor(fSpeedEmission) != fSpeedEmission ? CultureInfo.CurrentCulture.NumberFormat : nfi), sSpeedEmission);
            builder.AppendLine();
            builder.AppendFormat("{0} : \t{1} {2}/s\n", str_AvgReceptionSpeed, fAvgSpeedReception.ToString(Math.Floor(fAvgSpeedReception) != fAvgSpeedReception ? CultureInfo.CurrentCulture.NumberFormat : nfi), sAvgSpeedReception);
            builder.AppendFormat("{0} : \t{1} {2}/s\n", str_AvgEmissionSpeed, fAvgSpeedEmission.ToString(Math.Floor(fAvgSpeedEmission) != fAvgSpeedEmission ? CultureInfo.CurrentCulture.NumberFormat : nfi), sAvgSpeedEmission);
            builder.AppendLine();
            builder.AppendFormat("{0} : \t{1} {2} ({3} {4})\n", new Object[] { str_Received, bytesReceived.ToString("n", nfi), str_Bytes, fReceived.ToString(Math.Floor(fReceived) != fReceived ? CultureInfo.CurrentCulture.NumberFormat : nfi), sReceived });
            builder.AppendFormat("{0} : \t{1} {2} ({3} {4})\n", new Object[] { str_Sent, bytesSent.ToString("n", nfi), str_Bytes, fSent.ToString(Math.Floor(fSent) != fSent ? CultureInfo.CurrentCulture.NumberFormat : nfi), sSent });

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

        internal static void UpdateInfos(
            long prawSpeedReception,
            long prawSpeedEmission,
            long plAvgSpeedReception,
            long plAvgSpeedEmission,
            long pbytesReceived,
            long pbytesSent)
        {
            rawSpeedReception = prawSpeedReception;
            rawSpeedEmission = prawSpeedEmission;
            lAvgSpeedReception = plAvgSpeedReception;
            lAvgSpeedEmission = plAvgSpeedEmission;
            bytesReceived = pbytesReceived;
            bytesSent = pbytesSent;
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
            e.Cancel = true;
            this.Hide();
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
                frmAdv = new frmAdvanced();
            }
            frmAdv.Show();
        }

        private void chkAutoClose_CheckStateChanged(object sender, EventArgs e)
        {
            counter = 0;
            Settings.Default.Save();
        }

        private void frmBalloon_Resize(object sender, EventArgs e)
        {
            this.gabTracker1.MaxDataInMemory = this.Width / 10 + 1;
        }

        private void frmBalloon_Load(object sender, EventArgs e)
        {

        }

        private void btn_settings_Click(object sender, EventArgs e)
        {
            MainForm mf = (MainForm)Application.OpenForms["MainForm"];
            mf.showSettings();
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
    }
}
