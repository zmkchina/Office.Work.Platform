using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Threading.Tasks;
using System.Windows;

namespace Office.Work.Platform.AppDataService
{
    public static class DataFileRepository
    {
        /// <summary>
        /// 上传文件信息
        /// </summary>
        /// <param name="WillFileInfo"></param>
        /// <returns></returns>
        public static async Task<ModelResult> UpLoadFileInfo(ModelFile UpFileInfo, Stream PostFileStream, string PostFileKey, string PostFileName, ProgressMessageHandler showUploadProgress = null)
        {
            MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(UpFileInfo, PostFileStream, PostFileKey, PostFileName);
            ModelResult JsonResult = await DataApiRepository.PostApiUri<ModelResult>(AppSettings.ApiUrlBase + "FileInfo", V_MultFormDatas, showUploadProgress);
            return JsonResult;
        }
        /// <summary>
        /// 更新文件信息
        /// </summary>
        /// <param name="UpFile">预更新文件信息</param>
        /// <returns></returns>
        public static async Task<ModelResult> UpdateFileInfo(ModelFile UpFile)
        {
            UpFile.UpDateTime = DateTime.Now;
            MultipartFormDataContent V_MultFormDatas = DataApiRepository.SetFormData(UpFile);
            ModelResult JsonResult = await DataApiRepository.PutApiUri<ModelResult>(AppSettings.ApiUrlBase + "FileInfo", V_MultFormDatas);
            return JsonResult;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="DelFile">预删除的文件</param>
        /// <returns></returns>
        public static async Task<ModelResult> DeleteFileInfo(ModelFile DelFile)
        {
            ModelResult JsonResult = await DataApiRepository.DeleteApiUri<ModelResult>(AppSettings.ApiUrlBase + "FileInfo/?P_FileId=" + DelFile.Id + "&P_FileExtName=" + DelFile.ExtendName);
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
        public static async Task<string> DownloadFile(ModelFile WillDownFile, bool ReDownLoad = false, ProgressMessageHandler showDownProgress = null)
        {
            //合成目录
            string tempFileDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DownFiles", WillDownFile.OwnerType);
            //合成目录+文件
            string tempFilePath = System.IO.Path.Combine(tempFileDir, WillDownFile.Name + "(" + WillDownFile.Id + ")" + WillDownFile.ExtendName);
            if (!File.Exists(tempFilePath) || ReDownLoad)
            {
                HttpResponseMessage httpResponseMessage = await DataApiRepository.GetApiUri<HttpResponseMessage>(AppSettings.ApiUrlBase + @"FileDown/" + WillDownFile.Id, showDownProgress);
                if (httpResponseMessage == null || httpResponseMessage.Content == null)
                {
                    //说明此文件读取失败，有可能是服务器上文件被删除了。
                    return null;
                }
                Stream responseStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                //以上两句代码不能用下一句代替，否则进度报告出现卡顿。
                //Stream responseStream = await DataApiRepository.GetApiUri<Stream>(AppSettings.ApiUrlBase + @"FileDown/" + WillDownFile.Id, showDownProgress);
                if (httpResponseMessage.Content.Headers.ContentLength > 0 && responseStream != null)
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
                else
                {
                    MessageBox.Show("下载失败，可能该文件已被从服务器上删除", "不存在", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            if (File.Exists(tempFilePath))
            {
                return tempFilePath;
            }
            return null;
        }

        ///// <summary>
        ///// 读取所有文件列表
        ///// </summary>
        ///// <returns></returns>
        //public static async Task<List<ModelFile>> ReadFiles()
        //{
        //    List<ModelFile> FileList = await DataApiRepository.GetApiUri<List<ModelFile>>(AppSettings.ApiUrlBase + "FileInfo");
        //    return FileList;
        //}
        /// <summary>
        /// 读取指定类型、指定宿主的文件列表。比如：读取“计划附件”编号为“201999999”的文件列表
        /// 示例：ReadPlanFiles("计划附件","201999999");
        /// </summary>
        /// <param name="mSearchFile">查询条件类的实例</param>
        /// <returns></returns>
        public static async Task<IEnumerable<ModelFile>> ReadFiles(MSearchFile mSearchFile)
        {
            mSearchFile.CanReadUserId = AppSettings.LoginUser.Id;
            IEnumerable<ModelFile> FileList = null;
            //创建查询url参数
            string urlParams = DataApiRepository.CreateUrlParams(mSearchFile);

            if (urlParams.Length > 0)
            {
                FileList = await DataApiRepository.GetApiUri<IEnumerable<ModelFile>>(AppSettings.ApiUrlBase + "FileInfo/Search" + urlParams);
            }
            return FileList;
        }
    }
}
