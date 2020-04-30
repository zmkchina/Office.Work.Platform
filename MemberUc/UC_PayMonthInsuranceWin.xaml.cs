using System;
using System.Windows;

namespace Office.Work.Platform.MemberUc
{
    /// <summary>
    /// 此类用于新增或编辑月度待遇发放记录。
    /// 此窗体作为对话框使用，当DialogResult不被设为true时，将始终为false
    /// </summary>
    public partial class UC_PayMonthInsuranceWin : Window
    {
        public Lib.MemberPayMonthInsurance _PayMonthInsurance { get; set; }
        public UC_PayMonthInsuranceWin(Lib.MemberPayMonthInsurance PayMonthInsurance)
        {
            InitializeComponent();
            _PayMonthInsurance = PayMonthInsurance;
            DataContext = PayMonthInsurance;
        }
        private void BtnSaveClickAsync(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _PayMonthInsurance.PayYear = _PayMonthInsurance.UpDateTime.Year;
            _PayMonthInsurance.PayMonth = _PayMonthInsurance.UpDateTime.Month;
            _PayMonthInsurance.UpDateTime = DateTime.Now;
            this.Close();
        }

        private void BtnCancelClickAsync(object sender, RoutedEventArgs e)
        {
            _PayMonthInsurance = null;
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
