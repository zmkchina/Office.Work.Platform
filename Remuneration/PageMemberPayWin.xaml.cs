using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Remuneration
{
    /// <summary>
    /// 此类用于新增或编辑月度待遇发放记录。
    /// 此窗体作为对话框使用，当DialogResult不被设为true时，将始终为false
    /// </summary>
    public partial class PageMemberPayWin : Window
    {
        public Lib.Member CurMember { get; set; }
        public Lib.MemberPay CurMemberPay { get; set; }
        public IEnumerable<MemberPayItem> MemberPayItems { get; set; }
        public List<string> MemberPayItemNameList { get; set; }
        public MemberPayItem SelectPayItem { get; set; }

        public PageMemberPayWin(Lib.MemberPay PMemberPay, Lib.Member PMember)
        {
            this.Owner = Application.Current.MainWindow;
            InitializeComponent();
            CurMember = PMember;
            CurMemberPay = PMemberPay;
            SelectPayItem = new MemberPayItem();

        }
        private async void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            MemberPayItems = await DataMemberPayItemRepository.GetRecords(new Lib.MemberPayItemSearch() { UnitName=AppSettings.LoginUser.UnitName }).ConfigureAwait(false);
            MemberPayItemNameList = MemberPayItems.Select(t => t.Name).ToList();
            DataContext = this;
        }

        private void BtnSaveClickAsync(object sender, RoutedEventArgs e)
        {
            CurMemberPay.AddOrCut = SelectPayItem.AddOrCut;
            CurMemberPay.InTableType = SelectPayItem.InTableType;
            CurMemberPay.InCardinality = SelectPayItem.InCardinality;
            DialogResult = true;
            this.Close();
        }

        private void BtnCancelClickAsync(object sender, RoutedEventArgs e)
        {

            CurMemberPay = null;
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
        /// 发放项目选择发生变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            MemberPayItem TempPayItem = MemberPayItems.Where(e => e.Name.Equals(CurMemberPay.PayName, StringComparison.Ordinal)).FirstOrDefault();
            if (TempPayItem != null)
            {
                PropertyInfo[] TargetAttris = SelectPayItem.GetType().GetProperties();
                PropertyInfo[] SourceAttris = TempPayItem.GetType().GetProperties();
                foreach (PropertyInfo item in SourceAttris)
                {
                    var tempObj = TargetAttris.Where(x => x.Name.Equals(item.Name, StringComparison.Ordinal)).FirstOrDefault();
                    if (tempObj != null)
                    {
                        item.SetValue(SelectPayItem, item.GetValue(TempPayItem));
                    }
                }
            }
            else
            {
                PropertyInfo[] TargetAttris = SelectPayItem.GetType().GetProperties();
                foreach (PropertyInfo item in TargetAttris)
                {
                    item.SetValue(SelectPayItem, null);
                }
            }
        }
    }
}
