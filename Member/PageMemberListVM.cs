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

namespace Office.Work.Platform.Member
{
    public class PageMemberListVM : NotificationObject
    {

        public string[] FileContentTypes => AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
        public ObservableCollection<Lib.Member> EntityList { get; set; }

        public MemberSearch mSearchMember { get; set; }
        #region "方法"
        /// <summary>
        /// 构造函数
        /// </summary>
        public PageMemberListVM()
        {
            EntityList = new ObservableCollection<Lib.Member>();
            mSearchMember = new MemberSearch();
        }

        #endregion
    }
}
