using System.Collections.Generic;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    public static class DataUserRepository
    {
        private static string _ApiUrlBase = AppSet.LocalSetting.ResApiUrl;
        /// <summary>
        /// 读取所有用户
        /// </summary>
        /// <returns></returns>
        public static async Task<List<User>> GetAllRecords()
        {
            List<User> SysUsers = await DataApiRepository.GetApiUri<List<User>>(_ApiUrlBase + "User").ConfigureAwait(false);
            return SysUsers;
        }
        /// <summary>
        /// 根据ID查询指定用户信息
        /// </summary>
        /// <param name="mSearchMember">查询条件类的实例</param>
        /// <returns></returns>
        public static async Task<Lib.User> GetOneById(string UserId)
        {
           return await DataApiRepository.GetApiUri<User>(_ApiUrlBase + "User/" + UserId);
        }
        /// <summary>
        /// 单个新增数据
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddRecord(Lib.User PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(_ApiUrlBase + "User", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <param name="P_Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddRecords(List<Lib.User> Entitys)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(_ApiUrlBase + "User/AddRange", Entitys).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 更新信息（采用PUT）
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdateRecord(Lib.User PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PutApiUriAsync(_ApiUrlBase + "User", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> DeleteRecord(string UserId)
        {
            ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(_ApiUrlBase + "User/?Id=" + UserId).ConfigureAwait(false);
            return JsonResult;
        }
       
    }
}
