using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System;
using System.Net.Http.Handlers;
using System.Windows;
using System.Windows.Controls;

namespace Office.Work.Platform.Files
{
    /// <summary>
    /// UC_FileInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UC_FileInfo : UserControl
    {
        private UC_FileInfoVM _UC_FileInfoVM;
        private Action<ModelFile> _DelFileCallBackFun = null;
        public UC_FileInfo()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 构造函数 重载1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UCFileInfo_Loaded(object sender, RoutedEventArgs e)
        {
            _UC_FileInfoVM = new UC_FileInfoVM();
            DataContext = _UC_FileInfoVM;
        }
        /// <summary>
        /// 初始化视图模型属性值。
        /// </summary>
        /// <param name="P_File">待修改、删除的文件对象</param>
        /// <param name="P_DelFileCallBackFun">删除文件后回调的函数</param>
        public void Init_FileInfo(ModelFile P_File, ModelPlan P_Plan=null, Action<ModelFile> P_DelFileCallBackFun = null)
        {
            _UC_FileInfoVM = new UC_FileInfoVM();
            _UC_FileInfoVM.InitPropValus(P_File, P_Plan);
            _DelFileCallBackFun = P_DelFileCallBackFun;
            DataContext = _UC_FileInfoVM;
        }
        private async void btn_OpenFileAsync(object sender, RoutedEventArgs e)
        {
            Button V_Button = sender as Button;
            V_Button.IsEnabled = false;
            ProgressMessageHandler progress = new System.Net.Http.Handlers.ProgressMessageHandler();
            progress.HttpReceiveProgress += (object sender, System.Net.Http.Handlers.HttpProgressEventArgs e) =>
            {
                _UC_FileInfoVM.DownIntProgress = e.ProgressPercentage;// (double)(e.BytesTransferred / e.TotalBytes) * 100;
            };
            string theDownFileName = await DataFileRepository.DownloadFile(_UC_FileInfoVM.EntityFileInfo, false, progress);
            if (theDownFileName == null)
            {
                MessageBox.Show("文件下载失败,可能已被删除！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                V_Button.IsEnabled = true;
                return;
            }
            _UC_FileInfoVM.DownIntProgress = 100;
            DataFileRepository.OpenFileInfo(theDownFileName);
            V_Button.IsEnabled = true;
        }
        private async void btn_SaveChange(object sender, RoutedEventArgs e)
        {
            _UC_FileInfoVM.EntityFileInfo.ReadGrant = _UC_FileInfoVM.GetSelectUserIds();

            ModelResult JsonResult = await DataFileRepository.UpdateFileInfo(_UC_FileInfoVM.EntityFileInfo);
            if (JsonResult.State == 0)
            {
                AppSettings.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
                //MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async void btn_DeleFile(object sender, RoutedEventArgs e)
        {
            ModelResult JsonResult = await DataFileRepository.DeleteFileInfo(_UC_FileInfoVM.EntityFileInfo);
            if (JsonResult.State == 0)
            {
                if (_DelFileCallBackFun != null) _DelFileCallBackFun(_UC_FileInfoVM.EntityFileInfo);
                AppSettings.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
                //MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


    }
}
