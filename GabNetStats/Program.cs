using System;
using System.Windows.Forms;
using System.Threading;

namespace GabNetStats
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static Mutex m_Mutex;

        /// <summary>Set to true when Windows initiates a shutdown, to suppress restart on crash.</summary>
        public static bool IsWindowsShuttingDown { get; set; }

        /// <summary>Set to true when --show-network-details was passed on the command line.</summary>
        public static bool ShowNetworkDetailsOnStart { get; private set; }

        /// <summary>Set to true when --show-statistics was passed on the command line.</summary>
        public static bool ShowStatisticsOnStart { get; private set; }

        [STAThread]
        static void Main()
        {
            try
            {
                foreach (string arg in Environment.GetCommandLineArgs())
                {
                    if (arg.Equals("--show-network-details", StringComparison.OrdinalIgnoreCase))
                        ShowNetworkDetailsOnStart = true;
                    if (arg.Equals("--show-statistics", StringComparison.OrdinalIgnoreCase))
                        ShowStatisticsOnStart = true;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                m_Mutex = new Mutex(true, "GabNetStatsMutex");

                Thread.CurrentThread.Name = "MainThread";

                if (m_Mutex.WaitOne(0, false))
                {
                    Application.Run(new FormMain());
                }
            }
            catch (Exception ex)
            {
                if (!IsWindowsShuttingDown && ex.GetType() != typeof(ThreadAbortException))
                {
                    MessageBox.Show(
                        Res.str_ErrorCrash +
                        "\n\n" + "Thread : " +
                        Thread.CurrentThread.Name +
                        "\n\n" +
                        ex.ToString(), "GabNetStats", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Restart();
                }
            }
        }
    }
}