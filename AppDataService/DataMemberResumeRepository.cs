﻿using System.Collections.Generic;
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
        private static string _ApiUrlBase = AppSet.LocalSetting.ResApiUrl;
        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Lib.MemberResumeDto>> GetRecords(Lib.MemberResumeDtoSearch SearchCondition)
        {
            IEnumerable<Lib.MemberResumeDto> RecList = null;
            //创建查询url参数
            string urlParams = DataApiRepository.CreateUrlParams(SearchCondition);

            if (urlParams.Length > 0)
            {
                RecList = await DataApiRepository.GetApiUri<IEnumerable<Lib.MemberResumeDto>>(_ApiUrlBase + "MemberResume/Search" + urlParams).ConfigureAwait(false);
            }
            return RecList;
        }
        /// <summary>
        /// 单个新增数据
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddRecord(Lib.MemberResumeEntity PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(_ApiUrlBase + "MemberResume", PEntity).ConfigureAwait(false);
            return JsonResult;
        }

        /// <summary>
        /// 更新信息（采用PUT）
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdateRecord(Lib.MemberResumeEntity PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PutApiUriAsync(_ApiUrlBase + "MemberResume", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> DeleteRecord(Lib.MemberResumeEntity PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(_ApiUrlBase + "MemberResume/?Id=" + PEntity.Id).ConfigureAwait(false);
            return JsonResult;
        }
    }
}
