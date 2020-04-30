using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Office.Work.Platform.MemberUc
{
    public partial class UC_Resume : UserControl
    {
        private UC_ResumeVM _UCResumeVM;
        public UC_Resume()
        {
            InitializeComponent();
            _UCResumeVM = new UC_ResumeVM();
        }
        public async void initControlAsync(Lib.Member PMember)
        {
            await _UCResumeVM.InitVMAsync(PMember);
            this.DataContext = _UCResumeVM;
        }
        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            await _UCResumeVM.SearchRecords();
        }
        /// <summary>
        ///  新增记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddClickAsync(object sender, RoutedEventArgs e)
        {
            Lib.MemberResume NewRecord = new Lib.MemberResume()
            {
                MemberId = _UCResumeVM.CurMember.Id,
                UserId = AppSettings.LoginUser.Id
            };

            UC_ResumeWin AddWin = new UC_ResumeWin(NewRecord);
            AddWin.Owner = AppSettings.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                IEnumerable<MemberResume> MemberPlayMonths = await DataMemberResumeRepository.GetRecords(new MemberResumeSearch()
                {
                    MemberId = NewRecord.MemberId,
                    UserId = NewRecord.UserId
                });

                ExcuteResult excuteResult = await DataMemberResumeRepository.AddRecord(NewRecord);
                if (excuteResult.State == 0)
                {
                    NewRecord.Id = excuteResult.Tag;
                    _UCResumeVM.CurRecords.Add(NewRecord);
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
            if (RecordListBox.SelectedItem is Lib.MemberResume SelectedRec)
            {
                if (MessageBox.Show($"确认要删除该条简历吗？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    ExcuteResult excuteResult = await DataMemberResumeRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _UCResumeVM.CurRecords.Remove(SelectedRec);
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
            if (RecordListBox.SelectedItem is Lib.MemberResume SelectedRec)
            {
                Lib.MemberResume RecCloneObj = CloneObject<Lib.MemberResume, Lib.MemberResume>.Trans(SelectedRec);

                UC_ResumeWin AddWin = new UC_ResumeWin(RecCloneObj);
                AddWin.Owner = AppSettings.AppMainWindow;

                if (AddWin.ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberResumeRepository.UpdateRecord(RecCloneObj);
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
