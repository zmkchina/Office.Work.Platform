using System.Windows.Controls;
using System.Windows.Input;
using Office.Work.Platform.AppCodes;

namespace Office.Work.Platform.Remuneration
{
    /// <summary>
    /// PageMenu.xaml 的交互逻辑
    /// </summary>
    public partial class PagePlayMenu : Page
    {
        public PagePlayMenu()
        {
            InitializeComponent();
            //AppSettings.AppMainWindow;
        }
        /// <summary>
        /// 打印待遇表格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PagePaySheet_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PageMemberPaySheet CurPage = new PageMemberPaySheet();
            AppSettings.AppMainWindow.FrameContentPage.Content = CurPage;
        }
       
        /// <summary>
        /// 快速发放（批量发放）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MemberPayFast_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PageMemberPayFast CurPage = new PageMemberPayFast();
            AppSettings.AppMainWindow.FrameContentPage.Content = CurPage;
        }
        /// <summary>
        /// 待遇项目设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_PayItem_Click(object sender, MouseButtonEventArgs e)
        {
            PageMemberPayItem CurPage = new PageMemberPayItem();
            AppSettings.AppMainWindow.FrameContentPage.Content = CurPage;
        }
        /// <summary>
        /// 发放待遇（个人明细）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MemberPay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PageMemberPay CurPage = new PageMemberPay();
            AppSettings.AppMainWindow.FrameContentPage.Content = CurPage;
        }
    }
}
