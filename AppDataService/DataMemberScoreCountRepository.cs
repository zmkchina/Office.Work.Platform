using System.Collections.Generic;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    public static class DataMemberScoreCountRepository
    {
        private static string _ApiUrlBase = AppSet.LocalSetting.ResApiUrl;
        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<List<Lib.MemberScoreCountDto>> GetRecords(MemberScoreCountDtoSearch SearchCondition)
        {
            List<Lib.MemberScoreCountDto> RecordList = new List<Lib.MemberScoreCountDto>();
            //创建查询url参数
            string urlParams = DataApiRepository.CreateUrlParams(SearchCondition);

            if (urlParams.Length > 0)
            {
                RecordList = await DataApiRepository.GetApiUri<List<Lib.MemberScoreCountDto>>(_ApiUrlBase + "MemberScoreCount/Search" + urlParams).ConfigureAwait(false);
            }
            return RecordList;
        }
    }
}
