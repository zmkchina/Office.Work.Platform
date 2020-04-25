using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Office.Work.Platform.Files
{
    public class UC_FileInfoVM : NotificationObject
    {
        private PlanFile _EntityFileInfo;
        private double _DownIntProgress;
        private FileOperateGrant _OperateGrant;
        public UC_FileInfoVM()
        {
            OperateGrant = new FileOperateGrant();
            UserSelectList = new ObservableCollection<SelectObj<User>>();
        }
        public void InitPropValus(PlanFile P_File, Lib.Plan P_Plan = null)
        {
            if (P_File == null) return;
            EntityFileInfo = P_File;
            FileContentTypes = AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            foreach (User item in AppSettings.SysUsers)
            {
                UserSelectList.Add(new SelectObj<User>(P_File.ReadGrant.Contains(item.Id), item));
            }
            OperateGrant = new FileOperateGrant(AppSettings.LoginUser, P_File, P_Plan);
        }
        /// <summary>
        /// 文件类别
        /// </summary>
        public string[] FileContentTypes { get; set; }

        /// <summary>
        /// 文件对象
        /// </summary>
        public PlanFile EntityFileInfo
        {
            get { return _EntityFileInfo; }
            set { _EntityFileInfo = value; this.RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用户选择标志集合
        /// </summary>
        public ObservableCollection<SelectObj<User>> UserSelectList { get; set; }

        /// <summary>
        /// 下载进度百分比
        /// </summary>
        public double DownIntProgress
        {
            get { return _DownIntProgress; }
            set { _DownIntProgress = value; this.RaisePropertyChanged(); }
        }
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

        #region "方法"




        private void InitUserList()
        {
            UserSelectList = new ObservableCollection<SelectObj<User>>();
            foreach (User item in AppSettings.SysUsers)
            {
                UserSelectList.Add(new SelectObj<User>
                (EntityFileInfo.ReadGrant != null ? EntityFileInfo.ReadGrant.Contains(item.Id) : false,
                 item
                ));
            }
        }
        public string GetSelectUserIds()
        {
            List<string> SelectIds = UserSelectList.Where(x => x.IsSelect).Select(y => y.Obj.Id).ToList();

            return string.Join(",", SelectIds.ToArray());
        }
        #endregion
        /// <summary>
        /// 用户对该计划的权限 A:读取文件 B:删除文件 C:更新文件表述
        /// </summary>
        public class FileOperateGrant
        {
            public string CanRead { get; set; }
            public string CanDelete { get; set; }
            public string CanUpdate { get; set; }
            public FileOperateGrant()
            {
                CanDelete = CanRead = CanUpdate = "Collapsed";
            }
            public FileOperateGrant(User P_LoginUser, PlanFile P_CurFile, Lib.Plan P_OwnerPlan)
            {
                CanDelete = CanRead = CanUpdate = "Collapsed";
                if (P_LoginUser.Post.Equals("SysAdmin"))
                {
                    CanDelete = CanRead = CanUpdate = "Visible";
                }
                else if (P_LoginUser.Post.Equals("ZgkLeader"))
                {
                    CanDelete = CanRead = CanUpdate = "Visible";
                }
                else if (P_LoginUser.Id.Equals(P_CurFile.UserId))
                {
                    if (P_OwnerPlan != null && P_OwnerPlan.CurrectState != "已经完成")
                    {
                        //文件上传人员：A—E
                        CanDelete = CanRead = CanUpdate = "Visible";
                    }
                    else
                    {
                        CanRead = CanUpdate = "Visible";
                    }
                }
                if (!string.IsNullOrWhiteSpace(P_CurFile.ReadGrant) && P_CurFile.ReadGrant.Contains(P_LoginUser.Id))
                {
                    //有权读限人员
                    CanRead = "Visible";
                }
            }
        }
    }
}
