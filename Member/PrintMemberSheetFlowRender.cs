using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;

namespace Office.Work.Platform.Member
{
    public class PrintMemberSheetFlowRender
    {

        private DocViewModel DocVM = null;
        public void Render(FlowDocument FlowDoc, string Caption, string DateStr, MemoryStream UserHeadStream, Lib.MemberInfoEntity PCurMember)
        {
            if (PCurMember == null) return;

            DocVM = new DocViewModel
            {
                CurMember = PCurMember
            };

            //设置标题

            if (string.IsNullOrWhiteSpace(Caption))
            {
                DocVM.Caption = "事 业 编 制 人 员 基 本 情 况 表";
            }
            else
            {
                DocVM.Caption = Caption;
            }
            DocVM.DateStr = DateStr;

            //设置员工头像
            if (UserHeadStream != null)
            {
                //TableCell Cell_UserPhoto = FlowDoc.FindName("UserPhotoCell") as TableCell;
                //Paragraph TParagraph = new Paragraph
                //{
                //    TextAlignment = TextAlignment.Center
                //};
                //ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                //ImageBrush HeadImgBrush = new ImageBrush((ImageSource)imageSourceConverter.ConvertFrom(UserHeadStream));
                //Border HeadImg = new Border
                //{
                //    Width = 120,
                //    Height = 170,
                //    Background = HeadImgBrush
                //};
                //TParagraph.Inlines.Add(HeadImg);
                //Cell_UserPhoto.Blocks.Add(TParagraph);

            }
            //填充职工简历
            var TempResumes = DataMemberResumeRepository.GetRecords(new Lib.MemberResumeSearch()
            {
                UserId = AppSet.LoginUser.Id,
                MemberId = DocVM.CurMember.Id
            }).Result;
            TableCell Cell_Resume = FlowDoc.FindName("Cell_Resume") as TableCell;
            if (TempResumes != null && TempResumes.Count() > 0)
            {

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
            else
            {
                Paragraph TParagraph = new Paragraph();
                TParagraph.TextAlignment = TextAlignment.Left;
                TParagraph.Inlines.Add(new TextBlock() { Height = 400 });
                Cell_Resume.Blocks.Add(TParagraph);
            }
            //填充职工奖惩信息
            var TempPrizePunish = DataMemberPrizePunishRepository.GetRecords(new Lib.MemberPrizePunishSearch()
            {
                UserId = AppSet.LoginUser.Id,
                MemberId = DocVM.CurMember.Id
            }).Result;
            if (TempPrizePunish != null && TempPrizePunish.Count() > 0)
            {
                TableCell Cell_PrizePunish = FlowDoc.FindName("Cell_PrizePunish") as TableCell;
                List<Lib.MemberPrizePunishEntity> MemberPrizePunishs = TempPrizePunish.OrderByDescending(x => x.PrizrOrPunishType).ToList();
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
                MemberId = DocVM.CurMember.Id
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
                MemberId = DocVM.CurMember.Id
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

            FlowDoc.DataContext = DocVM;
        }
        private class DocViewModel
        {
            public string Caption { get; set; }
            public string DateStr { get; set; }
            public Lib.MemberInfoEntity CurMember { get; set; }
            /// <summary>
            /// 显示的用户图片
            /// </summary>
            public BitmapImage UseHeadImage { get; set; }
        }
    }
}
