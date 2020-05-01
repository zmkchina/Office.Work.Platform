using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Handlers;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Plan
{
    /// <summary>
    /// WinUpLoadFile.xaml 的交互逻辑
    /// </summary>
    public partial class PageEditPlan : Page
    {
        private PageEditPlanVM _PageEditPlanVM = null;
        public PageEditPlan(Lib.Plan P_Plan = null)
        {
            InitializeComponent();
            _PageEditPlanVM = new PageEditPlanVM(P_Plan);
        }
        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await _PageEditPlanVM.InitPropValueAsync();
            DataContext = _PageEditPlanVM;
            this.TBPlanCaption.Focus();
        }
        /// <summary>
        /// 保存新增的计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddPlan_Click(object sender, RoutedEventArgs e)
        {
            BtnAddPlan.IsEnabled = false;

            if (!_PageEditPlanVM.EntityPlan.ModelIsValid())
            {
                (new WinMsgDialog("输入不正确，请完善或更正相关数据！", "警告")).ShowDialog();
                BtnAddPlan.IsEnabled = true;
                return;
            }
            _PageEditPlanVM.EntityPlan.CurrectState = string.IsNullOrWhiteSpace(_PageEditPlanVM.EntityPlan.CurrectState) ? PlanStatus.Running : PlanStatus.WaitBegin;

            _PageEditPlanVM.EntityPlan.Helpers = _PageEditPlanVM.GetSelectUserIds(_PageEditPlanVM.UserHelperSelectList);

            List<SelectObj<User>> NeedAddUsers = _PageEditPlanVM.UserHelperSelectList.Where(x => x.IsSelect && !_PageEditPlanVM.UserGrantSelectList.Where(y => y.IsSelect).Contains(x)).ToList();

            NeedAddUsers.ForEach(x =>
            {
                _PageEditPlanVM.UserGrantSelectList.Where(y => y.Obj.Id.Equals(x.Obj.Id, StringComparison.Ordinal)).FirstOrDefault().IsSelect = true;
            });

            _PageEditPlanVM.EntityPlan.ReadGrant = _PageEditPlanVM.GetSelectUserIds(_PageEditPlanVM.UserGrantSelectList);


            if (!_PageEditPlanVM.EntityPlan.ReadGrant.Contains(AppSettings.LoginUser.Id))
            {
                (new WinMsgDialog("你本人必须有读取该计划的权限！", Caption: "警告")).ShowDialog();
                BtnAddPlan.IsEnabled = true;
                return;
            }
            ExcuteResult JsonResult = new ExcuteResult();
            if (_PageEditPlanVM.IsEditFlag)
            {
                //更新计划
                JsonResult = await DataPlanRepository.UpdatePlan(_PageEditPlanVM.EntityPlan);

            }
            else
            {
                //新增计划
                JsonResult = await DataPlanRepository.AddNewPlan(_PageEditPlanVM.EntityPlan);
                if (JsonResult.State == 0)
                {
                    //如服务器保存成功，将返回服务器为计划计划生成的Id号。更新到当前计划，以便供编辑计划使用。
                    _PageEditPlanVM.EntityPlan.Id = JsonResult.Tag;
                }
            }
            if (JsonResult.State == 0)
            {
                _PageEditPlanVM.IsEditFlag = true;
                foreach (PlanFile FileItem in _PageEditPlanVM.PlanFiles)
                {
                    if (FileItem.UpIntProgress == 0)//只上传未上传过的文件。
                    {
                        ProgressMessageHandler UpProgress = new System.Net.Http.Handlers.ProgressMessageHandler();
                        UpProgress.HttpSendProgress += (object sender, System.Net.Http.Handlers.HttpProgressEventArgs e) =>
                        {
                            FileItem.UpIntProgress = e.ProgressPercentage;
                        };
                        FileItem.PlanId = _PageEditPlanVM.EntityPlan.Id;
                        ExcuteResult result = await DataPlanFileRepository.UpLoadFileInfo(FileItem, FileItem.FileInfo.OpenRead(), "planfile", "pf", UpProgress);
                        if (result == null || result.State != 0)
                        {
                            FileItem.UpIntProgress = 0;
                        }
                        else
                        {
                            FileItem.Id = result.Tag;
                        }
                    }
                }
            }
             (new WinMsgDialog(JsonResult.Msg)).ShowDialog();
            BtnAddPlan.IsEnabled = true;
        }
        /// <summary>
        /// 继续新增计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddContinue_ClickAsync(object sender, RoutedEventArgs e)
        {
            _PageEditPlanVM = new PageEditPlanVM(null);
            await _PageEditPlanVM.InitPropValueAsync();
            DataContext = _PageEditPlanVM;
        }

        //上传文件到内存中。
        private void upFiles_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            System.IO.FileInfo theFile = FileOperation.SelectFile();
            if (theFile != null)
            {
                _PageEditPlanVM.PlanFiles.Add(new PlanFile()
                {
                    Name = theFile.Name.Substring(0, theFile.Name.LastIndexOf('.')),
                    UserId = AppSettings.LoginUser.Id,
                    Length = theFile.Length,
                    ExtendName = theFile.Extension,
                    PlanId = _PageEditPlanVM.EntityPlan.Id,
                    FileInfo = theFile,
                    UpIntProgress = 0,
                    DownIntProgress = 100
                });
            }
        }
        //打开选定的文件
        private async void OpenFile_PreviewMouseLeftButtonUpAsync(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            TextBlock tb = sender as TextBlock;
            PlanFile SelectFile = tb.DataContext as PlanFile;
            if (SelectFile.FileInfo != null && System.IO.File.Exists(SelectFile.FileInfo.FullName))
            {
                FileOperation.UseDefaultAppOpenFile(SelectFile.FileInfo.FullName);
                return;
            }
            //否则下载该文件（在编辑已有计划时可能会有此操作）
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

        }
        //删除选定的文件
        private async void deleFile_PreviewMouseLeftButtonUpAsync(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            e.Handled = true;
            Image img = sender as Image;
            PlanFile pf = img.DataContext as PlanFile;
            if (pf.UpIntProgress == 100)//说明该文件已经上传到了服务器，需删除之
            {
                ExcuteResult excuteResult = await DataPlanFileRepository.DeleteFileInfo(pf);
                if (excuteResult.State == 0)
                {
                    _PageEditPlanVM.PlanFiles.Remove(pf);
                }
            }
            else
            {
                _PageEditPlanVM.PlanFiles.Remove(pf);
            }
        }
    }
}
