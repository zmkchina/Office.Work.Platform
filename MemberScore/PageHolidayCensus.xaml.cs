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

namespace Office.Work.Platform.MemberScore
{
    /// <summary>
    /// 录入积分考核信息
    /// </summary>
    public partial class PageHolidayCensus : Page
    {
        private PageViewModel _PageViewModel;
        public PageHolidayCensus()
        {
            InitializeComponent();
            _PageViewModel = new PageViewModel();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("正在统计休假信息。");
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
                _PageViewModel.CanOperation = true;
                AppFuns.SetStateBarText($"正在统计[{_PageViewModel.CurMember.Name}]休假信息。");
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
            public Lib.Member CurMember { get; set; }

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
                //1.先查询用户信息
                List<Lib.Member> TempMembers = await DataMemberRepository.ReadMembers(new MemberSearch() { Id = SearchCondition.MemberId }).ConfigureAwait(false);

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
                            MemberHolidays.Add(e);
                        });
                    });
                }
            }
        }
    }
}
