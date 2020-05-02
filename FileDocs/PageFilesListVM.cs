using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.FileDocs
{
    public class PageFilesListVM : NotificationObject
    {

        public ObservableCollection<FileDoc> FileDocs { get; set; }
        public PageFilesListVM()
        {
            FileDocs = new ObservableCollection<FileDoc>();
        }
        public async Task GetFilesAsync(FileDocSearch mSearchFile)
        {
            FileDocs.Clear();
            var files = await DataFileDocRepository.ReadFiles(mSearchFile);
            if (files != null)
            {
                files.ToList().ForEach(e =>
                {
                    FileDocs.Add(e);
                });
            }
        }
    }
}
