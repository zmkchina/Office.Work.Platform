using Office.Work.Platform.AppCodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Office.Work.Platform.Settings
{
    /// <summary>
    /// PagePlanMenu.xaml 的交互逻辑
    /// </summary>
    public partial class PageSettingsMenu : Page
    {
        public PageSettingsMenu()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxItem_SetTheme_MouseLeftButtonUp(null, null);
        }

        /// <summary>
        /// 系统参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_0(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PageSettingsSys();
        }

        /// <summary>
        /// 实用工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_3(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PageSettingsTools(null);
        }

        /// <summary>
        /// 设置主题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_SetTheme_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PageSettingsTheme();
        }

        /// <summary>
        /// 个人中心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_PersonCenter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PagePersonCenter();
        }
        /// <summary>
        /// 版权信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_About_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PageAboutApp(); 
        }

        /// <summary>
        /// 用户权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_UserGrants_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PageSettingsGrant();
        }
    }
}
