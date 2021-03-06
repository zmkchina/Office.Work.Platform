﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private PageViewModel _PageViewModel = null;
        public PageEditPlan(Lib.PlanEntity P_Plan = null)
        {
            InitializeComponent();
            _PageViewModel = new PageViewModel(P_Plan);
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _PageViewModel.InitPropValue();
            TBPlanCaption.Focus();
            DataContext = _PageViewModel;
            AppFuns.SetStateBarText("录入或编辑一个工作计划。");
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("就绪。");
        }
        /// <summary>
        /// 保存新增的计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddPlan_Click(object sender, RoutedEventArgs e)
        {
            BtnAddPlan.IsEnabled = false;

            if (string.IsNullOrWhiteSpace(_PageViewModel.EntityPlan.Id))
            {
                //说明是新计划
                _PageViewModel.EntityPlan.CurrectState = PlanStatus.WaitBegin;
                _PageViewModel.EntityPlan.UnitName = AppSet.LoginUser.UnitName;
            }
            else if (_PageViewModel.EntityPlan.CurrectState.Equals(PlanStatus.Finished))
            {
                _PageViewModel.EntityPlan.CurrectState = PlanStatus.Running;
            }

            if (!_PageViewModel.EntityPlan.ModelIsValid())
            {
                AppFuns.ShowMessage("输入不正确，请完善或更正相关数据！", "警告");
                BtnAddPlan.IsEnabled = true;
                return;
            }
            

            _PageViewModel.EntityPlan.Helpers = _PageViewModel.GetSelectUserIds(_PageViewModel.UserHelperSelectList);

            List<SelectObj<Lib.UserEntity>> NeedAddUsers = _PageViewModel.UserHelperSelectList.Where(x => x.IsSelect && !_PageViewModel.UserGrantSelectList.Where(y => y.IsSelect).Contains(x)).ToList();

            NeedAddUsers.ForEach(x =>
            {
                _PageViewModel.UserGrantSelectList.Where(y => y.Obj.Id.Equals(x.Obj.Id, StringComparison.Ordinal)).FirstOrDefault().IsSelect = true;
            });

            _PageViewModel.EntityPlan.ReadGrant = _PageViewModel.GetSelectUserIds(_PageViewModel.UserGrantSelectList);


            if (!_PageViewModel.EntityPlan.ReadGrant.Contains(AppSet.LoginUser.Id))
            {
                AppFuns.ShowMessage("你本人必须有读取该计划的权限！", Caption: "警告");
                BtnAddPlan.IsEnabled = true;
                return;
            }
            ExcuteResult JsonResult = new ExcuteResult();
            if (_PageViewModel.IsEditFlag)
            {
                //更新计划
                JsonResult = await DataPlanRepository.UpdatePlan(_PageViewModel.EntityPlan);
            }
            else
            {
                //新增计划
                JsonResult = await DataPlanRepository.AddNewPlan(_PageViewModel.EntityPlan);
                if (JsonResult.State == 0)
                {
                    //如服务器保存成功，将返回服务器为计划计划生成的Id号。更新到当前计划，以便供编辑计划使用。
                    _PageViewModel.EntityPlan.Id = JsonResult.Tag;
                    _PageViewModel.IsEditFlag = true;
                }
            }
            BtnAddPlan.IsEnabled = true;
            AppFuns.ShowMessage(JsonResult.Msg);
        }
        /// <summary>
        /// 继续新增计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddContinue_Click(object sender, RoutedEventArgs e)
        {
            _PageViewModel = new PageViewModel(null);
            _PageViewModel.InitPropValue();
            DataContext = _PageViewModel;
        }

        //*****************************************************************************************************
        /// <summary>
        /// 类的内部类：本页面的视图模型类
        /// </summary>
        private class PageViewModel : NotificationObject
        {
            public PageViewModel(Lib.PlanEntity CurPlan)
            {
                EntityPlan = CurPlan;
                ServerSettings = AppSet.ServerSetting;
            }
            /// <summary>
            /// 初始化相关属性值
            /// </summary>
            public void InitPropValue()
            {
                if (EntityPlan != null) { IsEditFlag = true; }
                else
                {
                    IsEditFlag = false;
                    EntityPlan = new Lib.PlanEntity()
                    {
                        CreateUserId = AppSet.LoginUser.Id,
                        ResponsiblePerson = AppSet.LoginUser.Id,
                        Department = AppSet.LoginUser.Department,
                        BeginDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(10),
                        FinishNote = "",
                        CurrectState = PlanStatus.WaitBegin,
                        ReadGrant = "all"
                    };
                }
                UserGrantSelectList = new ObservableCollection<SelectObj<Lib.UserEntity>>();
                UserHelperSelectList = new ObservableCollection<SelectObj<Lib.UserEntity>>();
                foreach (Lib.UserEntity item in AppSet.SysUsers.Where(e => !e.Id.Equals("admin", StringComparison.Ordinal)).OrderBy(x => x.OrderIndex))
                {
                    UserGrantSelectList.Add(new SelectObj<Lib.UserEntity>(EntityPlan.ReadGrant != null && (EntityPlan.ReadGrant.Contains(item.Id) || EntityPlan.ReadGrant.Equals("all", StringComparison.Ordinal)), item));
                    UserHelperSelectList.Add(new SelectObj<Lib.UserEntity>(EntityPlan.Helpers != null && (EntityPlan.Helpers.Contains(item.Id) || EntityPlan.Helpers.Equals("all", StringComparison.Ordinal)), item));
                }
            }
          
            /// <summary>
            /// 是编辑还是新增一个计划。
            /// </summary>
            public bool IsEditFlag { get; set; }
            /// <summary>
            /// 当前正在操作的计划
            /// </summary>
            public Lib.PlanEntity EntityPlan { get; set; }

            /// <summary>
            /// 系统参数配置
            /// </summary>
            public Lib.SettingServerEntity ServerSettings { get; set; }

            /// <summary>
            /// 有权读取该计划的用户选择
            /// </summary>
            public ObservableCollection<SelectObj<Lib.UserEntity>> UserGrantSelectList { get; set; }
            /// <summary>
            /// 该计划的协助用户选择标志
            /// </summary>
            public ObservableCollection<SelectObj<Lib.UserEntity>> UserHelperSelectList { get; set; }
            /// <summary>
            /// 获取所有选中的用记信息
            /// </summary>
            /// <param name="UserSelectList"></param>
            /// <returns></returns>
            public string GetSelectUserIds(ObservableCollection<SelectObj<Lib.UserEntity>> UserSelectList)
            {
                List<string> SelectIds = UserSelectList.Where(x => x.IsSelect).Select(y => y.Obj.Id).ToList();
                return string.Join(",", SelectIds.ToArray());
            }
        }
    }
}
