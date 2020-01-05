using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
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
                        _PagePlansListVM.mSearchPlan.CurrectState = "等待执行,正在实施";
                        break;
                    case "AllNoFinishPlans":
                        _PagePlansListVM.mSearchPlan.CurrectState = "等待执行,正在实施";
                        break;
                    case "AllFinihPlans":
                        _PagePlansListVM.mSearchPlan.CurrectState = "已经完成";
                        break;
                    case "AllPlans":
                        break;
                }
                btn_Refrash_Click(null, null);
            }
        }
        private void ListBox_FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UCPlanInfo.Init_PlanInfo(this.LB_PlanList.SelectedItem as ModelPlan, (thePlan) =>
            {
                _PagePlansListVM.EntityPlans.Remove(thePlan);
                col_panInfo.Width = new GridLength(0);
                col_panInfo.MinWidth = 0d;
            });
            if (col_panInfo.Width.Value == 0)
            {
                col_panInfo.Width = new GridLength(1, GridUnitType.Star);
                col_panInfo.MinWidth = 200d;
            }
        }
        private async void btn_Refrash_Click(object sender, RoutedEventArgs e)
        {
            string SearchKeys = tb_SearchKeys.Text.Trim().Length > 0 ? tb_SearchKeys.Text.Trim() : null;
            //设置查询条件类
            _PagePlansListVM.mSearchPlan.KeysInMultiple = SearchKeys;

            _PagePlansListVM.EntityPlans = await DataPlanRepository.ReadPlans(_PagePlansListVM.mSearchPlan);
            DataContext = _PagePlansListVM;
        }
    }
}
