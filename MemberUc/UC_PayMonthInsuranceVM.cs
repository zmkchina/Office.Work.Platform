using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberUc
{
    public class UC_PayMonthInsuranceVM : NotificationObject
    {
        public UC_PayMonthInsuranceVM()
        {
            PayMonthInsurances = new ObservableCollection<MemberPayMonthInsurance>();
            SearchCondition = new MemberPayMonthInsuranceSearch();
        }
        public async System.Threading.Tasks.Task InitVMAsync(Lib.Member PMember)
        {
            CurMember = PMember;
            if (PMember != null)
            {
                MemberPayMonthInsuranceSearch SearchCondition = new MemberPayMonthInsuranceSearch() { MemberId = PMember.Id, UserId = AppSettings.LoginUser.Id };
                IEnumerable<MemberPayMonthInsurance> MemberPlayMonths = await DataMemberPayMonthInsuranceRepository.GetRecords(SearchCondition);
                PayMonthInsurances.Clear();
                MemberPlayMonths.ToList().ForEach(e =>
                {
                    PayMonthInsurances.Add(e);
                });
            }
        }
        public async System.Threading.Tasks.Task SearchRecords()
        {
            if (SearchCondition != null)
            {
                SearchCondition.MemberId = CurMember.Id;
                SearchCondition.UserId = AppSettings.LoginUser.Id;

                IEnumerable<MemberPayMonthInsurance> MemberPlayMonths = await DataMemberPayMonthInsuranceRepository.GetRecords(SearchCondition);
                PayMonthInsurances.Clear();
                MemberPlayMonths.ToList().ForEach(e =>
                {
                    PayMonthInsurances.Add(e);
                });
            }
        }
        /// <summary>
        /// 查询条件类对象
        /// </summary>
        public MemberPayMonthInsuranceSearch SearchCondition { get; set; }
        /// <summary>
        /// 当前职工信息
        /// </summary>
        public Lib.Member CurMember { get; set; }
        /// <summary>
        /// 当前职工工资月度发放记录
        /// </summary>
        public ObservableCollection<MemberPayMonthInsurance> PayMonthInsurances { get; set; }

    }

}
