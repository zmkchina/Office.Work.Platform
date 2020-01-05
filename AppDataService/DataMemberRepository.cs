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
    public static class DataMemberRepository
    {
        /// <summary>
        /// 新增或更新信息（如该信息已在数据库中存在，则更新之）
        /// </summary>
        /// <param name="P_Entity"></param>
        /// <returns></returns>
        public static async Task<ModelResult> AddOrUpdate(ModelMember P_Entity)
        {
            MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(P_Entity);
            ModelResult JsonResult = await DataApiRepository.PostApiUri<ModelResult>(AppSettings.ApiUrlBase + "MemberInfo", V_MultFormDatas);
            return JsonResult;
        }
        /// <summary>
        /// 新增或更新信息（如该信息已在数据库中存在，则更新之）
        /// </summary>
        /// <param name="P_Entity"></param>
        /// <returns></returns>
        public static async Task<ModelResult> AddRange(List<ModelMember> P_Entitys)
        {
            ModelResult JsonResult = await DataApiRepository.PostApiUri<ModelResult>(AppSettings.ApiUrlBase + "MemberInfo/AddRange", P_Entitys);
            return JsonResult;
        }
        /// <summary>
        /// 更新信息（采用PUT）
        /// </summary>
        /// <param name="P_Entity"></param>
        /// <returns></returns>
        public static async Task<ModelResult> UpdateInfo(ModelMember P_Entity)
        {
            MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(P_Entity);
            ModelResult JsonResult = await DataApiRepository.PutApiUri<ModelResult>(AppSettings.ApiUrlBase + "MemberInfo", V_MultFormDatas);
            return JsonResult;
        }
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="P_Entity"></param>
        /// <returns></returns>
        public static async Task<ModelResult> DeletePlanInfo(ModelMember P_Entity)
        {
            ModelResult JsonResult = await DataApiRepository.DeleteApiUri<ModelResult>(AppSettings.ApiUrlBase + "MemberInfo/?P_Id=" + P_Entity.Id);
            return JsonResult;
        }
        /// <summary>
        /// 读取满足指定条件的计划信息
        /// </summary>
        /// <param name="mSearchMember">查询条件类的实例</param>
        /// <returns></returns>
        public static async Task<ObservableCollection<ModelMember>> ReadMembers(MSearchMember mSearchMember)
        {
            ObservableCollection<ModelMember> MemberList = new ObservableCollection<ModelMember>();
            string urlParams = DataApiRepository.CreateUrlParams(mSearchMember);

            if (urlParams.Length > 0)
            {
                MemberList = await DataApiRepository.GetApiUri<ObservableCollection<ModelMember>>(AppSettings.ApiUrlBase + "MemberInfo/Search" + urlParams);
            }
            return MemberList;
        }
    }
}
