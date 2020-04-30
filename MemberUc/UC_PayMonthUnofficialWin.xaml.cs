using System;
using System.Windows;

namespace Office.Work.Platform.MemberUc
{
    /// <summary>
    /// 此类用于新增或编辑月度待遇发放记录。
    /// 此窗体作为对话框使用，当DialogResult不被设为true时，将始终为false
    /// </summary>
    public partial class UC_PayMonthUnofficialWin : Window
    {
        public Lib.MemberPayMonthUnofficial _PayMonthUnofficial { get; set; }
        public UC_PayMonthUnofficialWin(Lib.MemberPayMonthUnofficial PayMonth)
        {
            InitializeComponent();
            _PayMonthUnofficial = PayMonth;
            DataContext = PayMonth;
        }
        private void BtnSaveClickAsync(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _PayMonthUnofficial.PayYear = _PayMonthUnofficial.UpDateTime.Year;
            _PayMonthUnofficial.PayMonth = _PayMonthUnofficial.UpDateTime.Month;
            _PayMonthUnofficial.UpDateTime = DateTime.Now;
            this.Close();
        }

        private void BtnCancelClickAsync(object sender, RoutedEventArgs e)
        {
            _PayMonthUnofficial = null;
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
