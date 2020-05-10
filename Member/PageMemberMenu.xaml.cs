using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Office.Work.Platform.AppCodes;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// PagePersonMenu.xaml 的交互逻辑
    /// </summary>
    public partial class PageMemberMenu : Page
    {
        public PageMemberMenu()
        {
            InitializeComponent();
            //AppSettings.AppMainWindow;
        }

        private void ListBoxItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }
        //新增/编辑员工
        private void ListBoxItem_MouseLeftButtonUp_0(object sender, MouseButtonEventArgs e)
        {
            PageEditMember pageEditMember = new PageEditMember(null);
            AppSet.AppMainWindow.FrameContentPage.Content = pageEditMember;
        }
        //员工列表
        private void ListBoxItem_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            PageMemberList pageMemberList = new PageMemberList();
            AppSet.AppMainWindow.FrameContentPage.Content = pageMemberList;
        }
        //批量新增
        private void ListBoxItem_MouseLeftButtonUp_2(object sender, MouseButtonEventArgs e)
        {
            PageAddMembers pageAddMembers = new PageAddMembers();
            AppSet.AppMainWindow.FrameContentPage.Content = pageAddMembers;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxItem_MouseLeftButtonUp_1(null, null);
        }

    }
}
