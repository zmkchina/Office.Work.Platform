using System.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.FileDocs
{
    public class UC_FileInfoVM : NotificationObject
    {
        private FileDoc _CurFile;
        private FileOperateGrant _OperateGrant;
        /// <summary>
        /// 文件对象
        /// </summary>
        public FileDoc CurFile
        {
            get { return _CurFile; }
            set { _CurFile = value; this.RaisePropertyChanged(); }
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
        public void InitPropValusAsync(FileDoc PFile)
        {
            if (PFile == null) return;
            CurFile = PFile;
            OperateGrant = new FileOperateGrant(AppSettings.LoginUser, PFile);
            if (PFile.CanReadUserIds != null)
            {
                CurFileHasGrantNames = string.Join(",", AppSettings.SysUsers.Where(e => PFile.CanReadUserIds.Contains(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name)?.ToArray());
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
            public FileOperateGrant(User P_LoginUser, FileDoc P_CurFile)
            {
                CanRead = CanUpdate = "Collapsed";
                if (P_LoginUser.Post.Equals("管理员"))
                {
                    CanRead = CanUpdate = "Visible";
                }
                else if (P_CurFile.UserId.Equals(AppSettings.LoginUser.Id,System.StringComparison.Ordinal))
                {
                    CanRead = CanUpdate = "Visible";
                }
                else if (P_CurFile.CanReadUserIds.Contains(AppSettings.LoginUser.Id, System.StringComparison.Ordinal))
                {
                    CanRead = "Visible";
                }
            }
        }
    }
}
