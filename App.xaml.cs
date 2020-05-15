using System.Diagnostics;
using System.Windows;

namespace Office.Work.Platform
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Get Reference to the current Process
            Process thisProc = Process.GetCurrentProcess();
            // Check how many total processes have the same name as the current one
            Process[] Plist = Process.GetProcessesByName(thisProc.ProcessName);
            if (Plist.Length > 1)
            {
                // If ther is more than one, than it is already running.
                MessageBox.Show("业务平台已经启动，无法运行第二个实例！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
                return;
            }

            base.OnStartup(e);
        }
    }

}
