using System;
using System.Net.Http.Handlers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Files;
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
        public void Init_PlanInfo(Lib.Plan P_Entity, Action<Lib.Plan> P_CallBack = null)
        {
            _UCPlanInfoVM = new UC_PlanInfoVM();
            _UCPlanInfoVM.Init_PlanInfoVMAsync(P_Entity);
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
            AppSettings.AppMainWindow.FrameContentPage.Content = new PageEditPlan(_UCPlanInfoVM.EntityPlan);
        }
        /// <summary>
        /// 删除当前计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_DelePlan(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("删除计划《" + _UCPlanInfoVM.EntityPlan.Caption + "》？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            ExcuteResult JsonResult = await DataPlanRepository.DeletePlanInfo(_UCPlanInfoVM.EntityPlan);
            if (JsonResult.State == 0)
            {
                AppSettings.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
                _CallBack(_UCPlanInfoVM.EntityPlan);
                //MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnUpFile_Click(object sender, RoutedEventArgs e)
        {
            WinUpLoadFile winUpLoadFile = new WinUpLoadFile((UpFile) =>
            {
                _UCPlanInfoVM.UploadFiles.Add(UpFile);
            },
            _UCPlanInfoVM.EntityPlan.Id, "计划附件", _UCPlanInfoVM.EntityPlan.PlanType);
            winUpLoadFile.ShowDialog();
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

        private async void Image_Delete_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PlanFile SelectFile = LB_FileList.SelectedItem as PlanFile;
            if (MessageBox.Show("删除文件《" + SelectFile.Name + "》？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            ExcuteResult delResult = await DataPlanFileRepository.DeleteFileInfo(SelectFile);
            if (delResult.State == 0)
            {
                _UCPlanInfoVM.UploadFiles.Remove(SelectFile);
            }
        }
        /// <summary>
        /// 打开所选文件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TB_OpenFile_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock curTextBlock = sender as TextBlock;
            curTextBlock.IsEnabled = false;
            PlanFile SelectFile = LB_FileList.SelectedItem as PlanFile;

            ProgressMessageHandler progress = new System.Net.Http.Handlers.ProgressMessageHandler();

            progress.HttpReceiveProgress += (object sender, System.Net.Http.Handlers.HttpProgressEventArgs e) =>
            {
                SelectFile.DownIntProgress = e.ProgressPercentage;
            };

            string theDownFileName = await DataPlanFileRepository.DownloadFile(SelectFile, false, progress);
            if (theDownFileName != null)
            {
                SelectFile.DownIntProgress = 100;
                DataMemberFileRepository.OpenFileInfo(theDownFileName);
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
            _UCPlanInfoVM.EntityPlan.CurrectState = "正在实施";
            ExcuteResult JsonResult = await DataPlanRepository.UpdatePlanInfo(_UCPlanInfoVM.EntityPlan);
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
            if (string.IsNullOrWhiteSpace(_UCPlanInfoVM.EntityPlan.FinishNote))
            {
                //MessageBox.Show("请输入完成情况！", "消息", MessageBoxButton.OK, MessageBoxImage.Warning);
                Text_FinishNote.Focus();
                return;
            }
            _UCPlanInfoVM.EntityPlan.CurrectState = "已经完成";
            ExcuteResult JsonResult = await DataPlanRepository.UpdatePlanInfo(_UCPlanInfoVM.EntityPlan);
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
            _UCPlanInfoVM.EntityPlan.CurrectState = "正在实施";
            ExcuteResult JsonResult = await DataPlanRepository.UpdatePlanInfo(_UCPlanInfoVM.EntityPlan);
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
