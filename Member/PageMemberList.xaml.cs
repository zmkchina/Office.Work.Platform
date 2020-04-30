using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private MemberSearch mSearch;

        public PageMemberList()
        {
            InitializeComponent();
        }
        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            if (_PageMemberListVM == null)
            {
                _PageMemberListVM = new PageMemberListVM();
                mSearch = new MemberSearch()
                {
                    UnitName = AppSettings.LoginUser.UnitName
                };
                await SearchMember(mSearch);
                DataContext = _PageMemberListVM;
            }
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_Refrash_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_PageMemberListVM.FieldValue))
            {
                return;
            }
            if (!_PageMemberListVM.SearchInResult)
            {
                mSearch = new MemberSearch();
            }
            //设置查询条件
            PropertyInfo[] SourceAttris = mSearch.GetType().GetProperties();
            var tempObj = SourceAttris.Where(x => x.Name.Equals(_PageMemberListVM.FieldEnName, StringComparison.Ordinal)).FirstOrDefault();
            if (tempObj != null)
            {
                tempObj.SetValue(mSearch, _PageMemberListVM.FieldValue);
            }
            await SearchMember(mSearch);
        }
        /// <summary>
        /// 查询指定条件的记录
        /// </summary>
        /// <param name="mSearch"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task SearchMember(MemberSearch mSearch)
        {
            List<Lib.Member> MemberList = await DataMemberRepository.ReadMembers(mSearch);
            if (MemberList != null && MemberList.Count > 0)
            {
                _PageMemberListVM.EntityList.Clear();
                MemberList.ForEach(e => { _PageMemberListVM.EntityList.Add(e); });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (((System.Windows.FrameworkElement)sender).DataContext is Lib.Member CurMember)
            {
                PageEditMember pageEditMember = new PageEditMember(CurMember);
                AppSettings.AppMainWindow.FrameContentPage.Content = pageEditMember;
            }
        }
    }
}
