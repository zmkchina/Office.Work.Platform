using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Handlers;
using System.Windows;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.FileDocs
{
    /// <summary>
    /// WinUpLoadFile.xaml 的交互逻辑
    /// </summary>
    public partial class WinUpLoadFile : Window
    {
        private readonly Action<FileDoc> _CallBackFunc;
        private WinUpLoadFileVM _WinUpLoadFileVM = null;

        public WinUpLoadFile(Action<FileDoc> P_CallBackFunc, FileInfo P_FileInfo, string P_OwnerType, string P_OwnerId,string P_OwnerContentType)
        {
            this.Owner =AppSet.AppMainWindow;
            this.Height = 300;
            InitializeComponent();
            _WinUpLoadFileVM = new WinUpLoadFileVM(P_FileInfo, P_OwnerType, P_OwnerId, P_OwnerContentType);
            _CallBackFunc = P_CallBackFunc;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _WinUpLoadFileVM;
        }
        private async void BtnUploadFile_ClickAsync(object sender, RoutedEventArgs e)
        {
            ExcuteResult JsonResult = new ExcuteResult();
            if (_WinUpLoadFileVM.EntityFile.ContentType == null)
            {
                (new WinMsgDialog("请选择文件内容类型！", Caption: "警告")).ShowDialog();
                return;
            }
            _WinUpLoadFileVM.EntityFile.CanReadUserIds = _WinUpLoadFileVM.GetSelectUserIds();

            if (!_WinUpLoadFileVM.EntityFile.CanReadUserIds.Contains(AppSet.LoginUser.Id))
            {
                if(!(new WinMsgDialog("你本人没有读取该文件的权限？", Caption: "确认", showYesNo: true)).ShowDialog().Value)
                {
                    return;
                }
            }

            BtnUpFile.IsEnabled = false;
            ProgressMessageHandler progress = new ProgressMessageHandler();
            progress.HttpSendProgress += (object sender, HttpProgressEventArgs e) =>
            {
                _WinUpLoadFileVM.EntityFile.UpIntProgress = e.ProgressPercentage;
            };

            JsonResult = await DataFileDocRepository.UpLoadFileInfo(_WinUpLoadFileVM.EntityFile, _WinUpLoadFileVM.UpFileInfo.OpenRead(), "upfilekey", "upfilename", progress);
            if (JsonResult.State == 0)
            {
                _WinUpLoadFileVM.EntityFile.Id = JsonResult.Tag;
                _WinUpLoadFileVM.EntityFile.UpIntProgress = 100;
                _WinUpLoadFileVM.EntityFile.DownIntProgress = 0;
                _CallBackFunc(_WinUpLoadFileVM.EntityFile);
                this.Close();
            }
            else
            {
                (new WinMsgDialog(JsonResult.Msg, Caption: "错误", isErr: true)).ShowDialog();
                BtnUpFile.IsEnabled = true;
            }
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        /// <summary>
        /// 拖动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Dispatcher.BeginInvoke(new Action(() => this.DragMove()));
            }
        }
    }


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
                UserId = AppSet.LoginUser.Id,
                Length = UpFileInfo.Length,
                ExtendName = UpFileInfo.Extension,
                UpIntProgress = 0,
                DownIntProgress = 100,
                Pubdate = System.DateTime.Now,
                UpDateTime = System.DateTime.Now,
                Describe = "",
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
                FileContentTypes = AppSet.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
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
            foreach (User item in AppSet.SysUsers)
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
