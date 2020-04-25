using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    public static class DataPlanRepository
    {
        /// <summary>
        /// 新增或更新一个计划（如该计划已在数据库中存在，则更新之）
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddOrUpdatePlan(Lib.Plan Entity)
        {
            MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(Entity);
            ExcuteResult JsonResult = await DataApiRepository.PostApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "Plan", V_MultFormDatas).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 更新计划信息（采用PUT）
        /// </summary>
        /// <param name="UpdatePlan"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdatePlanInfo(Lib.Plan UpdatePlan)
        {
            MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(UpdatePlan);
            ExcuteResult JsonResult = await DataApiRepository.PutApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "Plan", V_MultFormDatas).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 删除一个计划信息
        /// </summary>
        /// <param name="DelePlan"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> DeletePlanInfo(Lib.Plan DelePlan)
        {
            ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "Plan/?P_Id=" + DelePlan.Id).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 读取满足指定条件的计划信息
        /// </summary>
        /// <param name="mSearchPlan">查询条件类的实例</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Lib.Plan>> ReadPlans(PlanSearch mSearchPlan)
        {
            IEnumerable<Lib.Plan> PlansList =null;
            string urlParams = DataApiRepository.CreateUrlParams(mSearchPlan);

            if (urlParams.Length > 0)
            {
                PlansList = await DataApiRepository.GetApiUri<IEnumerable<Lib.Plan>>(AppSettings.ApiUrlBase + "Plan/Search" + urlParams).ConfigureAwait(false);
            }
            return PlansList;
        }
    }
}
