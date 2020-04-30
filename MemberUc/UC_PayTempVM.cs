using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberUc
{
    public class UC_PayTempVM : NotificationObject
    {
        public UC_PayTempVM()
        {
            PayTemps = new ObservableCollection<MemberPayTemp>();
            SearchCondition = new MemberPayTempSearch();
        }
        public async System.Threading.Tasks.Task InitVMAsync(Lib.Member PMember)
        {
            CurMember = PMember;
            if (PMember != null)
            {
                MemberPayTempSearch SearchCondition = new MemberPayTempSearch() { MemberId = PMember.Id, UserId = AppSettings.LoginUser.Id };
                IEnumerable<MemberPayTemp> MemberPayTempss = await DataMemberPayTempRepository.GetRecords(SearchCondition);
                PayTemps.Clear();
                MemberPayTempss.ToList().ForEach(e =>
                {
                    PayTemps.Add(e);
                });
            }
        }
        public async System.Threading.Tasks.Task SearchRecords()
        {
            if (SearchCondition != null)
            {
                SearchCondition.MemberId = CurMember.Id;
                SearchCondition.UserId = AppSettings.LoginUser.Id;

                IEnumerable<MemberPayTemp> MemberPayTempss = await DataMemberPayTempRepository.GetRecords(SearchCondition);
                PayTemps.Clear();
                MemberPayTempss.ToList().ForEach(e =>
                {
                    PayTemps.Add(e);
                });
            }
        }
        /// <summary>
        /// 查询条件类对象
        /// </summary>
        public MemberPayTempSearch SearchCondition { get; set; }
        /// <summary>
        /// 当前职工信息
        /// </summary>
        public Lib.Member CurMember { get; set; }
        /// <summary>
        /// 当前职工工资月度发放记录
        /// </summary>
        public ObservableCollection<MemberPayTemp> PayTemps { get; set; }
    }

}
