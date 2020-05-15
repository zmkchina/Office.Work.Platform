using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.FileDocs;
using Office.Work.Platform.Lib;
using Office.Work.Platform.Member;
using Office.Work.Platform.Note;
using Office.Work.Platform.Plan;
using Office.Work.Platform.Remuneration;
using Office.Work.Platform.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Office.Work.Platform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly NotifyIcon notifyIcon;
        private readonly PagePlanMenu _PagePlanMenu = null;
        private readonly PageNoteMenu _PageNodeMenu = null;
        private readonly PageFileMenu _PageFileMenu = null;
        private readonly PagePlayMenu _PagePlayMenu = null;
        private readonly PageMemberMenu _PageMemberMenu = null;
        private readonly PageSettingsMenu _PageSettingsMenu = null;

        public MainWindow()
        {
            InitializeComponent();
            AppSet.AppMainWindow = this;
            //以下代码，修复窗体全屏时覆盖任务栏以及大小不正确问题。
            FullScreenManager.RepairWpfWindowFullScreenBehavior(this);

            #region 显示系统托盘图标
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
                if (WinLockDialog.ThisWinObj != null)
                {
                    //说明该锁定窗口已经打开
                    WinLockDialog.ThisWinObj.Activate();
                    return;
                }
                if (AppSet.AppIsLocked)
                {
                    WinLockDialog wld = new WinLockDialog();
                    if (!wld.ShowDialog().Value)
                    {
                        return;
                    }
                }
                AppSet.AppIsLocked = false;
                this.Visibility = System.Windows.Visibility.Visible;
                this.ShowInTaskbar = true;
                this.Activate();
            });
            this.notifyIcon.ContextMenuStrip.Items.Add("关闭显示器", null, (sender, eventArgs) =>
            {
                CloseScreen.Close();
            });
            this.notifyIcon.ContextMenuStrip.Items.Add("锁定软件", null, (sender, eventArgs) =>
            {
                LockApp();
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
            #endregion
        }

        private async void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            //1.检查是否需要更新。
            List<string> NeedUpdateFiles = new List<string>();
            //读取服务器端本系统程序的信息。
            AppUpdateInfo UpdateInfo = await DataFileUpdateAppRepository.GetAppUpdateInfo();
            //如果服务器程序升级信息比本地记录的晚，则升级之。
            if (UpdateInfo.UpdateDate > AppSet.LocalSetting.AppUpDateTime)
            {
                NeedUpdateFiles = UpdateInfo.UpdateFiles.ToList<string>();
            }

            if (NeedUpdateFiles != null && NeedUpdateFiles.Count > 0)
            {
                WinUpdateDialog winUpdate = new WinUpdateDialog(NeedUpdateFiles);
                winUpdate.ShowDialog();
                if (!CheckDownResult(UpdateInfo))
                {
                    AppFuns.ShowMessage("下载升级文件不正确，无法更新。", "错误", isErr: true);
                    ShutDownApp();
                    return;
                }
                //升级程序路径。
                string updateProgram = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateApp.exe");
                if (File.Exists(updateProgram))
                {
                    //启动升级程序
                    _ = Task.Run(() =>
                      {
                          int UpdateExitCode = System.Diagnostics.Process.Start(updateProgram).ExitCode;
                      });
                    //下载成功，更新本地时间。
                    AppSet.LocalSetting.AppUpDateTime = UpdateInfo.UpdateDate;
                    DataRWLocalFileRepository.SaveObjToFile(AppSet.LocalSetting, AppSet.LocalSettingFileName);
                }
                else
                {
                    AppFuns.ShowMessage("未找到更新程序,请与开发人员联系。", "错误", isErr: true);
                }
                //关闭本程序
                ShutDownApp();
            }
            //2.读取系统用户列表
            AppSet.SysUsers = await DataUserRepository.GetAllRecords();
            if (AppSet.SysUsers == null || AppSet.SysUsers.Count < 2)
            {
                (new WinMsgDialog("读取用户列表时出错，程序无法运行。", "错误", isErr: true)).ShowDialog();
                //关闭本程序
                ShutDownApp();
            }
            //3.读取服务器设置
            AppSet.ServerSetting = await DataSystemRepository.ReadServerSettings();
            if (AppSet.ServerSetting == null)
            {
                (new WinMsgDialog("读取系统设置信息时出错，程序无法运行。", "错误", isErr: true)).ShowDialog();
                //关闭本程序
                ShutDownApp();
            }
            lblLoginMsg.Text = $"当前用户：{AppSet.LoginUser.Name}-{AppSet.LoginUser.UnitName}";
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
        /// 人事管理菜单
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
            this.notifyIcon?.Dispose();
            System.Windows.Application.Current.Shutdown(0);
        }
        /// <summary>
        /// 劳资管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItemPay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LoadPageMenu(_PagePlayMenu);
        }
        public void LockApp()
        {
            AppSet.AppIsLocked = true;
            this.ShowInTaskbar = false;
            this.Visibility = System.Windows.Visibility.Hidden;
            this.notifyIcon.ShowBalloonTip(20, "信息:", "本软件已锁定。", ToolTipIcon.Info);
        }
        /// <summary>
        /// 判断需要下载的升级文件是否已经全部正确下载。
        /// </summary>
        /// <param name="appUpdateInfo"></param>
        /// <returns></returns>
        public bool CheckDownResult(AppUpdateInfo appUpdateInfo)
        {
            //合成目录
            string tempFileDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateApp");
            DirectoryInfo directoryInfo = new DirectoryInfo(tempFileDir);

            if (!directoryInfo.Exists)
            {

                return false;
            }
            string[] UpdateFiles = directoryInfo.GetFiles().Select(x => x.Name).ToArray();
            if (appUpdateInfo.UpdateFiles.Count != UpdateFiles.Length)
            {
                return false;
            }
            foreach (string item in UpdateFiles)
            {
                if (!appUpdateInfo.UpdateFiles.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
