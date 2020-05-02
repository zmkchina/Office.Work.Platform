using System.Collections.Generic;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    // <summary>
    /// 职工综合信息处理类
    /// </summary>
    public static class DataMemberCombiningRepository
    {
        /// <summary>
        /// 获取正式人员月度工资表
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<MemberPayMonthOfficialSheet>> GetMemberPayMonthOfficialSheet(int PayYear, int PayMonth)
        {
            IEnumerable<MemberPayMonthOfficialSheet> RecList = null;
            RecList = await DataApiRepository.GetApiUri<IEnumerable<MemberPayMonthOfficialSheet>>(AppSettings.ApiUrlBase + $"MemberCombining/GetMemberPayMonthOfficialSheet/{PayYear}/{PayMonth}").ConfigureAwait(false);
            return RecList;
        }
        /// <summary>
        /// 生成正式人员月度工资表
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> PostMemberPayMonthOfficialSheet(int PayYear, int PayMonth)
        {
            return await DataApiRepository.GetApiUri<ExcuteResult>(AppSettings.ApiUrlBase + $"MemberCombining/PostMemberPayMonthOfficialSheet/{PayYear}/{PayMonth}").ConfigureAwait(false);
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
