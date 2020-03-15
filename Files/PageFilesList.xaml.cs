using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Files
{
    /// <summary>
    /// PageFilesList.xaml 的交互逻辑
    /// </summary>
    public partial class PageFilesList : Page
    {
        private PageFilesListVM _PageFilesListVM;
        private string _FileOwnerType;

        public PageFilesList(string FileOwnerType = null)
        {
            InitializeComponent();
            _FileOwnerType = FileOwnerType;
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_PageFilesListVM == null)
            {
                //设置查询条件类

                _PageFilesListVM = new PageFilesListVM();
                _PageFilesListVM.mSearchFile.OwnerType = _FileOwnerType;
                await _PageFilesListVM.GetFilesAsync();
                DataContext = _PageFilesListVM;
            }
        }
        private void btn_UploadFile_Click(object sender, RoutedEventArgs e)
        {
            WinUpLoadFile winUpLoadFile = new WinUpLoadFile((upFile) =>
            {
                _PageFilesListVM.EntityFiles.Add(upFile); LB_FileList.Items.Refresh();
            });
            winUpLoadFile.ShowDialog();
        }

        private void ListBox_FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UCFileInfo.Init_FileInfo((ModelFile)LB_FileList.SelectedItem, null, (DelFile) =>
             {
                 _PageFilesListVM.EntityFiles.Remove(DelFile);
             });
        }
        private async void btn_Refrash_Click(object sender, RoutedEventArgs e)
        {
            string SearchNoValue = tb_search.Text.Trim().Length > 0 ? tb_search.Text.Trim() : null;

            //设置查询条件类
            _PageFilesListVM.mSearchFile.KeysInMultiple = SearchNoValue;
            await _PageFilesListVM.GetFilesAsync();
            DataContext = _PageFilesListVM;
        }
    }
}
