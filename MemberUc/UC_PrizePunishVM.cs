using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberUc
{
    public class UC_PrizePunishVM : NotificationObject
    {
        public UC_PrizePunishVM()
        {
            CurRecords = new ObservableCollection<MemberPrizePunish>();
            SearchCondition = new MemberPrizePunishSearch();
        }
        public async System.Threading.Tasks.Task InitVMAsync(Lib.Member PMember)
        {
            CurMember = PMember;
            if (PMember != null)
            {
                MemberPrizePunishSearch SearchCondition = new MemberPrizePunishSearch() { MemberId = PMember.Id, UserId = AppSettings.LoginUser.Id };
                IEnumerable<MemberPrizePunish> MemberPrizePunishss = await DataMemberPrizePunishRepository.GetRecords(SearchCondition);
                CurRecords.Clear();
                MemberPrizePunishss.ToList().ForEach(e =>
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

                IEnumerable<MemberPrizePunish> TempRecords = await DataMemberPrizePunishRepository.GetRecords(SearchCondition);
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
        public MemberPrizePunishSearch SearchCondition { get; set; }
        /// <summary>
        /// 当前职工信息
        /// </summary>
        public Lib.Member CurMember { get; set; }
        /// <summary>
        /// 当前职工工资月度发放记录
        /// </summary>
        public ObservableCollection<MemberPrizePunish> CurRecords { get; set; }
    }

}
