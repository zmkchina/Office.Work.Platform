using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberUc
{
    public class UC_PayMonthOfficialVM : NotificationObject
    {
        public UC_PayMonthOfficialVM()
        {
            PayMonths = new ObservableCollection<MemberPayMonthOfficial>();
            SearchCondition = new MemberPayMonthOfficialSearch();
        }
        public async System.Threading.Tasks.Task InitVMAsync(Lib.Member PMember)
        {
            CurMember = PMember;
            if (PMember != null)
            {
                MemberPayMonthOfficialSearch SearchCondition = new MemberPayMonthOfficialSearch() { MemberId = PMember.Id, UserId = AppSettings.LoginUser.Id };
                IEnumerable<MemberPayMonthOfficial> MemberPlayMonths = await DataMemberPayMonthOfficialRepository.GetRecords(SearchCondition);
                PayMonths.Clear();
                MemberPlayMonths.ToList().ForEach(e =>
                {
                    PayMonths.Add(e);
                });
            }
        }
        public async System.Threading.Tasks.Task SearchRecords()
        {
            if (SearchCondition != null)
            {
                SearchCondition.MemberId = CurMember.Id;
                SearchCondition.UserId = AppSettings.LoginUser.Id;

                IEnumerable<MemberPayMonthOfficial> MemberPlayMonths = await DataMemberPayMonthOfficialRepository.GetRecords(SearchCondition);
                PayMonths.Clear();
                MemberPlayMonths.ToList().ForEach(e =>
                {
                    PayMonths.Add(e);
                });
            }
        }
        /// <summary>
        /// 查询条件类对象
        /// </summary>
        public MemberPayMonthOfficialSearch SearchCondition { get; set; }
        /// <summary>
        /// 当前职工信息
        /// </summary>
        public Lib.Member CurMember { get; set; }
        /// <summary>
        /// 当前职工工资月度发放记录
        /// </summary>
        public ObservableCollection<MemberPayMonthOfficial> PayMonths { get; set; }

    }

}
