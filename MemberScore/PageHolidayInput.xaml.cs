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

namespace Office.Work.Platform.MemberScore
{
    /// <summary>
    /// 录入积分考核信息
    /// </summary>
    public partial class PageHolidayInput : Page
    {
        private PageViewModel _PageViewModel;
        public PageHolidayInput()
        {
            InitializeComponent();
            _PageViewModel = new PageViewModel();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("添加或删除员工绩效考核信息。");
            this.DataContext = _PageViewModel;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("就绪。");
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            await _PageViewModel.SearchRecords();
            if (_PageViewModel.CurMember != null)
            {
                await UcMemberHolidayFile.InitFileDatasAsync(_PageViewModel.CurMember.Id, "休假信息", true);
                _PageViewModel.CanOperation = true;
                AppFuns.SetStateBarText($"正在录入[{_PageViewModel.CurMember.Name}]的休假信息。");
            }
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
                UnitName = AppSet.LoginUser.UnitName,
                Member = _PageViewModel.CurMember,
                MemberId = _PageViewModel.CurMember.Id,
                UserId = AppSet.LoginUser.Id

            };

            PageHolidayInputWin AddWin = new PageHolidayInputWin(NewRecord);
            AddWin.Owner = AppSet.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                ExcuteResult excuteResult = await DataMemberHolidayRepository.AddRecord(NewRecord);
                if (excuteResult != null)
                {
                    if (excuteResult.State == 0)
                    {
                        NewRecord.Id = excuteResult.Tag;
                        _PageViewModel.MemberHolidays.Add(NewRecord);
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
            if (RecordListBox.SelectedItem is Lib.MemberHoliday SelectedRec)
            {

                if (AppFuns.ShowMessage($"确认要删除该请假信息吗？", Caption: "确认", showYesNo: true))
                {
                    ExcuteResult excuteResult = await DataMemberHolidayRepository.DeleteRecord(SelectedRec.Id);
                    if (excuteResult.State == 0)
                    {
                        _PageViewModel.MemberHolidays.Remove(SelectedRec);
                    }
                    else
                    {
                        AppFuns.ShowMessage(excuteResult.Msg, Caption: "失败");
                    }
                }
            }
        }

        /// <summary>
        /// 编辑记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnEditClickAsync(object sender, RoutedEventArgs e)
        {
            if (RecordListBox.SelectedItem is Lib.MemberHoliday SelectedRec)
            {
                Lib.MemberHoliday RecCloneObj = CloneObject<Lib.MemberHoliday, Lib.MemberHoliday>.Trans(SelectedRec);

                PageHolidayInputWin AddWin = new PageHolidayInputWin(RecCloneObj);
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
                        AppFuns.ShowMessage(excuteResult.Msg, Caption: "失败");
                    }
                }
            }
        }

        //****************************************************************************************************************************************
        /// <summary>
        /// 该页面的视图模型类
        /// </summary>
        private class PageViewModel : NotificationObject
        {
            private bool _CanOperation = false;

            /// <summary>
            /// 是否可以操作
            /// </summary>
            public bool CanOperation
            {
                get { return _CanOperation; }
                set { _CanOperation = value; RaisePropertyChanged(); }
            }
            /// <summary>
            /// 查询得到的用户信息
            /// </summary>
            public Lib.MemberInfoEntity CurMember { get; set; }

            /// <summary>
            /// 查询条件类对象
            /// </summary>
            public MemberHolidaySearch SearchCondition { get; set; }

            /// <summary>
            /// 当前职工工资月度发放记录
            /// </summary>
            public ObservableCollection<Lib.MemberHoliday> MemberHolidays { get; set; }

            public PageViewModel()
            {
                SearchCondition = new MemberHolidaySearch()
                {
                    UserId = AppSet.LoginUser.Id,
                    OccurYear = DateTime.Now.Year
                };
                CanOperation = false;
                CurMember = null;
                MemberHolidays = new ObservableCollection<Lib.MemberHoliday>();
            }
            /// <summary>
            /// 查询数据
            /// </summary>
            /// <returns></returns>
            public async Task SearchRecords()
            {
                if (string.IsNullOrWhiteSpace(SearchCondition.MemberId))
                {
                    return;
                }
                //1.先查询用户信息
                List<Lib.MemberInfoEntity> TempMembers = await DataMemberRepository.ReadMembers(new MemberSearch() { Id = SearchCondition.MemberId }).ConfigureAwait(false);

                if (TempMembers != null && TempMembers.Count > 0)
                {
                    CurMember = TempMembers[0];
                }
                if (CurMember == null)
                {
                    AppFuns.ShowMessage("未找到此用户信息！");
                    return;
                }
                if (SearchCondition != null)
                {
                    List<Lib.MemberHoliday> MemberHolidayList = await DataMemberHolidayRepository.GetRecords(SearchCondition);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        MemberHolidays.Clear();
                        MemberHolidayList?.ToList().ForEach(e =>
                        {
                            e.Member = CurMember;
                            MemberHolidays.Add(e);
                        });
                    });
                }
            }
        }
    }
}
