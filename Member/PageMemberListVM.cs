using System.Collections.Generic;
using System.Collections.ObjectModel;
using Office.Work.Platform.AppCodes;

namespace Office.Work.Platform.Member
{
    public class PageMemberListVM : NotificationObject
    {

        public ObservableCollection<Lib.Member> EntityList { get; set; }
        public Dictionary<string, string> FieldCn2En { get; set; }

        public string FieldEnName { get; set; }
        public string FieldValue { get; set; }
        public bool SearchInResult { get; set; }

        #region "方法"
        /// <summary>
        /// 构造函数
        /// </summary>
        public PageMemberListVM()
        {
            EntityList = new ObservableCollection<Lib.Member>();
            FieldCn2En = new Dictionary<string, string>() { { "Name", "姓名" }, { "UnitName", "单位" },
                { "Job", "岗位性质" }, { "JobGrade", "岗位级别" }, { "EducationTop", "最高学历" }, { "Age", "年龄" },{ "Remarks", "备注" }
        };
        }

        #endregion
    }
}
