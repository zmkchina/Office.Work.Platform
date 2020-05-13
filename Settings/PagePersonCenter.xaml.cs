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
    public partial class PagePersonCenter : Page
    {
        private PageViewModel _PageViewModel = null;
        public PagePersonCenter()
        {
            InitializeComponent();
        }
        private void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            _PageViewModel = new PageViewModel();
            this.DataContext = _PageViewModel;
        }

        private void Btn_SavePwd_Click(object sender, RoutedEventArgs e)
        {
           _PageViewModel.UpdatePwd();

        }
        /// <summary>
        /// 本页面的视图模型
        /// </summary>
        private class PageViewModel : NotificationObject
        {

            public string CurPwd { get; set; }
            public string NewPwd { get; set; }
            public async void UpdatePwd()
            {
                if (CurPwd != AppSet.LoginUser.PassWord)
                {
                    AppFuns.ShowMessage("原密码输入不正确！");
                    return;
                }
                if (string.IsNullOrWhiteSpace(NewPwd) || NewPwd.Trim().Length < 3)
                {
                    AppFuns.ShowMessage("新密码长度不能小于 3 个字符。");
                    return;
                }
                AppSet.LoginUser.PassWord = NewPwd;
                ExcuteResult excuteResult= await DataUserRepository.UpdateRecord(AppSet.LoginUser);
                if (excuteResult.State == 0)
                {
                    AppFuns.ShowMessage(excuteResult.Msg);
                }
            }
        }
    }
}
