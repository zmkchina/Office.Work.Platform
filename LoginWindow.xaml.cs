using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System;
using System.Windows;
using System.Windows.Input;

namespace Office.Work.Platform
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //读取本地设置
            AppSet.LocalSetting = DataRWLocalFileRepository.ReadObjFromFile<SettingLocal>(AppSet.LocalSettingFileName);
            if (AppSet.LocalSetting == null)
            {
                AppSet.LocalSetting = new SettingLocal();
            }
            Text_UserId.Text = AppSet.LocalSetting.LoginUserId;
            DataContext = AppSet.LocalSetting;
            Text_UserPwd.Focus();
        }
        private async void Btn_Login_ClickAsync(object sender, RoutedEventArgs e)
        {
            string V_UserId = Text_UserId.Text.Trim();
            string V_UserPwd = Text_UserPwd.Password.Trim();
            if (V_UserId.Length < 1 || V_UserPwd.Length < 1)
            {
                AppFuns.ShowMessage("请输入用户名和密码。", "警告");
                return;
            }
            //显示Loading
            this.CanVas_loadding.Visibility = Visibility.Visible;
            try
            {
                //请求token
                string TokenResult = await DataApiRepository.GetAccessToken(V_UserId, V_UserPwd);
                if (TokenResult != "Ok")
                {
                    if (TokenResult.Trim().Equals("invalid_grant", StringComparison.Ordinal))
                    {
                        AppFuns.ShowMessage("用户名或密码错误！", "警告");
                    }
                    else
                    {
                        AppFuns.ShowMessage(TokenResult,"错误", isErr: true);
                    }
                    CanVas_loadding.Visibility = Visibility.Collapsed;
                    return;
                }
                //从服务器读取指定的用户并在服务器上登陆
                User LoginUser = await DataUserRepository.GetOneById(V_UserId);
                if (LoginUser != null)
                {
                    DataRWLocalFileRepository.SaveObjToFile<SettingLocal>(AppSet.LocalSetting, AppSet.LocalSettingFileName);
                    AppSet.LoginUser = LoginUser;
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    mainWindow.Activate();
                    this.Close();
                }
                else
                {
                    this.CanVas_loadding.Visibility = Visibility.Collapsed;
                    AppFuns.ShowMessage("读取当前用户信息出错！", "警告");
                }
            }
            catch (Exception ex)
            {
                this.CanVas_loadding.Visibility = Visibility.Collapsed;
                AppFuns.ShowMessage(ex.Message, "错误", isErr: true);
            }
        }
        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Dispatcher.BeginInvoke(new Action(() => this.DragMove()));
            }
        }
        /// <summary>
        /// 显示设置网址对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_UrlSet_Click(object sender, RoutedEventArgs e)
        {
            CanVas_UrlSet.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// 保存设置的网址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            DataRWLocalFileRepository.SaveObjToFile<SettingLocal>(AppSet.LocalSetting, AppSet.LocalSettingFileName);
            CanVas_UrlSet.Visibility = Visibility.Collapsed;
            CanVas_loadding.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 退出设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Return_Click(object sender, RoutedEventArgs e)
        {
            CanVas_UrlSet.Visibility = Visibility.Collapsed;
            CanVas_loadding.Visibility = Visibility.Collapsed;
        }
    }
}
