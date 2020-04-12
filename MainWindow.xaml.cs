using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Files;
using Office.Work.Platform.Member;
using Office.Work.Platform.Node;
using Office.Work.Platform.Plan;
using Office.Work.Platform.Remuneration;
using Office.Work.Platform.Settings;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Office.Work.Platform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PagePlanMenu _PagePlanMenu = null;
        private readonly PageNodeMenu _PageNodeMenu = null;
        private readonly PageFileMenu _PageFileMenu = null;
        private readonly PagePlayMenu _PagePlayMenu = null;
        private readonly PageMemberMenu _PageMemberMenu = null;
        private readonly PageSettingsMenu _PageSettingsMenu = null;
        public MainWindow()
        {
            InitializeComponent();
            AppSettings.AppMainWindow = this;
            //以下代码，修复窗体全屏时覆盖任务栏以及大小不正确问题。
            FullScreenManager.RepairWpfWindowFullScreenBehavior(this);
        }
        private async void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            //检查是否需要更新。
            await CheckUpdate.CheckAsync();
            //读取系统用户列表
            AppSettings.SysUsers = await DataSystemRepository.ReadAllSysUsers();
            //读取服务器设置
            AppSettings.ServerSetting = await DataSystemRepository.ReadServerSettings();
            ListBoxItem_MouseLeftButtonUp_0(null, null);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Dispatcher.BeginInvoke(new Action(() => this.DragMove()));
            }
        }



        private void Window_StateChanged(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 计划管理菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_0(object sender, MouseButtonEventArgs e)
        {
            LoadPageMenu(_PagePlanMenu);
        }
        /// <summary>
        /// 全部文件菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            LoadPageMenu(_PageFileMenu);
        }
        /// <summary>
        /// 待遇发放菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_2(object sender, MouseButtonEventArgs e)
        {
            LoadPageMenu(_PagePlayMenu);
        }
        /// <summary>
        /// 员工信息菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_3(object sender, MouseButtonEventArgs e)
        {
            LoadPageMenu(_PageMemberMenu);
        }
        /// <summary>
        /// 备忘信息菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_4(object sender, MouseButtonEventArgs e)
        {
            LoadPageMenu(_PageNodeMenu);
        }
        /// <summary>
        /// 系统设置菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_5(object sender, MouseButtonEventArgs e)
        {
            LoadPageMenu(_PageSettingsMenu);

        }
        private void LoadPageMenu<T>(T PageMenu) where T : class, new()
        {
            PageMenu ??= new T();
            this.FrameMenuPage.Content = PageMenu;
            this.FrameContentPage.Content = null;
        }

        #region 窗口大小、关闭控制
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState ^= WindowState.Maximized;
            }
        }
        private void WinState_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is System.Windows.Controls.Button btn)
            {
                string tagStr = btn.Tag.ToString();
                switch (tagStr)
                {
                    case "tbWinMin":
                        WindowState = WindowState.Minimized;
                        break;
                    case "tbWinMax":
                        WindowState ^= WindowState.Maximized; //采用“或等于”可以同时执行恢复和最大化两个操作。
                        break;
                    case "tbWinCose":
                        Application.Current.Shutdown(0);
                        break;
                }
            }
        }
        #endregion

    }
    [ValueConversion(typeof(string), typeof(string))]
    public class RatioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var size = 0d;
            if (value != null)
                size = System.Convert.ToDouble(value, CultureInfo.InvariantCulture) *
                       System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);

            return size;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
