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

        public ObservableCollection<PlanFile> PlanFiles { get; set; }
        public PageFilesListVM()
        {
            PlanFiles = new ObservableCollection<PlanFile>();
        }
        public async Task GetFilesAsync(PlanFileSearch mSearchFile)
        {
            PlanFiles.Clear();
            var files = await DataPlanFileRepository.ReadFiles(mSearchFile);
            files.ToList().ForEach(e =>
            {
                PlanFiles.Add(e);
            });
        }
    }
}
