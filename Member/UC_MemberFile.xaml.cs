using System;
using System.Net.Http.Handlers;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// UC_BasicInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UC_MemberFile : UserControl
    {
        private UC_MemberFileVM _UCMemberFileVM;
        public UC_MemberFile()
        {
            InitializeComponent();
            _UCMemberFileVM = new UC_MemberFileVM();
        }
        public void InitFileDatasAsync(string MemberId, string FileType, string OtherRecordId = null, bool ReadFlag = true)
        {
            _UCMemberFileVM.Init_MemberFileVMAsync(MemberId, FileType, OtherRecordId, ReadFlag);
            DataContext = _UCMemberFileVM;

        }
        /// <summary>
        /// 查询文件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_search_ClickAsync(object sender, RoutedEventArgs e)
        {
            MemberFileSearch mfsearch = new MemberFileSearch()
            {
                SearchNameOrDesc = _UCMemberFileVM.SearchValues,
            };
            await _UCMemberFileVM.SearchMemberFiles(mfsearch);
        }
        /// <summary>
        /// 上传文件到服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnUpFile_ClickAsync(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            System.IO.FileInfo theFile = FileOperation.SelectFile();
            if (theFile != null)
            {
                MemberFile newFile = new MemberFile()
                {
                    Id = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    Name = theFile.Name.Substring(0, theFile.Name.LastIndexOf('.')),
                    UserId = AppSettings.LoginUser.Id,
                    Length = theFile.Length,
                    ExtendName = theFile.Extension,
                    MemberId = _UCMemberFileVM.MemberId,
                    FileInfo = theFile,
                    FileType = _UCMemberFileVM.FileType,
                    UpIntProgress = 0
                };
                ProgressMessageHandler UpProgress = new ProgressMessageHandler();
                UpProgress.HttpSendProgress += (object sender, HttpProgressEventArgs e) =>
                {
                    newFile.UpIntProgress = e.ProgressPercentage;
                };
                ExcuteResult result = await DataMemberFileRepository.UpLoadFileInfo(newFile, newFile.FileInfo.OpenRead(), "memberfile", "mf", UpProgress);
                if (result == null || result.State != 0)
                {
                    newFile.UpIntProgress = 0;
                }
                if (result.State == 0)
                {
                    _UCMemberFileVM.MFiles.Add(newFile);
                }
            }
        }

        /// <summary>
        /// 从服务器删除文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Image_Delete_MouseLeftButtonUpAsync(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MemberFile SelectFile = LB_FileList.SelectedItem as MemberFile;
            if (MessageBox.Show("删除文件《" + SelectFile.Name + "》？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            ExcuteResult delResult = await DataMemberFileRepository.DeleteFileInfo(SelectFile);
            if (delResult != null && delResult.State == 0)
            {
                _UCMemberFileVM.MFiles.Remove(SelectFile);
            }
        }
        /// <summary>
        /// 打开选定的文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OpenFile_PreviewMouseLeftButtonUpAsync(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            TextBlock curTextBlock = sender as TextBlock;
            MemberFile SelectFile = curTextBlock.DataContext as MemberFile;
            if (SelectFile == null)
            {
                MessageBox.Show("未读到选取文件信息！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            ProgressMessageHandler progress = new ProgressMessageHandler();

            progress.HttpReceiveProgress += (object sender, HttpProgressEventArgs e) =>
            {
                SelectFile.DownIntProgress = e.ProgressPercentage;
            };

            string theDownFileName = await DataMemberFileRepository.DownloadFile(SelectFile, false, progress);
            if (theDownFileName != null)
            {
                SelectFile.DownIntProgress = 100;
                FileOperation.UseDefaultAppOpenFile(theDownFileName);
            }
            else
            {
                MessageBox.Show("文件下载失败，可能该文件已被删除！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            curTextBlock.IsEnabled = true;
        }
    }
}
