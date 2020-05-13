using System.Collections.Generic;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    /// <summary>
    /// 职工月度发放的待遇数据处理类
    /// </summary>
    public static class DataMemberPaySetRepository
    {
        private static string _ApiUrlBase = AppSet.LocalSetting.ResApiUrl;
        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<MemberPaySet>> GetRecords(MemberPaySetSearch SearchCondition)
        {
            IEnumerable<MemberPaySet> RecList = null;
            //创建查询url参数
            string urlParams = DataApiRepository.CreateUrlParams(SearchCondition);

            if (urlParams.Length > 0)
            {
                RecList = await DataApiRepository.GetApiUri<IEnumerable<MemberPaySet>>(_ApiUrlBase + "MemberPaySet/Search" + urlParams).ConfigureAwait(false);
            }
            return RecList;
        }
       
        /// <summary>
        /// 单个新增或者更新数据信息（如数据库中没有则新增之，如有则更新之）
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddOrUpdateRecord(List<MemberPaySet> PaySetList)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(_ApiUrlBase + "MemberPaySet/AddOrUpdate", PaySetList).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 单个新增数据
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddRecord(Lib.MemberPaySet PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(_ApiUrlBase + "MemberPaySet", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 更新信息（采用PUT）
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdateRecord(Lib.MemberPaySet PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PutApiUriAsync(_ApiUrlBase + "MemberPaySet", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> DeleteRecord(Lib.MemberPaySet PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(_ApiUrlBase + "MemberPaySet/?MemberId=" + PEntity.MemberId).ConfigureAwait(false);
            return JsonResult;
        }
    }
}
