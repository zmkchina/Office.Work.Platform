using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberUc
{
    /// <summary>
    /// UC_PayTemp.xaml 的交互逻辑
    /// </summary>
    public partial class UC_PayMonth : UserControl
    {
        private UC_PayMonthVM _UCPayMonthVM;
        public UC_PayMonth()
        {
            InitializeComponent();
            _UCPayMonthVM = new UC_PayMonthVM();
        }
        public async void initControlAsync(Lib.Member PMember)
        {
            await _UCPayMonthVM.InitVMAsync(PMember);
            this.DataContext = _UCPayMonthVM;
        }
        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            await _UCPayMonthVM.SearchRecords();
        }
        /// <summary>
        ///  新增记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddClickAsync(object sender, RoutedEventArgs e)
        {
            Lib.MemberPayMonth NewPayMonth = new Lib.MemberPayMonth()
            {
                Id = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                MemberId = _UCPayMonthVM.CurMember.Id,
                UserId = AppSettings.LoginUser.Id
            };

            UC_PayMonthWin AddWin = new UC_PayMonthWin(NewPayMonth);
            AddWin.Owner = AppSettings.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                IEnumerable<MemberPayMonth> MemberPlayMonths = await DataMemberPayMonthRepository.GetRecords(new MemberPayMonthSearch()
                {
                    MemberId = NewPayMonth.MemberId,
                    PayYear = NewPayMonth.PayYear,
                    PayMonth = NewPayMonth.PayMonth,
                    UserId = NewPayMonth.UserId
                });
                if (MemberPlayMonths.Count() > 0)
                {
                    MessageBox.Show($"该工作人员 {NewPayMonth.PayYear} 年 {NewPayMonth.PayMonth} 月份待遇已发放，无法新增。", "失败");
                    return;
                }

                ExcuteResult excuteResult = await DataMemberPayMonthRepository.AddRecord(NewPayMonth);
                if (excuteResult.State == 0)
                {
                    _UCPayMonthVM.PayMonths.Add(NewPayMonth);
                }
                else
                { MessageBox.Show(excuteResult.Msg, "失败"); }
            }
        }
        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnDelClickAsync(object sender, RoutedEventArgs e)
        {
            if (RecordDataGrid.SelectedItem is Lib.MemberPayMonth SelectedRec)
            {
                if (MessageBox.Show($"确认要删除 {SelectedRec.PayMonth} 月份待遇吗？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    ExcuteResult excuteResult = await DataMemberPayMonthRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _UCPayMonthVM.PayMonths.Remove(SelectedRec);
                    }
                    else
                    { MessageBox.Show(excuteResult.Msg, "失败"); }
                }
            }
        }
        /// <summary>
        /// 编辑一条记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnEditClickAsync(object sender, RoutedEventArgs e)
        {
            if (RecordDataGrid.SelectedItem is Lib.MemberPayMonth SelectedRec)
            {
                Lib.MemberPayMonth RecCloneObj = CloneObject<Lib.MemberPayMonth, Lib.MemberPayMonth>.Trans(SelectedRec);

                UC_PayMonthWin AddWin = new UC_PayMonthWin(RecCloneObj);
                AddWin.Owner = AppSettings.AppMainWindow;

                if (AddWin.ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberPayMonthRepository.UpdateRecord(RecCloneObj);
                    if (excuteResult.State == 0)
                    {
                        PropertyInfo[] TargetAttris = SelectedRec.GetType().GetProperties();
                        PropertyInfo[] SourceAttris = RecCloneObj.GetType().GetProperties();
                        foreach (PropertyInfo item in SourceAttris)
                        {
                            var tempObj = TargetAttris.Where(x => x.Name.Equals(item.Name, StringComparison.Ordinal)).FirstOrDefault();
                            if (tempObj != null)
                            {
                                item.SetValue(SelectedRec, item.GetValue(RecCloneObj));
                            }
                        }
                    }
                    else
                    { MessageBox.Show(excuteResult.Msg, "失败"); }
                }
            }
        }
    }
}
