﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    /// <summary>
    /// 职工月度发放的待遇数据处理类
    /// </summary>
    public static class DataMemberSalaryRepository
    {
        private static string _ApiUrlBase = AppSet.LocalSetting.ResApiUrl;
        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Lib.MemberSalaryDto>> GetRecords(MemberSalaryDtoSearch SearchCondition)
        {
            IEnumerable<Lib.MemberSalaryDto> RecList = null;
            //创建查询url参数
            string urlParams = DataApiRepository.CreateUrlParams(SearchCondition);

            if (urlParams.Length > 0)
            {
                RecList = await DataApiRepository.GetApiUri<IEnumerable<Lib.MemberSalaryDto>>(_ApiUrlBase + "MemberSalary/Search" + urlParams).ConfigureAwait(false);
            }
            return RecList;
        }
        /// <summary>
        /// 单个新增数据
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddRecord(Lib.MemberSalaryEntity PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(_ApiUrlBase + "MemberSalary", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        
        /// <summary>
        /// 更新信息（采用PUT）
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdateRecord(Lib.MemberSalaryEntity PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PutApiUriAsync(_ApiUrlBase + "MemberSalary", PEntity).ConfigureAwait(false);
            return JsonResult;
        }

        /// <summary>
        /// 新增或更新一个实体
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddOrUpdate(Lib.MemberSalaryEntity PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(_ApiUrlBase + "MemberSalary/AddOrUpdate", PEntity).ConfigureAwait(false);
            return JsonResult;
        }

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> DeleteRecord(Lib.MemberSalaryEntity PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(_ApiUrlBase + "MemberSalary/?Id=" + PEntity.Id).ConfigureAwait(false);
            return JsonResult;
        }
    }
}
