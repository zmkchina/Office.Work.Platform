using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Office.Work.Platform.Plan
{
    /// <summary>
    /// PageFilesList.xaml 的交互逻辑
    /// </summary>
    public partial class PagePlansList : Page
    {
        private PagePlansListVM _PagePlansListVM;
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

            if (_PagePlansListVM == null)
            {
                _PagePlansListVM = new PagePlansListVM();

                switch (_SearchPlanType)
                {
                    case "MyNoFinishPlans":
                        _PagePlansListVM.mSearchPlan.CreateUserId = AppSettings.LoginUser.Id;
                        _PagePlansListVM.mSearchPlan.CurrectState = PlanStatus.WaitBegin + "," + PlanStatus.Running;
                        break;
                    case "AllNoFinishPlans":
                        _PagePlansListVM.mSearchPlan.CurrectState = PlanStatus.WaitBegin + "," + PlanStatus.Running;
                        break;
                    case "AllFinihPlans":
                        _PagePlansListVM.mSearchPlan.CurrectState = PlanStatus.Finished;
                        break;
                    case "AllPlans":
                        break;
                }
                btn_Refrash_Click(null, null);
            }
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
                       _PagePlansListVM.EntityPlans.Remove(thePlan);
                       col_panInfo.Width = new GridLength(0);
                       col_panInfo.MinWidth = 0d;
                   });
                UCPlanInfo.Visibility = Visibility.Visible;

                if (col_panInfo.Width.Value == 0)
                {
                    col_panInfo.Width = new GridLength(1, GridUnitType.Star);
                    col_panInfo.MinWidth = 200d;
                }
            }
        }
        /// <summary>
        /// 查询计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Refrash_Click(object sender, RoutedEventArgs e)
        {
            string SearchKeys = tb_SearchKeys.Text.Trim().Length > 0 ? tb_SearchKeys.Text.Trim() : null;
            //设置查询条件类
            _PagePlansListVM.mSearchPlan.KeysInMultiple = SearchKeys;

            _PagePlansListVM.GetPlansAsync();
            DataContext = _PagePlansListVM;
        }
    }


    public class PagePlansListVM : NotificationObject
    {

        public PagePlansListVM()
        {
            FileContentTypes = AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            mSearchPlan = new PlanSearch();
            EntityPlans = new ObservableCollection<Lib.Plan>();
        }
        public async void GetPlansAsync()
        {
            EntityPlans.Clear();
            var plans = await DataPlanRepository.ReadPlans(mSearchPlan);
            plans.ToList().ForEach(e =>
            {
                EntityPlans.Add(e);
            });
        }
        public string[] FileContentTypes { get; set; }
        /// <summary>
        /// 查询到的计划列表
        /// </summary>
        public ObservableCollection<Lib.Plan> EntityPlans { get; set; }

        public PlanSearch mSearchPlan { get; set; }

        #region "方法"

        #endregion
    }
}
