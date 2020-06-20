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
            _CurPageViewModel.SearchCondition.PageIndex = 1;
            await _CurPageViewModel.GetFilesAsync();
            Col_UCFileInfo.Width = new GridLength(0);
            AppFuns.SetStateBarText($"共查询到 :{_CurPageViewModel.SearchCondition.RecordCount}条记录,每页{_CurPageViewModel.SearchCondition.PageSize}条，共{_CurPageViewModel.SearchCondition.PageCount}页！");
        }

        private void ListBox_FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LB_FileList.SelectedItem is Lib.PlanFile curFile)
            {
                if (Col_UCFileInfo.Width.Value == 0)
                {
                    Col_UCFileInfo.Width = new System.Windows.GridLength(2, System.Windows.GridUnitType.Star);
                    Col_FileList.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
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
            if (_CurPageViewModel.SearchCondition.PageIndex <= 1)
            {
                _CurPageViewModel.SearchCondition.PageIndex = _CurPageViewModel.SearchCondition.PageCount;
            }
            else
            {
                _CurPageViewModel.SearchCondition.PageIndex--;
            }
            await _CurPageViewModel.GetFilesAsync();
        }

        private async void btn_NextPage_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_CurPageViewModel.SearchCondition.PageIndex >= _CurPageViewModel.SearchCondition.PageCount)
            {
                _CurPageViewModel.SearchCondition.PageIndex = 1;
            }
            else
            {
                _CurPageViewModel.SearchCondition.PageIndex++;
            }
            await _CurPageViewModel.GetFilesAsync();
        }

        /// <summary>
        /// 该页面的视图类
        /// </summary>
        private class CurPageViewModel : NotificationObject
        {
            private string _CanVisible;

            public PlanFileSearch SearchCondition { get; set; }
            public ObservableCollection<Lib.PlanFile> PlanFiles { get; set; }
            public CurPageViewModel(string FilePlanType)
            {
                SearchCondition = new PlanFileSearch()
                {
                    PageIndex = 1,
                    PageSize=15,
                    UserId = AppSet.LoginUser.Id,
                    ContentType = FilePlanType

                };
                PlanFiles = new ObservableCollection<Lib.PlanFile>();
                CanVisible = "Collapsed";
            }
            public async Task GetFilesAsync()
            {
                PlanFiles.Clear();
                PlanFileSearchResult SearchResult = await DataPlanFileRepository.ReadFiles(SearchCondition);
                if (SearchResult == null) { return; }

                SearchCondition.RecordCount = SearchResult.SearchCondition.RecordCount;
                
                if (SearchResult.RecordList != null && SearchResult.RecordList.Count > 0)
                {
                    SearchResult.RecordList.ToList().ForEach(e =>
                    {
                        PlanFiles.Add(e);
                    });
                }
                if (SearchCondition.PageCount > 1)
                {
                    CanVisible = "Visible";
                }
                else
                {
                    CanVisible = "Collapsed";
                }
            }

            /// <summary>
            /// 上下页按钮是否可见
            /// </summary>
            public string CanVisible
            {
                get { return _CanVisible; }
                set { _CanVisible = value; RaisePropertyChanged(); }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("就绪。");
        }
    }
}
