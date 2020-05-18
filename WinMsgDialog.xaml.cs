using Office.Work.Platform.AppCodes;
using System;
using System.Windows;
using System.Windows.Input;

namespace Office.Work.Platform
{
    /// <summary>
    /// WinMsgDialog.xaml 的交互逻辑
    /// </summary>
    public partial class WinMsgDialog : Window
    {
        public WinMsgDialog()
        {
        }
        public void InitWinData(string Message, string Caption = "信息", bool isErr = false, bool showYesNo = false, bool ShowOk = true)
        {

            InitializeComponent();
            this.Owner = AppSet.AppMainWindow;
            if (this.Owner is null)
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            if (showYesNo) { ShowOk = false; } else { ShowOk = true; }
            this.Message = Message;
            this.Caption = Caption;
            this.ShowYes = this.ShowNo = showYesNo ? "Visible" : "Collapsed";
            this.ShowOk = ShowOk ? "Visible" : "Collapsed";
            if (isErr)
            {
                this.CaptionBackGround = "Red";
            }
            else
            {
                this.CaptionBackGround = "DodgerBlue";
            }
            this.DataContext = this;
        }

        public string Caption { get; set; }
        public string Message { get; set; }
        public string ShowYes { get; set; }
        public string ShowNo { get; set; }
        public string ShowOk { get; set; }
        public string CaptionBackGround { get; set; }

        private void BtnYesClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnOkClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Dispatcher.BeginInvoke(new Action(() => this.DragMove()));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
