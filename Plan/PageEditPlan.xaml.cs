using System;
using System.Net.Http.Handlers;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Files;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Plan
{
    /// <summary>
    /// WinUpLoadFile.xaml 的交互逻辑
    /// </summary>
    public partial class PageEditPlan : Page
    {
        private readonly Lib.Plan _CurPlan;
        private PageEditPlanVM _PageEditPlanVM = null;
        public PageEditPlan(Lib.Plan P_Plan = null)
        {
            InitializeComponent();
            _CurPlan = P_Plan;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _PageEditPlanVM = new PageEditPlanVM();
            _PageEditPlanVM.InitPropValueAsync(_CurPlan);
            DataContext = _PageEditPlanVM;
        }
        /// <summary>
        /// 保存新增的计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddPlan_Click(object sender, RoutedEventArgs e)
        {
            BtnAddPlan.IsEnabled = false;
            if (_PageEditPlanVM.EntityPlan.PlanType == null)
            {
                MessageBox.Show("请选择计划类型！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                BtnAddPlan.IsEnabled = true;
                return;
            }
            _PageEditPlanVM.EntityPlan.Helpers = _PageEditPlanVM.GetSelectUserIds(_PageEditPlanVM.UserHelperSelectList);
            _PageEditPlanVM.EntityPlan.ReadGrant = _PageEditPlanVM.GetSelectUserIds(_PageEditPlanVM.UserGrantSelectList);
            if (!_PageEditPlanVM.EntityPlan.ReadGrant.Contains(AppSettings.LoginUser.Id))
            {
                if (MessageBox.Show("你本人没有读取该计划的权限，确认？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    BtnAddPlan.IsEnabled = true;
                    return;
                }
            }
            //保存计划
            ExcuteResult JsonResult = await DataPlanRepository.AddOrUpdatePlan(_PageEditPlanVM.EntityPlan);

            if (JsonResult.State == 0)
            {

                foreach (PlanFile item in _PageEditPlanVM.EntityPlan.Files)
                {
                    if (item.UpIntProgress == 0)//只上传未上传过的文件。
                    {
                        ProgressMessageHandler UpProgress = new System.Net.Http.Handlers.ProgressMessageHandler();
                        UpProgress.HttpSendProgress += (object sender, System.Net.Http.Handlers.HttpProgressEventArgs e) =>
                        {
                            item.UpIntProgress = e.ProgressPercentage;
                        };
                        ExcuteResult result = await DataPlanFileRepository.UpLoadFileInfo(item, item.FileInfo.OpenRead(), "planfile", "pf", UpProgress);
                        if (result.State != 0)
                        {
                            item.UpIntProgress = 0;
                        }
                    }
                }
                MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            BtnAddPlan.IsEnabled = true;
        }
        /// <summary>
        /// 继续新增计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddContinue_Click(object sender, RoutedEventArgs e)
        {
            _PageEditPlanVM = new PageEditPlanVM();
            _PageEditPlanVM.InitPropValueAsync(null);
            DataContext = _PageEditPlanVM;
        }

        //上传文件到内存中。
        private void upFiles_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            System.IO.FileInfo theFile = FileOperation.SelectFile();
            if (theFile != null)
            {
                _PageEditPlanVM.EntityPlan.Files.Add(new PlanFile()
                {
                    Id = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    Name = theFile.Name.Substring(0, theFile.Name.LastIndexOf('.')),
                    UserId = AppSettings.LoginUser.Id,
                    Length = theFile.Length,
                    ExtendName = theFile.Extension,
                    PlanId = _PageEditPlanVM.EntityPlan.Id,
                    FileInfo = theFile,
                    UpIntProgress = 0
                });
            }
        }
        //打开选定的文件
        private void openFile_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            TextBlock tb = sender as TextBlock;
            PlanFile pf = tb.DataContext as PlanFile;
            FileOperation.OpenFile(pf.FileInfo);
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
                    _PageEditPlanVM.EntityPlan.Files.Remove(pf);
                }
            }
            else
            {
                _PageEditPlanVM.EntityPlan.Files.Remove(pf);
            }
        }
    }
}
