using System;
using System.Linq;
using System.Net.Http.Handlers;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.PlanFile
{
    /// <summary>
    /// UC_FileInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UC_PlanFileInfo : UserControl
    {
        private CurUCViewModel _CurUCViewModel;
        private Action<Lib.PlanFile> _DelFileCallBackFun = null;
        public UC_PlanFileInfo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化视图模型属性值。
        /// </summary>
        /// <param name="P_File">待修改、删除的文件对象</param>
        /// <param name="P_DelFileCallBackFun">删除文件后回调的函数</param>
        public void Init_FileInfo(Lib.PlanFile P_File, Action<Lib.PlanFile> P_DelFileCallBackFun = null)
        {
            _CurUCViewModel = new CurUCViewModel();
            _CurUCViewModel.InitPropValus(P_File);
            _DelFileCallBackFun = P_DelFileCallBackFun;
            DataContext = _CurUCViewModel;
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
                _CurUCViewModel.CurFile.DownIntProgress = e.ProgressPercentage;// (double)(e.BytesTransferred / e.TotalBytes) * 100;
            };
            string theDownFileName = await DataPlanFileRepository.DownloadFile(_CurUCViewModel.CurFile, false, progress);
            if (theDownFileName == null)
            {
                AppFuns.ShowMessage("文件下载失败,可能已被删除！", "警告");
                V_Button.IsEnabled = true;
                return;
            }
            _CurUCViewModel.CurFile.DownIntProgress = 100;
            DataPlanFileRepository.OpenFileInfo(theDownFileName);
            V_Button.IsEnabled = true;
        }
        /// <summary>
        /// 更新文件信息（文件名及文件描述）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_SaveChange(object sender, RoutedEventArgs e)
        {
            ExcuteResult JsonResult = await DataPlanFileRepository.UpdateFileInfo(_CurUCViewModel.CurFile);
            if (JsonResult.State == 0)
            {
                AppFuns.SetStateBarText(JsonResult.Msg);
            }
            AppFuns.ShowMessage(JsonResult.Msg, "消息");
        }
        /// <summary>
        /// 删除该文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_DeleteFileAsync(object sender, RoutedEventArgs e)
        {
            if (AppFuns.ShowMessage($"确定要删除[{_CurUCViewModel.CurFile.Name}]文件吗？", Caption: "确认", showYesNo: true))
            {
                ExcuteResult JsonResult = await DataPlanFileRepository.DeleteFileInfo(_CurUCViewModel.CurFile);
                if (JsonResult.State == 0)
                {
                    _DelFileCallBackFun?.Invoke(_CurUCViewModel.CurFile);
                    AppFuns.SetStateBarText(JsonResult.Msg);
                }
                else
                {
                    AppFuns.ShowMessage(JsonResult.Msg, "消息");
                }
            }
        }

        private class CurUCViewModel : NotificationObject
        {
            private Lib.PlanFile _CurFile;
            /// <summary>
            /// 文件对象
            /// </summary>
            public Lib.PlanFile CurFile
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
            public string CanRead { get; set; }
            public string CanDelete { get; set; }
            public string CanUpdate { get; set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            public CurUCViewModel()
            {
                CanRead = CanDelete = CanUpdate = "Collapsed";
            }
            public void InitPropValus(Lib.PlanFile PFile)
            {
                if (PFile == null) return;
                CurFile = PFile;
                if (PFile.CanReadUserIds != null)
                {
                    CurFileHasGrantNames = string.Join(",", AppSet.SysUsers.Where(e => PFile.CanReadUserIds.Contains(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name)?.ToArray());
                }
                if (PFile.UserId != null)
                {
                    CurFileCreateUserName = string.Join(",", AppSet.SysUsers.Where(e => PFile.UserId.Equals(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name)?.ToArray());
                }

                if (AppSet.LoginUser.Post.Equals("管理员"))
                {
                    CanRead = CanUpdate = CanDelete = "Visible";
                    return;
                }
                if (CurFile.UserId.Equals(AppSet.LoginUser.Id, System.StringComparison.Ordinal))
                {
                    CanRead = CanDelete = CanUpdate = "Visible";
                    return;
                }

                if (CurFile.CanReadUserIds.Contains(AppSet.LoginUser.Id, System.StringComparison.Ordinal))
                {
                    CanRead = "Visible";
                }
            }
        }
    }
}
