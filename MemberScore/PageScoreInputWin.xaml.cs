using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using Office.Work.Platform.AppCodes;

namespace Office.Work.Platform.MemberScore
{
    /// <summary>
    /// 此类用于新增或编辑职工绩效考核得分记录。
    /// 此窗体作为对话框使用，当DialogResult不被设为true时，将始终为false
    /// </summary>
    public partial class PageScoreInputWin : Window
    {
        public Lib.MemberScore _CurMemberScore { get; set; }
        public ReadOnlyCollection<string> _MemberScoreTypes { get; set; } = AppSet.ServerSetting.MemberScoreTypeArr;
        private DispatcherTimer _UpdateInfoTimer = new System.Windows.Threading.DispatcherTimer();
        public PageScoreInputWin(Lib.MemberScore PMemberScore)
        {
            InitializeComponent();
            this.Owner = AppSet.AppMainWindow;
            _CurMemberScore = PMemberScore;
        }
        private void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            _UpdateInfoTimer.Interval = new TimeSpan(0, 0, 3);

            _UpdateInfoTimer.Tick += new EventHandler((x, y) =>
            {
                InputResultMsg.Text = "";
                _UpdateInfoTimer.Stop();
            });
            DataContext = this;
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSaveClickAsync(object sender, RoutedEventArgs e)
        {
            if (!_CurMemberScore.ModelIsValid())
            {
                InputResultMsg.Text = "数据输入不合法或不完整！";
                _UpdateInfoTimer.Start();
                return;
            }
            DialogResult = true;
            this.Close();
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancelClickAsync(object sender, RoutedEventArgs e)
        {
            _CurMemberScore = null;
            DialogResult = false;
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


    }
}
