using System.Collections.Generic;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    public static class DataMemberScoreRepository
    {
        private static string _ApiUrlBase = AppSet.LocalSetting.ResApiUrl;
        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<List<Lib.MemberScore>> GetRecords(MemberScoreSearch SearchCondition)
        {
            List<Lib.MemberScore> RecordList = new List<Lib.MemberScore>();
            //创建查询url参数
            string urlParams = DataApiRepository.CreateUrlParams(SearchCondition);

            if (urlParams.Length > 0)
            {
                RecordList = await DataApiRepository.GetApiUri<List<Lib.MemberScore>>(_ApiUrlBase + "MemberScore/Search" + urlParams).ConfigureAwait(false);
            }
            return RecordList;
        }
        /// <summary>
        /// 根据ID查询指定记录信息
        /// </summary>
        /// <param name="Id">查询条件类的实例</param>
        /// <returns></returns>
        public static async Task<Lib.MemberScore> GetOneById(string Id)
        {
            return await DataApiRepository.GetApiUri<Lib.MemberScore>(_ApiUrlBase + "MemberScore/" + Id);
        }
        /// <summary>
        /// 单个新增数据
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddRecord(Lib.MemberScore PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(_ApiUrlBase + "MemberScore", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
       
        /// <summary>
        /// 更新信息（采用PUT）
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdateRecord(Lib.MemberScore PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PutApiUriAsync(_ApiUrlBase + "MemberScore", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> DeleteRecord(string PId)
        {
            ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(_ApiUrlBase + "MemberScore/?Id=" + PId).ConfigureAwait(false);
            return JsonResult;
        }

    }
}
