using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Office.Work.Platform.Files
{
    public class UC_FileInfoVM : NotificationObject
    {
        private ModelFile _EntityFileInfo;
        private double _DownIntProgress;
        private FileOperateGrant _OperateGrant;

        public UC_FileInfoVM()
        {
            OperateGrant = new FileOperateGrant();
        }
        public void Init_FileInfoVM(ModelFile P_File)
        {
            if (P_File == null) return;
            EntityFileInfo = P_File;
            FileContentTypes = AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            OperateGrant = new FileOperateGrant(AppSettings.LoginUser, P_File);
        }
        /// <summary>
        /// 文件类别
        /// </summary>
        public string[] FileContentTypes { get; set; }

        /// <summary>
        /// 文件对象
        /// </summary>
        public ModelFile EntityFileInfo
        {
            get { return _EntityFileInfo; }
            set { _EntityFileInfo = value; this.RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用户选择标志集合
        /// </summary>
        public ObservableCollection<ModelSelectObj<ModelUser>> UserSelectList { get; set; }

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
            UserSelectList = new ObservableCollection<ModelSelectObj<ModelUser>>();
            foreach (ModelUser item in AppSettings.SysUsers)
            {
                UserSelectList.Add(new ModelSelectObj<ModelUser>
                {

                    IsSelect = EntityFileInfo.ReadGrant != null ? EntityFileInfo.ReadGrant.Contains(item.Id) : false,
                    Obj = item
                });
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
            public FileOperateGrant(ModelUser P_LoginUser, ModelFile P_CurFile)
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
                    //文件上传人员：A—E
                    CanDelete = CanRead = CanUpdate = "Collapsed";
                }
                else if (!string.IsNullOrWhiteSpace(P_CurFile.ReadGrant) && P_CurFile.ReadGrant.Contains(P_LoginUser.Id))
                {
                    //有权读限人员
                    CanRead = "Visible";
                }
            }
        }
    }
}
