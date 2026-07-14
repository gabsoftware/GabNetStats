using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using GabNetStats.Properties;

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

        /// <summary>Set to true when --no-upgrade-message was passed on the command line.</summary>
        public static bool NoUpgradeMessage { get; private set; }

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
                    if (arg.Equals("--no-upgrade-message", StringComparison.OrdinalIgnoreCase))
                        NoUpgradeMessage = true;
                }

                LanguageManager.ApplyLanguage(Settings.Default.Language);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += Application_ThreadException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

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
                    ReportFatalException(ex, true);
                }
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ReportFatalException(e.Exception, true);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ReportFatalException(e.ExceptionObject as Exception, false);
        }

        internal static void ReportWorkerException(Exception ex)
        {
            ReportFatalException(ex, false);
        }

        private static void ReportFatalException(Exception ex, bool showMessage)
        {
            if (IsWindowsShuttingDown || ex == null || ex.GetType() == typeof(ThreadAbortException))
            {
                return;
            }

            string threadName = Thread.CurrentThread.Name ?? Environment.CurrentManagedThreadId.ToString(CultureInfo.InvariantCulture);
            string message =
                Res.str_ErrorCrash +
                "\n\n" + "Thread : " +
                threadName +
                "\n\n" +
                ex.ToString();

            LogException(message);

            if (!showMessage)
            {
                return;
            }

            try
            {
                MessageBox.Show(message, "GabNetStats", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException) { }
        }

        private static void LogException(string message)
        {
            try
            {
                string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GabNetStats");
                Directory.CreateDirectory(directory);

                string logPath = Path.Combine(directory, "GabNetStats.log");
                File.AppendAllText(
                    logPath,
                    DateTime.Now.ToString("u") +
                    Environment.NewLine +
                    message +
                    Environment.NewLine +
                    Environment.NewLine);
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            catch (System.Security.SecurityException) { }
            catch (NotSupportedException) { }
        }
    }
}
