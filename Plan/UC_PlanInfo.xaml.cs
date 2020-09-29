using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Handlers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.PlanFile;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Plan
{
    /// <summary>
    /// Interaction logic for UC_PlanInfo.xaml
    /// </summary>
    public partial class UC_PlanInfo : UserControl
    {
        private CurUcViewModel _UCPlanInfoVM;
        private Action<Lib.PlanEntity> _CallBack = null;
        public UC_PlanInfo()
        {
            InitializeComponent();
        }
        public async void Init_PlanInfoAsync(Lib.PlanEntity P_Entity, Action<Lib.PlanEntity> P_CallBack = null)
        {
            _CallBack = P_CallBack;
            _UCPlanInfoVM = new CurUcViewModel();
            await _UCPlanInfoVM.Init_PlanInfoVMAsync(P_Entity);
            _UCPlanInfoVM.SetPlanOperateGrant();
            _UCPlanInfoVM.SetPlanFileDelete();
            DataContext = _UCPlanInfoVM;
        }


        /// <summary>
        /// 拷贝文件到剪贴板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuItem_CopyFile_ClickAsync(object sender, RoutedEventArgs e)
        {
            Lib.PlanFile SelectFile = LB_FileList.SelectedItem as Lib.PlanFile;
            string theDownFileName = await DownLoadFile(SelectFile, false);
            if (theDownFileName != null)
            {
                System.Collections.Specialized.StringCollection files = new System.Collections.Specialized.StringCollection();
                files.Add(theDownFileName);
                Clipboard.SetFileDropList(files);
            }
            else
            {
                AppFuns.ShowMessage("文件下载失败，可能该文件已被删除！", "警告");
            }

        }
        /// <summary>
        /// 重新下载并打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuItem_ReDwonLoad_ClickAsync(object sender, RoutedEventArgs e)
        {
            MenuItem curMenuItem = sender as MenuItem;
            curMenuItem.IsEnabled = false;
            Lib.PlanFile SelectFile = LB_FileList.SelectedItem as Lib.PlanFile;

            string theDownFileName = await DownLoadFile(SelectFile, true);
            if (theDownFileName != null)
            {
                FileOperation.UseDefaultAppOpenFile(theDownFileName);
            }
            else
            {
                AppFuns.ShowMessage("文件下载失败，可能该文件已被删除！", "警告");
            }
            curMenuItem.IsEnabled = true;
        }
        /// <summary>
        /// 转到文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuItem_ToFolder_ClickAsync(object sender, RoutedEventArgs e)
        {
            Lib.PlanFile SelectFile = LB_FileList.SelectedItem as Lib.PlanFile;
            string theDownFileName = await DownLoadFile(SelectFile, false);
            if (theDownFileName != null)
            {
                System.Diagnostics.Process.Start("Explorer", "/select," + theDownFileName);
            }
            else
            {
                AppFuns.ShowMessage("文件下载失败，可能该文件已被删除！", "警告");
            }
        }


        /// <summary>
        /// 编辑该计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_EditPlan(object sender, RoutedEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PageEditPlan(_UCPlanInfoVM.CurPlan);
        }
        /// <summary>
        /// 删除当前计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_DelePlan(object sender, RoutedEventArgs e)
        {
            if (!AppFuns.ShowMessage($"删除计划《{_UCPlanInfoVM.CurPlan.Caption }》？", "确认", showYesNo: true))
            {
                return;
            }
            ExcuteResult JsonResult = await DataPlanRepository.DeletePlan(_UCPlanInfoVM.CurPlan);
            if (JsonResult.State == 0)
            {
                AppSet.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
                _CallBack(_UCPlanInfoVM.CurPlan);
            }
            else
            {
                AppFuns.ShowMessage(JsonResult.Msg, "消息");
            }
        }
        /// <summary>
        /// 上传文件到服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpFile_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            try
            {
                System.IO.FileInfo theFile = FileOperation.SelectFile();
                if (theFile != null)
                {
                    WinUpPlanFile winUpLoadFile = new WinUpPlanFile(new Action<Lib.PlanFile>(newFile =>
                    {
                        _UCPlanInfoVM.PlanFiles.Add(newFile);
                    }), theFile, "计划附件", _UCPlanInfoVM.CurPlan.Id, _UCPlanInfoVM.CurPlan.ContentType);
                    winUpLoadFile.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                AppFuns.ShowMessage(ex.Message, "错误", isErr: true);
            }

        }

        /// <summary>
        /// 从服务器删除文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Image_Delete_MouseLeftButtonUpAsync(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Lib.PlanFile SelectFile = LB_FileList.SelectedItem as Lib.PlanFile;
            if (!AppFuns.ShowMessage($"删除文件《{SelectFile.Name}》？", "确认", showYesNo: true))
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
            Lib.PlanFile SelectFile = LB_FileList.SelectedItem as Lib.PlanFile;

            string theDownFileName = await DownLoadFile(SelectFile, false);
            if (theDownFileName != null)
            {
                FileOperation.UseDefaultAppOpenFile(theDownFileName);
            }
            else
            {
                AppFuns.ShowMessage("文件下载失败，可能该文件已被删除！", "警告");
            }
            curTextBlock.IsEnabled = true;
        }

        /// <summary>
        /// 下载文件，成功返回带路径的文件名，失败返回Null
        /// </summary>
        /// <param name="WillDownFile"></param>
        /// <param name="ReDownLoad"></param>
        /// <returns></returns>
        private async Task<string> DownLoadFile(Lib.PlanFile WillDownFile, bool ReDownLoad = false)
        {
            ProgressMessageHandler progress = new ProgressMessageHandler();

            progress.HttpReceiveProgress += (object sender, HttpProgressEventArgs e) =>
            {
                WillDownFile.DownIntProgress = e.ProgressPercentage;
            };

            string theDownFileName = await DataPlanFileRepository.DownloadFile(WillDownFile, false, progress);
            if (theDownFileName != null)
            {
                WillDownFile.DownIntProgress = 100;
                return theDownFileName;
            }
            else
            {
                return null;
            }
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
            AppFuns.ShowMessage(JsonResult.Msg);
            _UCPlanInfoVM.SetPlanOperateGrant();
            _UCPlanInfoVM.SetPlanFileDelete();
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
                AppSet.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
            }
            AppFuns.ShowMessage(JsonResult.Msg);
            _UCPlanInfoVM.SetPlanOperateGrant();
            _UCPlanInfoVM.SetPlanFileDelete();
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
                AppSet.AppMainWindow.lblCursorPosition.Text = JsonResult.Msg;
                AppFuns.ShowMessage("计划重置成功！");
                _UCPlanInfoVM.SetPlanOperateGrant();
                _UCPlanInfoVM.SetPlanFileDelete();
            }
            else
            {
                AppFuns.ShowMessage(JsonResult.Msg);
            }
        }

        /// <summary>
        /// 本控件的视图模型
        /// </summary>
        private class CurUcViewModel : NotificationObject
        {
            private string _CanReset;
            private string _CanFinish;
            private string _CanUpFile;
            private string _CanDelete;
            private string _CanEdit;
            private string _CanUpdate;

            public CurUcViewModel()
            {
                PlanFiles = new ObservableCollection<Lib.PlanFile>();
            }
            public async Task Init_PlanInfoVMAsync(Lib.PlanEntity P_Entity)
            {
                CurPlan = P_Entity;
                if (PlanFiles.Count < 1)
                {
                    //如果该计划的附件文件没有读取则读取之。
                    PlanFileSearch mSearchFile = new PlanFileSearch() { PlanId = P_Entity.Id, UserId = AppSet.LoginUser.Id };
                    PlanFileSearchResult planFileSearchResult = await DataPlanFileRepository.ReadFiles(mSearchFile);
                    if (planFileSearchResult != null && planFileSearchResult.RecordList != null)
                    {
                        planFileSearchResult.RecordList.ToList().ForEach(e =>
                        {
                            e.UpIntProgress = 100;

                            PlanFiles.Add(e);
                        });
                    }
                }
                if (CurPlan.CreateUserId != null)
                {
                    CurPlanCreateUserName = AppSet.SysUsers.Where(e => CurPlan.CreateUserId.Equals(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name).FirstOrDefault()?.Trim();
                }
                if (CurPlan.ResponsiblePerson != null)
                {
                    CurPlanResponsibleName = AppSet.SysUsers.Where(e => CurPlan.ResponsiblePerson.Equals(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name).FirstOrDefault()?.Trim();
                }
                if (CurPlan.ReadGrant != null)
                {
                    CurPlanHasGrantNames = string.Join(",", AppSet.SysUsers.Where(e => CurPlan.ReadGrant.Contains(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name)?.ToArray());
                }
                if (CurPlan.Helpers != null)
                {
                    CurPlanHelperNames = string.Join(",", AppSet.SysUsers.Where(e => CurPlan.Helpers.Contains(e.Id, System.StringComparison.Ordinal)).Select(x => x.Name)?.ToArray());
                }
            }

            /// <summary>
            /// 当前所选计划信息
            /// </summary>
            public Lib.PlanEntity CurPlan { get; set; }
            /// <summary>
            /// 当前计划的文件。
            /// </summary>
            public ObservableCollection<Lib.PlanFile> PlanFiles { get; set; }
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
            /// 设置该计划附件的可删除状态
            /// </summary>
            public void SetPlanFileDelete()
            {
                for (int i = 0; i < PlanFiles.Count; i++)
                {
                    PlanFiles[i].CanDelte = "Collapsed";
                    if (AppSet.LoginUser.Post.Equals("管理员"))
                    {
                        PlanFiles[i].CanDelte = "Visible";
                    }
                    else
                    {
                        if (!CurPlan.CurrectState.Equals(PlanStatus.Finished))
                        {
                            if (CurPlan.CreateUserId.Equals(AppSet.LoginUser.Id) || CurPlan.Helpers.Contains(AppSet.LoginUser.Id))
                            {
                                PlanFiles[i].CanDelte = "Visible";
                            }
                            if (CurPlan.Department.Equals(AppSet.LoginUser.Department) && AppSet.LoginUser.Post.Equals("部门负责人"))
                            {
                                PlanFiles[i].CanDelte = "Visible";
                            }
                        }

                    }
                }
            }


            //用户对该计划的权限 A:计划修改 B:计划删除 C:计划进度更新 D:上传计划附件 E:计划完结 F:计划状态重置
            public string CanEdit
            {
                get { return _CanEdit; }
                set { _CanEdit = value; RaisePropertyChanged(); }
            }
            public string CanDelete
            {
                get { return _CanDelete; }
                set { _CanDelete = value; RaisePropertyChanged(); }
            }
            public string CanUpdate
            {
                get { return _CanUpdate; }
                set { _CanUpdate = value; RaisePropertyChanged(); }
            }
            public string CanUpFile
            {
                get { return _CanUpFile; }
                set { _CanUpFile = value; RaisePropertyChanged(); }
            }
            public string CanFinish
            {
                get { return _CanFinish; }
                set { _CanFinish = value; RaisePropertyChanged(); }
            }
            public string CanReset
            {
                get { return _CanReset; }
                set { _CanReset = value; RaisePropertyChanged(); }
            }
            /// <summary>
            /// 根据当前用户和计划状态，确定各类操作权限
            /// </summary>
            public void SetPlanOperateGrant()
            {
                CanDelete = CanEdit = CanUpFile = CanFinish = CanReset = CanUpdate = "Collapsed";
                User P_LoginUser = AppSet.LoginUser;

                if (P_LoginUser.Post.Equals("管理员"))
                {
                    CanDelete = CanEdit = CanUpFile = CanFinish = CanReset = CanUpdate = "Visible";
                    return;
                }
                if (CurPlan.CurrectState.Equals(PlanStatus.Finished))
                {
                    if (CurPlan.Department.Equals(P_LoginUser.Department) && P_LoginUser.Post.Equals("部门负责人"))
                    {
                        CanReset = "Visible";
                    }
                    return;
                }

                if (P_LoginUser.Id.Equals(CurPlan.CreateUserId))
                {
                    //计划创建者：A—E
                    CanDelete = CanEdit = CanUpFile = CanFinish = CanUpdate = "Visible";
                    return;
                }
                if (P_LoginUser.Id.Equals(CurPlan.ResponsiblePerson))
                {
                    //责任者
                    CanUpFile = CanFinish = CanUpdate = "Visible";
                    return;
                }
                if (!string.IsNullOrWhiteSpace(CurPlan.Helpers) && CurPlan.Helpers.Contains(P_LoginUser.Id))
                {
                    //协助者
                    CanUpFile = CanUpdate = "Visible";
                    return;
                }
            }
        }
    }
}
