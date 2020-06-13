using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberPay
{
    /// <summary>
    /// PageMemberPay.xaml 的交互逻辑
    /// </summary>
    public partial class PageMemberPay : Page
    {
        private PageViewModel _PageViewModel;
        public PageMemberPay()
        {
            InitializeComponent();
            _PageViewModel = new PageViewModel();

        }

        private async void Page_LoadedAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            await _PageViewModel.InitPropValuesAsync();
            //因为此函数为异步（即使用的是后台线程或者说非UI线程），故要更新界面需使用 Dispatcher 来向WPF的UI线程添加任务。
            App.Current.Dispatcher.Invoke(() =>
            {
                this.DataContext = _PageViewModel;
            });

        }

        //选择时间发生变化
        private async void DatePicker_SelectedDateChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            //if (_PageViewModel.CurMember != null)
            //{
            //    await _PageViewModel.SearchRecords(); //查询记录
            //}
        }

        /// <summary>
        ///  新增记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddClickAsync(object sender, RoutedEventArgs e)
        {
            Lib.MemberSalary NewRecord = new Lib.MemberSalary()
            {
                //MemberId = _PageViewModel.CurMember.MemberId,
                //MemberName = _PageViewModel.CurMember.MemberName,
                //MemberIndex = _PageViewModel.CurMember.OrderIndex,
                //MemberType = _PageViewModel.CurMember.MemberType,
                UserId = AppSet.LoginUser.Id
            };

            PageMemberPayWin AddWin = new PageMemberPayWin()
            {
                CurMemberSalary = NewRecord,
                MemberPayItems = _PageViewModel.MemberPayItems.ToList(),
                SelectPayDate = _PageViewModel.SearchDate
            };
            if (AddWin.ShowDialog().Value)
            {
                IEnumerable<Lib.MemberSalarySearchResult> MemberPlays = await DataMemberSalaryRepository.GetRecords(new MemberSalarySearch()
                {
                    UserId = AppSet.LoginUser.Id,
                    MemberId = NewRecord.MemberId,
                    PayYear = NewRecord.PayYear,
                    PayMonth = NewRecord.PayMonth,
                });
                if (MemberPlays.Count() > 0)
                {
                   // AppFuns.ShowMessage($"该工作人员{NewRecord.PayYear} 年 {NewRecord.PayMonth} 月份的[{NewRecord.PayName}]已经发放。", "无法新增");
                    return;
                }

                ExcuteResult excuteResult = await DataMemberSalaryRepository.AddRecord(NewRecord);
                if (excuteResult != null)
                {
                    if (excuteResult.State == 0)
                    {
                        NewRecord.Id = excuteResult.Tag;
                        _PageViewModel.MemberSalarys.Add(NewRecord);
                    }
                    else
                    {
                        AppFuns.ShowMessage(excuteResult.Msg, Caption: "失败");
                    }
                }
                else
                {
                    AppFuns.ShowMessage("数据输入不正确！", Caption: "失败");
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
            if (RecordDataGrid.SelectedItem is Lib.MemberSalary SelectedRec)
            {

                if (AppFuns.ShowMessage($"确认要删除 {SelectedRec.PayMonth} 月份待遇吗？", Caption: "确认", showYesNo: true))
                {
                    ExcuteResult excuteResult = await DataMemberSalaryRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        _PageViewModel.MemberSalarys.Remove(SelectedRec);
                    }
                    else
                    {
                        AppFuns.ShowMessage(excuteResult.Msg, Caption: "失败");
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
            _PageViewModel.MemberSalarys.Clear();
            //if (ListBox_PaySetMembers.SelectedItem is Lib.MemberPaySet MemberPset)
            //{
            //    _PageViewModel.CanOperation = true;
            //    _PageViewModel.CurMember = MemberPset;
            //    await UcMemberPayFile.InitFileDatasAsync(MemberPset.MemberId, "个人待遇", true);
            //    await _PageViewModel.SearchRecords();
            //}
            //else
            //{
            //    _PageViewModel.CanOperation = false;
            //    _PageViewModel.CurMember = null;
            //}

        }
        //****************************************************************************************************************************************
        /// <summary>
        /// 该页面的视图模型类
        /// </summary>
        private class PageViewModel : NotificationObject
        {
            private bool _CanOperation = false;

            public PageViewModel()
            {
                //MemberSalarys = new ObservableCollection<Lib.MemberSalary>();
                //PaySetMembers = new ObservableCollection<MemberPaySet>();
                //SearchCondition = new MemberSalarySearch();
            }
            /// <summary>
            /// 初始化各类属性值
            /// </summary>
            /// <returns></returns>
            public async Task InitPropValuesAsync()
            {
                ////读取可发放的所有待遇项目列表
                //var TempPayItems = await DataMemberPayItemRepository.GetRecords(new Lib.MemberPayItemSearch()
                //{
                //    PayUnitName = AppSet.LoginUser.UnitName,
                //    UserId = AppSet.LoginUser.Id
                //}).ConfigureAwait(false);
                //if (TempPayItems == null || TempPayItems.Count() < 1)
                //{
                //    //AppFuns.ShowMessage("未读到待遇项目数据，请稍候再试！");
                //    return;
                //}
                //MemberPayItems = TempPayItems.ToList();
                //MemberPayItems.Sort((x, y) => x.OrderIndex - y.OrderIndex);

                ////读取可发放待遇的所有用户列表
                //var TempMemberPaySets = await DataMemberPaySetRepository.GetRecords(new MemberPaySetSearch()
                //{
                //    PayUnitName = AppSet.LoginUser.UnitName,
                //    UserId = AppSet.LoginUser.Id
                //}).ConfigureAwait(false);
                //PaySetMembers.Clear();
                //TempMemberPaySets?.ToList().ForEach(e =>
                //{
                //    PaySetMembers.Add(e);
                //});
            }
            /// <summary>
            /// 查询数据
            /// </summary>
            /// <returns></returns>
            public async Task SearchRecords()
            {
                //if (SearchCondition != null)
                //{
                //    IEnumerable<Lib.MemberSalary> MemberPlayMonths = await DataMemberSalaryRepository.GetRecords(new MemberSalarySearch()
                //    {
                //        PayYear = SearchDate.Year,
                //        PayMonth = SearchDate.Month,
                //        MemberId = CurMember.MemberId,
                //        UserId = AppSet.LoginUser.Id
                //    });
                //    MemberSalarys.Clear();
                //    MemberPlayMonths?.ToList().ForEach(e =>
                //    {
                //        MemberSalarys.Add(e);
                //    });
                //}
            }


            /// <summary>
            /// 是否可以操作（只有选中一个用户后，才能操作）
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
            /// 查询条件类对象
            /// </summary>
            public MemberSalarySearch SearchCondition { get; set; }

            /// <summary>
            /// 当前职工工资月度发放记录
            /// </summary>
            public ObservableCollection<Lib.MemberSalary> MemberSalarys { get; set; }
            /// <summary>
            /// 当前用户所在单位设置可发放待遇项目。
            /// </summary>
            public List<MemberPayItem> MemberPayItems { get; set; }

            ///// <summary>
            ///// 当前用户所在单位设置可发放待遇的所有人员（即在MemberPaySet中配置的人员）。
            ///// </summary>
            //public ObservableCollection<Lib.MemberPaySet> PaySetMembers { get; set; }

        }


    }


}
