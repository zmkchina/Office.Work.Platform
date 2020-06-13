using Office.Work.Platform.AppCodes;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Office.Work.Platform.MemberUc
{
    /// <summary>
    /// 此类用于新增或编辑月度待遇发放记录。
    /// 此窗体作为对话框使用，当DialogResult不被设为true时，将始终为false
    /// </summary>
    public partial class UC_RelationsWin : Window
    {
        private DispatcherTimer _UpdateInfoTimer = new System.Windows.Threading.DispatcherTimer();
        private CurWinViewModel _CurWinViewModel;
        public UC_RelationsWin(Lib.MemberRelations ParamRecord)
        {
            InitializeComponent();
            this.Owner = AppSet.AppMainWindow;
            _CurWinViewModel = new CurWinViewModel(ParamRecord);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _UpdateInfoTimer.Interval = new TimeSpan(0, 0, 3);

            _UpdateInfoTimer.Tick += new EventHandler((x, y) =>
            {
                InputResultMsg.Text = "";
                _UpdateInfoTimer.Stop();
            });
            DataContext = _CurWinViewModel;
        }

        private void BtnSaveClickAsync(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_CurWinViewModel.CurRecord.Relation) || string.IsNullOrWhiteSpace(_CurWinViewModel.CurRecord.Name))
            {
                InputResultMsg.Text = "数据输入不正确!";
                _UpdateInfoTimer.Start();
                return;
            }
            DialogResult = true;
            _CurWinViewModel.CurRecord.UpDateTime = DateTime.Now;
            this.Close();
        }

        private void BtnCancelClickAsync(object sender, RoutedEventArgs e)
        {
            _CurWinViewModel.CurRecord = null;
            this.Close();
        }
        /// <summary>
        /// 拖动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Dispatcher.BeginInvoke(new Action(() => this.DragMove()));
            }
        }

        /// <summary>
        /// 当前窗口视图模型
        /// </summary>
        private class CurWinViewModel
        {
            public Lib.SettingServer ServerSettings { get; set; }
            public Lib.MemberRelations CurRecord { get; set; }
            public CurWinViewModel(Lib.MemberRelations ParamRecord)
            {
                ServerSettings = AppSet.ServerSetting;
                CurRecord = ParamRecord;
            }
        }
    }
}
