using Office.Work.Platform.AppCodes;
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

        public static async Task<string> DownLoadNewVerFile(string PFileName, ProgressMessageHandler showDownProgress = null)
        {
            //合成目录
            string tempFileDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateApp");
            //合成目录+文件
            string tempFilePath = System.IO.Path.Combine(tempFileDir, PFileName);

            HttpResponseMessage httpResponseMessage = await DataApiRepository.GetApiUri<HttpResponseMessage>(_ApiUrlBase + @"UpdateFile/DownFile/" + PFileName, showDownProgress).ConfigureAwait(false);
            if (httpResponseMessage != null && httpResponseMessage.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                Stream responseStream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
                //以上两句代码不能用下一句代替，否则进度报告出现卡顿。
                //Stream responseStream = await DataApiRepository.GetApiUri<Stream>(AppSettings.ApiUrlBase + @"FileDown/" + WillDownFile.Id, showDownProgress);
                if (responseStream != null && responseStream.Length > 0)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(tempFileDir);
                    directoryInfo.Delete(true);
                    directoryInfo.Create();

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
