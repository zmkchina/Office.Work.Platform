using System;
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
        private PageEditPlanVM _PageEditPlanVM = null;
        public PageEditPlan(Lib.Plan P_Plan = null)
        {
            InitializeComponent();
            _PageEditPlanVM = new PageEditPlanVM(P_Plan);
        }
        private  void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _PageEditPlanVM.InitPropValueAsync();
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
             (new WinMsgDialog(JsonResult.Msg)).ShowDialog();
            BtnAddPlan.IsEnabled = true;
        }
        /// <summary>
        /// 继续新增计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void BtnAddContinue_Click(object sender, RoutedEventArgs e)
        {
            _PageEditPlanVM = new PageEditPlanVM(null);
            _PageEditPlanVM.InitPropValueAsync();
            DataContext = _PageEditPlanVM;
        }
    }


    public class PageEditPlanVM : NotificationObject
    {
        public PageEditPlanVM(Lib.Plan CurPlan)
        {
            EntityPlan = CurPlan;
        }
        public void InitPropValueAsync()
        {
            if (EntityPlan != null)
            {
                IsEditFlag = true;
                //设置查询条件类
            }
            else
            {
                IsEditFlag = false;
                EntityPlan = new Lib.Plan()
                {
                    CreateUserId = AppSettings.LoginUser.Id,
                    ResponsiblePerson = AppSettings.LoginUser.Id,
                    Department = AppSettings.LoginUser.Department,
                    BeginDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(10),
                    FinishNote = "",
                    CurrectState = PlanStatus.WaitBegin,
                    ReadGrant = "all"
                };
            }
            InitSelectUserList();
        }
        #region "属性"
        public string[] WorkContentTypes { get { return AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries); } }
        public string[] PlanStateTypes { get { return PlanStatus.PlanStatusArr; } }
        /// <summary>
        /// 是编辑还是新增一个计划。
        /// </summary>
        public bool IsEditFlag { get; set; }
        public Lib.Plan EntityPlan { get; set; }

        /// <summary>
        /// 有权读取该计划的用户选择
        /// </summary>
        public ObservableCollection<SelectObj<User>> UserGrantSelectList { get; set; }
        /// <summary>
        /// 该计划的协助用户选择标志
        /// </summary>
        public ObservableCollection<SelectObj<User>> UserHelperSelectList { get; set; }
        #endregion


        #region "方法"
        public void InitSelectUserList()
        {
            UserGrantSelectList = new ObservableCollection<SelectObj<User>>();
            UserHelperSelectList = new ObservableCollection<SelectObj<User>>();
            foreach (User item in AppSettings.SysUsers.Where(e => !e.Id.Equals("admin", StringComparison.Ordinal)).OrderBy(x => x.OrderIndex))
            {
                UserGrantSelectList.Add(new SelectObj<User>(EntityPlan.ReadGrant != null && (EntityPlan.ReadGrant.Contains(item.Id) || EntityPlan.ReadGrant.Equals("all", StringComparison.Ordinal)), item));
                UserHelperSelectList.Add(new SelectObj<User>(EntityPlan.Helpers != null && (EntityPlan.Helpers.Contains(item.Id) || EntityPlan.Helpers.Equals("all", StringComparison.Ordinal)), item));
            }
        }
        public string GetSelectUserIds(ObservableCollection<SelectObj<User>> UserSelectList)
        {
            List<string> SelectIds = UserSelectList.Where(x => x.IsSelect).Select(y => y.Obj.Id).ToList();

            return string.Join(",", SelectIds.ToArray());
        }
        #endregion
    }
}
