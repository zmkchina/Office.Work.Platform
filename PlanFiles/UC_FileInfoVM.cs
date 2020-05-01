using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Office.Work.Platform.PlanFiles
{
    public class UC_FileInfoVM : NotificationObject
    {
        private PlanFile _CurPlanFile;
        private FileOperateGrant _OperateGrant;
        /// <summary>
        /// 文件对象
        /// </summary>
        public PlanFile CurPlanFile
        {
            get { return _CurPlanFile; }
            set { _CurPlanFile = value; this.RaisePropertyChanged(); }
        }

        /// <summary>
        /// 上传者的姓名（中文）。
        /// </summary>
        public string CurFileCreateUserName { get; set; }

        /// <summary>
        /// 文件有限读取人员（同计划有权读取人员）（中文）。
        /// </summary>
        public string CurFileHasGrantNames { get; set; }

        /// <summary>
        /// 用户对此文件的操作权限类对象。
        /// </summary>
        public FileOperateGrant OperateGrant
        {
            get { return _OperateGrant; }
            set
            {
                _OperateGrant = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public UC_FileInfoVM()
        {
            OperateGrant = new FileOperateGrant();
        }
        public void InitPropValusAsync(PlanFile PFile)
        {
            if (PFile == null) return;
            CurPlanFile = PFile;
            OperateGrant = new FileOperateGrant(AppSettings.LoginUser, PFile);
            if (PFile.Plan.ReadGrant != null)
            {
                CurFileHasGrantNames = string.Join(",", AppSettings.SysUsers.Where(e => PFile.Plan.ReadGrant.Contains(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name)?.ToArray());
            }
            if (PFile.UserId != null)
            {
                CurFileCreateUserName = string.Join(",", AppSettings.SysUsers.Where(e => PFile.UserId.Equals(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name)?.ToArray());
            }
        }
        
        /// <summary>
        /// 用户对该计划的权限 A:读取文件 B:删除文件 C:更新文件表述
        /// </summary>
        public class FileOperateGrant
        {
            public string CanRead { get; set; }
            public string CanUpdate { get; set; }
            public FileOperateGrant()
            {
                CanRead = CanUpdate = "Collapsed";
            }
            public FileOperateGrant(User P_LoginUser, PlanFile P_CurFile)
            {
                CanRead = CanUpdate = "Collapsed";
                if (P_LoginUser.Post.Equals("管理员"))
                {
                    CanRead = CanUpdate = "Visible";
                }
                else if (P_LoginUser.Post.Equals("部门负责人"))
                {
                    CanRead = CanUpdate = "Visible";
                }
                else if (P_CurFile.Plan.ReadGrant.Contains(P_CurFile.UserId))
                {
                    if (P_LoginUser.Id.Equals(P_CurFile.Plan.CreateUserId) || P_CurFile.Plan.Helpers.Contains(P_LoginUser.Id))
                    {
                        //文件上传人员：A—E
                        CanRead = CanUpdate = "Visible";
                    }
                    else
                    {
                        CanRead = "Visible";
                    }
                }
            }
        }
    }
}
