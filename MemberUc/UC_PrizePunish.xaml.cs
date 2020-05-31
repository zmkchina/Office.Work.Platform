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
    public partial class UC_PrizePunish : UserControl
    {
        private UC_PrizePunishVM _UCPrizePunishVM;
        public UC_PrizePunish()
        {
            InitializeComponent();
            _UCPrizePunishVM = new UC_PrizePunishVM();
        }
        public async void initControlAsync(Lib.Member PMember)
        {
            await _UCPrizePunishVM.InitVMAsync(PMember);
            this.DataContext = _UCPrizePunishVM;
        }
        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            await _UCPrizePunishVM.SearchRecords();
        }
        /// <summary>
        ///  新增记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddClickAsync(object sender, RoutedEventArgs e)
        {
            Lib.MemberPrizePunish NewRecord = new Lib.MemberPrizePunish()
            {
                MemberId = _UCPrizePunishVM.CurMember.Id,
                UserId = AppSet.LoginUser.Id
            };

            UC_PrizePunishWin AddWin = new UC_PrizePunishWin(NewRecord);
            AddWin.Owner = AppSet.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                IEnumerable<MemberPrizePunish> MemberPlayMonths = await DataMemberPrizePunishRepository.GetRecords(new MemberPrizePunishSearch()
                {
                    MemberId = NewRecord.MemberId,
                    UserId = NewRecord.UserId
                });

                ExcuteResult excuteResult = await DataMemberPrizePunishRepository.AddRecord(NewRecord);
                if (excuteResult.State == 0)
                {
                    NewRecord.Id = excuteResult.Tag;
                    _UCPrizePunishVM.CurRecords.Add(NewRecord);
                }
                else
                {
                     AppFuns.ShowMessage(excuteResult.Msg, Caption: "失败");
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
            if (RecordListBox.SelectedItem is Lib.MemberPrizePunish SelectedRec)
            {
                if ( AppFuns.ShowMessage($"确认要删除该条简历吗？", Caption: "确认", showYesNo: true))
                {
                    ExcuteResult excuteResult = await DataMemberPrizePunishRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _UCPrizePunishVM.CurRecords.Remove(SelectedRec);
                    }
                    else
                    {
                         AppFuns.ShowMessage(excuteResult.Msg, Caption: "失败");
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
            if (RecordListBox.SelectedItem is Lib.MemberPrizePunish SelectedRec)
            {
                Lib.MemberPrizePunish RecCloneObj = CloneObject<Lib.MemberPrizePunish, Lib.MemberPrizePunish>.Trans(SelectedRec);

                UC_PrizePunishWin AddWin = new UC_PrizePunishWin(RecCloneObj);
                AddWin.Owner = AppSet.AppMainWindow;

                if (AddWin.ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberPrizePunishRepository.UpdateRecord(RecCloneObj);
                    if (excuteResult != null && excuteResult.State == 0)
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
                         AppFuns.ShowMessage(excuteResult.Msg, Caption: "失败");
                    }
                }
            }
        }
    }


    public class UC_PrizePunishVM : NotificationObject
    {
        public UC_PrizePunishVM()
        {
            CurRecords = new ObservableCollection<MemberPrizePunish>();
            SearchCondition = new MemberPrizePunishSearch();
        }
        public async System.Threading.Tasks.Task InitVMAsync(Lib.Member PMember)
        {
            CurMember = PMember;
            if (PMember != null)
            {
                MemberPrizePunishSearch SearchCondition = new MemberPrizePunishSearch() { MemberId = PMember.Id, UserId = AppSet.LoginUser.Id };
                IEnumerable<MemberPrizePunish> MemberPrizePunishss = await DataMemberPrizePunishRepository.GetRecords(SearchCondition);
                CurRecords.Clear();
                MemberPrizePunishss?.ToList().ForEach(e =>
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

                IEnumerable<MemberPrizePunish> TempRecords = await DataMemberPrizePunishRepository.GetRecords(SearchCondition);
                CurRecords.Clear();
                TempRecords?.ToList().ForEach(e =>
                {
                    CurRecords.Add(e);
                });
            }
        }
        /// <summary>
        /// 查询条件类对象
        /// </summary>
        public MemberPrizePunishSearch SearchCondition { get; set; }
        /// <summary>
        /// 当前职工信息
        /// </summary>
        public Lib.Member CurMember { get; set; }
        /// <summary>
        /// 当前职工工资月度发放记录
        /// </summary>
        public ObservableCollection<MemberPrizePunish> CurRecords { get; set; }
    }

}
