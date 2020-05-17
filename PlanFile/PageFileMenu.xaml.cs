using Office.Work.Platform.AppCodes;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Office.Work.Platform.PlanFile
{
    /// <summary>
    /// PageFileMenu.xaml 的交互逻辑
    /// </summary>
    public partial class PageFileMenu : Page
    {
        public PageFileMenu()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxItem_MouseLeftButtonUp_0(null, null);

            List<string> MenuList = AppSet.ServerSetting.GetWorkContentTypes();
            foreach (string item in MenuList)
            {
                ListBoxItem tempItem = new ListBoxItem();
                tempItem.Tag = item;
                tempItem.MouseLeftButtonUp += TempItem_MouseLeftButtonUp;
                StackPanel tempPanel = new StackPanel();
                tempPanel.Orientation = Orientation.Horizontal;
                TextBlock tempBlock1 = new TextBlock();
                tempBlock1.Style = (Style)Application.Current.Resources["FIcon"];
                tempBlock1.Text = "\xe662";
                tempBlock1.FontSize = 18;
                tempBlock1.Margin = new Thickness(0, 0, 5, 0);
                tempBlock1.Foreground = new SolidColorBrush(Colors.Green);
                TextBlock tempBlock2 = new TextBlock();
                tempBlock2.Text = item;
                tempBlock2.VerticalAlignment = VerticalAlignment.Center;
                tempPanel.Children.Add(tempBlock1);
                tempPanel.Children.Add(tempBlock2);
                tempItem.Content = tempPanel;
                LbMenuList.Items.Add(tempItem);
            }
        }

        private void TempItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem tempItem)
            {
                string contentType = tempItem.Tag as string;
                AppSet.AppMainWindow.FrameContentPage.Content = new PageFilesList(contentType);
            }
        }

        /// <summary>
        /// 全部文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_0(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PageFilesList(null);
        }
    }
}
