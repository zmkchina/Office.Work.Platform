using Newtonsoft.Json;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Office.Work.Platform.FileDocs
{
    public class PageSettingsSysVM : NotificationObject
    {

        public string[] FileContentTypes => AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
        public SettingServer EntitySettingServer { get; set; }

        #region "方法"
        /// <summary>
        /// 构造函数
        /// </summary>
        public PageSettingsSysVM()
        {
        }
        public async Task GetEntityInfoAsync()
        {
            EntitySettingServer = await DataSystemRepository.ReadServerSettings();
        }
        #endregion
    }
}
