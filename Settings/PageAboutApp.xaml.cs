using System.Windows;
using System.Windows.Controls;

namespace Office.Work.Platform.Settings
{
    /// <summary>
    /// PageAboutApp.xaml 的交互逻辑
    /// </summary>
    public partial class PageAboutApp : Page
    {
        public PageAboutApp()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString();
            AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            DataContext = this;
        }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
    }
}
