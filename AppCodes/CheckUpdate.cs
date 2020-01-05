using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Office.Work.Platform.AppCodes
{
    public static class CheckUpdate
    {
        public static async System.Threading.Tasks.Task CheckAsync()
        {
            //读取服务器端本系统程序的信息，以便确定是否需要更新。
            List<ModelUpdateFile> ServerUpdateFiles = await DataSystemRepository.GetServerUpdateFiles();
            DataRWLocalFileRepository.DeleLocalFile(AppSettings.LocalUpdateFileName);//删除本地含有需要更新文件名称的文件
                                                                                     //检查程序是否需要更新
            List<string> NeedUpdateFiles = new List<string>();
            if (ServerUpdateFiles != null && ServerUpdateFiles.Count > 0)
            {
                foreach (ModelUpdateFile item in ServerUpdateFiles)
                {
                    string localFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, item.FileName);
                    if (File.Exists(localFileName))
                    {
                        string FileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(localFileName).FileVersion;
                        if (FileVersion != item.Version)// || theFile.LastWriteTime != item.LastWriteTime)
                        {
                            NeedUpdateFiles.Add(item.FileName);
                        }
                    }
                    else
                    {
                        NeedUpdateFiles.Add(item.FileName);
                    }
                }
            }
            if (NeedUpdateFiles != null && NeedUpdateFiles.Count > 0)
            {
                DataRWLocalFileRepository.SaveObjToFile<List<string>>(NeedUpdateFiles, AppSettings.LocalUpdateFileName);
                if (File.Exists(AppSettings.LocalUpdateFileName))
                {
                    MessageBox.Show("系统有新的版本，本程序将自动关闭并运行更新程序！", "更新", MessageBoxButton.OK, MessageBoxImage.Information);
                    //说明有需要升级的文件，下面开展关闭本程序并启动升级程序。
                    //启动外部程式
                    Application.Current.Shutdown();

                    string updateProgram = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Office.Work.Platform.Update.exe");
                    if (File.Exists(updateProgram))
                    {
                        string V_UpdateProgram = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Office.Work.Platform.Update.exe");
                        System.Diagnostics.Process.Start(V_UpdateProgram);
                    }
                    else
                    {
                        MessageBox.Show("未找到更新程序,请与开发人员联系。", "更新", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
