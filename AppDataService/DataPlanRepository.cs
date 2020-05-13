using System.Collections.Generic;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    public static class DataPlanRepository
    {
        private static string _ApiUrlBase = AppSet.LocalSetting.ResApiUrl;
        /// <summary>
        /// 读取满足指定条件的计划信息
        /// </summary>
        /// <param name="mSearchPlan">查询条件类的实例</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Lib.Plan>> ReadPlans(PlanSearch mSearchPlan)
        {
            IEnumerable<Lib.Plan> PlansList = null;
            string urlParams = DataApiRepository.CreateUrlParams(mSearchPlan);

            if (urlParams.Length > 0)
            {
                PlansList = await DataApiRepository.GetApiUri<IEnumerable<Lib.Plan>>(_ApiUrlBase + "Plan/Search" + urlParams).ConfigureAwait(false);
            }
            return PlansList;
        }
        /// <summary>
        /// 新增一个计划
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddNewPlan(Lib.Plan PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(_ApiUrlBase + "Plan", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 更新计划信息（采用PUT）
        /// </summary>
        /// <param name="UpdatePlan"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdatePlan(Lib.Plan PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PutApiUriAsync(_ApiUrlBase + "Plan", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 删除一个计划信息
        /// </summary>
        /// <param name="DelePlan"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> DeletePlan(Lib.Plan DelePlan)
        {
            ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(_ApiUrlBase + "Plan/" + DelePlan.Id).ConfigureAwait(false);
            return JsonResult;
        }
    }
}
