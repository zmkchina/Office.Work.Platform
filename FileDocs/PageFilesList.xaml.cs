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
        private PageFilesListVM _PageFilesListVM;
        private FileDocSearch _mSearchFile;

        public PageFilesList(string FilePlanType = null)
        {
            InitializeComponent();
            _PageFilesListVM = new PageFilesListVM();
            _mSearchFile = new FileDocSearch();
            _mSearchFile.UserId = AppSet.LoginUser.Id;
            _mSearchFile.ContentType = FilePlanType;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            btn_Refrash_ClickAsync(null, null);

        }

        private void ListBox_FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UCFileInfo.Init_FileInfo((FileDoc)LB_FileList.SelectedItem, (DelFile) =>
             {
                 _PageFilesListVM.FileDocs.Remove(DelFile);
             });
        }
        private async void btn_Refrash_ClickAsync(object sender, RoutedEventArgs e)
        {
            string SearchNoValue = tb_search.Text.Trim().Length > 0 ? tb_search.Text.Trim() : null;
            _mSearchFile.SearchNameOrDesc = SearchNoValue;
            await _PageFilesListVM.GetFilesAsync(_mSearchFile);
            AppFuns.SetStateBarText($"共查询到 :{_PageFilesListVM.FileDocs.Count}条记录！");
            DataContext = _PageFilesListVM;
        }

        private void btn_UpLoadFile_Click(object sender, RoutedEventArgs e)
        {
            System.IO.FileInfo theFile = FileOperation.SelectFile();
            if (theFile != null)
            {
                WinUpLoadFile winUpLoadFile = new WinUpLoadFile(new Action<FileDoc>(newFile =>
                {
                    _PageFilesListVM.FileDocs.Add(newFile);
                }), theFile, "无所有者", "000", null);
                winUpLoadFile.ShowDialog();
            }
        }
    }



    public class PageFilesListVM : NotificationObject
    {

        public ObservableCollection<FileDoc> FileDocs { get; set; }
        public PageFilesListVM()
        {
            FileDocs = new ObservableCollection<FileDoc>();
        }
        public async Task GetFilesAsync(FileDocSearch mSearchFile)
        {
            FileDocs.Clear();
            var files = await DataFileDocRepository.ReadFiles(mSearchFile);
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
