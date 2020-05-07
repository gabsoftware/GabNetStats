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

        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                m_Mutex = new Mutex(true, "GabNetStatsMutex");

                Thread.CurrentThread.Name = "MainThread";

                if (m_Mutex.WaitOne(0, false))
                {
                    Application.Run(new MainForm());
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ThreadAbortException))
                {
                    MessageBox.Show(
                        Res.str_ErrorCrash +
                        "\n\n" + "Thread : " +
                        Thread.CurrentThread.Name +
                        "\n\n" +
                        ex.ToString(), "GabNetStats", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Application.Restart();
            }
        }
    }
}