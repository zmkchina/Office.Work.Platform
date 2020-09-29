using System.Collections.Generic;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    public static class DataMemberHolidayCountRepository
    {
        private static string _ApiUrlBase = AppSet.LocalSetting.ResApiUrl;
        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<List<Lib.MemberHolidayCountDto>> GetRecords(MemberHolidayCountDtoSearch SearchCondition)
        {
            List<Lib.MemberHolidayCountDto> RecordList = new List<Lib.MemberHolidayCountDto>();
            //创建查询url参数
            string urlParams = DataApiRepository.CreateUrlParams(SearchCondition);

            if (urlParams.Length > 0)
            {
                RecordList = await DataApiRepository.GetApiUri<List<Lib.MemberHolidayCountDto>>(_ApiUrlBase + "MemberHolidayCount/Search" + urlParams).ConfigureAwait(false);
            }
            return RecordList;
        }
    }
}
