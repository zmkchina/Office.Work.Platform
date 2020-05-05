using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Remuneration
{
    /// <summary>
    /// PageMemberPay.xaml 的交互逻辑
    /// </summary>
    public partial class PageMemberPay : Page
    {
        private PageMemberPayVM _PageMemberPayVM;
        public PageMemberPay()
        {
            InitializeComponent();
            _PageMemberPayVM = new PageMemberPayVM();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //await _PageMemberPayVM.InitVMAsync(PMember);
            //this.DataContext = _PageMemberPayVM;
            //UcMemberPay.initControlAsync(_PageEditMemberVM.EntityMember);
            //await UcMemberPayFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember, "月度税费", isRead);
        }
        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            await _PageMemberPayVM.SearchRecords();
        }
        /// <summary>
        ///  新增记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddClickAsync(object sender, RoutedEventArgs e)
        {
            Lib.MemberPay NewRecord = new Lib.MemberPay()
            {
                MemberId = _PageMemberPayVM.CurMember.Id,
                UserId = AppSettings.LoginUser.Id
            };

            PageMemberPayWin AddWin = new PageMemberPayWin(NewRecord, _PageMemberPayVM.CurMember);
            AddWin.Owner = AppSettings.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                IEnumerable<MemberPay> MemberPlays = await DataMemberPayRepository.GetRecords(new MemberPaySearch()
                {
                    MemberId = NewRecord.MemberId,
                    PayYear = NewRecord.PayDate.Year,
                    PayMonth = NewRecord.PayDate.Month,
                    UserId = NewRecord.UserId
                });
                if (MemberPlays.Count() > 0)
                {
                    (new WinMsgDialog($"该工作人员{NewRecord.PayName}项目的 {NewRecord.PayDate.Year} 年 {NewRecord.PayDate.Month} 月份待遇已发放，无法新增。")).ShowDialog();
                    return;
                }

                ExcuteResult excuteResult = await DataMemberPayRepository.AddRecord(NewRecord);
                if (excuteResult.State == 0)
                {
                    NewRecord.Id = excuteResult.Tag;
                    _PageMemberPayVM.PayMonths.Add(NewRecord);
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
            if (RecordDataGrid.SelectedItem is Lib.MemberPay SelectedRec)
            {

                if ((new WinMsgDialog($"确认要删除 {SelectedRec.PayDate.Month} 月份待遇吗？", Caption: "确认", showYesNo: true)).ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberPayRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _PageMemberPayVM.PayMonths.Remove(SelectedRec);
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
            if (RecordDataGrid.SelectedItem is Lib.MemberPay SelectedRec)
            {
                Lib.MemberPay RecCloneObj = CloneObject<Lib.MemberPay, Lib.MemberPay>.Trans(SelectedRec);

                PageMemberPayWin AddWin = new PageMemberPayWin(RecCloneObj, _PageMemberPayVM.CurMember);
                AddWin.Owner = AppSettings.AppMainWindow;

                if (AddWin.ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberPayRepository.UpdateRecord(RecCloneObj);
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


    public class PageMemberPayVM : NotificationObject
    {
        public PageMemberPayVM()
        {
            PayMonths = new ObservableCollection<MemberPay>();
            SearchCondition = new MemberPaySearch();
        }
        public async System.Threading.Tasks.Task InitVMAsync(Lib.Member PMember)
        {
            CurMember = PMember;
            if (PMember != null)
            {
                MemberPaySearch SearchCondition = new MemberPaySearch() { MemberId = PMember.Id, UserId = AppSettings.LoginUser.Id };
                IEnumerable<MemberPay> MemberPlayMonths = await DataMemberPayRepository.GetRecords(SearchCondition);
                PayMonths.Clear();
                MemberPlayMonths.ToList().ForEach(e =>
                {
                    PayMonths.Add(e);
                });
            }
        }
        public async System.Threading.Tasks.Task SearchRecords()
        {
            if (SearchCondition != null)
            {
                SearchCondition.MemberId = CurMember.Id;
                SearchCondition.UserId = AppSettings.LoginUser.Id;

                IEnumerable<MemberPay> MemberPlayMonths = await DataMemberPayRepository.GetRecords(SearchCondition);
                PayMonths.Clear();
                MemberPlayMonths.ToList().ForEach(e =>
                {
                    PayMonths.Add(e);
                });
            }
        }
        /// <summary>
        /// 查询条件类对象
        /// </summary>
        public MemberPaySearch SearchCondition { get; set; }
        /// <summary>
        /// 当前职工信息
        /// </summary>
        public Lib.Member CurMember { get; set; }
        /// <summary>
        /// 当前职工工资月度发放记录
        /// </summary>
        public ObservableCollection<MemberPay> PayMonths { get; set; }

    }
}
