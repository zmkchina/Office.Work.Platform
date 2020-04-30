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
    public partial class UC_PayTemp : UserControl
    {
        private UC_PayTempVM _UCPayTempVM;
        public UC_PayTemp()
        {
            InitializeComponent();
            _UCPayTempVM = new UC_PayTempVM();
        }
        public async void initControlAsync(Lib.Member PMember)
        {
            await _UCPayTempVM.InitVMAsync(PMember);
            this.DataContext = _UCPayTempVM;
        }
        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            await _UCPayTempVM.SearchRecords();
        }
        /// <summary>
        ///  新增记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddClickAsync(object sender, RoutedEventArgs e)
        {
            Lib.MemberPayTemp NewRecord = new Lib.MemberPayTemp()
            {
                MemberId = _UCPayTempVM.CurMember.Id,
                UserId = AppSettings.LoginUser.Id
            };

            UC_PayTempWin AddWin = new UC_PayTempWin(NewRecord);
            AddWin.Owner = AppSettings.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                IEnumerable<MemberPayTemp> MemberPlayMonths = await DataMemberPayTempRepository.GetRecords(new MemberPayTempSearch()
                {
                    MemberId = NewRecord.MemberId,
                    UserId = NewRecord.UserId
                });

                ExcuteResult excuteResult = await DataMemberPayTempRepository.AddRecord(NewRecord);
                if (excuteResult.State == 0)
                {
                    NewRecord.Id = excuteResult.Tag;
                    _UCPayTempVM.PayTemps.Add(NewRecord);
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
            if (RecordDataGrid.SelectedItem is Lib.MemberPayTemp SelectedRec)
            {
                if (MessageBox.Show($"确认要删除名为 {SelectedRec.TypeName} 的补充待遇记录吗？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    ExcuteResult excuteResult = await DataMemberPayTempRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _UCPayTempVM.PayTemps.Remove(SelectedRec);
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
            if (RecordDataGrid.SelectedItem is Lib.MemberPayTemp SelectedRec)
            {
                Lib.MemberPayTemp RecCloneObj = CloneObject<Lib.MemberPayTemp, Lib.MemberPayTemp>.Trans(SelectedRec);

                UC_PayTempWin AddWin = new UC_PayTempWin(RecCloneObj);
                AddWin.Owner = AppSettings.AppMainWindow;

                if (AddWin.ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberPayTempRepository.UpdateRecord(RecCloneObj);
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
