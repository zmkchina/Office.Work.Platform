using System;
using System.IO;
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
            this.Owner = Application.Current.MainWindow;
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

            if (!_WinUpLoadFileVM.EntityFile.CanReadUserIds.Contains(AppSettings.LoginUser.Id))
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
}
