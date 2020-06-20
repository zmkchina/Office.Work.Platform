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

            _PageViewModel = new PageViewModel();
        }
        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await _PageViewModel.GetEntityInfoAsync();
            if (AppSet.LoginUser.Post.Equals("管理员"))
            {
                _PageViewModel.CanOperation = true;
            }
            this.DataContext = _PageViewModel;
        }

        private void Btn_UpdateSettings_Click(object sender, RoutedEventArgs e)
        {
            _PageViewModel.UpdateEntityInfoAsync();
        }

        private void Btn_RestoreSettings_Click(object sender, RoutedEventArgs e)
        {
            _PageViewModel.EntitySettingServer = new SettingServer();
            this.DataContext = null;
            this.DataContext = _PageViewModel;
        }

        private class PageViewModel : NotificationObject
        {
            private bool _CanOperation;

            public bool CanOperation
            {
                get
                {
                    return _CanOperation;
                }
                set
                {
                    _CanOperation = value; RaisePropertyChanged();
                }
            }
            public SettingServer EntitySettingServer { get; set; }

            #region "方法"
            /// <summary>
            /// 构造函数
            /// </summary>
            public PageViewModel()
            {
                CanOperation = false;
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
                    AppSet.ServerSetting = EntitySettingServer;
                }
            }
            #endregion
        }


    }
}
