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
    public partial class UC_Relations : UserControl
    {
        private CurUcViewModel _CurUcViewModel;
        public UC_Relations()
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
            Lib.MemberRelations NewRecord = new Lib.MemberRelations()
            {
                MemberId = _CurUcViewModel.CurMember.Id,
                Birthday = _CurUcViewModel.CurMember.Birthday.AddYears(-25),
                UserId = AppSet.LoginUser.Id
            };

            UC_RelationsWin AddWin = new UC_RelationsWin(NewRecord);
            AddWin.Owner = AppSet.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                ExcuteResult excuteResult = await DataMemberRelationsRepository.AddRecord(NewRecord);
                if (excuteResult.State == 0)
                {
                    NewRecord.Id = excuteResult.Tag;
                    _CurUcViewModel.CurRecords.Add(NewRecord);
                    _CurUcViewModel.CurRecords.OrderBy(x => x.OrderIndex);
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
            if (RecordListBox.SelectedItem is Lib.MemberRelations SelectedRec)
            {
                if (AppFuns.ShowMessage($"确认要删除该条社会关系吗？", Caption: "确认", showYesNo: true))
                {
                    ExcuteResult excuteResult = await DataMemberRelationsRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _CurUcViewModel.CurRecords.Remove(SelectedRec);
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
            if (RecordListBox.SelectedItem is Lib.MemberRelations SelectedRec)
            {
                Lib.MemberRelations RecCloneObj = CloneObject<Lib.MemberRelations, Lib.MemberRelations>.Trans(SelectedRec);

                UC_RelationsWin AddWin = new UC_RelationsWin(RecCloneObj);
                AddWin.Owner = AppSet.AppMainWindow;

                if (AddWin.ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberRelationsRepository.UpdateRecord(RecCloneObj);
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
                        AppFuns.ShowMessage(excuteResult.Msg, Caption: "失败");
                    }
                }
            }
        }


        /// <summary>
        /// 当前控件的视图模型
        /// </summary>
        private class CurUcViewModel : NotificationObject
        {
            public CurUcViewModel()
            {
                CurRecords = new ObservableCollection<MemberRelations>();
                SearchCondition = new MemberRelationsSearch();
            }
            public async System.Threading.Tasks.Task InitVMAsync(Lib.MemberInfoEntity PMember)
            {
                CurMember = PMember;
                CurRecords.Clear();
                await SearchRecords();
            }
            public async System.Threading.Tasks.Task SearchRecords()
            {
                if (CurMember == null) { return; }
                CurRecords.Clear();
                SearchCondition.MemberId = CurMember.Id;
                SearchCondition.UserId = AppSet.LoginUser.Id;
                IEnumerable<MemberRelations> TempRecords = await DataMemberRelationsRepository.GetRecords(SearchCondition);
                TempRecords?.ToList().ForEach(e =>
                {
                    CurRecords.Add(e);
                });
            }
            /// <summary>
            /// 查询条件类对象
            /// </summary>
            public MemberRelationsSearch SearchCondition { get; set; }
            /// <summary>
            /// 当前职工信息
            /// </summary>
            public Lib.MemberInfoEntity CurMember { get; set; }
            /// <summary>
            /// 当前职工工资月度发放记录
            /// </summary>
            public ObservableCollection<MemberRelations> CurRecords { get; set; }
        }
    }
}
