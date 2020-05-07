using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Office.Work.Platform.AppDataService;

namespace Office.Work.Platform.Remuneration
{
    /// <summary>
    /// 此类用于新增快速发放工资人员对话框
    /// </summary>
    public partial class PageMemberPayFastWin : Window
    {
        public string InputMemberId { get; set; }
        public Lib.Member CurMember { get; set; }

        public PageMemberPayFastWin(Lib.Member PMember)
        {
            InitializeComponent();
            this.Owner = App.Current.MainWindow;
            CurMember = PMember;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Btn_Ok.IsEnabled = false;
            this.DataContext = this;
        }

        private void BtnSaveClickAsync(object sender, RoutedEventArgs e)
        {
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
        /// <summary>
        /// 查询用户。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearch_ClickAsync(object sender, RoutedEventArgs e)
        {
            List<Lib.Member> MemberList = await DataMemberRepository.ReadMembers(new Lib.MemberSearch() { Id = InputMemberId }).ConfigureAwait(false);
            Dispatcher.Invoke(new Action(() =>
            {
                if (MemberList.Count == 1)
                {
                    Btn_Ok.IsEnabled = true;
                    PropertyInfo[] TargetAttris = CurMember.GetType().GetProperties();
                    PropertyInfo[] SourceAttris = MemberList[0].GetType().GetProperties();
                    foreach (PropertyInfo item in SourceAttris)
                    {
                        var tempObj = TargetAttris.Where(x => x.Name.Equals(item.Name, StringComparison.Ordinal)).FirstOrDefault();
                        if (tempObj != null)
                        {
                            item.SetValue(CurMember, item.GetValue(MemberList[0]));
                        }
                        Btn_Ok.IsEnabled = true;
                    }
                }
                else
                {
                    (new WinMsgDialog("未找到该身份号码的职工信息！")).ShowDialog();
                }
            }));
        }
    }
}
