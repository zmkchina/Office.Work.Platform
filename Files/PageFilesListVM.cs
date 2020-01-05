using Newtonsoft.Json;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Office.Work.Platform.Files
{
    public class PageFilesListVM : NotificationObject
    {

        public string[] FileContentTypes => AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
        public ObservableCollection<ModelFile> EntityFiles { get; set; }

        public MSearchFile mSearchFile { get; set; }
        #region "方法"
        /// <summary>
        /// 构造函数
        /// </summary>
        public PageFilesListVM()
        {
            EntityFiles = new ObservableCollection<ModelFile>();
            mSearchFile = new MSearchFile();
        }

        #endregion
    }
}
