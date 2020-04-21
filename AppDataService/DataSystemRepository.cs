using Microsoft.Extensions.Logging;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Office.Work.Platform.AppDataService
{
    public static class DataSystemRepository
    {
        
        /// <summary>
        /// 读取所有用户
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ModelUser>> ReadAllSysUsers()
        {
            List<ModelUser> SysUsers = await DataApiRepository.GetApiUri<List<ModelUser>>(AppSettings.ApiUrlBase + "User").ConfigureAwait(false);
            if (SysUsers == null)
            {
                MessageBox.Show("从服务器读取系统用户列表时出错，请检查是否连接上网。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return SysUsers;
        }
        /// <summary>
        /// 从服务器读取系统设置
        /// </summary>
        /// <returns></returns>
        public static async Task<ModelSettingServer> ReadServerSettings()
        {
            ModelSettingServer ServerSetting = await DataApiRepository.GetApiUri<ModelSettingServer>(AppSettings.ApiUrlBase + "Settings").ConfigureAwait(false);
            if (ServerSetting == null)
            {
                MessageBox.Show("从服务器读取系统设置时出现错误，请检查是否连接上网。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return ServerSetting;
        }
        /// <summary>
        /// 更新服务器系统设置
        /// </summary>
        /// <returns></returns>
        public static async Task<ModelSettingServer> UpdateServerSettings()
        {
            ModelSettingServer ServerSetting = await DataApiRepository.GetApiUri<ModelSettingServer>(AppSettings.ApiUrlBase + "Settings").ConfigureAwait(false);
            if (ServerSetting == null)
            {
                MessageBox.Show("从服务器读取系统设置时出现错误，请检查是否连接上网。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return ServerSetting;
        }
        /// <summary>
        /// 从服务器读取系统设置
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ModelUpdateFile>> GetServerUpdateFiles()
        {
            List<ModelUpdateFile> updateFiles = await DataApiRepository.GetApiUri<List<ModelUpdateFile>>(AppSettings.ApiUrlBase + "UpdateFile").ConfigureAwait(false);

            if (updateFiles == null)
            {
               // MessageBox.Show("从服务器读取升级文件时出错，请检查是否连接上网。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return updateFiles;
        }
    }
}
