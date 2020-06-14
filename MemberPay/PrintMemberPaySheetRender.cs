using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace Office.Work.Platform.MemberPay
{
    public class PrintMemberPaySheetRender
    {
        public void Render(FlowDocument FlowDoc, string Caption, string DateStr, JArray JArrayResult)
        {
            if (JArrayResult.Count <= 0) return;
            //0.设置表标题
            Run Run_Caption = FlowDoc.FindName("Run_Caption") as Run;
            Run_Caption.Text = Caption;
            //0.月份
            Run Run_PayDate = FlowDoc.FindName("Run_PayDate") as Run;
            Run_PayDate.Text = $"{DateStr}";
            //1.生成打印表头
            string[] PropertiesNameArr;
            TableRow TempRow = new TableRow();
            JObject FirstJObject = (JObject)JArrayResult[0];
            List<JProperty> properties = FirstJObject.Properties().ToList();
            PropertiesNameArr = new string[properties.Count()];
            for (int k = 0; k < properties.Count(); k++)
            {
                //Console.WriteLine(item.Name + ":" + item.Value);
                PropertiesNameArr[k] = properties[k].Name;
                Paragraph TempParagraph = new Paragraph(new Run(properties[k].Name))
                {
                    Style = FlowDoc.FindResource("PgStyle") as Style
                };
                TempRow.Cells.Add(new TableCell(TempParagraph));
            }

            TableRowGroup TRGroupHeader = FlowDoc.FindName("TableRowGroupHeader") as TableRowGroup;
            TRGroupHeader.Rows.Add(TempRow);

            //2.生成打印数据行
            for (int i = 0; i < JArrayResult.Count; i++)
            {
                TempRow = new TableRow();
                JObject TempJObject = (JObject)JArrayResult[i];
                for (int p = 0; p < PropertiesNameArr.Length; p++)
                {
                    Paragraph TempParagraph = new Paragraph(new Run(TempJObject[PropertiesNameArr[p]].ToString()))
                    {
                        Style = FlowDoc.FindResource("PgStyle") as Style,
                    };
                    TempRow.Cells.Add(new TableCell(TempParagraph));
                }
                TableRowGroup TRGroupContent = FlowDoc.FindName("TableContentRows") as TableRowGroup;
                TRGroupContent.Rows.Add(TempRow);
            }

        }
    }
    public class PaginatorHeaderFooter : DocumentPaginator
    {
        readonly DocumentPaginator m_paginator;

        public PaginatorHeaderFooter(DocumentPaginator paginator)
        {
            m_paginator = paginator;
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            DocumentPage page = m_paginator.GetPage(pageNumber);
            ContainerVisual newpage = new ContainerVisual();

            //页眉:公司名称
            DrawingVisual header = new DrawingVisual();
            using (DrawingContext ctx = header.RenderOpen())
            {
                string HeaderStr = "市港航中心工资福利表";
                FormattedText formattedHeaderText = new FormattedText(HeaderStr, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    new Typeface("Verdana"), 15, Brushes.Black, VisualTreeHelper.GetDpi(page.Visual).PixelsPerDip);

                ctx.DrawText(formattedHeaderText, new Point(page.ContentBox.Left, page.ContentBox.Top));
                //ctx.DrawLine(new Pen(Brushes.Black, 0.5), new Point(page.ContentBox.Left, page.ContentBox.Top + 16), new Point(page.ContentBox.Right, page.ContentBox.Top + 16));
            }

            //页脚:第几页
            DrawingVisual footer = new DrawingVisual();
            using (DrawingContext ctx = footer.RenderOpen())
            {
                string BottomStr = $"      审核人：                              证明人：                                   制表人：                                页码：{pageNumber + 1}"; //{PageCount}";
                FormattedText formattedBottomText = new FormattedText(BottomStr, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    new Typeface("Verdana"), 15, Brushes.Black, VisualTreeHelper.GetDpi(page.Visual).PixelsPerDip);
                ctx.DrawText(formattedBottomText, new Point(page.ContentBox.Left, page.ContentBox.Bottom - 15));
            }

            //将原页面微略压缩(使用矩阵变换)
            ContainerVisual mainPage = new ContainerVisual();
            mainPage.Children.Add(page.Visual);
            mainPage.Transform = new MatrixTransform(1, 0, 0, 0.95, 0, 0.04 * page.ContentBox.Height);

            //在现页面中加入原页面，页眉和页脚
            newpage.Children.Add(mainPage);
            newpage.Children.Add(header);
            newpage.Children.Add(footer);

            return new DocumentPage(newpage, page.Size, page.BleedBox, page.ContentBox);
        }

        public override bool IsPageCountValid
        {
            get
            {
                return m_paginator.IsPageCountValid;
            }
        }

        public override int PageCount
        {
            get
            {
                return m_paginator.PageCount;
            }
        }

        public override Size PageSize
        {
            get
            {
                return m_paginator.PageSize;
            }

            set
            {
                m_paginator.PageSize = value;
            }
        }

        public override IDocumentPaginatorSource Source
        {
            get
            {
                return m_paginator.Source;
            }
        }
    }
}
