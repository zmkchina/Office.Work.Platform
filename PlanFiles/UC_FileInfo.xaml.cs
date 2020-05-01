using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System;
using System.Net.Http.Handlers;
using System.Windows;
using System.Windows.Controls;

namespace Office.Work.Platform.PlanFiles
{
    /// <summary>
    /// UC_FileInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UC_FileInfo : UserControl
    {
        private UC_FileInfoVM _UC_FileInfoVM;
        private Action<PlanFile> _DelFileCallBackFun = null;
        public UC_FileInfo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化视图模型属性值。
        /// </summary>
        /// <param name="P_File">待修改、删除的文件对象</param>
        /// <param name="P_DelFileCallBackFun">删除文件后回调的函数</param>
        public void Init_FileInfo(PlanFile P_File, Action<PlanFile> P_DelFileCallBackFun = null)
        {
            _UC_FileInfoVM = new UC_FileInfoVM();
            _UC_FileInfoVM.InitPropValusAsync(P_File);
            _DelFileCallBackFun = P_DelFileCallBackFun;
            DataContext = _UC_FileInfoVM;
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
                _UC_FileInfoVM.CurPlanFile.DownIntProgress = e.ProgressPercentage;// (double)(e.BytesTransferred / e.TotalBytes) * 100;
            };
            string theDownFileName = await DataPlanFileRepository.DownloadFile(_UC_FileInfoVM.CurPlanFile, false, progress);
            if (theDownFileName == null)
            {
                (new WinMsgDialog("文件下载失败,可能已被删除！", "警告")).ShowDialog();
                V_Button.IsEnabled = true;
                return;
            }
            _UC_FileInfoVM.CurPlanFile.DownIntProgress = 100;
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
            ExcuteResult JsonResult = await DataPlanFileRepository.UpdateFileInfo(_UC_FileInfoVM.CurPlanFile);
            if (JsonResult.State == 0)
            {
                AppSettings.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
            }
               (new WinMsgDialog(JsonResult.Msg, "消息")).ShowDialog();
        }
        private async void btn_DeleFile(object sender, RoutedEventArgs e)
        {
            ExcuteResult JsonResult = await DataPlanFileRepository.DeleteFileInfo(_UC_FileInfoVM.CurPlanFile);
            if (JsonResult.State == 0)
            {
                if (_DelFileCallBackFun != null) _DelFileCallBackFun(_UC_FileInfoVM.CurPlanFile);
                AppSettings.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
            }
            else
            {
                (new WinMsgDialog(JsonResult.Msg, "消息")).ShowDialog();
            }
        }


    }
}
