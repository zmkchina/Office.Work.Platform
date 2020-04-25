using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System;
using System.Net.Http.Handlers;
using System.Windows;

namespace Office.Work.Platform.Files
{
    /// <summary>
    /// WinUpLoadFile.xaml 的交互逻辑
    /// </summary>
    public partial class WinUpLoadFile : Window
    {
        private WinUpLoadFileVM _WinUpLoadFileVM = null;
        private readonly Action<PlanFile> _CallBackFunc;

        public WinUpLoadFile(Action<PlanFile> P_CallBackFunc, string P_OnerId = "0000", string P_OwnerType = "普通文件", string P_ContentType = null)
        {
            InitializeComponent();
            _WinUpLoadFileVM = new WinUpLoadFileVM(P_OnerId, P_OwnerType, P_ContentType);
            _CallBackFunc = P_CallBackFunc;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.SizeToContent = SizeToContent.Height;
            DataContext = _WinUpLoadFileVM;
        }
        private void BtnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            // 在WPF中， OpenFileDialog位于Microsoft.Win32名称空间
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "工作文档|*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx;*.wps;*.pdf;*.jpg;*.jpeg;*.png;*.gif;|压缩文档|*.rar;*.zip|所有文件|*.*"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _WinUpLoadFileVM.UpFileInfo = new System.IO.FileInfo(dialog.FileName);
                    if (_WinUpLoadFileVM.UpFileInfo.Length > 1073741824)//1G
                    {
                        MessageBox.Show("文件大于1G,无法保存！");
                        return;
                    }
                    _WinUpLoadFileVM.EntityFile.Length = _WinUpLoadFileVM.UpFileInfo.Length;
                    _WinUpLoadFileVM.EntityFile.Name = _WinUpLoadFileVM.UpFileInfo.Name.Substring(0, _WinUpLoadFileVM.UpFileInfo.Name.LastIndexOf('.'));
                    _WinUpLoadFileVM.EntityFile.ExtendName = _WinUpLoadFileVM.UpFileInfo.Extension;
                    _WinUpLoadFileVM.EntityFile.Describe = "该文件制发人是： \r\n内容摘要（或关键字):";
                    _WinUpLoadFileVM.SelectFilePath = _WinUpLoadFileVM.UpFileInfo.FullName;

                }
                catch (Exception VE)
                {
                    MessageBox.Show("读取文件出错(正在使用？)！" + VE.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private async void BtnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            BtnUploadFile.IsEnabled = false;
            ExcuteResult JsonResult = new ExcuteResult();
          
            _WinUpLoadFileVM.EntityFile.ReadGrant = _WinUpLoadFileVM.GetSelectUserIds();

            if (!_WinUpLoadFileVM.EntityFile.ReadGrant.Contains(AppSettings.LoginUser.Id))
            {
                if (MessageBox.Show("你本人没有读取该文件的权限，确认？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    BtnUploadFile.IsEnabled = true;
                    return;
                }
            }
            _WinUpLoadFileVM.EntityFile.UpDateTime = DateTime.Now;

            //设定进度报告委托
            ProgressMessageHandler progress = new System.Net.Http.Handlers.ProgressMessageHandler();

            progress.HttpSendProgress += (object sender, System.Net.Http.Handlers.HttpProgressEventArgs e) =>
            {
                _WinUpLoadFileVM.UploadIntProgress = e.ProgressPercentage;
            };

            JsonResult = await DataPlanFileRepository.UpLoadFileInfo(_WinUpLoadFileVM.EntityFile, _WinUpLoadFileVM.UpFileInfo.OpenRead(), "UpLoadFile", _WinUpLoadFileVM.EntityFile.Name, progress);

            if (JsonResult.State == 0)
            {
                _CallBackFunc(_WinUpLoadFileVM.EntityFile);
                AppSettings.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
                //MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnUploadFile.IsEnabled = true;
            }
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
