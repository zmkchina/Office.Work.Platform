using System;
using System.Diagnostics;
using System.Net.Http.Handlers;
using Microsoft.Win32;

namespace Office.Work.Platform.AppCodes
{
    public static class FileOperation
    {
        /// <summary>
        /// 打开系统对话框选择一个文件。
        /// </summary>
        /// <returns></returns>
        public static System.IO.FileInfo SelectFile()
        {
            System.IO.FileInfo theFile = null;
            // 在WPF中， OpenFileDialog位于Microsoft.Win32名称空间
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "工作文档|*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx;*.wps;*.pdf;*.jpg;*.jpeg;*.png;*.gif;|压缩文档|*.rar;*.zip|所有文件|*.*"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    theFile = new System.IO.FileInfo(dialog.FileName);
                    if (theFile.Length > 1073741824)//1G
                    {
                        AppFuns.ShowMessage("文件大于1G,无法保存！");
                        return null;
                    }
                }
                catch (Exception Error)
                {
                    AppFuns.ShowMessage($"读取文件出错(正在使用？){Error.Message}");
                    return null;
                }
            }
            return theFile;
        }
        /// <summary>
        /// 用默认程序打开指定路径、文件名的文件。
        /// </summary>
        /// <param name="FileFullPathName"></param>
        public static void UseDefaultAppOpenFile(string FileFullPathName)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = FileFullPathName
            };
            Process.Start(startInfo);
        }
    }
}
