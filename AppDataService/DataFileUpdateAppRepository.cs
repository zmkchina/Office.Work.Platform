using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Threading.Tasks;

namespace Office.Work.Platform.AppDataService
{
    public static class DataFileUpdateAppRepository
    {
        private static string _ApiUrlBase = AppSet.LocalSetting.ResApiUrl;
        /// <summary>
        /// 从服务器读取本程序最新升级信息
        /// </summary>
        /// <returns></returns>
        public static async Task<AppUpdateInfo> GetAppUpdateInfo()
        {
            AppUpdateInfo UpdateInfo = await DataApiRepository.GetApiUri<AppUpdateInfo>(_ApiUrlBase + "UpdateFile/GetUpdateInfo").ConfigureAwait(false);
            return UpdateInfo;
        }
        public static async Task<string> DownLoadNewVerFile(string PFileName, string TargetDir, ProgressMessageHandler showDownProgress = null)
        {
            //合成目录+文件
            string tempFilePath = System.IO.Path.Combine(TargetDir, PFileName);

            HttpResponseMessage httpResponseMessage = await DataApiRepository.GetApiUri<HttpResponseMessage>(_ApiUrlBase + @"UpdateFile/DownFile/" + PFileName, showDownProgress).ConfigureAwait(false);
            if (httpResponseMessage != null && httpResponseMessage.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                Stream responseStream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
                //以上两句代码不能用下一句代替，否则进度报告出现卡顿。
                //Stream responseStream = await DataApiRepository.GetApiUri<Stream>(AppSettings.ApiUrlBase + @"FileDown/" + WillDownFile.Id, showDownProgress);
                if (responseStream != null && responseStream.Length > 0)
                {
                    //创建一个文件流
                    FileStream fileStream = new FileStream(tempFilePath, FileMode.Create);
                    //await responseStream.CopyToAsync(fileStream);
                    byte[] buffer = new byte[2048];
                    int readLength;
                    while ((readLength = await responseStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        // 写入到文件
                        fileStream.Write(buffer, 0, readLength);
                    }
                    responseStream.Close();
                    fileStream.Close();
                }
            }
            if (File.Exists(tempFilePath))
            {
                return PFileName;
            }
            else
            {
                return null;
            }
        }
    }
}
