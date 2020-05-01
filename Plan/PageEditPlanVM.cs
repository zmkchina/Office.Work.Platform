using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Plan
{
    public class PageEditPlanVM : NotificationObject
    {
        public PageEditPlanVM(Lib.Plan CurPlan)
        {
            EntityPlan = CurPlan;
            PlanFiles = new ObservableCollection<PlanFile>();
        }
        public async Task InitPropValueAsync()
        {
            if (EntityPlan != null)
            {
                IsEditFlag = true;
                //设置查询条件类
                PlanFiles.Clear();
                PlanFileSearch mSearchFile = new PlanFileSearch();
                mSearchFile.UserId = AppSettings.LoginUser.Id;
                mSearchFile.PlanId = EntityPlan.Id;
                IEnumerable<PlanFile> UpFiles = await DataPlanFileRepository.ReadFiles(mSearchFile);
                UpFiles.ToList().ForEach(e =>
                {
                    e.UpIntProgress = 100;
                    PlanFiles.Add(e);
                });

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
        /// 当前计划的文件。
        /// </summary>
        public ObservableCollection<PlanFile> PlanFiles { get; set; }
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
