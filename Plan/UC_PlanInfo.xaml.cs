using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.FileDocs;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private  void BtnUpFile_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            System.IO.FileInfo theFile = FileOperation.SelectFile();
            if (theFile != null)
            {
                WinUpLoadFile winUpLoadFile = new WinUpLoadFile(new Action<FileDoc>(newFile =>
                {
                    _UCPlanInfoVM.PlanFiles.Add(newFile);
                }), theFile, "计划附件",_UCPlanInfoVM.CurPlan.Id,_UCPlanInfoVM.CurPlan.ContentType);
                winUpLoadFile.ShowDialog();
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
            FileDoc SelectFile = LB_FileList.SelectedItem as FileDoc;
            if ((new WinMsgDialog($"删除文件《{SelectFile.Name}》？", "确认", showYesNo: true)).ShowDialog().Value == false)
            {
                return;
            }
            ExcuteResult delResult = await DataFileDocRepository.DeleteFileInfo(SelectFile);
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
            FileDoc SelectFile = LB_FileList.SelectedItem as FileDoc;

            ProgressMessageHandler progress = new ProgressMessageHandler();

            progress.HttpReceiveProgress += (object sender, HttpProgressEventArgs e) =>
            {
                SelectFile.DownIntProgress = e.ProgressPercentage;
            };

            string theDownFileName = await DataFileDocRepository.DownloadFile(SelectFile, false, progress);
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


    public class UC_PlanInfoVM : NotificationObject
    {
        private PlanOperateGrant _OperateGrant;

        public UC_PlanInfoVM()
        {
            PlanFiles = new ObservableCollection<FileDoc>();
        }
        public async System.Threading.Tasks.Task Init_PlanInfoVMAsync(Lib.Plan P_Entity)
        {
            CurPlan = P_Entity;
            OperateGrant = new PlanOperateGrant(AppSettings.LoginUser, P_Entity);

            if (PlanFiles.Count < 1)
            {
                //如果该计划的附件文件没有读取则读取之。
                FileDocSearch mSearchFile = new FileDocSearch() { OwnerId = P_Entity.Id, UserId = AppSettings.LoginUser.Id };
                IEnumerable<FileDoc> UpFiles = await DataFileDocRepository.ReadFiles(mSearchFile);
                UpFiles.ToList().ForEach(e =>
                {
                    e.UpIntProgress = 100;
                    PlanFiles.Add(e);
                });
            }
            if (CurPlan.CreateUserId != null)
            {
                CurPlanCreateUserName = AppSettings.SysUsers.Where(e => CurPlan.CreateUserId.Equals(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name).FirstOrDefault()?.Trim();
            }
            if (CurPlan.ResponsiblePerson != null)
            {
                CurPlanResponsibleName = AppSettings.SysUsers.Where(e => CurPlan.ResponsiblePerson.Equals(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name).FirstOrDefault()?.Trim();
            }
            if (CurPlan.ReadGrant != null)
            {
                CurPlanHasGrantNames = string.Join(",", AppSettings.SysUsers.Where(e => CurPlan.ReadGrant.Contains(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name)?.ToArray());
            }
            if (CurPlan.Helpers != null)
            {
                CurPlanHelperNames = string.Join(",", AppSettings.SysUsers.Where(e => CurPlan.Helpers.Contains(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name)?.ToArray());
            }
        }
        /// <summary>
        /// 当前所选计划信息
        /// </summary>
        public Lib.Plan CurPlan { get; set; }
        /// <summary>
        /// 当前计划的文件。
        /// </summary>
        public ObservableCollection<FileDoc> PlanFiles { get; set; }
        /// <summary>
        /// 计划创建者的姓名（中文）。
        /// </summary>
        public string CurPlanCreateUserName { get; set; }
        /// <summary>
        /// 计划责任者的姓名（中文）。
        /// </summary>
        public string CurPlanResponsibleName { get; set; }
        /// <summary>
        ///计划协助人员姓名列表（中文）
        /// </summary>
        public string CurPlanHelperNames { get; set; }
        /// <summary>
        /// 计划有限读取人员的姓名列表（中文）。
        /// </summary>
        public string CurPlanHasGrantNames { get; set; }

        /// <summary>
        /// 用户对此计划的操作权限类对象。
        /// </summary>
        public PlanOperateGrant OperateGrant
        {
            get { return _OperateGrant; }
            set
            {
                _OperateGrant = value;
                this.RaisePropertyChanged();
            }
        }
        #region "方法"

        #endregion
        /// <summary>
        /// 用户对该计划的权限 A:计划修改 B:计划删除 C:计划进度更新 D:上传计划附件 E:计划完结 F:计划状态重置
        /// </summary>
        public class PlanOperateGrant
        {
            public string CanEdit { get; set; }
            public string CanDelete { get; set; }
            public string CanUpdate { get; set; }
            public string CanUpFile { get; set; }
            public string CanFinish { get; set; }
            public string CanReset { get; set; }
            public PlanOperateGrant(User P_LoginUser, Lib.Plan P_CurPlan)
            {
                CanDelete = CanEdit = CanUpFile = CanFinish = CanReset = CanUpdate = "Collapsed";
                if (P_LoginUser.Post.Equals("SysAdmin"))
                {
                    CanDelete = CanEdit = CanUpFile = CanFinish = CanReset = CanUpdate = "Visible";
                }
                else if (P_LoginUser.Post.Equals("部门负责人"))
                {
                    CanDelete = CanEdit = CanUpFile = CanFinish = CanReset = CanUpdate = "Visible";
                }
                else if (P_CurPlan.CurrectState.Contains(PlanStatus.Finished))
                {
                    CanDelete = CanEdit = CanUpFile = CanFinish = CanReset = CanUpdate = "Collapsed";
                }
                else if (P_LoginUser.Id.Equals(P_CurPlan.CreateUserId))
                {
                    //计划创建者：A—E
                    CanDelete = CanEdit = CanUpFile = CanFinish = CanUpdate = "Visible";
                }
                else if (P_LoginUser.Id.Equals(P_CurPlan.ResponsiblePerson))
                {
                    //责任者
                    CanUpFile = CanFinish = CanUpdate = "Visible";
                }
                else if (!string.IsNullOrWhiteSpace(P_CurPlan.Helpers) && P_CurPlan.Helpers.Contains(P_LoginUser.Id))
                {
                    //协助者
                    CanUpFile = CanUpdate = "Visible";
                }
            }
        }
    }
}
