using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Settings
{
    /// <summary>
    /// WinUpLoadFile.xaml 的交互逻辑
    /// </summary>
    public partial class PageSettingsTheme : Page
    {
        private PageViewModel _PageViewModel = null;
        public PageSettingsTheme()
        {
            InitializeComponent();
        }
        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            _PageViewModel = new PageViewModel();
            await _PageViewModel.GetEntityInfoAsync();
            this.DataContext = _PageViewModel;
        }

        private void BtnUpdateSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnUpdateTheme_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["ColorMainWinTitle"] = Brushes.Blue;
        }
        /// <summary>
        /// 本页面的视图模型
        /// </summary>
        public class PageViewModel : NotificationObject
        {
            private byte _RedColorValue;
            private byte _GreenColorValue;
            private byte _BlueColorValue;
            private SolidColorBrush _BrushMainWinTitleBar;

            public string[] FileContentTypes => AppSet.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            public SettingServer EntitySettingServer { get; set; }

            public byte RedColorValue
            {
                get { return _RedColorValue; }
                set
                {
                    _RedColorValue = value; SetColorBrush();
                }
            }
            public byte GreenColorValue
            {
                get { return _GreenColorValue; }
                set
                {
                    _GreenColorValue = value; SetColorBrush();
                }
            }
            public byte BlueColorValue
            {
                get { return _BlueColorValue; }
                set
                {
                    _BlueColorValue = value; SetColorBrush();
                }
            }
            public SolidColorBrush BrushMainWinTitleBar
            {
                get { return _BrushMainWinTitleBar; }
                set
                {
                    _BrushMainWinTitleBar = value; RaisePropertyChanged();
                }
            }
            #region "方法"
            /// <summary>
            /// 构造函数
            /// </summary>
            public PageViewModel()
            {
                BrushMainWinTitleBar = new SolidColorBrush();
            }
            private void SetColorBrush()
            {
                BrushMainWinTitleBar.Color = Color.FromArgb(0xFF, RedColorValue, GreenColorValue, BlueColorValue);
            }
            public async Task GetEntityInfoAsync()
            {
                EntitySettingServer = await DataSystemRepository.ReadServerSettings();
            }
            #endregion
        }

        private void Ellipse_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }



}
