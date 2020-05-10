using Office.Work.Platform.AppCodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Office.Work.Platform.Plan
{
    /// <summary>
    /// PagePlanMenu.xaml 的交互逻辑
    /// </summary>
    public partial class PagePlanMenu : Page
    {
        public PagePlanMenu()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxItem_MouseLeftButtonUp_0(null, null);
        }

        /// <summary>
        /// 新增计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_0(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PageEditPlan(null);
        }
        /// <summary>
        /// 我的计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PagePlansList("MyNoFinishPlans");
        }
        /// <summary>
        /// 全部待办
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_2(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PagePlansList("AllNoFinishPlans"); ;
        }
        /// <summary>
        /// 完结计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_3(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PagePlansList("AllFinihPlans"); ;
        }
        /// <summary>
        /// 全部计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_4(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PagePlansList("AllPlans"); ;
        }
    }
}
