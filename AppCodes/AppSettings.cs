using Office.Work.Platform.Lib;
using System.Collections.Generic;

namespace Office.Work.Platform.AppCodes
{
    public static class AppSettings
    {
        /// <summary>
        /// 系统所有用户对象列表
        /// </summary>
        public static List<User> SysUsers { get; set; }
        /// <summary>
        /// 系统当前登陆的用户对象
        /// </summary>
        public static User LoginUser { get; set; }
        /// <summary>
        /// 系统本地设置类对象
        /// </summary>
        public static SettingLocal LocalSetting { get; set; }
        /// <summary>
        /// 系统服务器端设置类的对象
        /// </summary>
        public static SettingServer ServerSetting { get; set; }
        /// <summary>
        /// 系统主窗口对象
        /// </summary>
        public static MainWindow AppMainWindow { get; set; }
        /// <summary>
        /// 存储本地设置的文件名称。存在主程序目录下
        /// </summary>
        public static string LocalSettingFileName = "LocalSettings.zgk";
        /// <summary>
        /// 存储需要升级的文件列表的文件名称。存在主程序目录下
        /// </summary>
        public static string LocalUpdateFileName = "LocalUpdate.zgk";
        /// <summary>
        /// 请求API的基础路径
        /// </summary>
        //public static string ApiUrlBase = @"http://Localhost/Api/";
        public static string ApiUrlBase = @"http://Localhost:5000/Api/";
        //public static string ApiUrlBase = @"http://172.16.0.9/Api/";

    }
}
