using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Office.Work.Platform.Member
{
    public class PageEditMemberVM : NotificationObject
    {


        public PageEditMemberVM(Lib.Member P_EntityMember)
        {
            if (P_EntityMember == null)
            {
                EntityMember = new Lib.Member();
            }
            else
            {
                EntityMember = P_EntityMember;
            }
            JobGrades = new string[] { "管理4级", "管理5级", "管理6级", "管理7级", "管理8级", "管理9级", "管理9级", "管理10级",
                "专技3级", "专技4级", "专技5级", "专技6级", "专技7级", "专技8级", "专技9级", "专技10级", "专技11级", "专技12级", "专技13级",
                "高级技师","技师","高级工","中级工","初级工" ,"普通工" };
            DepartmentNames = new string[] { "综合科", "政工科", "发展计划科", "科技信息科", "航道养护科", "船闸事务科", "工程建设科", "港口业务科",
                "财务审计科", "工会"};
            PostNames = new string[] { "办事员", "副科长", "科长", "副主任", "主任", "工会主席", "工会副主席" };
            UnitNames = new string[] { "市港航事业发展中心", "市大柳巷船闸管理处", "市成子河船闸管理处", "市古泊河船闸管理处" };
        }

        #region "属性"
        public Lib.Member EntityMember { get; set; }
        public string[] JobGrades { get; private set; }
        public string[] DepartmentNames { get; private set; }
        public string[] PostNames { get; private set; }
        public string[] UnitNames { get; private set; }

        /// <summary>
        /// 家庭成员集合
        /// </summary>
        public ObservableCollection<MemberFamily> MemberFamily { get; set; }
        #endregion


        #region "方法"
        public async Task<ExcuteResult> AddOrUpdate()
        {
            return await DataMemberRepository.AddOrUpdate(EntityMember);

        }
        #endregion
    }
}
