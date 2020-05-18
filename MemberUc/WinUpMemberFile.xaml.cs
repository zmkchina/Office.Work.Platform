using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Handlers;
using System.Windows;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberUc
{
    /// <summary>
    /// WinUpLoadFile.xaml 的交互逻辑
    /// </summary>
    public partial class WinUpMemberFile : Window
    {
        private readonly Action<MemberFile> _CallBackFunc;
        private CurWinViewModel _CurWinViewModel = null;

        public WinUpMemberFile(Action<MemberFile> P_CallBackFunc, FileInfo P_FileInfo, string P_MemberId,string P_OwnerContentType)
        {
            this.Owner =AppSet.AppMainWindow;
            this.Height = 300;
            InitializeComponent();
            _CurWinViewModel = new CurWinViewModel(P_FileInfo, P_MemberId, P_OwnerContentType);
            _CallBackFunc = P_CallBackFunc;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _CurWinViewModel;
        }
        private async void BtnUploadFile_ClickAsync(object sender, RoutedEventArgs e)
        {
            ExcuteResult JsonResult = new ExcuteResult();
            if (_CurWinViewModel.EntityFile.ContentType == null)
            {
                 AppFuns.ShowMessage("请选择文件内容类型！", Caption: "警告");
                return;
            }
            _CurWinViewModel.EntityFile.CanReadUserIds = _CurWinViewModel.GetSelectUserIds();

            if (!_CurWinViewModel.EntityFile.CanReadUserIds.Contains(AppSet.LoginUser.Id))
            {
                if(!AppFuns.ShowMessage("你本人没有读取该文件的权限？", Caption: "确认", showYesNo: true))
                {
                    return;
                }
            }

            BtnUpFile.IsEnabled = false;
            ProgressMessageHandler progress = new ProgressMessageHandler();
            progress.HttpSendProgress += (object sender, HttpProgressEventArgs e) =>
            {
                _CurWinViewModel.EntityFile.UpIntProgress = e.ProgressPercentage;
            };

            JsonResult = await DataMemberFileRepository.UpLoadFileInfo(_CurWinViewModel.EntityFile, _CurWinViewModel.UpFileInfo.OpenRead(), "upfilekey", "upfilename", progress);
            if (JsonResult.State == 0)
            {
                _CurWinViewModel.EntityFile.Id = JsonResult.Tag;
                _CurWinViewModel.EntityFile.UpIntProgress = 100;
                _CurWinViewModel.EntityFile.DownIntProgress = 0;
                _CallBackFunc(_CurWinViewModel.EntityFile);
                this.Close();
            }
            else
            {
                 AppFuns.ShowMessage(JsonResult.Msg, Caption: "错误", isErr: true);
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

        /// <summary>
        /// 本窗口的视图模型
        /// </summary>
        private class CurWinViewModel : NotificationObject
        {
            private List<SelectObj<User>> _UserSelectList;
            private double _UploadIntProgress;

            public CurWinViewModel(FileInfo P_FileInfo, string P_MemberId, string P_OwnerContentType)
            {
                UpFileInfo = P_FileInfo;
                EntityFile = new MemberFile
                {
                    ContentType = P_OwnerContentType,
                    MemberId = P_MemberId,
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
            }
            public MemberFile EntityFile { get; set; }
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
}
