using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Member
{
    public class UC_PayTempVM : NotificationObject
    {
        public UC_PayTempVM()
        {
        }
        public async System.Threading.Tasks.Task Init_PayTempVMAsync(Lib.Member P_Entity)
        {
            PlayTemps = new ObservableCollection<MemberPayTemp>();
            if (P_Entity!=null)
            {
                MemberPayTempSearch mSearchFile = new MemberPayTempSearch() { MemberId = P_Entity.Id, UserId = AppSettings.LoginUser.Id };
                IEnumerable<MemberPayTemp> MemberPayTemps = await DataMemberPlayTempRepository.ReadPayTemps(mSearchFile);
                MemberPayTemps.ToList().ForEach(e =>
                {
                    PlayTemps.Add(e);
                });
            }
        }
        /// <summary>
        /// 当前所选计划信息
        /// </summary>
        public ObservableCollection<MemberPayTemp> PlayTemps { get; set; }

    }

}
