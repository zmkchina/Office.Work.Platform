using System.Windows.Controls;
using System.Windows.Input;
using Office.Work.Platform.AppCodes;

namespace Office.Work.Platform.Node
{
    /// <summary>
    /// PageNodeMenu.xaml 的交互逻辑
    /// </summary>
    public partial class PageNodeMenu : Page
    {
        public PageNodeMenu()
        {
            InitializeComponent();
            //AppSettings.AppMainWindow 
        }
        /// <summary>
        /// 新增备注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_0(object sender, MouseButtonEventArgs e)
        {
            PageEditNode pageEditMember = new PageEditNode(null);
            AppSettings.AppMainWindow.FrameContentPage.Content = pageEditMember;
        }

        private void ListBoxItem_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void ListBoxItem_MouseLeftButtonUp_2(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
