using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Xml;
using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Dml.WordProcessing;
using NPOI.Util;
using NPOI.XWPF.UserModel;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// 职工基本信息表
    /// </summary>
    public partial class PageMemberSheetFixed : Page
    {
        private MemberSearch _MemberSearch;
        private FixedDocument _FixedDoc = null;
        private FixedDocViewModel _FixedDocVM = null;
        public PageMemberSheetFixed()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _MemberSearch = new MemberSearch();
            this.DataContext = _MemberSearch;
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _FixedDoc = null;
            _FixedDocVM = null;
            AppFuns.SetStateBarText("就绪");
        }
        /// <summary>
        /// 查询待发放信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_MemberSearch.Id)) { return; }
            List<Lib.Member> Members = await DataMemberRepository.ReadMembers(_MemberSearch);
            if (Members == null || Members.Count < 1)
            {
                AppFuns.ShowMessage("未找到此用户信息！");
                return;
            }
            AppFuns.SetStateBarText($"查看或打印[{Members[0].Name}]信息表。");

            _FixedDocVM = new FixedDocViewModel(Members[0]);
            await _FixedDocVM.InitPropsAsync();
            //设定打印标题
            _FixedDocVM.PrintCaption = $"事 业 编 制 人 员 基 本 情 况 表";
            if (_FixedDocVM.CurMember.MemberType.Equals("劳动合同制"))
            {
                _FixedDocVM.PrintCaption = $"劳 动 用 工 人 员 基 本 情 况 表";
            }
            if (_FixedDocVM.CurMember.MemberType.Equals("劳务派遣制"))
            {
                _FixedDocVM.PrintCaption = $"劳 务 用 工 人 员 基 本 情 况 表";
            }
            //设定打印日期
            _FixedDocVM.PrintDate = DateTime.Now;
            //导入格式模板
            string SheetTemplet = "PrintMemberSheetFixedDot.xaml";
            _FixedDoc = (FixedDocument)Application.LoadComponent(new Uri($"/Office.Work.Platform;component/Member/{SheetTemplet}", UriKind.RelativeOrAbsolute));
            _FixedDoc.AddPages();
            _FixedDoc.DataContext = _FixedDocVM;
            docViewer.Document = _FixedDoc;
            this.BtnExport.IsEnabled = true;
        }

        private void BtnExportClick(object sender, RoutedEventArgs e)
        {
            if (_FixedDoc == null)
            {
                this.BtnExport.IsEnabled = false;
                return;
            }
            System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();
            fileDialog.Filter = "Docx|*.Docx";
            fileDialog.FileName = _FixedDocVM.CurMember.Name;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            try
            {
                this.BtnExport.IsEnabled = false;
                string MemberTemplet = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OWPMemberInfo.dat");
                if (!File.Exists(MemberTemplet))
                {
                    this.BtnExport.IsEnabled = true;
                    AppFuns.ShowMessage("未找到欲导出的模板文件！", Caption: "失败");
                    return;
                }
                FileStream FStream = File.OpenRead(MemberTemplet);
                if (FStream == null)
                {
                    this.BtnExport.IsEnabled = true;
                    AppFuns.ShowMessage("读取模板文件时出现错误，是否文件正在使用？", Caption: "失败");
                    return;
                }
                InputStream WordStream = new FileInputStream(FStream);
                if (WordStream == null)
                {
                    this.BtnExport.IsEnabled = true;
                    AppFuns.ShowMessage("转换模板文件成Word对象流时出现错误？", Caption: "失败");
                    return;
                }
                XWPFDocument WDoc = new XWPFDocument(WordStream);
                if (WDoc == null)
                {
                    this.BtnExport.IsEnabled = true;
                    AppFuns.ShowMessage("转换模板文件成Word对象时出现错误？", Caption: "失败");
                    return;
                }
                //释放不必须的资源
                FStream.Dispose();
                WordStream.Dispose();

                //开始导出
                ExportWord(WDoc, _FixedDocVM);

                //写入输出文件
                if (WDoc != null)
                {
                    FileStream NewWordDoc = File.Create(fileDialog.FileName);
                    WDoc.Write(NewWordDoc);
                    NewWordDoc.Close();
                    WDoc.Close();
                    AppFuns.ShowMessage("数据导出成功！", Caption: "完成");
                    FileOperation.UseDefaultAppOpenFile(fileDialog.FileName);
                }
                else
                {
                    AppFuns.ShowMessage("数据导出失败！", Caption: "失败");
                }
                this.BtnExport.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                this.BtnExport.IsEnabled = true;
                AppFuns.ShowMessage(Ex.Message, Caption: "失败");
            }
        }
        /// <summary>
        /// 替换模板，生成新文档
        /// </summary>
        /// <param name="WDoc"></param>
        /// <param name="PMember"></param>
        /// <param name="PResumes"></param>
        /// <param name="PRelations"></param>
        /// <param name="PPrizePunishs"></param>
        /// <param name="PAppraises"></param>
        private void ExportWord(XWPFDocument WDoc, FixedDocViewModel PDocVM)
        {
            //遍历段落
            foreach (var para in WDoc.Paragraphs)
            {
                ReplaceKey(para, PDocVM.CurMember);
            }
            //遍历表格
            var tables = WDoc.Tables;
            foreach (var table in tables)
            {
                foreach (var row in table.Rows)
                {
                    foreach (var CurCell in row.GetTableCells())
                    {
                        ReplaceKeyInCell(WDoc, CurCell, PDocVM.CurMember, PDocVM.Resumes, PDocVM.Relations, PDocVM.PrizePunishs, PDocVM.Appraises);
                    }
                }
            }
        }
        private void ReplaceKey(XWPFParagraph CurPara, Lib.Member PMember)
        {
            if (CurPara.ParagraphText.Trim().Contains("[PrintCaption]"))
            {
                CurPara.ReplaceText("[PrintCaption]", _FixedDocVM.PrintCaption);
                return;
            }
            if (CurPara.ParagraphText.Trim().Contains("[PrintDate]"))
            {
                CurPara.ReplaceText("[PrintDate]", $"填表时间：{_FixedDocVM.PrintDate:yyyy年MM月dd日}");
                return;
            }
            //以上是段落内容替换。下面代码是段内不同的“Run”内容部分替换。
            //所谓“Run”是某段落内具有完全相关样式的内容。
            //var runs = CurPara.Runs;
            //for (int i = 0; i < runs.Count; i++)
            //{
            //    var CurRun = runs[i];
            //    string text = CurRun.ToString();
            //    if (text.Contains("[XXX]"))
            //    {
            //        text = text.Replace("[XXX]", "YYY");
            //    }
            //    CurRun.SetText(text, 0);
            //}
        }
        /// <summary>
        /// 表格中查找替换
        /// </summary>
        /// <param name="CurCell"></param>
        /// <param name="PMember"></param>
        /// <param name="PResumes"></param>
        /// <param name="PRelations"></param>
        /// <param name="PPrizePunishs"></param>
        /// <param name="PAppraises"></param>
        private void ReplaceKeyInCell(XWPFDocument WDoc, XWPFTableCell CurCell, Lib.Member PMember, List<MemberResume> PResumes, List<MemberRelations> PRelations, List<MemberPrizePunish> PPrizePunishs, List<MemberAppraise> PAppraises)
        {
            for (int i = 0; i < CurCell.Paragraphs.Count; i++)
            {
                XWPFParagraph CurPara = CurCell.Paragraphs[i];
                //头像
                if (CurPara.ParagraphText.Trim().Contains("[HeadImage]"))
                {
                    while (CurPara.Runs.Count > 0)
                    {
                        CurPara.RemoveRun(0);
                    }

                    if (_FixedDocVM.HeadImage != null && _FixedDocVM.HeadImage.StreamSource != null)
                    {
                        XWPFRun imageCellRun = CurPara.CreateRun();

                        //必须将流位置指针设置到开始处，否则显示不出来。
                        _FixedDocVM.HeadImage.StreamSource.Position = 0;

                        var picID = WDoc.AddPictureData(_FixedDocVM.HeadImage.StreamSource, (int)PictureType.PNG);

                        CreatePictureInLine(imageCellRun, picID, 100, 140);
                        // CreatePictureWithAnchor(imageCellRun, picID, 90, 120);
                    }
                    break;
                }
                //简历
                if (CurPara.ParagraphText.Trim().Contains("[Resume]"))
                {

                    if (PResumes != null && PResumes.Count > 0)
                    {
                        for (int pr = 0; pr < PResumes.Count; pr++)
                        {
                            string tempValue = $"{PResumes[pr].BeginDate:yyyy.MM}--{PResumes[pr].EndDate:yyy.MM}  {PResumes[pr].Content}";
                            if (pr == 0)
                            {
                                CurPara.ReplaceText("[Resume]", tempValue);
                            }
                            else
                            {
                                XWPFParagraph NewParagraph = CurCell.AddParagraph();
                                XWPFRun NewRun = NewParagraph.CreateRun();
                                NewRun.FontFamily = CurPara.Runs[0].FontFamily;
                                NewRun.FontSize = CurPara.Runs[0].FontSize;
                                NewRun.SetText(tempValue);
                            }
                        }
                    }
                    else
                    {
                        while (CurPara.Runs.Count > 0)
                        {
                            CurPara.RemoveRun(0);
                        }
                    }
                    break;
                }
                //奖情惩况
                if (CurPara.ParagraphText.Trim().Contains("[PrizePunish]"))
                {
                    if (PPrizePunishs != null && PPrizePunishs.Count > 0)
                    {
                        for (int pr = 0; pr < PPrizePunishs.Count; pr++)
                        {
                            string tempValue = $"{PPrizePunishs[pr].OccurDate:yyyy.MM}  {PPrizePunishs[pr].PrizrOrPunishUnit} {PPrizePunishs[pr].PrizrOrPunishName}";
                            if (pr == 0)
                            {
                                CurPara.ReplaceText("[PrizePunish]", tempValue);
                            }
                            else
                            {
                                XWPFParagraph NewParagraph = CurCell.AddParagraph();
                                XWPFRun NewRun = NewParagraph.CreateRun();
                                NewRun.FontFamily = CurPara.Runs[0].FontFamily;
                                NewRun.FontSize = CurPara.Runs[0].FontSize;
                                NewRun.SetText(tempValue);
                            }
                        }
                    }
                    else
                    {
                        while (CurPara.Runs.Count > 0)
                        {
                            CurPara.RemoveRun(0);
                        }
                    }
                    break;
                }

                //近三年考核情况
                if (CurPara.ParagraphText.Trim().Contains("[Appraise]"))
                {

                    if (PAppraises != null && PAppraises.Count > 0)
                    {
                        for (int pr = 0; pr < PAppraises.Count; pr++)
                        {
                            string tempValue = $"{PAppraises[pr].Year:yyyy}  {PAppraises[pr].Result}";
                            if (pr == 0)
                            {
                                CurPara.ReplaceText("[Appraise]", tempValue);
                            }
                            else
                            {
                                XWPFParagraph NewParagraph = CurCell.AddParagraph();
                                XWPFRun NewRun = NewParagraph.CreateRun();
                                NewRun.FontFamily = CurPara.Runs[0].FontFamily;
                                NewRun.FontSize = CurPara.Runs[0].FontSize;
                                NewRun.SetText(tempValue);
                            }
                        }
                    }
                    else
                    {
                        while (CurPara.Runs.Count > 0)
                        {
                            CurPara.RemoveRun(0);
                        }
                    }
                    break;
                }

                //主要社会关系
                if (CurPara.ParagraphText.Trim().Contains("[Relations"))
                {
                    string tempValue = "";
                    for (int p = 1; p <= 7; p++)
                    {
                        for (int k = 0; k <= 4; k++)
                        {
                            if (CurPara.ParagraphText.Trim().Equals($"[Relations{p}{k}]"))
                            {
                                if (PRelations != null && PRelations.Count >= p)
                                {
                                    switch (k)
                                    {
                                        case 0:
                                            tempValue = $"{PRelations[p - 1].Relation}";
                                            break;
                                        case 1:
                                            tempValue = $"{PRelations[p - 1].Name}";
                                            break;
                                        case 2:
                                            tempValue = $"{PRelations[p - 1].Birthday:yyyy.MM}";
                                            break;
                                        case 3:
                                            tempValue = $"{PRelations[p - 1].PoliticalStatus}";
                                            break;
                                        case 4:
                                            tempValue = $"{PRelations[p - 1].UnitName} {PRelations[p - 1].Role} {PRelations[p - 1].Remark}";
                                            break;
                                    }

                                }
                                CurPara.ReplaceText($"[Relations{p}{k}]", tempValue == null ? "" : tempValue.Trim());
                                break;
                            }
                        }
                    }
                }

                //基本信息
                {
                    Type t = PMember.GetType();
                    PropertyInfo[] pi = t.GetProperties();
                    foreach (PropertyInfo p in pi)
                    {
                        if (CurPara.ParagraphText.Trim().Contains($"[{p.Name}]"))
                        {

                            string CurValue = p.GetValue(PMember)?.ToString();
                            if (string.IsNullOrWhiteSpace(CurValue))
                            {
                                CurValue = "";
                            }
                            else
                            {
                                DateTime tempDate;
                                switch (p.Name)
                                {
                                    case "Birthday":
                                        tempDate = DateTime.Parse(CurValue);
                                        CurValue = $"{tempDate:yyyy.MM}";
                                        XWPFParagraph NewParagraph = CurCell.AddParagraph();
                                        NewParagraph.Alignment = ParagraphAlignment.CENTER;
                                        XWPFRun NewRun = NewParagraph.CreateRun();
                                        NewRun.FontFamily = CurPara.Runs[0].FontFamily;
                                        NewRun.FontSize = CurPara.Runs[0].FontSize;
                                        NewRun.SetText($"({DateTime.Now.Year - tempDate.Year}岁)");
                                        break;
                                    case "JoinCPC":
                                        tempDate = DateTime.Parse(CurValue);
                                        CurValue = $"{tempDate:yyyy.MM}";
                                        break;
                                    case "BeginWork":
                                        tempDate = DateTime.Parse(CurValue);
                                        CurValue = $"{tempDate:yyyy.MM}";
                                        break;
                                }
                            }
                            CurPara.ReplaceText("[" + p.Name + "]", CurValue);
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 行内插入方式。向WordDoc中插入图像，原控件有Bug,故采用自定义函数
        /// </summary>
        /// <param name="CurRun">当前段落中的Run</param>
        /// <param name="id"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void CreatePictureInLine(XWPFRun CurRun, string id, int width, int height)
        {
            int EMU = 9525;
            width *= EMU;
            height *= EMU;

            string picXml = ""
                    //+ "<a:graphic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">"
                    //+ "   <a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/picture\">"
                    + "      <pic:pic xmlns:pic=\"http://schemas.openxmlformats.org/drawingml/2006/picture\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">"
                    + "         <pic:nvPicPr>" + "<pic:cNvPr id=\"" + "0" + "\" name=\"Generated\"/>"
                    + "            <pic:cNvPicPr/>"
                    + "         </pic:nvPicPr>"
                    + "         <pic:blipFill>"
                    + "            <a:blip r:embed=\"" + id + "\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"/>"
                    + "            <a:stretch>"
                    + "               <a:fillRect/>"
                    + "            </a:stretch>"
                    + "         </pic:blipFill>"
                    + "         <pic:spPr>"
                    + "            <a:xfrm>"
                    + "               <a:off x=\"0\" y=\"20\"/>"
                    + "               <a:ext cx=\"" + width + "\" cy=\"" + height + "\"/>"
                    + "            </a:xfrm>"
                    + "            <a:prstGeom prst=\"rect\">"
                    + "               <a:avLst/>"
                    + "            </a:prstGeom>"
                    + "         </pic:spPr>"
                    + "      </pic:pic>";
            //+ "   </a:graphicData>" + "</a:graphic>";

            CT_Inline inline = CurRun.GetCTR().AddNewDrawing().AddNewInline();

            inline.graphic = new CT_GraphicalObject();
            inline.graphic.graphicData = new CT_GraphicalObjectData();
            inline.graphic.graphicData.uri = "http://schemas.openxmlformats.org/drawingml/2006/picture";

            // CT_GraphicalObjectData graphicData = inline.graphic.AddNewGraphicData();
            // graphicData.uri = "http://schemas.openxmlformats.org/drawingml/2006/picture";

            //XmlDocument xmlDoc = new XmlDocument();
            try
            {
                //xmlDoc.LoadXml(picXml);
                //var element = xmlDoc.DocumentElement;
                inline.graphic.graphicData.AddPicElement(picXml);
            }
            catch (XmlException xe)
            {
            }
            NPOI.OpenXmlFormats.Dml.WordProcessing.CT_PositiveSize2D extent = inline.AddNewExtent();
            extent.cx = width;
            extent.cy = height;

            NPOI.OpenXmlFormats.Dml.WordProcessing.CT_NonVisualDrawingProps docPr = inline.AddNewDocPr();
            docPr.id = 1;
            docPr.name = "Image" + id;

        }

        /// <summary>
        /// 锚点插入方式（暂时不可用）
        /// </summary>
        /// <param name="CurRun"></param>
        /// <param name="id"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void CreatePictureWithAnchor(XWPFRun CurRun, string id, int width, int height)
        {
            int EMU = 9525;
            width *= EMU;
            height *= EMU;
            string picXml = ""
                   //+ "<a:graphic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">"
                   //+ "   <a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/picture\">"
                   + "      <pic:pic xmlns:pic=\"http://schemas.openxmlformats.org/drawingml/2006/picture\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">"
                   + "         <pic:nvPicPr>" + "<pic:cNvPr id=\"" + "0" + "\" name=\"Generated\"/>"
                   + "            <pic:cNvPicPr/>"
                   + "         </pic:nvPicPr>"
                   + "         <pic:blipFill>"
                   + "            <a:blip r:embed=\"" + id + "\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"/>"
                   + "            <a:stretch>"
                   + "               <a:fillRect/>"
                   + "            </a:stretch>"
                   + "         </pic:blipFill>"
                   + "         <pic:spPr>"
                   + "            <a:xfrm>"
                   + "               <a:off x=\"0\" y=\"0\"/>"
                   + "               <a:ext cx=\"" + width + "\" cy=\"" + height + "\"/>"
                   + "            </a:xfrm>"
                   + "            <a:prstGeom prst=\"rect\">"
                   + "               <a:avLst/>"
                   + "            </a:prstGeom>"
                   + "         </pic:spPr>"
                   + "      </pic:pic>";
            //+ "   </a:graphicData>" + "</a:graphic>";

            List<CT_Anchor> AnList = CurRun.GetCTR().AddNewDrawing().GetAnchorList();

            AnList = AnList == null ? new List<CT_Anchor>() : AnList;

            //CT_Drawing drawing = CurRun.GetCTR().AddNewDrawing();
            CT_Anchor an = new CT_Anchor();
            //图片距正文上(distT)、下(distB)、左(distL)、右(distR)的距离。114300EMUS=3.1mm
            an.distB = (uint)(0);
            an.distL = 0;
            an.distR = 0;
            an.distT = 0;
            an.relativeHeight = 251658240u;
            an.behindDoc = false; //"0"，图与文字的上下关系
            an.locked = false;  //"0"
            an.layoutInCell = true;  //"1"
            an.allowOverlap = true;  //"1" 
            an.graphic = new CT_GraphicalObject();
            an.graphic.graphicData = new CT_GraphicalObjectData();
            an.graphic.graphicData.uri = "http://schemas.openxmlformats.org/drawingml/2006/picture";
            AnList.Add(an);

            //drawing.anchor = new List<CT_Anchor>();
            //drawing.anchor.Add(an);
            try
            {
                an.graphic.graphicData.AddPicElement(picXml);
            }
            catch (XmlException xe)
            {
            }
        }
    }

    /// <summary>
    /// 固定文档的视图模型类
    /// </summary>
    public class FixedDocViewModel
    {
        public FixedDocViewModel(Lib.Member PMember)
        {
            CurMember = PMember;
            MemberAge = DateTime.Now.Year - CurMember.Birthday.Year;
        }
        /// <summary>
        /// 读取用户的各类信息
        /// </summary>
        /// <returns></returns>
        public async Task InitPropsAsync()
        {
            //读取用户头像信息
            IEnumerable<MemberFile> UserPhotos = await DataMemberFileRepository.ReadFiles(new MemberFileSearch()
            {
                UserId = AppSet.LoginUser.Id,
                MemberId = CurMember.Id,
                Name = CurMember.Id
            });
            UserPhotos = UserPhotos.OrderByDescending(x => x.UpDateTime);
            if (UserPhotos != null && UserPhotos.Count() > 0)
            {
                MemoryStream UserHeadStream = null;
                MemoryStream TempStream = await DataMemberFileRepository.DownloadFileStream(UserPhotos.First().Id, null);
                if (TempStream != null)
                {
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(TempStream);
                    UserHeadStream = new MemoryStream();
                    bitmap.Save(UserHeadStream, System.Drawing.Imaging.ImageFormat.Png);
                    HeadImage = new BitmapImage();
                    HeadImage.BeginInit();
                    HeadImage.CacheOption = BitmapCacheOption.OnLoad;
                    HeadImage.StreamSource = UserHeadStream;
                    HeadImage.EndInit();
                    HeadImage.Freeze();
                    //UserHeadStream.Dispose();导出时要用此流
                }
            }
            //读取用户简历
            var Resumes = await DataMemberResumeRepository.GetRecords(new MemberResumeSearch()
            {
                MemberId = CurMember.Id,
                UserId = AppSet.LoginUser.Id

            }).ConfigureAwait(false);
            Resumes = Resumes?.OrderBy(x => x.BeginDate);
            this.Resumes = Resumes?.ToList();
            //读取用户社会关系
            var Relations = await DataMemberRelationsRepository.GetRecords(new MemberRelationsSearch()
            {
                MemberId = CurMember.Id,
                UserId = AppSet.LoginUser.Id

            }).ConfigureAwait(false);
            Relations = Relations?.OrderBy(x => x.OrderIndex);
            this.Relations = Relations?.ToList();
            //读取用户奖惩信息
            var PrizePunishs = await DataMemberPrizePunishRepository.GetRecords(new MemberPrizePunishSearch()
            {
                MemberId = CurMember.Id,
                UserId = AppSet.LoginUser.Id

            }).ConfigureAwait(false);
            PrizePunishs = PrizePunishs?.OrderByDescending(x => x.OccurDate).Take(5);
            this.PrizePunishs = PrizePunishs?.ToList();
            //读取用户近三年考核情况
            var Appraises = await DataMemberAppraiseRepository.GetRecords(new MemberAppraiseSearch()
            {
                MemberId = CurMember.Id,
                UserId = AppSet.LoginUser.Id

            }).ConfigureAwait(false);
            Appraises = Appraises?.OrderByDescending(x => x.Year).Take(3);
            this.Appraises = Appraises?.ToList();
            //确保各类集合不为空
            if (this.Resumes == null) { this.Resumes = new List<MemberResume>(); }
            if (this.Relations == null) { this.Relations = new List<MemberRelations>(); }
            if (this.PrizePunishs == null) { this.PrizePunishs = new List<MemberPrizePunish>(); }
            if (this.Appraises == null) { this.Appraises = new List<MemberAppraise>(); }
        }
        public Lib.Member CurMember { get; set; }
        public BitmapImage HeadImage { get; set; } = null;
        public List<MemberResume> Resumes { get; set; }
        public List<MemberRelations> Relations { get; set; }
        public List<MemberPrizePunish> PrizePunishs { get; set; }
        public List<MemberAppraise> Appraises { get; set; }
        public string PrintCaption { get; set; }
        public DateTime PrintDate { get; set; }
        public int MemberAge { get; set; }
    }
    public static class FixedDocumentExtended
    {
        /// <summary>
        /// 对WPF FixedDocument 进行扩展，该方法将XAML文件定义的资源读出为PageContent
        /// </summary>
        /// <param name="fixedDocument"></param>
        public static void AddPages(this FixedDocument fixedDocument)
        {
            var enumerator = fixedDocument.Resources.GetEnumerator();
            List<PageContent> PContentList = new List<PageContent>();
            while (enumerator.MoveNext())
            {
                var pageContent = ((DictionaryEntry)enumerator.Current).Value as PageContent;
                if (pageContent != null)
                {
                    PContentList.Add(pageContent);
                }
            }
            var OrderPageContents = PContentList.OrderBy(x => x.Name);
            foreach (var item in OrderPageContents)
            {
                fixedDocument.Pages.Add(item);
            }
            fixedDocument.Pages.OrderByDescending(x => x.Name);
        }
    }
}
