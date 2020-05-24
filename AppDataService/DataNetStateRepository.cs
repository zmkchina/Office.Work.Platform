using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;

namespace Office.Work.Platform.AppDataService
{
    public static class DataNetStateRepository
    {
        private static string _ApiUrlBase = AppSet.LocalSetting.ResApiUrl;
        /// <summary>
        /// 向服务器请求时间，以确定是否连接是否正常。
        /// </summary>
        /// <returns></returns>
        public static async Task<Lib.NetState> GetSeverState()
        {
            return await DataApiRepository.GetApiUri<Lib.NetState>(_ApiUrlBase + "NetState/GetTime");
        }
    }
}
