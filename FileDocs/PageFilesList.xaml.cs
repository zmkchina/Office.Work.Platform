using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.FileDocs
{
    /// <summary>
    /// PageFilesList.xaml 的交互逻辑
    /// </summary>
    public partial class PageFilesList : Page
    {
        private CurPageViewModel _CurPageViewModel;

        public PageFilesList(string FilePlanType = null)
        {
            InitializeComponent();
            _CurPageViewModel = new CurPageViewModel(FilePlanType);

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _CurPageViewModel;
            btn_Refrash_ClickAsync(null, null);
        }

        private async void btn_Refrash_ClickAsync(object sender, RoutedEventArgs e)
        {
            await _CurPageViewModel.GetFilesAsync();
            Col_UCFileInfo.Width = new GridLength(0);
            AppFuns.SetStateBarText($"共查询到 :{_CurPageViewModel.FileDocs.Count}条记录！");
        }

        private void ListBox_FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LB_FileList.SelectedItem is FileDoc curFile)
            {
                if (Col_UCFileInfo.Width.Value == 0)
                {
                    Col_UCFileInfo.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
                    Col_FileList.Width = new System.Windows.GridLength(2, System.Windows.GridUnitType.Star);
                }
                this.UCFileInfo.Init_FileInfo(curFile, (DelFile) =>
                {
                    Col_UCFileInfo.Width = new GridLength(0, System.Windows.GridUnitType.Star);
                    _CurPageViewModel.FileDocs.Remove(DelFile);
                });
            }
        }

        private void btn_UpLoadFile_Click(object sender, RoutedEventArgs e)
        {
            System.IO.FileInfo theFile = FileOperation.SelectFile();
            if (theFile != null)
            {
                WinUpLoadFile winUpLoadFile = new WinUpLoadFile(new Action<FileDoc>(newFile =>
                {
                    _CurPageViewModel.FileDocs.Add(newFile);
                }), theFile, "无所有者", "000", null);
                winUpLoadFile.ShowDialog();
            }
        }


        /// <summary>
        /// 该页面的视图类
        /// </summary>
        private class CurPageViewModel : NotificationObject
        {
            public FileDocSearch SearchFile { get; set; }
            public ObservableCollection<FileDoc> FileDocs { get; set; }
            public CurPageViewModel(string FilePlanType)
            {
                SearchFile = new FileDocSearch()
                {
                    UserId = AppSet.LoginUser.Id,
                    ContentType = FilePlanType
                };
                FileDocs = new ObservableCollection<FileDoc>();
            }
            public async Task GetFilesAsync()
            {
                FileDocs.Clear();
                var files = await DataFileDocRepository.ReadFiles(SearchFile);
                if (files != null)
                {
                    files.ToList().ForEach(e =>
                    {
                        FileDocs.Add(e);
                    });
                }
            }
        }
    }
}
