using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Settings
{
    /// <summary>
    /// WinUpLoadFile.xaml 的交互逻辑
    /// </summary>
    public partial class PageSettingsSys : Page
    {
        private PageViewModel _PageViewModel = null;
        public PageSettingsSys()
        {
            InitializeComponent();
        }
        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            _PageViewModel = new PageViewModel();
            await _PageViewModel.GetEntityInfoAsync();
            this.DataContext = _PageViewModel;
            if (AppSet.LoginUser.Post.Equals("管理员"))
            {
                BtnUpdateSettings.IsEnabled = true;
            }
        }

        private void Btn_UpdateSettings_Click(object sender, RoutedEventArgs e)
        {
            _PageViewModel.UpdateEntityInfoAsync();
        }

        private class PageViewModel : NotificationObject
        {

            public string[] FileContentTypes => AppSet.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            public SettingServer EntitySettingServer { get; set; }

            #region "方法"
            /// <summary>
            /// 构造函数
            /// </summary>
            public PageViewModel()
            {
            }
            public async Task GetEntityInfoAsync()
            {
                EntitySettingServer = await DataSystemRepository.ReadServerSettings();
            }
            public async void UpdateEntityInfoAsync()
            {
                ExcuteResult excute = await DataSystemRepository.UpdateServerSettings(EntitySettingServer);
                if (excute.State == 0)
                {
                    AppFuns.ShowMessage("更新成功！");
                }
            }
            #endregion
        }
    }
}
