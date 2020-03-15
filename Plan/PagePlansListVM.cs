using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Office.Work.Platform.Plan
{
    public class PagePlansListVM : NotificationObject
    {

        public PagePlansListVM()
        {
            FileContentTypes = AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            mSearchPlan = new MSearchPlan();
            EntityPlans = new ObservableCollection<ModelPlan>();
        }
        public async Task GetPlansAsync()
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
        public ObservableCollection<ModelPlan> EntityPlans { get; set; }

        public MSearchPlan mSearchPlan { get; set; }

        #region "方法"

        #endregion
    }
}
