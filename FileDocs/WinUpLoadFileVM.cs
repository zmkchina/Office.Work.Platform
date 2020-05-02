using System.Collections.Generic;
using System.IO;
using System.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.FileDocs
{
    public class WinUpLoadFileVM : NotificationObject
    {
        private List<SelectObj<User>> _UserSelectList;
        private double _UploadIntProgress;

        public WinUpLoadFileVM(FileInfo P_FileInfo, string P_OwnerType, string P_OwnerId, string P_OwnerContentType)
        {
            UpFileInfo = P_FileInfo;
            EntityFile = new FileDoc
            {
                OwnerType = P_OwnerType,
                ContentType = P_OwnerContentType,
                OwnerId = P_OwnerId,
                Name = UpFileInfo.Name.Substring(0, P_FileInfo.Name.LastIndexOf('.')),
                UserId = AppSettings.LoginUser.Id,
                Length = UpFileInfo.Length,
                ExtendName = UpFileInfo.Extension,
                UpIntProgress = 0,
                DownIntProgress = 100,
                Pubdate = System.DateTime.Now,
                UpDateTime=System.DateTime.Now,         
                Describe = "请输入文件描述，尽量包含搜索关键字",
            };
            InitUserList();
            if (P_OwnerType.Equals("人事附件", System.StringComparison.Ordinal))
            {
                FileContentTypes = new string[] { P_OwnerContentType };
            }
            else if (P_OwnerType.Equals("计划附件", System.StringComparison.Ordinal))
            {
                FileContentTypes = new string[] { P_OwnerContentType };
            }
            else
            {
                FileContentTypes = AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            }
        }
        public string[] FileContentTypes { get; set; }
        public FileDoc EntityFile { get; set; }
        /// <summary>
        /// 上传文件的信息
        /// </summary>
        public FileInfo UpFileInfo { get; set; }
        /// <summary>
        /// 用户选择标志集合
        /// </summary>
        public List<SelectObj<User>> UserSelectList
        {
            get { return _UserSelectList; }
            set { _UserSelectList = value; this.RaisePropertyChanged(); }
        }
        /// <summary>
        /// 文件上传进度百分比
        /// </summary>
        public double UploadIntProgress
        {
            get { return _UploadIntProgress; }
            set { _UploadIntProgress = value; this.RaisePropertyChanged(); }
        }
        public void InitUserList()
        {
            UserSelectList = new List<SelectObj<User>>();
            foreach (User item in AppSettings.SysUsers)
            {

                UserSelectList.Add(new SelectObj<User>(true, item));
            }
        }
        public string GetSelectUserIds()
        {
            List<string> SelectIds = UserSelectList.Where(x => x.IsSelect).Select(y => y.Obj.Id).ToList();

            return string.Join(",", SelectIds.ToArray());
        }
    }
}
