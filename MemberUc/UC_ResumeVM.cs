using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberUc
{
    public class UC_ResumeVM : NotificationObject
    {
        public UC_ResumeVM()
        {
            CurRecords = new ObservableCollection<MemberResume>();
            SearchCondition = new MemberResumeSearch();
        }
        public async System.Threading.Tasks.Task InitVMAsync(Lib.Member PMember)
        {
            CurMember = PMember;
            if (PMember != null)
            {
                MemberResumeSearch SearchCondition = new MemberResumeSearch() { MemberId = PMember.Id, UserId = AppSettings.LoginUser.Id };
                IEnumerable<MemberResume> MemberResumess = await DataMemberResumeRepository.GetRecords(SearchCondition);
                CurRecords.Clear();
                MemberResumess.ToList().ForEach(e =>
                {
                    CurRecords.Add(e);
                });
            }
        }
        public async System.Threading.Tasks.Task SearchRecords()
        {
            if (SearchCondition != null)
            {
                SearchCondition.MemberId = CurMember.Id;
                SearchCondition.UserId = AppSettings.LoginUser.Id;

                IEnumerable<MemberResume> TempRecords = await DataMemberResumeRepository.GetRecords(SearchCondition);
                CurRecords.Clear();
                TempRecords.ToList().ForEach(e =>
                {
                    CurRecords.Add(e);
                });
            }
        }
        /// <summary>
        /// 查询条件类对象
        /// </summary>
        public MemberResumeSearch SearchCondition { get; set; }
        /// <summary>
        /// 当前职工信息
        /// </summary>
        public Lib.Member CurMember { get; set; }
        /// <summary>
        /// 当前职工工资月度发放记录
        /// </summary>
        public ObservableCollection<MemberResume> CurRecords { get; set; }
    }

}
