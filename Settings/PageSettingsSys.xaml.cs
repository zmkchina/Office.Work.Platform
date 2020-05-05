using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.FileDocs;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Settings
{
    /// <summary>
    /// WinUpLoadFile.xaml 的交互逻辑
    /// </summary>
    public partial class PageSettingsSys : Page
    {
        private PageSettingsSysVM _PageSettingsSysVM = null;
        public PageSettingsSys()
        {
            InitializeComponent();
        }
        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            _PageSettingsSysVM = new PageSettingsSysVM();
            await _PageSettingsSysVM.GetEntityInfoAsync();
            this.DataContext = _PageSettingsSysVM;
        }

        private void BtnUpdateSettings_Click(object sender, RoutedEventArgs e)
        {

        }
    }


    public class PageSettingsSysVM : NotificationObject
    {

        public string[] FileContentTypes => AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
        public SettingServer EntitySettingServer { get; set; }

        #region "方法"
        /// <summary>
        /// 构造函数
        /// </summary>
        public PageSettingsSysVM()
        {
        }
        public async Task GetEntityInfoAsync()
        {
            EntitySettingServer = await DataSystemRepository.ReadServerSettings();
        }
        #endregion
    }
}
