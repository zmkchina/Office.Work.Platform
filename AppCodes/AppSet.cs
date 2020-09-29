using System.Collections.Generic;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppCodes
{
    public static class AppSet
    {
        /// <summary>
        /// 系统所有用户对象列表
        /// </summary>
        public static List<Lib.UserDto> SysUsers { get; set; }
        /// <summary>
        /// 系统当前登陆的用户对象
        /// </summary>
        public static Lib.UserDto LoginUser { get; set; }
        /// <summary>
        /// 系统本地设置类对象
        /// </summary>
        public static SettingLocal LocalSetting { get; set; }
        /// <summary>
        /// 系统服务器端设置类的对象
        /// </summary>
        public static Lib.SettingServerDto ServerSetting { get; set; }
        /// <summary>
        /// 系统主窗口对象
        /// </summary>
        public static MainWindow AppMainWindow { get; set; }
        /// <summary>
        /// 用户是否已锁定本软件
        /// </summary>
        public static bool AppIsLocked { get; set; } = false;
        /// <summary>
        /// 存储本地设置的文件名称。存在主程序目录下
        /// </summary>
        public static string LocalSettingFileName = "LocalSettings.zgk";
    }
}
