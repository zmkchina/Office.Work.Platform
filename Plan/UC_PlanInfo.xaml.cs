using System;
using System.Diagnostics;
using System.Net.Http.Handlers;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.PlanFiles;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Plan
{
    /// <summary>
    /// Interaction logic for UC_PlanInfo.xaml
    /// </summary>
    public partial class UC_PlanInfo : UserControl
    {
        private UC_PlanInfoVM _UCPlanInfoVM;
        private Action<Lib.Plan> _CallBack = null;
        public UC_PlanInfo()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        public async void Init_PlanInfoAsync(Lib.Plan P_Entity, Action<Lib.Plan> P_CallBack = null)
        {
            _UCPlanInfoVM = new UC_PlanInfoVM();
            await _UCPlanInfoVM.Init_PlanInfoVMAsync(P_Entity);
            _CallBack = P_CallBack;
            DataContext = _UCPlanInfoVM;
        }
        /// <summary>
        /// 编辑该计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_EditPlan(object sender, RoutedEventArgs e)
        {
            AppSettings.AppMainWindow.FrameContentPage.Content = new PageEditPlan(_UCPlanInfoVM.CurPlan);
        }
        /// <summary>
        /// 删除当前计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_DelePlan(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("删除计划《" + _UCPlanInfoVM.CurPlan.Caption + "》？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            ExcuteResult JsonResult = await DataPlanRepository.DeletePlan(_UCPlanInfoVM.CurPlan);
            if (JsonResult.State == 0)
            {
                AppSettings.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
                _CallBack(_UCPlanInfoVM.CurPlan);
                //MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        /// <summary>
        /// 上传文件到服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnUpFile_ClickAsync(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            System.IO.FileInfo theFile = FileOperation.SelectFile();
            if (theFile != null)
            {
                PlanFile newFile = new PlanFile()
                {
                    Name = theFile.Name.Substring(0, theFile.Name.LastIndexOf('.')),
                    UserId = AppSettings.LoginUser.Id,
                    Length = theFile.Length,
                    ExtendName = theFile.Extension,
                    PlanId = _UCPlanInfoVM.CurPlan.Id,
                    FileInfo = theFile,
                    UpIntProgress = 0
                };
                _UCPlanInfoVM.CurPlan.Files.Add(newFile);
                ProgressMessageHandler UpProgress = new ProgressMessageHandler();
                UpProgress.HttpSendProgress += (object sender, HttpProgressEventArgs e) =>
                {
                    newFile.UpIntProgress = e.ProgressPercentage;
                };
                ExcuteResult result = await DataPlanFileRepository.UpLoadFileInfo(newFile, newFile.FileInfo.OpenRead(), "planfile", "pf", UpProgress);
                if (result.State != 0)
                {
                    newFile.UpIntProgress = 0;
                }
            }
        }

        private void MenuItem_ReName_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_CopyFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_ReDwonLoad_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_ToFolder_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 从服务器删除文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Image_Delete_MouseLeftButtonUpAsync(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PlanFile SelectFile = LB_FileList.SelectedItem as PlanFile;
            if (MessageBox.Show("删除文件《" + SelectFile.Name + "》？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            ExcuteResult delResult = await DataPlanFileRepository.DeleteFileInfo(SelectFile);
            if (delResult.State == 0)
            {
                _UCPlanInfoVM.CurPlan.Files.Remove(SelectFile);
            }
        }
        /// <summary>
        /// 打开所选文件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TB_OpenFile_MouseLeftButtonUpAsync(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock curTextBlock = sender as TextBlock;
            curTextBlock.IsEnabled = false;
            PlanFile SelectFile = LB_FileList.SelectedItem as PlanFile;

            ProgressMessageHandler progress = new ProgressMessageHandler();

            progress.HttpReceiveProgress += (object sender, HttpProgressEventArgs e) =>
            {
                SelectFile.DownIntProgress = e.ProgressPercentage;
            };

            string theDownFileName = await DataPlanFileRepository.DownloadFile(SelectFile, false, progress);
            if (theDownFileName != null)
            {
                SelectFile.DownIntProgress = 100;
                FileOperation.UseDefaultAppOpenFile(theDownFileName);
            }
            else
            {
                MessageBox.Show("文件下载失败，可能该文件已被删除！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            curTextBlock.IsEnabled = true;
        }
        /// <summary>
        /// 更新计划进度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_UpdatePlan(object sender, RoutedEventArgs e)
        {
            _UCPlanInfoVM.CurPlan.CurrectState = PlanStatus.Running;
            ExcuteResult JsonResult = await DataPlanRepository.UpdatePlan(_UCPlanInfoVM.CurPlan);
            if (JsonResult.State == 0)
            {
                //AppSettings.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
                MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        /// <summary>
        /// 完结计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_FinishPlan(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_UCPlanInfoVM.CurPlan.FinishNote))
            {
                //MessageBox.Show("请输入完成情况！", "消息", MessageBoxButton.OK, MessageBoxImage.Warning);
                Text_FinishNote.Focus();
                return;
            }
            _UCPlanInfoVM.CurPlan.CurrectState = PlanStatus.Finished;
            ExcuteResult JsonResult = await DataPlanRepository.UpdatePlan(_UCPlanInfoVM.CurPlan);
            if (JsonResult.State == 0)
            {
                AppSettings.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
                //MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        /// <summary>
        /// 将计划状态重置为 正在实施
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_ResetPlan(object sender, RoutedEventArgs e)
        {
            _UCPlanInfoVM.CurPlan.CurrectState = PlanStatus.Running;
            ExcuteResult JsonResult = await DataPlanRepository.UpdatePlan(_UCPlanInfoVM.CurPlan);
            if (JsonResult.State == 0)
            {
                AppSettings.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
                //MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
