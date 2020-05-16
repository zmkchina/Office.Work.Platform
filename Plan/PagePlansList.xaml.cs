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
                _CurPageViewModel.mSearchPlan.UnitName = AppSet.LoginUser.UnitName;
                switch (_SearchPlanType)
                {
                    case "MyNoFinishPlans":
                        _CurPageViewModel.mSearchPlan.ResponsiblePerson = AppSet.LoginUser.Id;
                        _CurPageViewModel.mSearchPlan.CurrectState = PlanStatus.WaitBegin + "," + PlanStatus.Running;
                        break;
                    case "AllNoFinishPlans":
                        _CurPageViewModel.mSearchPlan.CurrectState = PlanStatus.WaitBegin + "," + PlanStatus.Running;
                        break;
                    case "AllFinihPlans":
                        _CurPageViewModel.mSearchPlan.CurrectState = PlanStatus.Finished;
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
            _CurPageViewModel.mSearchPlan.KeysInMultiple = SearchKeys;
            await _CurPageViewModel.GetPlansAsync();
            AppFuns.SetStateBarText($"共查询到 {_CurPageViewModel.EntityPlans.Count} 个计划");
            DataContext = _CurPageViewModel;
        }

        /// <summary>
        /// 本面页的视图模型
        /// </summary>
        private class CurPageViewModel : NotificationObject
        {

            public CurPageViewModel()
            {
                mSearchPlan = new PlanSearch();
                EntityPlans = new ObservableCollection<Lib.Plan>();
            }
            public async Task GetPlansAsync()
            {
                EntityPlans.Clear();
                var plans = await DataPlanRepository.ReadPlans(mSearchPlan);
                plans?.ToList().ForEach(e =>
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
            public PlanSearch mSearchPlan { get; set; }
        }
    }
}