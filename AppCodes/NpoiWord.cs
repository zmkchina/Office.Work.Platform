using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XWPF.UserModel;

namespace Office.Work.Platform.AppCodes
{
    public static class NpoiWord
    {
        public static void CreateParagraph(XWPFDocument WordDoc, string LeftRightCenter = "Center")
        {
            // 添加段落
            XWPFParagraph NewParagraph = WordDoc.CreateParagraph();
            switch (LeftRightCenter)
            {
                case "Center":
                    NewParagraph.Alignment = ParagraphAlignment.CENTER;//水平居中
                    break;
                case "Left":
                    NewParagraph.Alignment = ParagraphAlignment.LEFT;//水平居中
                    break;
                case "Right":
                    NewParagraph.Alignment = ParagraphAlignment.RIGHT;//水平居中
                    break;
            }
        }
    }
}
