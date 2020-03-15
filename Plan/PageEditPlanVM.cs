using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace Office.Work.Platform.Plan
{
    public class PageEditPlanVM : NotificationObject
    {
        private string _StrPlanSaved = "Visibled";

        public PageEditPlanVM()
        {
          
        }

        public async System.Threading.Tasks.Task InitPropValueAsync(ModelPlan NeedEditPlan = null)
        {
            PlayStateTypes = AppSettings.ServerSetting.PlanStateType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            WorkContentTypes = AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            UploadFiles = new ObservableCollection<ModelFile>();
            if (NeedEditPlan != null)
            {
                EntityPlan = NeedEditPlan;
                //设置查询条件类
                MSearchFile mSearchFile = new MSearchFile { OwnerType = "计划附件", OwnerId = EntityPlan.Id };
                IEnumerable<ModelFile> UpFiles = await DataFileRepository.ReadFiles(mSearchFile);
                UpFiles.ToList().ForEach(e =>
                {
                    UploadFiles.Add(e);
                });
            }
            else
            {
                EntityPlan = new ModelPlan
                {
                    Id = DateTime.Now.ToString("yyyyMMddHHmmssffff"),
                    CreateUserId = AppSettings.LoginUser.Id,
                    ResponsiblePerson = AppSettings.LoginUser.Id,
                    Department = AppSettings.LoginUser.Department,
                    BeginDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(10),
                    FinishNote = "",
                    CurrectState = PlayStateTypes[0]
                };
                StrPlanSaved = "Collapsed";
            }
            InitSelectUserList();
        }
        #region "属性"
        public string[] WorkContentTypes { get; set; }
        public string[] PlayStateTypes { get; set; }
        public ModelPlan EntityPlan { get; set; }
        /// <summary>
        /// 属于该计划的上传文件集合
        /// </summary>
        public ObservableCollection<ModelFile> UploadFiles { get; set; }

        /// <summary>
        /// 有权读取该计划的用户选择
        /// </summary>
        public ObservableCollection<ModelSelectObj<ModelUser>> UserGrantSelectList { get; set; }
        /// <summary>
        /// 该计划的协助用户选择标志
        /// </summary>
        public ObservableCollection<ModelSelectObj<ModelUser>> UserHelperSelectList { get; set; }
        /// <summary>
        /// 计划是否已保存了。
        /// </summary>
        public string StrPlanSaved
        {
            get { return _StrPlanSaved; }
            set { _StrPlanSaved = value; this.RaisePropertyChanged(); }
        }
        #endregion


        #region "方法"
        public void InitSelectUserList()
        {
            UserGrantSelectList = new ObservableCollection<ModelSelectObj<ModelUser>>();
            UserHelperSelectList = new ObservableCollection<ModelSelectObj<ModelUser>>();
            foreach (ModelUser item in AppSettings.SysUsers)
            {
                UserGrantSelectList.Add(new ModelSelectObj<ModelUser>(EntityPlan.ReadGrant != null && EntityPlan.ReadGrant.Contains(item.Id), item));
                UserHelperSelectList.Add(new ModelSelectObj<ModelUser>(EntityPlan.Helpers != null && EntityPlan.Helpers.Contains(item.Id), item));
            }
        }
        public string GetSelectUserIds(ObservableCollection<ModelSelectObj<ModelUser>> UserSelectList)
        {
            List<string> SelectIds = UserSelectList.Where(x => x.IsSelect).Select(y => y.Obj.Id).ToList();

            return string.Join(",", SelectIds.ToArray());
        }


        #endregion
    }
}
