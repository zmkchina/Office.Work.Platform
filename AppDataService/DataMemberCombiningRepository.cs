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
        /// 获取本单位聘用合同制人员月度工资发放表
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<string> GetPingYongMemberMonthPaySheet(int PayYear, int PayMonth)
        {
            return  await DataApiRepository.GetApiUri<string>(AppSettings.ApiUrlBase + $"MemberCombining/GetPingYongMemberMonthPaySheet/{PayYear}/{PayMonth}").ConfigureAwait(false);
        }
        /// <summary>
        /// 获取本单位聘用合同制人员月度补贴发放表
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<string> GetPingYongMemberMonthAllowanceSheet(int PayYear, int PayMonth)
        {
            return await DataApiRepository.GetApiUri<string>(AppSettings.ApiUrlBase + $"MemberCombining/GetPingYongMemberMonthAllowanceSheet/{PayYear}/{PayMonth}").ConfigureAwait(false);
        }
        /// <summary>
        /// 获取本单位劳动合同制人员月度工资发放表
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<string> GetLaoDongMemberMonthPaySheet(int PayYear, int PayMonth)
        {
            return await DataApiRepository.GetApiUri<string>(AppSettings.ApiUrlBase + $"MemberCombining/GetLaoDongMemberMonthPaySheet/{PayYear}/{PayMonth}").ConfigureAwait(false);
        }
        /// <summary>
        /// 获取本单位劳动合同制人员月度补贴发放表
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<string> GetLaoDongMemberMonthAllowanceSheet(int PayYear, int PayMonth)
        {
            return await DataApiRepository.GetApiUri<string>(AppSettings.ApiUrlBase + $"MemberCombining/GetLaoDongMemberMonthAllowanceSheet/{PayYear}/{PayMonth}").ConfigureAwait(false);
        }
        /// <summary>
        /// 获取本单位劳务合同制人员月度工资发放表
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<string> GetLaoWuMemberMonthPaySheet(int PayYear, int PayMonth)
        {
            return await DataApiRepository.GetApiUri<string>(AppSettings.ApiUrlBase + $"MemberCombining/GetLaoWuMemberMonthPaySheet/{PayYear}/{PayMonth}").ConfigureAwait(false);
        }
        /// <summary>
        /// 获取本单位离退休人员月度补贴发放表
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<string> GetTuiXiuMemberMonthAllowanceSheet(int PayYear, int PayMonth)
        {
            return await DataApiRepository.GetApiUri<string>(AppSettings.ApiUrlBase + $"MemberCombining/GetTuiXiuMemberMonthAllowanceSheet/{PayYear}/{PayMonth}").ConfigureAwait(false);
        }

        /// <summary>
        /// 按指定的规定生成指定月份的待遇信息
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> PostMemberPayMonthOfficialSheet(int PayYear, int PayMonth)
        {
            return await DataApiRepository.GetApiUri<ExcuteResult>(AppSettings.ApiUrlBase + $"MemberCombining/PostMemberPayMonthOfficialSheet/{PayYear}/{PayMonth}").ConfigureAwait(false);
        }
    }
}
