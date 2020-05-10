using System.Collections.Generic;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    // <summary>
    /// 职工工资综合查询类
    /// </summary>
    public static class DataMemberPaySheetRepository
    {
        /// <summary>
        /// 获取所有待遇表类型
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<string[]> GetPayTableTypes()
        {
            return await DataApiRepository.GetApiUri<string[]>(AppSet.ApiUrlBase + $"MemberPaySheet/GetPayTableTypes").ConfigureAwait(false);
        }

        /// <summary>
        /// 获取指定条件的待遇发放信息表
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<string> GetMemberPaySheet(MemberPaySheetSearch SearchCondition)
        {
            //创建查询url参数
            string urlParams = DataApiRepository.CreateUrlParams(SearchCondition);

            if (urlParams.Length > 0)
            {
                return await DataApiRepository.GetApiUri<string>(AppSet.ApiUrlBase + "MemberPaySheet/GetMemberPaySheet" + urlParams).ConfigureAwait(false);
            }
            return null;
        }
        /// <summary>
        /// 快速发放工资数据。
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> PostMemberPaySheet(MemberPayFastByPaySet PayFastInfo)
        {
            return await DataApiRepository.PostApiUriAsync(AppSet.ApiUrlBase + $"MemberPaySheet/PostMemberPaySheet", PayFastInfo, null).ConfigureAwait(false);
        }
    }
}
