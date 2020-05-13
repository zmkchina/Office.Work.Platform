using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Office.Work.Platform.MemberUc
{
    public partial class UC_Holiday : UserControl
    {
        private UcViewModel _UcViewModel;
        public UC_Holiday()
        {
            InitializeComponent();
            _UcViewModel = new UcViewModel();
        }
        public async void initControlAsync(Lib.Member PMember)
        {
            await _UcViewModel.InitDataAsync(PMember);
            this.DataContext = _UcViewModel;
        }
        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            await _UcViewModel.SearchRecords();
        }
        /// <summary>
        ///  新增记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddClickAsync(object sender, RoutedEventArgs e)
        {
            Lib.MemberHoliday NewRecord = new Lib.MemberHoliday()
            {
                MemberId = _UcViewModel.CurMember.Id,
                UserId = AppSet.LoginUser.Id
            };

            UC_HolidayWin AddWin = new UC_HolidayWin(NewRecord);
            AddWin.Owner = AppSet.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                ExcuteResult excuteResult = await DataMemberHolidayRepository.AddRecord(NewRecord);
                if (excuteResult.State == 0)
                {
                    NewRecord.Id = excuteResult.Tag;
                    _UcViewModel.CurRecords.Add(NewRecord);
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
            if (RecordListBox.SelectedItem is Lib.MemberHoliday SelectedRec)
            {
                if ((new WinMsgDialog($"确认要删除该条简历吗？", Caption: "确认", showYesNo: true)).ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberHolidayRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _UcViewModel.CurRecords.Remove(SelectedRec);
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
            if (RecordListBox.SelectedItem is Lib.MemberHoliday SelectedRec)
            {
                Lib.MemberHoliday RecCloneObj = CloneObject<Lib.MemberHoliday, Lib.MemberHoliday>.Trans(SelectedRec);

                UC_HolidayWin AddWin = new UC_HolidayWin(RecCloneObj);
                AddWin.Owner = AppSet.AppMainWindow;

                if (AddWin.ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberHolidayRepository.UpdateRecord(RecCloneObj);
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

        /// <summary>
        /// 本控件的视图模型
        /// </summary>
        private class UcViewModel : NotificationObject
        {
            public UcViewModel()
            {
                CurRecords = new ObservableCollection<MemberHoliday>();
                SearchCondition = new MemberHolidaySearch();
            }
            public async System.Threading.Tasks.Task InitDataAsync(Lib.Member PMember)
            {
                CurMember = PMember;
                if (PMember != null)
                {
                    MemberHolidaySearch SearchCondition = new MemberHolidaySearch() { MemberId = PMember.Id, UserId = AppSet.LoginUser.Id };
                    IEnumerable<MemberHoliday> MemberHolidayss = await DataMemberHolidayRepository.GetRecords(SearchCondition);
                    CurRecords.Clear();
                    MemberHolidayss.ToList().ForEach(e =>
                    {
                        CurRecords.Add(e);
                    });
                }
            }
            public async System.Threading.Tasks.Task SearchRecords()
            {
                if (SearchCondition != null)
                {
                    SearchCondition.MemberId = CurMember.Id;
                    SearchCondition.UserId = AppSet.LoginUser.Id;

                    IEnumerable<MemberHoliday> TempRecords = await DataMemberHolidayRepository.GetRecords(SearchCondition);
                    CurRecords.Clear();
                    TempRecords.ToList().ForEach(e =>
                    {
                        CurRecords.Add(e);
                    });
                }
            }
            /// <summary>
            /// 查询条件类对象
            /// </summary>
            public MemberHolidaySearch SearchCondition { get; set; }
            /// <summary>
            /// 当前职工信息
            /// </summary>
            public Lib.Member CurMember { get; set; }
            /// <summary>
            /// 当前职工工资月度发放记录
            /// </summary>
            public ObservableCollection<MemberHoliday> CurRecords { get; set; }
        }
    }
}
