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
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddOrUpdate(Lib.Member Entity)
        {
            MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(Entity);
            ExcuteResult JsonResult = await DataApiRepository.PostApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "MemberInfo", V_MultFormDatas).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 新增或更新信息（如该信息已在数据库中存在，则更新之）
        /// </summary>
        /// <param name="P_Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddRange(List<Lib.Member> Entitys)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "MemberInfo/AddRange", Entitys).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 更新信息（采用PUT）
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdateInfo(Lib.Member Entity)
        {
            MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(Entity);
            ExcuteResult JsonResult = await DataApiRepository.PutApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "MemberInfo", V_MultFormDatas).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> DeletePlanInfo(Lib.Member Entity)
        {
            ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "MemberInfo/?Id=" + Entity.Id).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 读取满足指定条件的计划信息
        /// </summary>
        /// <param name="mSearchMember">查询条件类的实例</param>
        /// <returns></returns>
        public static async Task<ObservableCollection<Lib.Member>> ReadMembers(MemberSearch mSearchMember)
        {
            ObservableCollection<Lib.Member> MemberList = new ObservableCollection<Lib.Member>();
            string urlParams = DataApiRepository.CreateUrlParams(mSearchMember);

            if (urlParams.Length > 0)
            {
                MemberList = await DataApiRepository.GetApiUri<ObservableCollection<Lib.Member>>(AppSettings.ApiUrlBase + "MemberInfo/Search" + urlParams);
            }
            return MemberList;
        }
    }
}
