using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberUc
{
    public class UC_PayMonthUnofficialVM : NotificationObject
    {
        public UC_PayMonthUnofficialVM()
        {
            PayMonthUnofficials = new ObservableCollection<MemberPayMonthUnofficial>();
            SearchCondition = new MemberPayMonthUnofficialSearch();
        }
        public async System.Threading.Tasks.Task InitVMAsync(Lib.Member PMember)
        {
            CurMember = PMember;
            if (PMember != null)
            {
                MemberPayMonthUnofficialSearch SearchCondition = new MemberPayMonthUnofficialSearch() { MemberId = PMember.Id, UserId = AppSettings.LoginUser.Id };
                IEnumerable<MemberPayMonthUnofficial> MemberPlayMonths = await DataMemberPayMonthUnofficialRepository.GetRecords(SearchCondition);
                PayMonthUnofficials.Clear();
                MemberPlayMonths.ToList().ForEach(e =>
                {
                    PayMonthUnofficials.Add(e);
                });
            }
        }
        public async System.Threading.Tasks.Task SearchRecords()
        {
            if (SearchCondition != null)
            {
                SearchCondition.MemberId = CurMember.Id;
                SearchCondition.UserId = AppSettings.LoginUser.Id;

                IEnumerable<MemberPayMonthUnofficial> MemberPlayMonths = await DataMemberPayMonthUnofficialRepository.GetRecords(SearchCondition);
                PayMonthUnofficials.Clear();
                MemberPlayMonths.ToList().ForEach(e =>
                {
                    PayMonthUnofficials.Add(e);
                });
            }
        }
        /// <summary>
        /// 查询条件类对象
        /// </summary>
        public MemberPayMonthUnofficialSearch SearchCondition { get; set; }
        /// <summary>
        /// 当前职工信息
        /// </summary>
        public Lib.Member CurMember { get; set; }
        /// <summary>
        /// 当前职工工资月度发放记录
        /// </summary>
        public ObservableCollection<MemberPayMonthUnofficial> PayMonthUnofficials { get; set; }

    }

}
