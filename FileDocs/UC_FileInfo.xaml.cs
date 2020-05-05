using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System;
using System.Linq;
using System.Net.Http.Handlers;
using System.Windows;
using System.Windows.Controls;

namespace Office.Work.Platform.FileDocs
{
    /// <summary>
    /// UC_FileInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UC_FileInfo : UserControl
    {
        private UC_FileInfoVM _UCFileInfoVM;
        private Action<FileDoc> _DelFileCallBackFun = null;
        public UC_FileInfo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化视图模型属性值。
        /// </summary>
        /// <param name="P_File">待修改、删除的文件对象</param>
        /// <param name="P_DelFileCallBackFun">删除文件后回调的函数</param>
        public void Init_FileInfo(FileDoc P_File, Action<FileDoc> P_DelFileCallBackFun = null)
        {
            _UCFileInfoVM = new UC_FileInfoVM();
            _UCFileInfoVM.InitPropValusAsync(P_File);
            _DelFileCallBackFun = P_DelFileCallBackFun;
            DataContext = _UCFileInfoVM;
        }
        /// <summary>
        /// 打开该文件，如本地不存在则下载之。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_OpenFileAsync(object sender, RoutedEventArgs e)
        {
            Button V_Button = sender as Button;
            V_Button.IsEnabled = false;
            ProgressMessageHandler progress = new System.Net.Http.Handlers.ProgressMessageHandler();
            progress.HttpReceiveProgress += (object sender, System.Net.Http.Handlers.HttpProgressEventArgs e) =>
            {
                _UCFileInfoVM.CurFile.DownIntProgress = e.ProgressPercentage;// (double)(e.BytesTransferred / e.TotalBytes) * 100;
            };
            string theDownFileName = await DataFileDocRepository.DownloadFile(_UCFileInfoVM.CurFile, false, progress);
            if (theDownFileName == null)
            {
                (new WinMsgDialog("文件下载失败,可能已被删除！", "警告")).ShowDialog();
                V_Button.IsEnabled = true;
                return;
            }
            _UCFileInfoVM.CurFile.DownIntProgress = 100;
            DataFileDocRepository.OpenFileInfo(theDownFileName);
            V_Button.IsEnabled = true;
        }
        /// <summary>
        /// 更新文件信息（文件名及文件描述）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_SaveChange(object sender, RoutedEventArgs e)
        {
            ExcuteResult JsonResult = await DataFileDocRepository.UpdateFileInfo(_UCFileInfoVM.CurFile);
            if (JsonResult.State == 0)
            {
                AppSettings.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
            }
               (new WinMsgDialog(JsonResult.Msg, "消息")).ShowDialog();
        }
        private async void btn_DeleFile(object sender, RoutedEventArgs e)
        {
            ExcuteResult JsonResult = await DataFileDocRepository.DeleteFileInfo(_UCFileInfoVM.CurFile);
            if (JsonResult.State == 0)
            {
                _DelFileCallBackFun?.Invoke(_UCFileInfoVM.CurFile);
                AppSettings.SetStateBarText(JsonResult.Msg);
            }
            else
            {
                (new WinMsgDialog(JsonResult.Msg, "消息")).ShowDialog();
            }
        }


    }


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
                else if (P_CurFile.UserId.Equals(AppSettings.LoginUser.Id, System.StringComparison.Ordinal))
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
