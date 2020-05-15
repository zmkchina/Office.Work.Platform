using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Handlers;
using System.Windows;
using System.Windows.Input;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;

namespace Office.Work.Platform
{
    /// <summary>
    /// WinUpdateDialog.xaml 的交互逻辑
    /// </summary>
    public partial class WinUpdateDialog : Window
    {
        private readonly CurWinViewModel _CurWinViewModel;

        public WinUpdateDialog(List<string> PDownFiles)
        {
            InitializeComponent();
            this.Owner = AppSet.AppMainWindow;
            _CurWinViewModel = new CurWinViewModel(PDownFiles);
            this.DataContext = _CurWinViewModel;
        }
        /// <summary>
        /// 下载新程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            //创建下载文件存放目录
            //合成目录
            string tempFileDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateApp");

            DirectoryInfo directoryInfo = new DirectoryInfo(tempFileDir);
            if (directoryInfo.Exists)
            {
                directoryInfo.Delete(true);
            }
            directoryInfo.Create();
            _CurWinViewModel.DownCount = _CurWinViewModel.NeedDownFiles.Count;
            for (int i = 0; i < _CurWinViewModel.NeedDownFiles.Count; i++)
            {
                _CurWinViewModel.DownFileName = $"{_CurWinViewModel.NeedDownFiles[i]}正在下载...。";

                ProgressMessageHandler DownProgress = new ProgressMessageHandler();
                DownProgress.HttpReceiveProgress += (object sender, HttpProgressEventArgs e) =>
                {
                    _CurWinViewModel.DownIntProgress = e.ProgressPercentage;
                };
                string downFileName = await DataFileUpdateAppRepository.DownLoadNewVerFile(_CurWinViewModel.NeedDownFiles[i], tempFileDir, DownProgress);
                if (!string.IsNullOrWhiteSpace(downFileName))
                {
                    _CurWinViewModel.DownIndex++;
                    _CurWinViewModel.DownFileName = $"{_CurWinViewModel.NeedDownFiles[i]}下载完成。";
                }
                else
                {
                    _CurWinViewModel.DownFileName = $"{_CurWinViewModel.NeedDownFiles[i]}下载失败。";
                }
            }
            this.Btn_Update.IsEnabled = true;
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Dispatcher.BeginInvoke(new Action(() => this.DragMove()));
            }
        }
        /// <summary>
        /// 该窗口对象的ViewModel类
        /// </summary>
        public class CurWinViewModel : NotificationObject
        {
            private int _DownIndex;
            private int _DownCount;
            private string _DownFileName;
            private int _DownIntProgress;

            public CurWinViewModel(List<string> PDownFiles)
            {
                NeedDownFiles = PDownFiles;
            }
            public int DownIntProgress
            {
                get { return _DownIntProgress; }
                set
                {
                    _DownIntProgress = value; RaisePropertyChanged();
                }
            }
            public int DownIndex
            {
                get { return _DownIndex; }
                set
                {
                    _DownIndex = value; RaisePropertyChanged();
                }
            }
            public int DownCount
            {
                get { return _DownCount; }
                set
                {
                    _DownCount = value; RaisePropertyChanged();
                }
            }
            public string DownFileName
            {
                get
                {
                    return _DownFileName;
                }
                set
                {
                    _DownFileName = value; RaisePropertyChanged();
                }
            }
            public List<string> NeedDownFiles { get; set; }
        }

        private void Btn_UpdateOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
