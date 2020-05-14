using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Newtonsoft.Json.Linq;
using Office.Work.Platform.AppCodes;

namespace Office.Work.Platform.Remuneration
{
    public class PrintMemberPaySheetRender : IDocumentRenderer
    {
        public void Render(FlowDocument FlowDoc, JArray JArrayResult, DateTime DataDateTime)
        {
            if (JArrayResult.Count <= 0) return;
            //0.月份
            Run Run_PayDate = FlowDoc.FindName("Run_PayDate") as Run;
            Run_PayDate.Text = $"({DataDateTime.Year}年{DataDateTime.Month}月)";
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
                Paragraph TempParagraph = new Paragraph();
                TempParagraph.Style = FlowDoc.FindResource("PgStyle") as Style;
                TempParagraph.Inlines.Add(new TextBlock() { Text = properties[k].Name });
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
                    Paragraph TempParagraph = new Paragraph();
                    TempParagraph.Inlines.Add(new TextBlock() { Text = TempJObject[PropertiesNameArr[p]].ToString() });
                    TempRow.Cells.Add(new TableCell(TempParagraph));
                }
                TableRowGroup TRGroupContent = FlowDoc.FindName("TableContentRows") as TableRowGroup;
                TRGroupContent.Rows.Add(TempRow);
            }
        }
    }
}
