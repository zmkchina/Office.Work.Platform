using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System.Windows;
using System.Windows.Controls;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// PageFilesList.xaml 的交互逻辑
    /// </summary>
    public partial class PageMemberList : Page
    {
        private PageMemberListVM _PageMemberListVM;
        private string _UnitName;

        public PageMemberList(string P_UnitName = null)
        {
            InitializeComponent();
            _UnitName = P_UnitName;
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_PageMemberListVM == null)
            {
                _PageMemberListVM = new PageMemberListVM();
                //设置查询条件类
                _PageMemberListVM.mSearchMember.UnitName = _UnitName;

                _PageMemberListVM.EntityList = await DataMemberRepository.ReadMembers(_PageMemberListVM.mSearchMember);
                DataContext = _PageMemberListVM;
            }
        }
        private void ListBox_FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //FrameFileInfo.Content = new PageFileInfo(this.LB_FileList.SelectedItem as ModelFile, (theFile) =>
            //{
            //    _PageMemberListVM.EntityFiles.Remove(theFile);
            //    LB_FileList.Items.Refresh();自动更新，不需要刷新
            //});
        }
        private async void btn_Refrash_Click(object sender, RoutedEventArgs e)
        {
            string SearchNoValue = tb_search.Text.Trim().Length > 0 ? tb_search.Text.Trim() : null;
            //设置查询条件
            _PageMemberListVM.mSearchMember.KeysInMultiple = SearchNoValue;

            _PageMemberListVM.EntityList = await DataMemberRepository.ReadMembers(_PageMemberListVM.mSearchMember);
            DataContext = _PageMemberListVM;
        }
    }
}
