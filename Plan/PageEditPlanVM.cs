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
        private string _StrPlanSaved;
        public PageEditPlanVM(ModelPlan NeedEditPlan = null)
        {
            PlayStateTypes = AppSettings.ServerSetting.PlanStateType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            WorkContentTypes = AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            if (NeedEditPlan != null)
            {
                EntityPlan = NeedEditPlan;
                Task.Run(async () =>
                {
                    //设置查询条件类
                    MSearchFile mSearchFile = new MSearchFile { OwnerType = "计划附件",OwnerId= EntityPlan.Id };

                    UploadFiles = await DataFileRepository.ReadFiles(mSearchFile);
                    UploadFiles ??= new ObservableCollection<ModelFile>();
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
                StrPlanSaved = "Hidden";
                UploadFiles = new ObservableCollection<ModelFile>();
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
                UserGrantSelectList.Add(new ModelSelectObj<ModelUser>
                {
                    IsSelect = EntityPlan.ReadGrant != null && EntityPlan.ReadGrant.Contains(item.Id),
                    Obj = item
                });
                UserHelperSelectList.Add(new ModelSelectObj<ModelUser>
                {
                    IsSelect = EntityPlan.Helpers != null && EntityPlan.Helpers.Contains(item.Id),
                    Obj = item
                });
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
