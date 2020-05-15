using Microsoft.Extensions.Logging;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Office.Work.Platform.AppDataService
{
    public static class DataSystemRepository
    {

        private static string _ApiUrlBase = AppSet.LocalSetting.ResApiUrl;

        /// <summary>
        /// 从服务器读取系统设置
        /// </summary>
        /// <returns></returns>
        public static async Task<SettingServer> ReadServerSettings()
        {
            SettingServer ServerSetting = await DataApiRepository.GetApiUri<SettingServer>(_ApiUrlBase + "Settings").ConfigureAwait(false);
            return ServerSetting;
        }

        /// <summary>
        /// 更新服务器系统设置
        /// </summary>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdateServerSettings(SettingServer PEntity)
        {
            ExcuteResult excuteResult = await DataApiRepository.PutApiUriAsync(_ApiUrlBase + "Settings", PEntity).ConfigureAwait(false);
            return excuteResult;
        }

        
    }
}
