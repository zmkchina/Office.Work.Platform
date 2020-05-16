using System.Drawing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

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

            TableRow TempRow = new TableRow();
            Paragraph TempParagraph = new Paragraph(new Run("防伪标志✦☪☸☭☢"))
            {
                // Style = FlowDoc.FindResource("PgStyle") as Style
            };
            TempRow.Cells.Add(new TableCell(TempParagraph)
            {
                ColumnSpan = 6,
                BorderBrush =System.Windows.Media.Brushes.Transparent,
                Padding=new Thickness(1,20,0,0),
                Foreground= System.Windows.Media.Brushes.Bisque

            });
            TableRowGroup TRGroupHeader = FlowDoc.FindName("TableBottomRows") as TableRowGroup;
            TRGroupHeader.Rows.Add(TempRow);

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
