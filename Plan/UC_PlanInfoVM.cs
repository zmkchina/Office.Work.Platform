using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Office.Work.Platform.Plan
{
    public class UC_PlanInfoVM : NotificationObject
    {
        private PlanOperateGrant _OperateGrant;

        public UC_PlanInfoVM()
        {
        }
        public async void Init_PlanInfoVMAsync(Lib.Plan P_Entity)
        {
            UploadFiles = new ObservableCollection<PlanFile>();
            EntityPlan = P_Entity;
            OperateGrant = new PlanOperateGrant(AppSettings.LoginUser, P_Entity);
            //设置查询条件类
            PlanFileSearch mSearchFile = new PlanFileSearch() { PlanId=P_Entity.Id,UserId=AppSettings.LoginUser.Id};
            IEnumerable<PlanFile> UpFiles = await DataPlanFileRepository.ReadFiles(mSearchFile);
            UpFiles.ToList().ForEach(e =>
            {
                UploadFiles.Add(e);
            });
        }
        /// <summary>
        /// 当前所选计划信息
        /// </summary>
        public Lib.Plan EntityPlan { get; set; }
        /// <summary>
        /// 上传的文件集合
        /// </summary>
        public ObservableCollection<PlanFile> UploadFiles { get; set; }

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
