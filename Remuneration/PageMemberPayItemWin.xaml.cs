using System;
using System.Windows;
using Office.Work.Platform.AppCodes;

namespace Office.Work.Platform.Remuneration
{
    /// <summary>
    /// 此类用于新增或编辑月度待遇发放记录。
    /// 此窗体作为对话框使用，当DialogResult不被设为true时，将始终为false
    /// </summary>
    public partial class PageMemberPayItemWin : Window
    {
        public Lib.MemberPayItem CurPayItem { get; set; }

        public PageMemberPayItemWin(Lib.MemberPayItem PPayItem)
        {
            this.Owner =AppSettings.AppMainWindow;
            InitializeComponent();
            CurPayItem = PPayItem;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CurPayItem.Name))
            {
                //说明是编辑项目。
                Tb_ItemName.IsReadOnly = true;
            }
            DataContext = this;
        }

        private void BtnSaveClickAsync(object sender, RoutedEventArgs e)
        {
            CurPayItem.UnitName = AppSettings.LoginUser.UnitName;
            CurPayItem.UserId = AppSettings.LoginUser.Id;
            DialogResult = true;
            this.Close();
        }

        private void BtnCancelClickAsync(object sender, RoutedEventArgs e)
        {
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
