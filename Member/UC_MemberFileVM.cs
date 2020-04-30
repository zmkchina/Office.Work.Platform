using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Member
{
    public class UC_MemberFileVM : NotificationObject
    {
        public UC_MemberFileVM()
        {
            MFiles = new ObservableCollection<MemberFile>();
        }
        public async void Init_MemberFileVMAsync(string MemberId, string FileType, string OtherRecordId = null, bool ReadFlag = true)
        {
            this.MemberId = MemberId;
            this.FileType = FileType;
            if (!ReadFlag) { return; }
            MemberFileSearch mfsearch = new MemberFileSearch()
            {
                MemberId = MemberId,
                FileType = FileType,
                OtherRecordId = OtherRecordId
            };
            await SearchMemberFiles(mfsearch);
        }
        public async System.Threading.Tasks.Task SearchMemberFiles(MemberFileSearch mfsearch)
        {
            if (mfsearch != null)
            {
                mfsearch.MemberId = MemberId;
                mfsearch.FileType = FileType;
                mfsearch.UserId = AppSettings.LoginUser.Id;

                IEnumerable<MemberFile> MemberPayTemps = await DataMemberFileRepository.ReadFiles(mfsearch);
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
        public string MemberId { get; set; }
        public string FileType { get; set; }

        //定义查询内容字符串
        public string SearchValues { get; set; }
        public ObservableCollection<MemberFile> MFiles { get; set; }

    }

}
