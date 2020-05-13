using System.Collections.Generic;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    public static class DataNoteRepository
    {
        private static string _ApiUrlBase = AppSet.LocalSetting.ResApiUrl;
        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Lib.Note>> GetRecords(NoteSearch SearchCondition)
        {
            IEnumerable<Lib.Note> RecList = null;
            //创建查询url参数
            string urlParams = DataApiRepository.CreateUrlParams(SearchCondition);

            if (urlParams.Length > 0)
            {
                RecList = await DataApiRepository.GetApiUri<IEnumerable<Lib.Note>>(_ApiUrlBase + "Note/Search" + urlParams).ConfigureAwait(false);
            }
            return RecList;
        }
        /// <summary>
        /// 根据ID查询指定记录信息
        /// </summary>
        /// <param name="mSearchMember">查询条件类的实例</param>
        /// <returns></returns>
        public static async Task<Lib.Note> GetOneById(string Id)
        {
           return await DataApiRepository.GetApiUri<Lib.Note>(_ApiUrlBase + "Note/" + Id);
        }
        /// <summary>
        /// 单个新增数据
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddRecord(Lib.Note PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(_ApiUrlBase + "Note", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <param name="P_Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> AddRecords(List<Lib.Note> Entitys)
        {
            ExcuteResult JsonResult = await DataApiRepository.PostApiUriAsync(_ApiUrlBase + "Note/AddRange", Entitys).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 更新信息（采用PUT）
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdateRecord(Lib.Note PEntity)
        {
            ExcuteResult JsonResult = await DataApiRepository.PutApiUriAsync(_ApiUrlBase + "Note", PEntity).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> DeleteRecord(string UserId)
        {
            ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(_ApiUrlBase + "Note/?Id=" + UserId).ConfigureAwait(false);
            return JsonResult;
        }
       
    }
}
