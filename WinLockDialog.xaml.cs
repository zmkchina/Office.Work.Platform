using Office.Work.Platform.AppCodes;
using System;
using System.Windows;
using System.Windows.Input;

namespace Office.Work.Platform
{
    /// <summary>
    /// WinMsgDialog.xaml 的交互逻辑
    /// </summary>
    public partial class WinLockDialog : Window
    {
        public WinLockDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            ThisWinObj = this;
            this.Text_Result.Focus();
        }

        public static Window ThisWinObj { get; set; } = null;

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Dispatcher.BeginInvoke(new Action(() => this.DragMove()));
            }
        }

        private void Btn_Yes_Click(object sender, RoutedEventArgs e)
        {
            if (Pwd_Box.Password.Trim() == AppSet.LoginUser.PassWord)
            {
                this.DialogResult = true;
                this.Close();
                return;
            }
            Text_Result.Text = "密码输入不正确！";
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Pwd_Box_KeyUp(object sender, KeyEventArgs e)
        {
            Text_Result.Text = "";
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            ThisWinObj = null;
        }
    }
}
