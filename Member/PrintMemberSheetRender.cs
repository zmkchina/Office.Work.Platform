using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;

namespace Office.Work.Platform.Member
{
    public class PrintMemberSheetRender
    {

        private DocViewModel DocVM = null;
        public void Render(FlowDocument FlowDoc, string Caption, string DateStr, MemoryStream UserHeadStream, Lib.Member CurMember)
        {
            if (CurMember == null) return;
            DocVM = new DocViewModel(CurMember);
            if (UserHeadStream != null)
            {
                //ImageBrush Img_UserPhotoImage = FlowDoc.FindName("Img_UserPhotoImage") as ImageBrush;
                //ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                //Img_UserPhotoImage.ImageSource = (ImageSource)imageSourceConverter.ConvertFrom(UserHeadStream);
                //Img_UserPhotoImage.Freeze();
            }
            //设置标题

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
            //填充职工简历
            var TempResumes = DataMemberResumeRepository.GetRecords(new Lib.MemberResumeSearch()
            {
                UserId = AppSet.LoginUser.Id,
                MemberId = CurMember.Id
            }).Result;
            if (TempResumes != null && TempResumes.Count() > 0)
            {
                TableCell Cell_Resume = FlowDoc.FindName("Cell_Resume") as TableCell;
                List<Lib.MemberResume> MemberResumes = TempResumes.OrderBy(x => x.BeginDate).ToList();
                for (int i = 0; i < MemberResumes.Count; i++)
                {
                    Paragraph TParagraph = new Paragraph();
                    TParagraph.TextAlignment = TextAlignment.Left;
                    TParagraph.Inlines.Add(new Run(MemberResumes[i].BeginDate.ToString("yyyy年MM月—")));
                    TParagraph.Inlines.Add(new Run(MemberResumes[i].EndDate.ToString("yyyy年MM月")));
                    TParagraph.Inlines.Add(new Run(MemberResumes[i].Content));
                    if (!string.IsNullOrWhiteSpace(MemberResumes[i].Remark))
                    {
                        TParagraph.Inlines.Add(new Run($"({MemberResumes[i].Remark})"));
                    }
                    Cell_Resume.Blocks.Add(TParagraph);
                }
            }
            //填充职工奖惩信息
            var TempPrizePunish = DataMemberPrizePunishRepository.GetRecords(new Lib.MemberPrizePunishSearch()
            {
                UserId = AppSet.LoginUser.Id,
                MemberId = CurMember.Id
            }).Result;
            if (TempPrizePunish != null && TempPrizePunish.Count() > 0)
            {
                TableCell Cell_PrizePunish = FlowDoc.FindName("Cell_PrizePunish") as TableCell;
                List<Lib.MemberPrizePunish> MemberPrizePunishs = TempPrizePunish.OrderByDescending(x => x.PrizrOrPunishType).ToList();
                for (int i = 0; i < MemberPrizePunishs.Count; i++)
                {
                    Paragraph TParagraph = new Paragraph();
                    TParagraph.TextAlignment = TextAlignment.Left;
                    TParagraph.Inlines.Add(new Run(MemberPrizePunishs[i].OccurDate.ToString("yyyy年MM月")));
                    TParagraph.Inlines.Add(new Run(MemberPrizePunishs[i].PrizrOrPunishName));
                    TParagraph.Inlines.Add(new Run(MemberPrizePunishs[i].PrizrOrPunishUnit));
                    if (!string.IsNullOrWhiteSpace(MemberPrizePunishs[i].Remark))
                    {
                        TParagraph.Inlines.Add(new Run($"({MemberPrizePunishs[i].Remark})"));
                    }
                    Cell_PrizePunish.Blocks.Add(TParagraph);
                }
            }
            //填充职工年度考核信息
            var TempAppraise = DataMemberAppraiseRepository.GetRecords(new Lib.MemberAppraiseSearch()
            {
                UserId = AppSet.LoginUser.Id,
                MemberId = CurMember.Id
            }).Result;
            if (TempAppraise != null && TempAppraise.Count() > 0)
            {
                TableCell Cell_Appraise = FlowDoc.FindName("Cell_Appraise") as TableCell;
                List<Lib.MemberAppraise> MemberAppraises = TempAppraise.OrderByDescending(x => x.Year).ToList();
                for (int i = 0; i < MemberAppraises.Count; i++)
                {
                    Paragraph TParagraph = new Paragraph();
                    TParagraph.TextAlignment = TextAlignment.Left;
                    TParagraph.Inlines.Add(new Run(MemberAppraises[i].Year));
                    TParagraph.Inlines.Add(new Run(MemberAppraises[i].Result));
                    if (!string.IsNullOrWhiteSpace(MemberAppraises[i].Remark))
                    {
                        TParagraph.Inlines.Add(new Run($"({MemberAppraises[i].Remark})"));
                    }
                    Cell_Appraise.Blocks.Add(TParagraph);
                }
            }
            //填充职工社会关系
            var TempRelations = DataMemberRelationsRepository.GetRecords(new Lib.MemberRelationsSearch()
            {
                UserId = AppSet.LoginUser.Id,
                MemberId = CurMember.Id
            }).Result;
            if (TempRelations != null && TempRelations.Count() > 0)
            {
                TableCell Cell_Appraise = FlowDoc.FindName("Cell_Relations") as TableCell;
                List<Lib.MemberRelations> MemberRelationses = TempRelations.OrderBy(x => x.OrderIndex).ToList();
                for (int i = 0; i < MemberRelationses.Count; i++)
                {
                    Paragraph TParagraph = new Paragraph();
                    TParagraph.TextAlignment = TextAlignment.Left;
                    TParagraph.Inlines.Add(new Run(MemberRelationses[i].Relation));
                    TParagraph.Inlines.Add(new Run(MemberRelationses[i].Name));
                    TParagraph.Inlines.Add(new Run(MemberRelationses[i].UnitName));
                    TParagraph.Inlines.Add(new Run(MemberRelationses[i].Role));
                    if (!string.IsNullOrWhiteSpace(MemberRelationses[i].Remark))
                    {
                        TParagraph.Inlines.Add(new Run($"({MemberRelationses[i].Remark})"));
                    }
                    Cell_Appraise.Blocks.Add(TParagraph);
                }
            }
            //添加防伪标志
            TableRow TempRow = new TableRow();
            Paragraph TempParagraph = new Paragraph(new Run("FW✦☪☸☭☢"))
            {
                // Style = FlowDoc.FindResource("PgStyle") as Style
            };
            TempRow.Cells.Add(new TableCell(TempParagraph)
            {
                ColumnSpan = 6,
                BorderBrush = System.Windows.Media.Brushes.Transparent,
                Padding = new Thickness(1, 20, 0, 0),
                Foreground = System.Windows.Media.Brushes.Bisque

            });
            TableRowGroup TRGroupHeader = FlowDoc.FindName("TableBottomRows") as TableRowGroup;
            TRGroupHeader.Rows.Add(TempRow);
        }
        private class DocViewModel
        {
            public DocViewModel(Lib.Member CurMember)
            {
                this.CurMember = CurMember;
                UseHeadImage = new BitmapImage();
            }
            public string Caption { get; set; }
            public string DateStr { get; set; }
            public Lib.Member CurMember { get; set; }
            /// <summary>
            /// 显示的用户图片
            /// </summary>
            public BitmapImage UseHeadImage { get; set; }
        }
    }
}
