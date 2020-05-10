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
        public static async System.Threading.Tasks.Task<bool> CheckAppUpdateAsync()
        {
            //1. 删除本地含有需要更新文件名称的文件
            if (System.IO.File.Exists(AppSet.LocalUpdateFileName))
            {
                DataRWLocalFileRepository.DeleLocalFile(AppSet.LocalUpdateFileName);
            }

            //2. 读取服务器端本系统程序的信息。
            List<UpdateFile> ServerUpdateFiles = await DataSystemRepository.GetServerUpdateFiles();

            //3. 与本地程序文件比列，以便确定是否需要更新。
            if (ServerUpdateFiles != null && ServerUpdateFiles.Count > 0)
            {
                List<string> NeedUpdateFiles = new List<string>();
                foreach (UpdateFile item in ServerUpdateFiles)
                {
                    string localFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, item.FileName);
                    if (File.Exists(localFileName))
                    {
                        //本地有同名文件，比较其版本号差异
                        string FileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(localFileName).FileVersion;
                        if (FileVersion != item.Version)// || theFile.LastWriteTime != item.LastWriteTime)
                        {
                            NeedUpdateFiles.Add(item.FileName);
                        }
                    }
                    else
                    {
                        //本地无同名文件，说明是新文件需要下载。
                        NeedUpdateFiles.Add(item.FileName);
                    }
                }
                //4. 将需要下载升级的文件名写入本地文件中，以便升级程序读取之。
                if (NeedUpdateFiles != null && NeedUpdateFiles.Count > 0)
                {
                    DataRWLocalFileRepository.SaveObjToFile<List<string>>(NeedUpdateFiles, AppSet.LocalUpdateFileName);
                    if (File.Exists(AppSet.LocalUpdateFileName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
