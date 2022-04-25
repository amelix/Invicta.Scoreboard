using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ObsScoreHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constants and Fields

        /// <summary>The event mutex name.</summary>
        private const string UniqueEventName = "62f0a0cf-290d-46a5-add9-12d6eb132539";

        /// <summary>The unique mutex name.</summary>
        private const string UniqueMutexName = "359a638f-8a79-4b3e-a19f-a46c610dab2a";

        /// <summary>The event wait handle.</summary>
        private EventWaitHandle eventWaitHandle;

        /// <summary>The mutex.</summary>
        private Mutex mutex;

        #endregion

        #region Methods

        /// <summary>The app on startup.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AppOnStartup(object sender, StartupEventArgs e)
        {
            bool createdNew;
            this.mutex = new Mutex(true, UniqueMutexName, out createdNew);
            this.eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);

            // So, R# would not give a warning that this variable is not used.
            GC.KeepAlive(this.mutex);

            if (createdNew)
            {
                Current.MainWindow = new MainWindow();

                Current.MainWindow.Show();
                // Spawn a thread which will be waiting for our event
                var thread = new Thread(
                    () =>
                    {
                        while (this.eventWaitHandle.WaitOne())
                        {
                            Current.Dispatcher.BeginInvoke((Action)(() => ((MainWindow)Current.MainWindow).BringToForeground()));
                        }
                    });

                // It is important mark it as background otherwise it will prevent app from exiting.
                thread.IsBackground = true;

                thread.Start();
                return;
            }

            // Notify other instance so it could bring itself to foreground.
            this.eventWaitHandle.Set();

            // Terminate this instance.
            this.Shutdown();
        }

        #endregion
    }
}
