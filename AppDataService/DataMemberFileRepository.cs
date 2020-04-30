using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Threading.Tasks;
using System.Windows;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppDataService
{
    public static class DataMemberFileRepository
    {
        /// <summary>
        /// 读取指定查询条件的文件列表。
        /// </summary>
        /// <param name="mSearchFile">查询条件类的实例</param>
        /// <returns></returns>
        public static async Task<IEnumerable<MemberFile>> ReadFiles(MemberFileSearch mfSearchFile)
        {
            IEnumerable<MemberFile> FileList = null;
            //创建查询url参数
            string urlParams = DataApiRepository.CreateUrlParams(mfSearchFile);

            if (urlParams.Length > 0)
            {
                FileList = await DataApiRepository.GetApiUri<IEnumerable<MemberFile>>(AppSettings.ApiUrlBase + "MemberFile/Search" + urlParams).ConfigureAwait(false);
            }
            return FileList;
        }
        /// <summary>
        /// 上传文件信息
        /// </summary>
        /// <param name="WillFileInfo"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpLoadFileInfo(MemberFile UpFileInfo, Stream PostFileStream, string PostFileKey, string PostFileName, ProgressMessageHandler showUploadProgress = null)
        {
            MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(UpFileInfo, PostFileStream, PostFileKey, PostFileName);
            ExcuteResult JsonResult = await DataApiRepository.PostApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "MemberFile/UpLoadFile", V_MultFormDatas, showUploadProgress).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 更新文件信息
        /// </summary>
        /// <param name="UpFile">预更新文件信息</param>
        /// <returns></returns>
        public static async Task<ExcuteResult> UpdateFileInfo(MemberFile UpFile)
        {
            UpFile.UpDateTime = DateTime.Now;
            MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(UpFile);
            ExcuteResult JsonResult = await DataApiRepository.PutApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "MemberFile", V_MultFormDatas).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="DelFile">预删除的文件</param>
        /// <returns></returns>
        public static async Task<ExcuteResult> DeleteFileInfo(MemberFile DelFile)
        {
            ExcuteResult JsonResult = await DataApiRepository.DeleteApiUri<ExcuteResult>(AppSettings.ApiUrlBase + "MemberFile/?FileId=" + DelFile.Id + "&FileExtName=" + DelFile.ExtendName).ConfigureAwait(false);
            return JsonResult;
        }
        /// <summary>
        /// 打开文件：如文件已经下载，直接打开；否则，先下载再打开。
        /// </summary>
        /// <param name="OpenFileFullName">预打开的文件全名，该文件已在下载完成。</param>
        /// <returns></returns>
        public static void OpenFileInfo(string OpenFileFullName)
        {
            try
            {
                if (OpenFileFullName != null && File.Exists(OpenFileFullName))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = OpenFileFullName
                    };
                    Process.Start(startInfo);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("打开文件时出错(正在使用？)" + err.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="WillDownFile">将下载的文件信息</param>
        /// <param name="ReDownLoad">是否重新下载，默认为false</param>
        /// <param name="showDownProgress">显示下载进度的委托方法,可为空</param>
        /// <returns>返回下载成功的文件目录（包括路径）</returns>
        public static async Task<string> DownloadFile(MemberFile WillDownFile, bool ReDownLoad = false, ProgressMessageHandler showDownProgress = null)
        {
            if (WillDownFile == null) { return null; }
            //合成目录
            string tempFileDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DownFiles", "MemberFiles");
            //合成目录+文件
            string tempFilePath = System.IO.Path.Combine(tempFileDir, WillDownFile.Name + "(" + WillDownFile.Id + ")" + WillDownFile.ExtendName);
            if (!File.Exists(tempFilePath) || ReDownLoad)
            {
                HttpResponseMessage httpResponseMessage = await DataApiRepository.GetApiUri<HttpResponseMessage>(AppSettings.ApiUrlBase + @"MemberFile/DownloadFile/" + WillDownFile.Id, showDownProgress).ConfigureAwait(false);
                if (httpResponseMessage != null && httpResponseMessage.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Stream responseStream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    //以上两句代码不能用下一句代替，否则进度报告出现卡顿。
                    //Stream responseStream = await DataApiRepository.GetApiUri<Stream>(AppSettings.ApiUrlBase + @"FileDown/" + WillDownFile.Id, showDownProgress);
                    if (responseStream != null && responseStream.Length > 0)
                    {
                        if (!System.IO.Directory.Exists(tempFileDir))
                        {
                            //创建目录
                            System.IO.Directory.CreateDirectory(tempFileDir);
                        }
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
            }
            if (File.Exists(tempFilePath))
            {
                return tempFilePath;
            }
            else
            {
                return null;
            }
        }
    }
}
