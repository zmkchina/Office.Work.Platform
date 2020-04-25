using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Files
{
    public class PageFilesListVM : NotificationObject
    {

        public string[] FileContentTypes => AppSettings.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
        public ObservableCollection<PlanFile> EntityFiles { get; set; }

        public PlanFileSearch mSearchFile { get; set; }
        #region "方法"
        /// <summary>
        /// 构造函数
        /// </summary>
        public PageFilesListVM()
        {
            EntityFiles = new ObservableCollection<PlanFile>();
            mSearchFile = new PlanFileSearch();
        }
        public async Task GetFilesAsync()
        {
            var files = await DataPlanFileRepository.ReadFiles(mSearchFile);
            files.ToList().ForEach(e =>
            {
                EntityFiles.Add(e);
            });
        }
        #endregion
    }
}
