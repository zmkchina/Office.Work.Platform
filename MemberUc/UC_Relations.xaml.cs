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
        private UC_RelationsVM _UCRelationsVM;
        public UC_Relations()
        {
            InitializeComponent();
            _UCRelationsVM = new UC_RelationsVM();
        }
        public async void initControlAsync(Lib.Member PMember)
        {
            await _UCRelationsVM.InitVMAsync(PMember);
            this.DataContext = _UCRelationsVM;
        }
        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            await _UCRelationsVM.SearchRecords();
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
                MemberId = _UCRelationsVM.CurMember.Id,
                UserId = AppSet.LoginUser.Id
            };

            UC_RelationsWin AddWin = new UC_RelationsWin(NewRecord);
            AddWin.Owner = AppSet.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                IEnumerable<MemberRelations> MemberPlayMonths = await DataMemberRelationsRepository.GetRecords(new MemberRelationsSearch()
                {
                    MemberId = NewRecord.MemberId,
                    UserId = NewRecord.UserId
                });

                ExcuteResult excuteResult = await DataMemberRelationsRepository.AddRecord(NewRecord);
                if (excuteResult.State == 0)
                {
                    NewRecord.Id = excuteResult.Tag;
                    _UCRelationsVM.CurRecords.Add(NewRecord);
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
            if (RecordListBox.SelectedItem is Lib.MemberRelations SelectedRec)
            {
                if ((new WinMsgDialog($"确认要删除该条社会关系吗？", Caption: "确认", showYesNo: true)).ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberRelationsRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _UCRelationsVM.CurRecords.Remove(SelectedRec);
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
                        (new WinMsgDialog(excuteResult.Msg, Caption: "失败")).ShowDialog();
                    }
                }
            }
        }
    }

    public class UC_RelationsVM : NotificationObject
    {
        public UC_RelationsVM()
        {
            CurRecords = new ObservableCollection<MemberRelations>();
            SearchCondition = new MemberRelationsSearch();
        }
        public async System.Threading.Tasks.Task InitVMAsync(Lib.Member PMember)
        {
            CurMember = PMember;
            if (PMember != null)
            {
                MemberRelationsSearch SearchCondition = new MemberRelationsSearch() { MemberId = PMember.Id, UserId = AppSet.LoginUser.Id };
                IEnumerable<MemberRelations> MemberRelationsss = await DataMemberRelationsRepository.GetRecords(SearchCondition);
                CurRecords.Clear();
                MemberRelationsss.ToList().ForEach(e =>
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

                IEnumerable<MemberRelations> TempRecords = await DataMemberRelationsRepository.GetRecords(SearchCondition);
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
        public MemberRelationsSearch SearchCondition { get; set; }
        /// <summary>
        /// 当前职工信息
        /// </summary>
        public Lib.Member CurMember { get; set; }
        /// <summary>
        /// 当前职工工资月度发放记录
        /// </summary>
        public ObservableCollection<MemberRelations> CurRecords { get; set; }
    }
}
