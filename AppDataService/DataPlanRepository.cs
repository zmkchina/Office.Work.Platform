using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Office.Work.Platform.AppDataService
{
    public static class DataPlanRepository
    {
        /// <summary>
        /// 新增或更新一个计划（如该计划已在数据库中存在，则更新之）
        /// </summary>
        /// <param name="P_Entity"></param>
        /// <returns></returns>
        public static async Task<ModelResult> AddOrUpdatePlan(ModelPlan P_Entity)
        {
            MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(P_Entity);
            ModelResult JsonResult = await DataApiRepository.PostApiUri<ModelResult>(AppSettings.ApiUrlBase + "PlanInfo", V_MultFormDatas);
            return JsonResult;
        }
        /// <summary>
        /// 更新计划信息（采用PUT）
        /// </summary>
        /// <param name="UpdatePlan"></param>
        /// <returns></returns>
        public static async Task<ModelResult> UpdatePlanInfo(ModelPlan UpdatePlan)
        {
            MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(UpdatePlan);
            ModelResult JsonResult = await DataApiRepository.PutApiUri<ModelResult>(AppSettings.ApiUrlBase + "PlanInfo", V_MultFormDatas);
            return JsonResult;
        }
        /// <summary>
        /// 删除一个计划信息
        /// </summary>
        /// <param name="DelePlan"></param>
        /// <returns></returns>
        public static async Task<ModelResult> DeletePlanInfo(ModelPlan DelePlan)
        {
            ModelResult JsonResult = await DataApiRepository.DeleteApiUri<ModelResult>(AppSettings.ApiUrlBase + "PlanInfo/?P_Id=" + DelePlan.Id);
            return JsonResult;
        }
        /// <summary>
        /// 读取满足指定条件的计划信息
        /// </summary>
        /// <param name="mSearchPlan">查询条件类的实例</param>
        /// <returns></returns>
        public static async Task<IEnumerable<ModelPlan>> ReadPlans(MSearchPlan mSearchPlan)
        {
            IEnumerable<ModelPlan> PlansList=null;
            string urlParams = DataApiRepository.CreateUrlParams(mSearchPlan);

            if (urlParams.Length > 0)
            {
                PlansList = await DataApiRepository.GetApiUri<IEnumerable<ModelPlan>>(AppSettings.ApiUrlBase + "PlanInfo/Search" + urlParams);
            }
            return PlansList;
        }
    }
}
