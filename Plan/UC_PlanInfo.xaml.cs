using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System;
using System.Net.Http.Handlers;
using System.Windows;
using System.Windows.Controls;

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
            if ((new WinMsgDialog($"删除计划《{_UCPlanInfoVM.CurPlan.Caption }》？", "确认", showYesNo: true)).ShowDialog().Value == false)
            {
                return;
            }
            ExcuteResult JsonResult = await DataPlanRepository.DeletePlan(_UCPlanInfoVM.CurPlan);
            if (JsonResult.State == 0)
            {
                AppSettings.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
                _CallBack(_UCPlanInfoVM.CurPlan);
            }
            else
            {
                (new WinMsgDialog(JsonResult.Msg, "消息")).ShowDialog();
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
                PlanFile NewPlanFile = new PlanFile()
                {
                    Name = theFile.Name.Substring(0, theFile.Name.LastIndexOf('.')),
                    UserId = AppSettings.LoginUser.Id,
                    Length = theFile.Length,
                    ExtendName = theFile.Extension,
                    PlanId = _UCPlanInfoVM.CurPlan.Id,
                    FileInfo = theFile,
                    UpIntProgress = 0
                };
                _UCPlanInfoVM.PlanFiles.Add(NewPlanFile);
                ProgressMessageHandler UpProgress = new ProgressMessageHandler();
                UpProgress.HttpSendProgress += (object sender, HttpProgressEventArgs e) =>
                {
                    NewPlanFile.UpIntProgress = e.ProgressPercentage;
                };
                ExcuteResult result = await DataPlanFileRepository.UpLoadFileInfo(NewPlanFile, NewPlanFile.FileInfo.OpenRead(), "planfile", "pf", UpProgress);
                if (result == null || result.State != 0)
                {
                    NewPlanFile.UpIntProgress = 0;
                }
                else
                {
                    //服务器保存该文件成功，将返回的Id更新到当前记录。
                    NewPlanFile.Id = result.Tag;
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
            if ((new WinMsgDialog($"删除文件《{SelectFile.Name}》？", "确认", showYesNo: true)).ShowDialog().Value == false)
            {
                return;
            }
            ExcuteResult delResult = await DataPlanFileRepository.DeleteFileInfo(SelectFile);
            if (delResult.State == 0)
            {
                _UCPlanInfoVM.PlanFiles.Remove(SelectFile);
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
                (new WinMsgDialog("文件下载失败，可能该文件已被删除！", "警告")).ShowDialog();
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
            (new WinMsgDialog(JsonResult.Msg)).ShowDialog();
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
                Text_FinishNote.Focus();
                return;
            }
            _UCPlanInfoVM.CurPlan.CurrectState = PlanStatus.Finished;
            ExcuteResult JsonResult = await DataPlanRepository.UpdatePlan(_UCPlanInfoVM.CurPlan);
            if (JsonResult.State == 0)
            {
                AppSettings.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
            }
            (new WinMsgDialog(JsonResult.Msg)).ShowDialog();
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
            }
            else
            {
                (new WinMsgDialog(JsonResult.Msg)).ShowDialog();
            }
        }
    }
}
