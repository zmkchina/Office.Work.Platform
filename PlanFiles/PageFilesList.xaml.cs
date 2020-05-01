using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.PlanFiles
{
    /// <summary>
    /// PageFilesList.xaml 的交互逻辑
    /// </summary>
    public partial class PageFilesList : Page
    {
        private PageFilesListVM _PageFilesListVM;
        private PlanFileSearch _mSearchFile;

        public PageFilesList(string FilePlanType = null)
        {
            InitializeComponent();
            _PageFilesListVM = new PageFilesListVM();
            _mSearchFile = new PlanFileSearch();
            _mSearchFile.UserId = AppSettings.LoginUser.Id;
            _mSearchFile.ContentType = FilePlanType;
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //设置查询条件类
            await _PageFilesListVM.GetFilesAsync(_mSearchFile);
            DataContext = _PageFilesListVM;
        }

        private void ListBox_FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UCFileInfo.Init_FileInfo((PlanFile)LB_FileList.SelectedItem, (DelFile) =>
             {
                 _PageFilesListVM.PlanFiles.Remove(DelFile);
             });
        }
        private async void btn_Refrash_Click(object sender, RoutedEventArgs e)
        {
            string SearchNoValue = tb_search.Text.Trim().Length > 0 ? tb_search.Text.Trim() : null;
            _mSearchFile.SearchNameOrDesc= SearchNoValue;
            await _PageFilesListVM.GetFilesAsync(_mSearchFile);
            DataContext = _PageFilesListVM;
        }
    }
}
