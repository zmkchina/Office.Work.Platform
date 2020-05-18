using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Handlers;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.PlanFile;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberUc
{
    /// <summary>
    /// UC_BasicInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UC_MemberFile : UserControl
    {
        private CurUcViewModel _CurUcViewModel;
        public UC_MemberFile()
        {
            InitializeComponent();
            _CurUcViewModel = new CurUcViewModel();
        }
        public async System.Threading.Tasks.Task InitFileDatasAsync(string PMemberId, string PContentType, bool ReadFlag = true)
        {
            await _CurUcViewModel.Init_MemberFileVMAsync(PMemberId, PContentType, ReadFlag);
            DataContext = _CurUcViewModel;

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
                UserId = AppSet.LoginUser.Id,
                MemberId=_CurUcViewModel.MemberId,
                ContentType= _CurUcViewModel.ContentType,
                SearchNameOrDesc = _CurUcViewModel.SearchValues,
            };
            await _CurUcViewModel.SearchMemberFiles(mfsearch);
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
                WinUpMemberFile winUpLoadFile = new WinUpMemberFile(new Action<Lib.MemberFile>(newFile =>
                {
                    _CurUcViewModel.MFiles.Add(newFile);
                }), theFile, _CurUcViewModel.MemberId, _CurUcViewModel.ContentType);

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
            Lib.MemberFile SelectFile = LB_FileList.SelectedItem as Lib.MemberFile;
            if (! AppFuns.ShowMessage($"删除文件《{ SelectFile.Name }》？", Caption: "确认", showYesNo: true))
            {
                return;
            }
            ExcuteResult delResult = await DataMemberFileRepository.DeleteFileInfo(SelectFile);
            if (delResult != null && delResult.State == 0)
            {
                _CurUcViewModel.MFiles.Remove(SelectFile);
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
            Lib.MemberFile SelectFile = curTextBlock.DataContext as Lib.MemberFile;
            if (SelectFile == null)
            {
                AppFuns.ShowMessage("未读到选取文件信息！", Caption: "错误", isErr: true);
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
                 AppFuns.ShowMessage("文件下载失败，可能该文件已被删除！", Caption: "警告");
            }
            curTextBlock.IsEnabled = true;
        }

        /// <summary>
        /// 本控件的视图类
        /// </summary>
        private class CurUcViewModel : NotificationObject
        {
            public CurUcViewModel()
            {
                MFiles = new ObservableCollection<Lib.MemberFile>();
            }
            public async System.Threading.Tasks.Task Init_MemberFileVMAsync(string PMemberId, string PContentType, bool ReadFlag = true)
            {
                MemberId = PMemberId;
                ContentType = PContentType;
                if (!ReadFlag) { return; }
                MemberFileSearch mfsearch = new MemberFileSearch()
                {
                    MemberId = MemberId,
                    ContentType = PContentType,
                    UserId = AppSet.LoginUser.Id
                };
                await SearchMemberFiles(mfsearch);
            }
            public async System.Threading.Tasks.Task SearchMemberFiles(MemberFileSearch mfsearch)
            {
                if (mfsearch != null)
                {
                    mfsearch.UserId = AppSet.LoginUser.Id;

                    IEnumerable<Lib.MemberFile> MemberPayTemps = await DataMemberFileRepository.ReadFiles(mfsearch);
                    MFiles.Clear();
                    MemberPayTemps?.ToList().ForEach(e =>
                    {
                        MFiles.Add(e);
                    });
                }
            }
            /// <summary>
            /// 当前所选信息
            /// </summary>
            public string MemberId { get; set; }
            public string ContentType { get; set; }

            //定义查询内容字符串
            public string SearchValues { get; set; }
            public ObservableCollection<Lib.MemberFile> MFiles { get; set; }

        }
    }
}
