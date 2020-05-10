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
        /// 查询满足指定条件的计划信息
        /// </summary>
        /// <param name="mSearchMember">查询条件类的实例</param>
        /// <returns></returns>
        public static async Task<List<Lib.Member>> ReadMembers(MemberSearch mSearchMember)
        {
            List<Lib.Member> MemberList = new List<Lib.Member>();
            string urlParams = DataApiRepository.CreateUrlParams(mSearchMember);

            if (urlParams.Length > 0)
            {
                MemberList = await DataApiRepository.GetApiUri<List<Lib.Member>>(AppSet.ApiUrlBase + "Member/Search" + urlParams);
            }
            return MemberList;
        }
        /// <summary>
        /// 单个新增数据
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddMember(Lib.Member PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(AppSet.ApiUrlBase + "Member", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <param name="P_Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddMembers(List<Lib.Member> Entitys)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(AppSet.ApiUrlBase + "Member/AddRange", Entitys).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 更新信息（采用PUT）
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdateMember(Lib.Member PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PutApiUriAsync(AppSet.ApiUrlBase + "Member", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> DeleteMember(Lib.Member Entity)
        {
            ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(AppSet.ApiUrlBase + "Member/?Id=" + Entity.Id).ConfigureAwait(false);
            return JsonResult;
        }        
    }
}
