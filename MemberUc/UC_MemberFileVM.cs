using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberUc
{
    public class UC_MemberFileVM : NotificationObject
    {
        public UC_MemberFileVM()
        {
            MFiles = new ObservableCollection<FileDoc>();
        }
        public async System.Threading.Tasks.Task Init_MemberFileVMAsync(Lib.Member PMember, string PContentType, bool ReadFlag = true)
        {
            CurMember = PMember;
            ContentType = PContentType;
            if (!ReadFlag) { return; }
            FileDocSearch mfsearch = new FileDocSearch()
            {
                OwnerId = PMember.Id,
                OwnerType = "人事附件",
                ContentType = PContentType,
                UserId = AppSettings.LoginUser.Id
            };
            await SearchMemberFiles(mfsearch);
        }
        public async System.Threading.Tasks.Task SearchMemberFiles(FileDocSearch mfsearch)
        {
            if (mfsearch != null)
            {
                mfsearch.UserId = AppSettings.LoginUser.Id;

                IEnumerable<FileDoc> MemberPayTemps = await DataFileDocRepository.ReadFiles(mfsearch);
                MFiles.Clear();
                MemberPayTemps.ToList().ForEach(e =>
                {
                    MFiles.Add(e);
                });
            }
        }
        /// <summary>
        /// 当前所选信息
        /// </summary>
        public Lib.Member CurMember { get; set; }
        public string ContentType { get; set; }

        //定义查询内容字符串
        public string SearchValues { get; set; }
        public ObservableCollection<FileDoc> MFiles { get; set; }

    }

}
