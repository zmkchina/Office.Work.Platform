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
    public partial class UC_PayMonthInsurance : UserControl
    {
        private UC_PayMonthInsuranceVM _UCPayMonthInsuranceVM;
        public UC_PayMonthInsurance()
        {
            InitializeComponent();
            _UCPayMonthInsuranceVM = new UC_PayMonthInsuranceVM();
        }
        public async void initControlAsync(Lib.Member PMember)
        {
            await _UCPayMonthInsuranceVM.InitVMAsync(PMember);
            this.DataContext = _UCPayMonthInsuranceVM;
        }
        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            await _UCPayMonthInsuranceVM.SearchRecords();
        }
        /// <summary>
        ///  新增记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddClickAsync(object sender, RoutedEventArgs e)
        {
            Lib.MemberPayMonthInsurance NewPayMonth = new Lib.MemberPayMonthInsurance()
            {
                Id = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                MemberId = _UCPayMonthInsuranceVM.CurMember.Id,
                UserId = AppSettings.LoginUser.Id
            };

            UC_PayMonthInsuranceWin AddWin = new UC_PayMonthInsuranceWin(NewPayMonth);
            AddWin.Owner = AppSettings.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                IEnumerable<MemberPayMonthInsurance> MemberPlayMonths = await DataMemberPayMonthInsuranceRepository.GetRecords(new MemberPayMonthInsuranceSearch()
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

                ExcuteResult excuteResult = await DataMemberPayMonthInsuranceRepository.AddRecord(NewPayMonth);
                if (excuteResult.State == 0)
                {
                    _UCPayMonthInsuranceVM.PayMonthInsurances.Add(NewPayMonth);
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
            if (RecordDataGrid.SelectedItem is Lib.MemberPayMonthInsurance SelectedRec)
            {
                if (MessageBox.Show($"确认要删除 {SelectedRec.PayMonth} 月份待遇吗？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    ExcuteResult excuteResult = await DataMemberPayMonthInsuranceRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _UCPayMonthInsuranceVM.PayMonthInsurances.Remove(SelectedRec);
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
            if (RecordDataGrid.SelectedItem is Lib.MemberPayMonthInsurance SelectedRec)
            {
                Lib.MemberPayMonthInsurance RecCloneObj = CloneObject<Lib.MemberPayMonthInsurance, Lib.MemberPayMonthInsurance>.Trans(SelectedRec);

                UC_PayMonthInsuranceWin AddWin = new UC_PayMonthInsuranceWin(RecCloneObj);
                AddWin.Owner = AppSettings.AppMainWindow;

                if (AddWin.ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberPayMonthInsuranceRepository.UpdateRecord(RecCloneObj);
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
