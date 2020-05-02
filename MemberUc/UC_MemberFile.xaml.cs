using System;
using System.Net.Http.Handlers;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.FileDocs;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberUc
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
        public async System.Threading.Tasks.Task InitFileDatasAsync(Lib.Member PMember, string PContentType, bool ReadFlag = true)
        {
            await _UCMemberFileVM.Init_MemberFileVMAsync(PMember, PContentType, ReadFlag);
            DataContext = _UCMemberFileVM;

        }
        /// <summary>
        /// 查询文件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_search_ClickAsync(object sender, RoutedEventArgs e)
        {
            FileDocSearch mfsearch = new FileDocSearch()
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
        private  void BtnUpFile_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            System.IO.FileInfo theFile = FileOperation.SelectFile();
            if (theFile != null)
            {
                WinUpLoadFile winUpLoadFile = new WinUpLoadFile(new Action<FileDoc>(newFile =>
                {
                    _UCMemberFileVM.MFiles.Add(newFile);
                }), theFile, "人事附件", _UCMemberFileVM.CurMember.Id,_UCMemberFileVM.ContentType);

                winUpLoadFile.ShowDialog();
            }
        }

        /// <summary>
        /// 从服务器删除文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Image_Delete_MouseLeftButtonUpAsync(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FileDoc SelectFile = LB_FileList.SelectedItem as FileDoc;
            if ((new WinMsgDialog($"删除文件《{ SelectFile.Name }》？", Caption: "确认", showYesNo: true)).ShowDialog().Value)
            {
                return;
            }
            ExcuteResult delResult = await DataFileDocRepository.DeleteFileInfo(SelectFile);
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
            FileDoc SelectFile = curTextBlock.DataContext as FileDoc;
            if (SelectFile == null)
            {
                (new WinMsgDialog("未读到选取文件信息！", Caption: "错误", isErr: true)).ShowDialog();
                return;
            }
            ProgressMessageHandler progress = new ProgressMessageHandler();

            progress.HttpReceiveProgress += (object sender, HttpProgressEventArgs e) =>
            {
                SelectFile.DownIntProgress = e.ProgressPercentage;
            };

            string theDownFileName = await DataFileDocRepository.DownloadFile(SelectFile, false, progress);
            if (theDownFileName != null)
            {
                SelectFile.DownIntProgress = 100;
                FileOperation.UseDefaultAppOpenFile(theDownFileName);
            }
            else
            {
                (new WinMsgDialog("文件下载失败，可能该文件已被删除！", Caption: "警告")).ShowDialog();
            }
            curTextBlock.IsEnabled = true;
        }
    }
}
