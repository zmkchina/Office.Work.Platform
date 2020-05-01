using System;
using System.Windows;

namespace Office.Work.Platform.MemberUc
{
    /// <summary>
    /// 此类用于新增或编辑月度待遇发放记录。
    /// 此窗体作为对话框使用，当DialogResult不被设为true时，将始终为false
    /// </summary>
    public partial class UC_PayTempWin : Window
    {
        public Lib.MemberPayTemp _CurRecord { get; set; }
        public UC_PayTempWin(Lib.MemberPayTemp PRecord)
        {
            InitializeComponent();
            _CurRecord = PRecord;
            DataContext = PRecord;
        }
        private void BtnSaveClickAsync(object sender, RoutedEventArgs e)
        {
            if (_CurRecord.Remark == null || _CurRecord.TypeName == null)
            {
                (new WinMsgDialog("数据输入不正确，类型及备注均必须输入!", Caption: "缺少数据")).ShowDialog();
                return;
            }
            DialogResult = true;
            _CurRecord.UpDateTime = DateTime.Now;
            this.Close();
        }

        private void BtnCancelClickAsync(object sender, RoutedEventArgs e)
        {
            _CurRecord = null;
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
