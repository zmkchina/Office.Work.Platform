using System.Collections.Generic;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    public static class DataMemberRepository
    {
        private static string _ApiUrlBase = AppSet.LocalSetting.ResApiUrl+ @"MemberInfo/";

        /// <summary>
        /// 查询满足指定条件的计划信息
        /// </summary>
        /// <param name="mSearchMember">查询条件类的实例</param>
        /// <returns></returns>
        public static async Task<Lib.MemberInfoEntity> ReadEntity(string EntityId)
        {
            Lib.MemberInfoEntity TheEntity = null;
            string urlParams = DataApiRepository.CreateUrlParams(EntityId);

            if (urlParams.Length > 0)
            {
                TheEntity = await DataApiRepository.GetApiUri<Lib.MemberInfoEntity>(_ApiUrlBase + "ReadEntity" + urlParams);
            }
            return TheEntity;
        }

        /// <summary>
        /// 读取满足指定条件的实体
        /// </summary>
        /// <param name="mSearchMember">查询条件类的实例</param>
        /// <returns></returns>
        public static async Task<List<Lib.MemberInfoDto>> ReadDtos(Lib.MemberInfoSearch mSearchMember)
        {
            List<Lib.MemberInfoDto> MemberList = new List<Lib.MemberInfoDto>();
            string urlParams = DataApiRepository.CreateUrlParams(mSearchMember);

            if (urlParams.Length > 0)
            {
                MemberList = await DataApiRepository.GetApiUri<List<Lib.MemberInfoDto>>(_ApiUrlBase + "ReadDtos" + urlParams);
            }
            return MemberList;
        }
        /// <summary>
        /// 添加单个实体
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddEntity(Lib.MemberInfoEntity PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(_ApiUrlBase + "AddEntity", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 新增或更新单个实体
        /// </summary>
        /// <param name="PEntity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddOrUpdateEntity(Lib.MemberInfoEntity PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(_ApiUrlBase + "AddOrUpdateEntity", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 更新信息（采用PUT）
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdateEntity(Lib.MemberInfoEntity PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PutApiUriAsync(_ApiUrlBase + "UpdateEntity", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> DeleteEntity(Lib.MemberInfoEntity Entity)
        {
            ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(_ApiUrlBase + "DeleteEntity/?Id=" + Entity.Id).ConfigureAwait(false);
            return JsonResult;
        }
    }
}
