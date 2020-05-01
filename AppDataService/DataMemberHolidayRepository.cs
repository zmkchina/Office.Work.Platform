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
    public static class DataMemberHolidayRepository
    {
        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<MemberHoliday>> GetRecords(MemberHolidaySearch SearchCondition)
        {
            IEnumerable<MemberHoliday> RecList = null;
            //创建查询url参数
            string urlParams = DataApiRepository.CreateUrlParams(SearchCondition);

            if (urlParams.Length > 0)
            {
                RecList = await DataApiRepository.GetApiUri<IEnumerable<MemberHoliday>>(AppSettings.ApiUrlBase + "MemberHoliday/Search" + urlParams).ConfigureAwait(false);
            }
            return RecList;
        }
        /// <summary>
        /// 单个新增数据
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddRecord(Lib.MemberHoliday PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(AppSettings.ApiUrlBase + "MemberHoliday", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        
        /// <summary>
        /// 更新信息（采用PUT）
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdateRecord(Lib.MemberHoliday PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PutApiUriAsync(AppSettings.ApiUrlBase + "MemberHoliday", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> DeleteRecord(Lib.MemberHoliday PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "MemberHoliday/?Id=" + PEntity.Id).ConfigureAwait(false);
            return JsonResult;
        }
    }
}
