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
    public partial class UC_Appraise : UserControl
    {
        private CurUcViewModel _CurUcViewModel;
        public UC_Appraise()
        {
            InitializeComponent();
            _CurUcViewModel = new CurUcViewModel();
        }
        public async void initControlAsync(Lib.MemberInfoEntity PMember)
        {
            await _CurUcViewModel.InitVMAsync(PMember);
            this.DataContext = _CurUcViewModel;
        }
        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            await _CurUcViewModel.SearchRecords();
        }
        /// <summary>
        ///  新增记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddClickAsync(object sender, RoutedEventArgs e)
        {
            Lib.MemberAppraise NewRecord = new Lib.MemberAppraise()
            {
                MemberId = _CurUcViewModel.CurMember.Id,
                UserId = AppSet.LoginUser.Id
            };

            UC_AppraiseWin AddWin = new UC_AppraiseWin(NewRecord);

            if (AddWin.ShowDialog().Value)
            {
                IEnumerable<MemberAppraise> MemberPlayMonths = await DataMemberAppraiseRepository.GetRecords(new MemberAppraiseSearch()
                {
                    MemberId = NewRecord.MemberId,
                    UserId = NewRecord.UserId
                });

                ExcuteResult excuteResult = await DataMemberAppraiseRepository.AddRecord(NewRecord);
                if (excuteResult.State == 0)
                {
                    NewRecord.Id = excuteResult.Tag;
                    _CurUcViewModel.CurRecords.Add(NewRecord);
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
            if (RecordListBox.SelectedItem is Lib.MemberAppraise SelectedRec)
            {
                if (AppFuns.ShowMessage($"确认要删除该条考核信息吗？", Caption: "确认", showYesNo: true))
                {
                    ExcuteResult excuteResult = await DataMemberAppraiseRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _CurUcViewModel.CurRecords.Remove(SelectedRec);
                    }
                    else
                    {
                        AppFuns.ShowMessage(excuteResult.Msg, Caption: "失败" ,isErr: true);
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
            if (RecordListBox.SelectedItem is Lib.MemberAppraise SelectedRec)
            {
                Lib.MemberAppraise RecCloneObj = CloneObject<Lib.MemberAppraise, Lib.MemberAppraise>.Trans(SelectedRec);

                UC_AppraiseWin AddWin = new UC_AppraiseWin(RecCloneObj);

                if (AddWin.ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberAppraiseRepository.UpdateRecord(RecCloneObj);
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
                        AppFuns.ShowMessage(excuteResult.Msg, Caption: "失败", isErr: true);
                    }
                }
            }
        }

        /// <summary>
        /// 本控件的视图模型
        /// </summary>
        private class CurUcViewModel : NotificationObject
        {
            public CurUcViewModel()
            {
                CurRecords = new ObservableCollection<MemberAppraise>();
                SearchCondition = new MemberAppraiseSearch();
            }
            public async System.Threading.Tasks.Task InitVMAsync(Lib.MemberInfoEntity PMember)
            {
                CurMember = PMember;
                if (PMember != null)
                {
                    MemberAppraiseSearch SearchCondition = new MemberAppraiseSearch() { MemberId = PMember.Id, UserId = AppSet.LoginUser.Id };
                    IEnumerable<MemberAppraise> MemberPrizePunishss = await DataMemberAppraiseRepository.GetRecords(SearchCondition);
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

                    IEnumerable<MemberAppraise> TempRecords = await DataMemberAppraiseRepository.GetRecords(SearchCondition);
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
            public MemberAppraiseSearch SearchCondition { get; set; }
            /// <summary>
            /// 当前职工信息
            /// </summary>
            public Lib.MemberInfoEntity CurMember { get; set; }
            /// <summary>
            /// 当前职工工资月度发放记录
            /// </summary>
            public ObservableCollection<MemberAppraise> CurRecords { get; set; }
        }
    }
}
