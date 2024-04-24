using System;
using System.Threading;
using System.Windows.Forms;

namespace SpotiHotKey
{
    internal static class Program
    {

        private const string MutexName = "SpotiFavKey";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew;
            using (Mutex mutex = new Mutex(true, MutexName, out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show("The SpotiFavKey is already running", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    // To customize application configuration such as set high DPI settings or default font,
                    // see https://aka.ms/applicationconfiguration.
                    ApplicationConfiguration.Initialize();
                    Application.Run(new SpotiFavKey());
                }
                catch (Exception ex)
                {
                    Logger.LogToFile(ex.Message);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
    }
}