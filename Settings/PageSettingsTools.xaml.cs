using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.PlanFiles;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Settings
{
    /// <summary>
    /// WinUpLoadFile.xaml 的交互逻辑
    /// </summary>
    public partial class PageSettingsTools : Page
    {
        public PageSettingsTools(Lib.Plan P_Plan = null)
        {
            InitializeComponent();
        }
        private void BtnCloseScreen_Click(object sender, RoutedEventArgs e)
        {
            CloseScreen.Close();
        }

        private void BtnLockApp_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
