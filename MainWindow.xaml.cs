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
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using System.IO;

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
        private readonly NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            AppSettings.AppMainWindow = this;
            //以下代码，修复窗体全屏时覆盖任务栏以及大小不正确问题。

            FullScreenManager.RepairWpfWindowFullScreenBehavior(this);
            //系统托盘显示
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.BalloonTipText = "系统监控中... ...";
            this.notifyIcon.ShowBalloonTip(2000);
            this.notifyIcon.Text = "政工科办公信息化平台";
            //this.notifyIcon.Icon = new System.Drawing.Icon(@"AppIcon.ico");
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this.notifyIcon.Visible = true;
            //托盘右键菜单项
            this.notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            this.notifyIcon.ContextMenuStrip.Items.Add("显示主窗口", null, (sender, eventArgs) =>
            {
                this.Visibility = System.Windows.Visibility.Visible;
                this.ShowInTaskbar = true;
                this.Activate();
            });
            this.notifyIcon.ContextMenuStrip.Items.Add("关闭显示器", null, (sender, eventArgs) =>
            {
                CloseScreen.Close();
            });
            this.notifyIcon.ContextMenuStrip.Items.Add("关闭程序", null, (sender, eventArgs) =>
            {
                ShutDownApp();
            });
            //托盘双击响应
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.notifyIcon.ContextMenuStrip.Items[0].PerformClick();
                }
            });
        }
        private async void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            //1.检查是否需要更新。
            if (await CheckUpdate.CheckAsync())
            {
                MessageBox.Show("发现新版本！", "更新", MessageBoxButton.OK, MessageBoxImage.Information);
                //升级程序路径。
                string updateProgram = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Office.Work.Platform.Update.exe");
                if (File.Exists(updateProgram))
                {
                    //启动升级程序
                    System.Diagnostics.Process.Start(updateProgram);
                }
                else
                {
                    MessageBox.Show("未找到更新程序,请与开发人员联系。", "更新", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                //关闭本程序
                ShutDownApp();
            }
            //2.读取系统用户列表
            AppSettings.SysUsers = await DataSystemRepository.ReadAllSysUsers();
            if (AppSettings.SysUsers == null || AppSettings.SysUsers.Count < 2)
            {
                MessageBox.Show("读取用户列表时出错，程序无法运行！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                //关闭本程序
                ShutDownApp();
            }
            //3.读取服务器设置
            AppSettings.ServerSetting = await DataSystemRepository.ReadServerSettings();
            if (AppSettings.ServerSetting == null)
            {
                MessageBox.Show("读取系统设置信息时出错，程序无法运行！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                //关闭本程序
                ShutDownApp();
            }
            ListBoxItem_MouseLeftButtonUp_0(null, null);
        }
        /// <summary>
        /// 窗体拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Dispatcher.BeginInvoke(new Action(() => this.DragMove()));
            }
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
                        this.ShowInTaskbar = false;
                        this.Visibility = System.Windows.Visibility.Hidden;
                        this.notifyIcon.ShowBalloonTip(20, "信息:", "工作平台已隐藏在这儿。", ToolTipIcon.Info);
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// 安全关闭本程序
        /// </summary>
        private void ShutDownApp()
        {
            this.notifyIcon ?.Dispose();
            System.Windows.Application.Current.Shutdown(0);
        }
    }
}
