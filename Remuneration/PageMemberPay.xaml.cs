using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
        }

        private async void Page_LoadedAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            _PageMemberPayVM = new PageMemberPayVM();
            await _PageMemberPayVM.InitVMAsync();
            //因为此函数为异步（即使用的是后台线程或者说非UI线程），故要更新界面需使用 Dispatcher 来向WPF的UI线程添加任务。
            App.Current.Dispatcher.Invoke(() =>
            {
                this.DataContext = _PageMemberPayVM;
            });

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
                MemberId = _PageMemberPayVM.CurMember.MemberId,
                MemberIndex=_PageMemberPayVM.CurMember.OrderIndex,
                UserId = AppSettings.LoginUser.Id
            };

            PageMemberPayWin AddWin = new PageMemberPayWin(NewRecord, _PageMemberPayVM.MemberPayItems.ToList());
            AddWin.Owner = AppSettings.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                IEnumerable<MemberPay> MemberPlays = await DataMemberPayRepository.GetRecords(new MemberPaySearch()
                {
                    UserId = AppSettings.LoginUser.Id,
                    MemberId = NewRecord.MemberId,
                    PayYear = NewRecord.PayYear,
                    PayMonth = NewRecord.PayMonth,
                    PayName = NewRecord.PayName,
                });
                if (MemberPlays.Count() > 0)
                {
                    (new WinMsgDialog($"该工作人员{NewRecord.PayYear} 年 {NewRecord.PayMonth} 月份的[{NewRecord.PayName}]已经发放。", "无法新增")).ShowDialog();
                    return;
                }

                ExcuteResult excuteResult = await DataMemberPayRepository.AddRecord(NewRecord);
                if (excuteResult != null)
                {
                    if (excuteResult.State == 0)
                    {
                        NewRecord.Id = excuteResult.Tag;
                        _PageMemberPayVM.MemberPays.Add(NewRecord);
                    }
                    else
                    {
                        (new WinMsgDialog(excuteResult.Msg, Caption: "失败")).ShowDialog();
                    }
                }
                else
                {
                    (new WinMsgDialog("数据输入不正确！", Caption: "失败")).ShowDialog();
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

                if ((new WinMsgDialog($"确认要删除 {SelectedRec.PayMonth} 月份待遇吗？", Caption: "确认", showYesNo: true)).ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberPayRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _PageMemberPayVM.MemberPays.Remove(SelectedRec);
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

                PageMemberPayWin AddWin = new PageMemberPayWin(RecCloneObj, _PageMemberPayVM.MemberPayItems.ToList());

                AddWin.Owner = AppSettings.AppMainWindow;

                if (AddWin.ShowDialog().Value)
                {
                    IEnumerable<MemberPay> MemberPlays = await DataMemberPayRepository.GetRecords(new MemberPaySearch()
                    {
                        UserId = AppSettings.LoginUser.Id,
                        MemberId = RecCloneObj.MemberId,
                        PayYear = RecCloneObj.PayYear,
                        PayMonth = RecCloneObj.PayMonth,
                        PayName = RecCloneObj.PayName,
                    });
                    if (MemberPlays.Count() > 0)
                    {
                        (new WinMsgDialog($"该工作人员{RecCloneObj.PayYear} 年 {RecCloneObj.PayMonth} 月份的[{RecCloneObj.PayName}]已经发放。", "无法修改")).ShowDialog();
                        return;
                    }

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
        /// <summary>
        /// 选择用户发放变化。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListBox_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            _PageMemberPayVM.MemberPays.Clear();
            if (ListBox_PaySetMembers.SelectedItem is Lib.MemberPaySet PSMember)
            {
                _PageMemberPayVM.CanOperation = true;
                _PageMemberPayVM.CurMember = PSMember;
                await UcMemberPayFile.InitFileDatasAsync(PSMember.MemberId, "个人待遇", true);

            }
            else
            {
                _PageMemberPayVM.CanOperation = false;
                _PageMemberPayVM.CurMember = null;
            }

        }
    }
    //****************************************************************************************************************************************

    public class PageMemberPayVM : NotificationObject
    {
        private bool _CanOperation = false;

        public PageMemberPayVM()
        {
            MemberPays = new ObservableCollection<MemberPay>();
            PaySetMembers = new ObservableCollection<MemberPaySet>();
            SearchCondition = new MemberPaySearch();
        }
        public async Task InitVMAsync()
        {
            PaySetMembers.Clear();
            //读取可发放待遇的所有用户列表
            var TempMemberPaySets = await DataMemberPaySetRepository.GetRecords(new MemberPaySetSearch()
            {
                UserId = AppSettings.LoginUser.Id,
                PayUnitName = AppSettings.LoginUser.UnitName
            });
            TempMemberPaySets.ToList().ForEach(e =>
            {
                PaySetMembers.Add(e);
            });
            //读取可发放的所有待遇项目列表
            MemberPayItems = await DataMemberPayItemRepository.GetRecords(new Lib.MemberPayItemSearch()
            {
                UnitName = AppSettings.LoginUser.UnitName,
                UserId = AppSettings.LoginUser.Id

            }).ConfigureAwait(false);
        }
        public async System.Threading.Tasks.Task SearchRecords()
        {
            if (SearchCondition != null)
            {
                IEnumerable<MemberPay> MemberPlayMonths = await DataMemberPayRepository.GetRecords(new MemberPaySearch()
                {
                    PayYear = SearchDate.Year,
                    PayMonth = SearchDate.Month,
                    MemberId = CurMember.MemberId,
                    UserId = AppSettings.LoginUser.Id
                });
                MemberPays.Clear();
                MemberPlayMonths.ToList().ForEach(e =>
                {
                    MemberPays.Add(e);
                });
            }
        }
        /// <summary>
        /// 当前用户所在单位设置可发放待遇项目。
        /// </summary>
        public bool CanOperation
        {
            get { return _CanOperation; }
            set { _CanOperation = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 查询时间
        /// </summary>
        public DateTime SearchDate { get; set; } = DateTime.Now;
        /// <summary>
        /// 当前选定可发放待遇的职工信息
        /// </summary>
        public Lib.MemberPaySet CurMember { get; set; }
        /// <summary>
        /// 查询条件类对象
        /// </summary>
        public MemberPaySearch SearchCondition { get; set; }

        /// <summary>
        /// 当前职工工资月度发放记录
        /// </summary>
        public ObservableCollection<MemberPay> MemberPays { get; set; }
        /// <summary>
        /// 当前用户所在单位设置可发放待遇项目。
        /// </summary>
        public IEnumerable<MemberPayItem> MemberPayItems { get; set; }

        /// <summary>
        /// 当前用户所在单位设置可发放待遇的所有人员（即在MemberPaySet中配置的人员）。
        /// </summary>
        public ObservableCollection<Lib.MemberPaySet> PaySetMembers { get; set; }

    }
}
