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
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //设置查询条件类
            if (_CurPageViewModel == null)
            {
                _CurPageViewModel = new CurPageViewModel();
                _CurPageViewModel.SearchPlan.UnitName = AppSet.LoginUser.UnitName;
                switch (_SearchPlanType)
                {
                    case "MyNoFinishPlans":
                        _CurPageViewModel.SearchPlan.ResponsiblePerson = AppSet.LoginUser.Id;
                        _CurPageViewModel.SearchPlan.CurrectState = PlanStatus.WaitBegin + "," + PlanStatus.Running;
                        break;
                    case "AllNoFinishPlans":
                        _CurPageViewModel.SearchPlan.CurrectState = PlanStatus.WaitBegin + "," + PlanStatus.Running;
                        break;
                    case "AllFinihPlans":
                        _CurPageViewModel.SearchPlan.CurrectState = PlanStatus.Finished;
                        break;
                    case "AllPlans":
                        break;
                }
                btn_Refrash_ClickAsync(null, null);
            }
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
            string SearchKeys = tb_SearchKeys.Text.Trim().Length > 0 ? tb_SearchKeys.Text.Trim() : null;
            //设置查询条件类
            _CurPageViewModel.SearchPlan.KeysInMultiple = SearchKeys;
            await _CurPageViewModel.GetPlansAsync();
            DataContext = _CurPageViewModel;
            AppFuns.SetStateBarText($"共查询到 :{_CurPageViewModel.SearchPlan.RecordCount}条记录,每页{_CurPageViewModel.SearchPlan.PageSize}条，共{_CurPageViewModel.SearchPlan.PageCount}页！");
        }

        private async void btn_PrevPage_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_CurPageViewModel.SearchPlan.PageIndex <= 1)
            {
                _CurPageViewModel.SearchPlan.PageIndex = _CurPageViewModel.SearchPlan.PageCount;
            }
            else
            {
                _CurPageViewModel.SearchPlan.PageIndex--;
            }
            await _CurPageViewModel.GetPlansAsync();
        }

        private async void btn_NextPage_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_CurPageViewModel.SearchPlan.PageIndex >= _CurPageViewModel.SearchPlan.PageCount)
            {
                _CurPageViewModel.SearchPlan.PageIndex = 1;
            }
            else
            {
                _CurPageViewModel.SearchPlan.PageIndex++;
            }
            await _CurPageViewModel.GetPlansAsync();
        }

        /// <summary>
        /// 本面页的视图模型
        /// </summary>
        private class CurPageViewModel : NotificationObject
        {

            public CurPageViewModel()
            {
                SearchPlan = new PlanSearch()
                {
                    PageIndex = 1,
                    PageSize = 15,
                };
                EntityPlans = new ObservableCollection<Lib.Plan>();
            }
            public async Task GetPlansAsync()
            {
                EntityPlans.Clear();
                PlanSearchResult PlanSearchResult = await DataPlanRepository.ReadPlans(SearchPlan);
                if (PlanSearchResult == null) { return; }
                
                SearchPlan.RecordCount = PlanSearchResult.SearchCondition.RecordCount;
                
                PlanSearchResult.RecordList?.ToList().ForEach(e =>
                {
                    EntityPlans.Add(e);
                });
            }
            /// <summary>
            /// 查询到的计划列表
            /// </summary>
            public ObservableCollection<Lib.Plan> EntityPlans { get; set; }
            /// <summary>
            /// 查询条件
            /// </summary>
            public PlanSearch SearchPlan { get; set; }
        }
    }
}