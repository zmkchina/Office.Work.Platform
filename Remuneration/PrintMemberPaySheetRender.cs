using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Newtonsoft.Json.Linq;

namespace Office.Work.Platform.Remuneration
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
}
