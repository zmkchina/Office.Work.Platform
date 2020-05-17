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
            AppFuns.SetStateBarText($"共查询到 :{_CurPageViewModel.PlanFiles.Count}条记录！");
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


        /// <summary>
        /// 该页面的视图类
        /// </summary>
        private class CurPageViewModel : NotificationObject
        {
            public PlanFileSearch SearchFile { get; set; }
            public ObservableCollection<Lib.PlanFile> PlanFiles { get; set; }
            public CurPageViewModel(string FilePlanType)
            {
                SearchFile = new PlanFileSearch()
                {
                    UserId = AppSet.LoginUser.Id,
                    ContentType = FilePlanType
                };
                PlanFiles = new ObservableCollection<Lib.PlanFile>();
            }
            public async Task GetFilesAsync()
            {
                PlanFiles.Clear();
                var files = await DataPlanFileRepository.ReadFiles(SearchFile);
                if (files != null)
                {
                    files.ToList().ForEach(e =>
                    {
                        PlanFiles.Add(e);
                    });
                }
            }
        }
    }
}
