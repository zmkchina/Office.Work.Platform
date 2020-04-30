using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    // <summary>
    /// 职工临时补发待遇数据处理类
    /// </summary>
    public static class DataMemberResumeRepository
    {
            /// <summary>
            /// 按条件查询数据
            /// </summary>
            /// <param name="SearchCondition"></param>
            /// <returns></returns>
            public static async Task<IEnumerable<MemberResume>> GetRecords(MemberResumeSearch SearchCondition)
            {
                IEnumerable<MemberResume> RecList = null;
                //创建查询url参数
                string urlParams = DataApiRepository.CreateUrlParams(SearchCondition);

                if (urlParams.Length > 0)
                {
                    RecList = await DataApiRepository.GetApiUri<IEnumerable<MemberResume>>(AppSettings.ApiUrlBase + "MemberResume/Search" + urlParams).ConfigureAwait(false);
                }
                return RecList;
            }
            /// <summary>
            /// 单个新增数据
            /// </summary>
            /// <param name="PEntity"></param>
            /// <returns></returns>
            public static async Task<ExcuteResult> AddRecord(Lib.MemberResume PEntity)
            {
                MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(PEntity);
                ExcuteResult JsonResult = await DataApiRepository.PostApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "MemberResume", V_MultFormDatas).ConfigureAwait(false);
                return JsonResult;
            }
            /// <summary>
            /// 批量新增数据
            /// </summary>
            /// <param name="P_Entity"></param>
            /// <returns></returns>
            public static async Task<ExcuteResult> AddRecords(List<Lib.MemberResume> Entitys)
            {
                ExcuteResult JsonResult = await DataApiRepository.PostApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "MemberResume/AddRange", Entitys).ConfigureAwait(false);
                return JsonResult;
            }
            /// <summary>
            /// 更新信息（采用PUT）
            /// </summary>
            /// <param name="PEntity"></param>
            /// <returns></returns>
            public static async Task<ExcuteResult> UpdateRecord(Lib.MemberResume PEntity)
            {
                MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(PEntity);
                ExcuteResult JsonResult = await DataApiRepository.PutApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "MemberResume", V_MultFormDatas).ConfigureAwait(false);
                return JsonResult;
            }
            /// <summary>
            /// 删除一个实体
            /// </summary>
            /// <param name="PEntity"></param>
            /// <returns></returns>
            public static async Task<ExcuteResult> DeleteRecord(Lib.MemberResume PEntity)
            {
                ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "MemberResume/?Id=" + PEntity.Id).ConfigureAwait(false);
                return JsonResult;
            }
    }
}
