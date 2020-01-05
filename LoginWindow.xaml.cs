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
            AppSettings.LocalSetting = DataRWLocalFileRepository.ReadObjFromFile<ModelSettingLocal>(AppSettings.LocalSettingFileName);
            Text_UserId.Text = AppSettings.LocalSetting.LoginUserId;
            DataContext = AppSettings.LocalSetting;
            Text_UserPwd.Focus();
        }
        private async void Btn_Login_ClickAsync(object sender, RoutedEventArgs e)
        {
            string V_UserId = Text_UserId.Text.Trim();
            string V_UserPwd = Text_UserPwd.Password.Trim();
            if (V_UserId.Length < 1 || V_UserPwd.Length < 1)
            {
                MessageBox.Show("请输入用户名和密码！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //显示Loading
            this.CanVas_loadding.Visibility = Visibility.Visible;
            //请求token
            await  DataApiRepository.GetAccessToken(V_UserId, V_UserPwd);
            //从服务器读取指定的用户并在服务器上登陆
            ModelUser LoginUser =await DataApiRepository.GetApiUri<ModelUser>(AppSettings.ApiUrlBase + "User/" + V_UserId);
            if (LoginUser != null && LoginUser.PassWord.Equals(V_UserPwd))
            {
                DataRWLocalFileRepository.SaveObjToFile<ModelSettingLocal>(AppSettings.LocalSetting, AppSettings.LocalSettingFileName);
                AppSettings.LoginUser = LoginUser;
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                mainWindow.Activate();
                this.Close();
            }
            else
            {
                MessageBox.Show("登陆失败，请检查网络或信息是否准确。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.CanVas_loadding.Visibility = Visibility.Collapsed;
            }
        }
        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Dispatcher.BeginInvoke(new Action(() => this.DragMove()));
            }
        }
    }
}
