using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Plan
{
    /// <summary>
    /// PageFilesList.xaml 的交互逻辑
    /// </summary>
    public partial class PagePlansList : Page
    {
        private CurPageViewModel _CurPageViewModel;
        private string _SearchPlanType;

        public PagePlansList(string SearchPlanType)
        {
            InitializeComponent();
            this.UCPlanInfo.Visibility = Visibility.Collapsed;
            col_panInfo.Width = new GridLength(0);
            _SearchPlanType = SearchPlanType;
            _CurPageViewModel = new CurPageViewModel();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //设置查询条件类
            switch (_SearchPlanType)
            {
                case "MyNoFinishPlans":
                    _CurPageViewModel.SearchCondition.ResponsiblePerson = AppSet.LoginUser.Id;
                    _CurPageViewModel.SearchCondition.LongPlan = "no";
                    _CurPageViewModel.SearchCondition.CurrectState = PlanStatus.WaitBegin + "," + PlanStatus.Running;
                    _CurPageViewModel.SearchCondition.PageSize = 50;
                    break;
                case "AllNoFinishPlans":
                    _CurPageViewModel.SearchCondition.CurrectState = PlanStatus.WaitBegin + "," + PlanStatus.Running;
                    _CurPageViewModel.SearchCondition.PageSize = 50;
                    _CurPageViewModel.SearchCondition.LongPlan = "no";
                    break;
                case "LongPlans":
                    _CurPageViewModel.SearchCondition.CurrectState = PlanStatus.WaitBegin + "," + PlanStatus.Running;
                    _CurPageViewModel.SearchCondition.PageSize = 50;
                    _CurPageViewModel.SearchCondition.LongPlan = "yes";
                    break;
                case "AllFinihPlans":
                    _CurPageViewModel.SearchCondition.CurrectState = PlanStatus.Finished;
                    _CurPageViewModel.SearchCondition.LongPlan = "all";
                    break;
                case "AllPlans":
                    break;
            }

            DataContext = _CurPageViewModel;

            btn_Refrash_ClickAsync(null, null);
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("就绪");
        }
        /// <summary>
        /// 选中一个计划，进行进一步操作。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.LB_PlanList.SelectedItem is Lib.Plan SelectPlan)
            {
                UCPlanInfo.Init_PlanInfoAsync(SelectPlan, (thePlan) =>
                   {
                       _CurPageViewModel.EntityPlans.Remove(thePlan);
                       col_panInfo.Width = new GridLength(0);
                   });
                UCPlanInfo.Visibility = Visibility.Visible;

                if (col_panInfo.Width.Value == 0)
                {
                    col_panInfo.Width = new GridLength(2, GridUnitType.Star);
                    Col_PlanList.Width = new GridLength(1, GridUnitType.Star);
                }
            }
        }
        /// <summary>
        /// 查询计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_Refrash_ClickAsync(object sender, RoutedEventArgs e)
        {
            //设置查询条件类
            _CurPageViewModel.SearchCondition.PageIndex = 1;
            await _CurPageViewModel.GetPlansAsync();
            AppFuns.SetStateBarText($"共查询到 :{_CurPageViewModel.SearchCondition.RecordCount}条记录,每页{_CurPageViewModel.SearchCondition.PageSize}条，共{_CurPageViewModel.SearchCondition.PageCount}页！");
        }

        private async void btn_PrevPage_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_CurPageViewModel.SearchCondition.PageIndex <= 1)
            {
                _CurPageViewModel.SearchCondition.PageIndex = _CurPageViewModel.SearchCondition.PageCount;
            }
            else
            {
                _CurPageViewModel.SearchCondition.PageIndex--;
            }
            await _CurPageViewModel.GetPlansAsync();
        }

        private async void btn_NextPage_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_CurPageViewModel.SearchCondition.PageIndex >= _CurPageViewModel.SearchCondition.PageCount)
            {
                _CurPageViewModel.SearchCondition.PageIndex = 1;
            }
            else
            {
                _CurPageViewModel.SearchCondition.PageIndex++;
            }
            await _CurPageViewModel.GetPlansAsync();
        }

        /// <summary>
        /// 本面页的视图模型
        /// </summary>
        private class CurPageViewModel : NotificationObject
        {
            private string _CanVisible;

            public CurPageViewModel()
            {
                SearchCondition = new PlanSearch()
                {
                    UnitName=AppSet.LoginUser.UnitName,
                    PageIndex = 1,
                    PageSize = 15
                };
                EntityPlans = new ObservableCollection<Lib.Plan>();
                CanVisible = "Collapsed";
            }
            public async Task GetPlansAsync()
            {
                EntityPlans.Clear();
                PlanSearchResult PlanSearchResult = await DataPlanRepository.ReadPlans(SearchCondition);
                if (PlanSearchResult == null) { return; }

                SearchCondition.RecordCount = PlanSearchResult.SearchCondition.RecordCount;

                PlanSearchResult.RecordList?.ToList().ForEach(e =>
                {
                    EntityPlans.Add(e);
                });
                if (SearchCondition.PageCount > 1)
                {
                    CanVisible = "Visible";
                }
                else
                {
                    CanVisible = "Collapsed";
                }
            }
            /// <summary>
            /// 查询到的计划列表
            /// </summary>
            public ObservableCollection<Lib.Plan> EntityPlans { get; set; }
            /// <summary>
            /// 查询条件
            /// </summary>
            public PlanSearch SearchCondition { get; set; }

            /// <summary>
            /// 上下页按钮是否可见
            /// </summary>
            public string CanVisible
            {
                get { return _CanVisible; }
                set { _CanVisible = value; RaisePropertyChanged(); }
            }
        }
    }
}