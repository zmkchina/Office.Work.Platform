using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Files;
using Office.Work.Platform.Member;
using Office.Work.Platform.Node;
using Office.Work.Platform.Plan;
using Office.Work.Platform.Remuneration;
using System;
using System.Windows;
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
        public MainWindow()
        {
            InitializeComponent();
            MainGrid.MaxWidth = SystemParameters.WorkArea.Width;
            MainGrid.MaxHeight = SystemParameters.WorkArea.Height;
            AppSettings.AppMainWindow = this;
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

        private void tbClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void tbWinState_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.tbWinState.Content.ToString().Equals("最大化"))
            {
                this.tbWinState.Content = "恢复";
            }
            else
            {
                this.tbWinState.Content = "最大化";
            }
        }

        
        /// <summary>
        /// 计划管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_0(object sender, MouseButtonEventArgs e)
        {
            LoadPageMenu(_PagePlanMenu);
        }
        /// <summary>
        /// 全部文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            LoadPageMenu(_PageFileMenu);
        }
        /// <summary>
        /// 待遇发放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_2(object sender, MouseButtonEventArgs e)
        {
            LoadPageMenu(_PagePlayMenu);
        }
        /// <summary>
        /// 员工信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_3(object sender, MouseButtonEventArgs e)
        {
            LoadPageMenu(_PageMemberMenu);
        }
        /// <summary>
        /// 备忘信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_4(object sender, MouseButtonEventArgs e)
        {
            LoadPageMenu(_PageNodeMenu);
        }
        private void LoadPageMenu<T>(T PageMenu) where T : class, new()
        {
            PageMenu ??= new T();
            this.FrameMenuPage.Content = PageMenu;
            this.FrameContentPage.Content = null;
        }
    }
}
