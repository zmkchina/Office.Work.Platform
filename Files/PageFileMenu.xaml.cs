using Office.Work.Platform.AppCodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Office.Work.Platform.Files
{
    /// <summary>
    /// PageFileMenu.xaml 的交互逻辑
    /// </summary>
    public partial class PageFileMenu : Page
    {
        private object _PageFilesListAll;
        private PageFilesList _PageFilesListPlan;
        private PageFilesList _PageFilesListNode;
        private PageFilesList _PageFilesListPerson;
        private PageFilesList _PageFilesListPay;
        private PageFilesList _PageFilesListOther;

        public PageFileMenu()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxItem_MouseLeftButtonUp_0(null, null);
        }
        /// <summary>
        /// 全部文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_0(object sender, MouseButtonEventArgs e)
        {
            _PageFilesListAll ??= new PageFilesList();
            AppSettings.AppMainWindow.FrameContentPage.Content = _PageFilesListAll;
        }
        /// <summary>
        /// 计划附件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            _PageFilesListPlan ??= new PageFilesList("计划附件");
            AppSettings.AppMainWindow.FrameContentPage.Content = _PageFilesListPlan;
        }
        /// <summary>
        /// 备忘附件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_2(object sender, MouseButtonEventArgs e)
        {
            _PageFilesListNode ??= new PageFilesList("备忘附件");
            AppSettings.AppMainWindow.FrameContentPage.Content = _PageFilesListNode;
        }
        /// <summary>
        /// 人事附件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_3(object sender, MouseButtonEventArgs e)
        {
            _PageFilesListPerson ??= new PageFilesList("人事附件");
            AppSettings.AppMainWindow.FrameContentPage.Content = _PageFilesListPerson;
        }
        /// <summary>
        /// 待遇附件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_4(object sender, MouseButtonEventArgs e)
        {
            _PageFilesListPay ??= new PageFilesList("待遇附件");
            AppSettings.AppMainWindow.FrameContentPage.Content = _PageFilesListPay;
        }
        /// <summary>
        /// 其他文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_5(object sender, MouseButtonEventArgs e)
        {
            _PageFilesListOther ??= new PageFilesList("其他文件");
            AppSettings.AppMainWindow.FrameContentPage.Content = _PageFilesListOther;
        }
    }
}
