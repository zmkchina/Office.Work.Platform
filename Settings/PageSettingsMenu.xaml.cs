﻿using Office.Work.Platform.AppCodes;
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
            ListBoxItem_MouseLeftButtonUp_0(null, null);
        }

        /// <summary>
        /// 系统参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_0(object sender, MouseButtonEventArgs e)
        {
            AppSettings.AppMainWindow.FrameContentPage.Content = new PageSettingsSys();
        }
        /// <summary>
        /// 个人中心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
        }
        /// <summary>
        /// 常用软件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_2(object sender, MouseButtonEventArgs e)
        {
        }
        /// <summary>
        /// 实用工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp_3(object sender, MouseButtonEventArgs e)
        {
            AppSettings.AppMainWindow.FrameContentPage.Content = new PageSettingsTools(null);
        }
    }
}