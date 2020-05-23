using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Office.Work.Platform.PlanFile
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
            AppFuns.SetStateBarText($"共查询到 :{_CurPageViewModel.SearchPlanFile.RecordCount}条记录,每页{_CurPageViewModel.SearchPlanFile.PageSize}条，共{_CurPageViewModel.SearchPlanFile.PageCount}页！");
        }

        private void ListBox_FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LB_FileList.SelectedItem is Lib.PlanFile curFile)
            {
                if (Col_UCFileInfo.Width.Value == 0)
                {
                    Col_UCFileInfo.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
                    Col_FileList.Width = new System.Windows.GridLength(2, System.Windows.GridUnitType.Star);
                }
                this.UCFileInfo.Init_FileInfo(curFile, (DelFile) =>
                {
                    Col_UCFileInfo.Width = new GridLength(0, System.Windows.GridUnitType.Star);
                    _CurPageViewModel.PlanFiles.Remove(DelFile);
                });
            }
        }

        private async void btn_PrevPage_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_CurPageViewModel.SearchPlanFile.PageIndex <= 1)
            {
                _CurPageViewModel.SearchPlanFile.PageIndex = _CurPageViewModel.SearchPlanFile.PageCount;
            }
            else
            {
                _CurPageViewModel.SearchPlanFile.PageIndex--;
            }
            await _CurPageViewModel.GetFilesAsync();
        }

        private async void btn_NextPage_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_CurPageViewModel.SearchPlanFile.PageIndex >= _CurPageViewModel.SearchPlanFile.PageCount)
            {
                _CurPageViewModel.SearchPlanFile.PageIndex = 1;
            }
            else
            {
                _CurPageViewModel.SearchPlanFile.PageIndex++;
            }
            await _CurPageViewModel.GetFilesAsync();
        }

        /// <summary>
        /// 该页面的视图类
        /// </summary>
        private class CurPageViewModel : NotificationObject
        {
            public PlanFileSearch SearchPlanFile { get; set; }
            public ObservableCollection<Lib.PlanFile> PlanFiles { get; set; }
            public CurPageViewModel(string FilePlanType)
            {
                SearchPlanFile = new PlanFileSearch()
                {
                    PageIndex = 1,
                    PageSize=15,
                    UserId = AppSet.LoginUser.Id,
                    ContentType = FilePlanType

                };
                PlanFiles = new ObservableCollection<Lib.PlanFile>();
            }
            public async Task GetFilesAsync()
            {
                PlanFiles.Clear();
                PlanFileSearchResult SearchResult = await DataPlanFileRepository.ReadFiles(SearchPlanFile);
                if (SearchResult == null) { return; }
                
                SearchPlanFile.RecordCount = SearchResult.SearchCondition.RecordCount;
                
                if (SearchResult.RecordList != null && SearchResult.RecordList.Count > 0)
                {
                    SearchResult.RecordList.ToList().ForEach(e =>
                    {
                        PlanFiles.Add(e);
                    });
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("就绪。");
        }
    }
}
