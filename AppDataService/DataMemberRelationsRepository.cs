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
    public static class DataMemberRelationsRepository
    {
            /// <summary>
            /// 按条件查询数据
            /// </summary>
            /// <param name="SearchCondition"></param>
            /// <returns></returns>
            public static async Task<IEnumerable<MemberRelations>> GetRecords(MemberRelationsSearch SearchCondition)
            {
                IEnumerable<MemberRelations> RecList = null;
                //创建查询url参数
                string urlParams = DataApiRepository.CreateUrlParams(SearchCondition);

                if (urlParams.Length > 0)
                {
                    RecList = await DataApiRepository.GetApiUri<IEnumerable<MemberRelations>>(AppSet.ApiUrlBase + "MemberRelations/Search" + urlParams).ConfigureAwait(false);
                }
                return RecList;
            }
            /// <summary>
            /// 单个新增数据
            /// </summary>
            /// <param name="PEntity"></param>
            /// <returns></returns>
            public static async Task<ExcuteResult> AddRecord(Lib.MemberRelations PEntity)
            {
                ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(AppSet.ApiUrlBase + "MemberRelations", PEntity).ConfigureAwait(false);
                return JsonResult;
            }
            /// <summary>
            /// 更新信息（采用PUT）
            /// </summary>
            /// <param name="PEntity"></param>
            /// <returns></returns>
            public static async Task<ExcuteResult> UpdateRecord(Lib.MemberRelations PEntity)
            {
                ExcuteResult JsonResult = await DataApiRepository.PutApiUriAsync(AppSet.ApiUrlBase + "MemberRelations", PEntity).ConfigureAwait(false);
                return JsonResult;
            }
            /// <summary>
            /// 删除一个实体
            /// </summary>
            /// <param name="PEntity"></param>
            /// <returns></returns>
            public static async Task<ExcuteResult> DeleteRecord(Lib.MemberRelations PEntity)
            {
                ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(AppSet.ApiUrlBase + "MemberRelations/?Id=" + PEntity.Id).ConfigureAwait(false);
                return JsonResult;
            }
    }
}
