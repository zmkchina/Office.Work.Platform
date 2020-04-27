using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.PlanFiles
{
    public class PageFilesListVM : NotificationObject
    {

        public ObservableCollection<PlanFile> EntityFiles { get; set; }
        public PageFilesListVM()
        {
            EntityFiles = new ObservableCollection<PlanFile>();
        }
        public async Task GetFilesAsync(PlanFileSearch mSearchFile)
        {
            EntityFiles.Clear();
            var files = await DataPlanFileRepository.ReadFiles(mSearchFile);
            files.ToList().ForEach(e =>
            {
                EntityFiles.Add(e);
            });
        }
    }
}
