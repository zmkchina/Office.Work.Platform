using System.Windows.Documents;

namespace Office.Work.Platform.Member
{
    public class PrintMemberSheetRender
    {

        private DocViewModel DocVM = null;
        public void Render(FlowDocument FlowDoc, string Caption, string DateStr, Lib.Member CurMember)
        {
            if (CurMember == null) return;
            DocVM = new DocViewModel(CurMember);
            if (string.IsNullOrWhiteSpace(Caption))
            {
                DocVM.Caption = "干部（职工）基本信息登记表";
            }
            else
            {
                DocVM.Caption = Caption;
            }
            DocVM.DateStr = DateStr;
           FlowDoc.DataContext = DocVM;
        }
        private class DocViewModel
        {
            public DocViewModel(Lib.Member CurMember)
            {
                this.CurMember = CurMember;
            }
            public Lib.Member CurMember { get; set; }
            public string Caption { get; set; }
            public string DateStr { get; set; }
        }
    }
}
