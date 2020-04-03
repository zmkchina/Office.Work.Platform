using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Files;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Settings
{
    /// <summary>
    /// WinUpLoadFile.xaml 的交互逻辑
    /// </summary>
    public partial class PageSettingsTools : Page
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        private const uint WM_SYSCOMMAND = 0x0112;
        private const int SC_MONITORPOWER = 0xf170;
        public PageSettingsTools(ModelPlan P_Plan = null)
        {
            InitializeComponent();
        }
        private void BtnCloseScreen_Click(object sender, RoutedEventArgs e)
        {
            _ = SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, 2);
        }
    }
}
