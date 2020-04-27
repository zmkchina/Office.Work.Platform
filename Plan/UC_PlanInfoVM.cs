using System.Collections.Generic;
using System.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Plan
{
    public class UC_PlanInfoVM : NotificationObject
    {
        private PlanOperateGrant _OperateGrant;

        public UC_PlanInfoVM()
        {
        }
        public async System.Threading.Tasks.Task Init_PlanInfoVMAsync(Lib.Plan P_Entity)
        {
            CurPlan = P_Entity;
            OperateGrant = new PlanOperateGrant(AppSettings.LoginUser, P_Entity);

            if (CurPlan.Files.Count < 1)
            {
                //如果该计划的附件文件没有读取则读取之。
                PlanFileSearch mSearchFile = new PlanFileSearch() { PlanId = P_Entity.Id, UserId = AppSettings.LoginUser.Id };
                IEnumerable<PlanFile> UpFiles = await DataPlanFileRepository.ReadFiles(mSearchFile);
                UpFiles.ToList().ForEach(e =>
                {
                    e.UpIntProgress = 100;
                    CurPlan.Files.Add(e);
                });
            }
            if (CurPlan.CreateUserId != null)
            {
                CurPlanCreateUserName = AppSettings.SysUsers.Where(e => CurPlan.CreateUserId.Equals(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name).FirstOrDefault()?.Trim();
            }
            if (CurPlan.ResponsiblePerson != null)
            {
                CurPlanResponsibleName = AppSettings.SysUsers.Where(e => CurPlan.ResponsiblePerson.Equals(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name).FirstOrDefault()?.Trim();
            }
            if (CurPlan.ReadGrant != null)
            {
                CurPlanHasGrantNames = string.Join(",", AppSettings.SysUsers.Where(e => CurPlan.ReadGrant.Contains(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name)?.ToArray());
            }
            if (CurPlan.Helpers != null)
            {
                CurPlanHelperNames = string.Join(",", AppSettings.SysUsers.Where(e => CurPlan.Helpers.Contains(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name)?.ToArray());
            }
        }
        /// <summary>
        /// 当前所选计划信息
        /// </summary>
        public Lib.Plan CurPlan { get; set; }

        /// <summary>
        /// 计划创建者的姓名（中文）。
        /// </summary>
        public string CurPlanCreateUserName { get; set; }
        /// <summary>
        /// 计划责任者的姓名（中文）。
        /// </summary>
        public string CurPlanResponsibleName { get; set; }
        /// <summary>
        ///计划协助人员姓名列表（中文）
        /// </summary>
        public string CurPlanHelperNames { get; set; }
        /// <summary>
        /// 计划有限读取人员的姓名列表（中文）。
        /// </summary>
        public string CurPlanHasGrantNames { get; set; }

        /// <summary>
        /// 用户对此计划的操作权限类对象。
        /// </summary>
        public PlanOperateGrant OperateGrant
        {
            get { return _OperateGrant; }
            set
            {
                _OperateGrant = value;
                this.RaisePropertyChanged();
            }
        }
        #region "方法"

        #endregion
        /// <summary>
        /// 用户对该计划的权限 A:计划修改 B:计划删除 C:计划进度更新 D:上传计划附件 E:计划完结 F:计划状态重置
        /// </summary>
        public class PlanOperateGrant
        {
            public string CanEdit { get; set; }
            public string CanDelete { get; set; }
            public string CanUpdate { get; set; }
            public string CanUpFile { get; set; }
            public string CanFinish { get; set; }
            public string CanReset { get; set; }
            public PlanOperateGrant(User P_LoginUser, Lib.Plan P_CurPlan)
            {
                CanDelete = CanEdit = CanUpFile = CanFinish = CanReset = CanUpdate = "Collapsed";
                if (P_LoginUser.Post.Equals("SysAdmin"))
                {
                    CanDelete = CanEdit = CanUpFile = CanFinish = CanReset = CanUpdate = "Visible";
                }
                else if (P_LoginUser.Post.Equals("部门负责人"))
                {
                    CanDelete = CanEdit = CanUpFile = CanFinish = CanReset = CanUpdate = "Visible";
                }
                else if (P_CurPlan.CurrectState.Contains(PlanStatus.Finished))
                {
                    CanDelete = CanEdit = CanUpFile = CanFinish = CanReset = CanUpdate = "Collapsed";
                }
                else if (P_LoginUser.Id.Equals(P_CurPlan.CreateUserId))
                {
                    //计划创建者：A—E
                    CanDelete = CanEdit = CanUpFile = CanFinish = CanUpdate = "Visible";
                }
                else if (P_LoginUser.Id.Equals(P_CurPlan.ResponsiblePerson))
                {
                    //责任者
                    CanUpFile = CanFinish = CanUpdate = "Visible";
                }
                else if (!string.IsNullOrWhiteSpace(P_CurPlan.Helpers) && P_CurPlan.Helpers.Contains(P_LoginUser.Id))
                {
                    //协助者
                    CanUpFile = CanUpdate = "Visible";
                }
            }
        }
    }

}
