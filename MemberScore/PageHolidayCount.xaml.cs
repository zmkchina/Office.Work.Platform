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
    /// 统计积分考核信息
    /// </summary>
    public partial class PageHolidayCount : Page
    {
        private PageViewModel _PageViewModel;
        public PageHolidayCount()
        {
            InitializeComponent();
            _PageViewModel = new PageViewModel();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("统计员工休假信息。");
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
        }

        //****************************************************************************************************************************************
        /// <summary>
        /// 该页面的视图模型类
        /// </summary>
        private class PageViewModel : NotificationObject
        {
            /// <summary>
            /// 查询条件类对象
            /// </summary>
            public MemberHolidayCountSearch SearchCondition { get; set; }

            /// <summary>
            /// 当前职工工资月度发放记录
            /// </summary>
            public ObservableCollection<Lib.MemberHolidayCount> MemberHolidayCounts { get; set; }

            public PageViewModel()
            {
                SearchCondition = new MemberHolidayCountSearch()
                {
                    UnitName = AppSet.LoginUser.UnitName,
                    UserId = AppSet.LoginUser.Id,
                    YearNumber = DateTime.Now.Year.ToString()
                };
                MemberHolidayCounts = new ObservableCollection<Lib.MemberHolidayCount>();
            }
            /// <summary>
            /// 查询数据
            /// </summary>
            /// <returns></returns>
            public async Task SearchRecords()
            {
                //1.先查询用户信息
                List<Lib.MemberHolidayCount> MemberHolidayCountsList = await DataMemberHolidayCountRepository.GetRecords(SearchCondition);


                App.Current.Dispatcher.Invoke(() =>
                {
                    MemberHolidayCounts.Clear();
                    MemberHolidayCountsList?.ToList().ForEach(e =>
                    {
                        MemberHolidayCounts.Add(e);
                    });
                });
            }
        }
    }
}
