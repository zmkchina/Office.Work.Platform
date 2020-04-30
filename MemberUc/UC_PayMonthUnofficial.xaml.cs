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
    public partial class UC_PayMonthUnofficial : UserControl
    {
        private UC_PayMonthUnofficialVM _UCPayMonthUnofficialVM;
        public UC_PayMonthUnofficial()
        {
            InitializeComponent();
            _UCPayMonthUnofficialVM = new UC_PayMonthUnofficialVM();
        }
        public async void initControlAsync(Lib.Member PMember)
        {
            await _UCPayMonthUnofficialVM.InitVMAsync(PMember);
            this.DataContext = _UCPayMonthUnofficialVM;
        }
        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            await _UCPayMonthUnofficialVM.SearchRecords();
        }
        /// <summary>
        ///  新增记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddClickAsync(object sender, RoutedEventArgs e)
        {
            Lib.MemberPayMonthUnofficial NewRecord = new Lib.MemberPayMonthUnofficial()
            {
                MemberId = _UCPayMonthUnofficialVM.CurMember.Id,
                UserId = AppSettings.LoginUser.Id
            };

            UC_PayMonthUnofficialWin AddWin = new UC_PayMonthUnofficialWin(NewRecord);
            AddWin.Owner = AppSettings.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                IEnumerable<MemberPayMonthUnofficial> MemberPayMonthUnofficials = await DataMemberPayMonthUnofficialRepository.GetRecords(new MemberPayMonthUnofficialSearch()
                {
                    MemberId = NewRecord.MemberId,
                    PayYear = NewRecord.PayYear,
                    PayMonth = NewRecord.PayMonth,
                    UserId = NewRecord.UserId
                });
                if (MemberPayMonthUnofficials.Count() > 0)
                {
                    MessageBox.Show($"该工作人员 {NewRecord.PayYear} 年 {NewRecord.PayMonth} 月份待遇已发放，无法新增。", "失败");
                    return;
                }

                ExcuteResult excuteResult = await DataMemberPayMonthUnofficialRepository.AddRecord(NewRecord);
                if (excuteResult.State == 0)
                {
                    NewRecord.Id = excuteResult.Tag;
                    _UCPayMonthUnofficialVM.PayMonthUnofficials.Add(NewRecord);
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
            if (RecordDataGrid.SelectedItem is Lib.MemberPayMonthUnofficial SelectedRec)
            {
                if (MessageBox.Show($"确认要删除 {SelectedRec.PayMonth} 月份待遇吗？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    ExcuteResult excuteResult = await DataMemberPayMonthUnofficialRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _UCPayMonthUnofficialVM.PayMonthUnofficials.Remove(SelectedRec);
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
            if (RecordDataGrid.SelectedItem is Lib.MemberPayMonthUnofficial SelectedRec)
            {
                Lib.MemberPayMonthUnofficial RecCloneObj = CloneObject<Lib.MemberPayMonthUnofficial, Lib.MemberPayMonthUnofficial>.Trans(SelectedRec);

                UC_PayMonthUnofficialWin AddWin = new UC_PayMonthUnofficialWin(RecCloneObj);
                AddWin.Owner = AppSettings.AppMainWindow;

                if (AddWin.ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberPayMonthUnofficialRepository.UpdateRecord(RecCloneObj);
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
