using System;
using System.ComponentModel;
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
        private readonly ModelPlan _CurPlan;
        private PageEditPlanVM _PageEditPlanVM = null;
        public PageEditPlan(ModelPlan P_Plan = null)
        {
            InitializeComponent();
            col_fileInfo.Width = new GridLength(0);
            _CurPlan = P_Plan;
        }
        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            _PageEditPlanVM = new PageEditPlanVM();
            await _PageEditPlanVM.InitPropValueAsync(_CurPlan);
            this.DataContext = _PageEditPlanVM;
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

            ModelResult JsonResult = await DataPlanRepository.AddOrUpdatePlan(_PageEditPlanVM.EntityPlan);

            if (JsonResult.State == 0)
            {
                _PageEditPlanVM.StrPlanSaved = "Visible";
                MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(JsonResult.Msg, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            BtnAddPlan.IsEnabled = true;
        }
        private void BtnUpFile_Click(object sender, RoutedEventArgs e)
        {
            WinUpLoadFile winUpLoadFile = new WinUpLoadFile((upFile) =>
            {
                _PageEditPlanVM.UploadFiles.Add(upFile); LB_FileList.Items.Refresh();
            },
            _PageEditPlanVM.EntityPlan.Id,
            "计划附件",
            _PageEditPlanVM.EntityPlan.PlanType);
            winUpLoadFile.ShowDialog();
        }
        /// <summary>
        /// 继续新增计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddContinue_Click(object sender, RoutedEventArgs e)
        {
            _PageEditPlanVM = new PageEditPlanVM();
            _ = _PageEditPlanVM.InitPropValueAsync();
            DataContext = _PageEditPlanVM;
        }

        private void LB_FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UCFileInfo.Init_FileInfo((ModelFile)LB_FileList.SelectedItem, _PageEditPlanVM.EntityPlan, (DelFile) =>
             {
                 _PageEditPlanVM.UploadFiles.Remove(DelFile);
                 col_fileInfo.Width = new GridLength(0);
                 col_fileInfo.MinWidth = 0d;
             });
            if (col_fileInfo.Width.Value == 0)
            {
                col_fileInfo.Width = new GridLength(1, GridUnitType.Star);
                col_fileInfo.MinWidth = 200d;
            }
        }


    }
}
