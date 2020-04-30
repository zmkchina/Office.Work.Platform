using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberUc
{
    public class UC_PayMonthVM : NotificationObject
    {
        public UC_PayMonthVM()
        {
            PayMonths = new ObservableCollection<MemberPayMonth>();
            SearchCondition = new MemberPayMonthSearch();
        }
        public async System.Threading.Tasks.Task InitVMAsync(Lib.Member PMember)
        {
            CurMember = PMember;
            if (PMember != null)
            {
                MemberPayMonthSearch SearchCondition = new MemberPayMonthSearch() { MemberId = PMember.Id, UserId = AppSettings.LoginUser.Id };
                IEnumerable<MemberPayMonth> MemberPlayMonths = await DataMemberPayMonthRepository.GetRecords(SearchCondition);
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

                IEnumerable<MemberPayMonth> MemberPlayMonths = await DataMemberPayMonthRepository.GetRecords(SearchCondition);
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
        public MemberPayMonthSearch SearchCondition { get; set; }
        /// <summary>
        /// 当前职工信息
        /// </summary>
        public Lib.Member CurMember { get; set; }
        /// <summary>
        /// 当前职工工资月度发放记录
        /// </summary>
        public ObservableCollection<MemberPayMonth> PayMonths { get; set; }

    }

}
