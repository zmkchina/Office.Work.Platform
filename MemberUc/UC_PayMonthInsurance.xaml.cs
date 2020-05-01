﻿using System;
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
            Lib.MemberPayMonthInsurance NewRecord = new Lib.MemberPayMonthInsurance()
            {
                MemberId = _UCPayMonthInsuranceVM.CurMember.Id,
                UserId = AppSettings.LoginUser.Id
            };

            UC_PayMonthInsuranceWin AddWin = new UC_PayMonthInsuranceWin(NewRecord);
            AddWin.Owner = AppSettings.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                IEnumerable<MemberPayMonthInsurance> MemberPlayMonths = await DataMemberPayMonthInsuranceRepository.GetRecords(new MemberPayMonthInsuranceSearch()
                {
                    MemberId = NewRecord.MemberId,
                    PayYear = NewRecord.PayYear,
                    PayMonth = NewRecord.PayMonth,
                    UserId = NewRecord.UserId
                });
                if (MemberPlayMonths.Count() > 0)
                {
                    (new WinMsgDialog($"该工作人员 {NewRecord.PayYear} 年 {NewRecord.PayMonth} 月份待遇已发放，无法新增。")).ShowDialog();
                    return;
                }

                ExcuteResult excuteResult = await DataMemberPayMonthInsuranceRepository.AddRecord(NewRecord);
                if (excuteResult.State == 0)
                {
                    NewRecord.Id = excuteResult.Tag;
                    _UCPayMonthInsuranceVM.PayMonthInsurances.Add(NewRecord);
                }
                else
                {
                    (new WinMsgDialog(excuteResult.Msg, Caption: "失败")).ShowDialog();
                }
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
                if ((new WinMsgDialog($"确认要删除 {SelectedRec.PayMonth} 月份待遇吗？", Caption: "确认", showYesNo: true)).ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberPayMonthInsuranceRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _UCPayMonthInsuranceVM.PayMonthInsurances.Remove(SelectedRec);
                    }
                    else
                    {
                        (new WinMsgDialog(excuteResult.Msg, Caption: "失败")).ShowDialog();
                    }
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
                    {
                        (new WinMsgDialog(excuteResult.Msg, Caption: "失败")).ShowDialog();
                    }
                }
            }
        }
    }
}
