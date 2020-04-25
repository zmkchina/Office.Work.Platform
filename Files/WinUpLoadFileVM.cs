using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Office.Work.Platform.Files
{
    public class WinUpLoadFileVM : NotificationObject
    {
        private string _SelectFilePath;
        private bool _SelectFileBool;
        private List<SelectObj<User>> _UserSelectList;
        private double _UploadIntProgress;

        public WinUpLoadFileVM(string P_OnerId = "0000", string P_OwnerType = "其他文件", string P_ContentType = null)
        {
            EntityFile = new PlanFile
            {
                Name = "输入文件名称",
                Describe = "请输入文件描述，尽量包含搜索关键字",
                UserId = AppSettings.LoginUser.Id,
                ReadGrant = AppSettings.LoginUser.Id
            };
            SelectFileBool = false;
            InitUserList();
            FileContentTypes = AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
        }
        public string[] FileContentTypes { get; set; }
        public PlanFile EntityFile { get; set; }
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
        public string SelectFilePath
        {
            get { return _SelectFilePath; }
            set { _SelectFilePath = value; if (value.Length > 0) SelectFileBool = true; this.RaisePropertyChanged(); }
        }
        public bool SelectFileBool
        {
            get { return _SelectFileBool; }
            set { _SelectFileBool = value; this.RaisePropertyChanged(); }
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
