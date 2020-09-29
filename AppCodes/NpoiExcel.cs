using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
namespace Office.Work.Platform.AppCodes
{
    public class NpoiExcel
    {
        /// <summary>
        /// 将excel文件内容读取到DataTable数据表中
        /// </summary>
        /// <param name="fileName">文件完整路径名</param>
        /// <param name="sheetName">指定读取excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名：true=是，false=否</param>
        /// <returns>DataTable数据表</returns>
        public static DataTable ReadExcelToDataTable(string fileName, string sheetName = null, bool isFirstRowColumn = true)
        {
            //定义要返回的datatable对象
            DataTable data = new DataTable();
            //excel工作表
            ISheet sheet = null;
            //数据开始行(排除标题行)
            int startRow = 0;
            try
            {
                if (!File.Exists(fileName))
                {
                    return null;
                }
                //根据指定路径读取文件
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                //根据文件流创建excel数据结构
                IWorkbook workbook = WorkbookFactory.Create(fs);
                //IWorkbook workbook = new HSSFWorkbook(fs);
                //如果有指定工作表名称
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheet(sheetName);
                    //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    //如果没有指定的sheetName，则尝试获取第一个sheet
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    //一行最后一个cell的编号 即总的列数
                    int cellCount = firstRow.LastCellNum;
                    //如果第一行是标题列名
                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }
                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        data.Rows.Add(dataRow);
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将文件流读取到DataTable数据表中
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="sheetName">指定读取excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名：true=是，false=否</param>
        /// <returns>DataTable数据表</returns>
        public static DataTable ReadStreamToDataTable(Stream fileStream, string sheetName = null, bool isFirstRowColumn = true)
        {
            //定义要返回的datatable对象
            DataTable data = new DataTable();
            //excel工作表
            ISheet sheet = null;
            try
            {
                //根据文件流创建excel数据结构,NPOI的工厂类WorkbookFactory会自动识别excel版本，创建出不同的excel数据结构
                IWorkbook workbook = WorkbookFactory.Create(fileStream);
                //如果有指定工作表名称
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheet(sheetName);
                    //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    //如果没有指定的sheetName，则尝试获取第一个sheet
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    //一行最后一个cell的编号 即总的列数
                    int cellCount = firstRow.LastCellNum;
                    //数据开始行(排除标题行)
                    int startRow;
                    //如果第一行是标题列名
                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }
                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null || row.FirstCellNum < 0) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            //同理，没有数据的单元格都默认是null
                            ICell cell = row.GetCell(j);
                            if (cell != null)
                            {
                                if (cell.CellType == CellType.Numeric)
                                {
                                    //判断是否日期类型
                                    if (DateUtil.IsCellDateFormatted(cell))
                                    {
                                        dataRow[j] = row.GetCell(j).DateCellValue;
                                    }
                                    else
                                    {
                                        dataRow[j] = row.GetCell(j).ToString().Trim();
                                    }
                                }
                                else
                                {
                                    dataRow[j] = row.GetCell(j).ToString().Trim();
                                }
                            }
                        }
                        data.Rows.Add(dataRow);
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 导出EXCEL文件
        /// </summary>
        /// <param name="xlsName">文件名</param>
        /// <param name="tableName">工作薄名称</param>
        /// <param name="data">导出的数据源,数据源类在定义时必须有描述特性</param>
        public static void ExportExcels<T>(string xlsName, string tableName, List<T> data)
        {
            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(tableName);

            var row = sheet.CreateRow(sheet.LastRowNum);

            //创建标题行
            int i = 0;
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            foreach (var property in propertyInfos)
            {
                object[] objs = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (objs.Length > 0)
                {
                    //创建一个列并且赋值
                    row.CreateCell(i).SetCellValue(((DescriptionAttribute)objs[0]).Description);
                    i++;
                }
            }

            //将数据填充到表格当中
            int j = sheet.LastRowNum + 1;
            foreach (var item in data)
            {
                int n = 0;
                row = sheet.CreateRow(j++);
                //获取一项的所有属性
                var itemProps = item.GetType().GetProperties();
                foreach (var itemPropSub in itemProps)
                {
                    var objs = itemPropSub.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (objs.Length > 0)
                    {
                        if (itemPropSub.PropertyType == typeof(System.DateTime))
                        {
                            string DateStr = itemPropSub.GetValue(item, null).ToString();
                            if (DateTime.TryParse(DateStr, out DateTime theDate))
                            {
                                row.CreateCell(n).SetCellValue(theDate.ToString("yyyy-MM-dd"));
                            }
                        }
                        else
                        {
                            row.CreateCell(n).SetCellValue(itemPropSub.GetValue(item, null) == null ? "" :
                                itemPropSub.GetValue(item, null).ToString());
                        }
                        n++;
                    }
                }
            }

            MemoryStream ms = new MemoryStream();

            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            FileStream fileStream = new FileStream(xlsName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            ms.WriteTo(fileStream);
            ms.Close();
            ms.Dispose();
            fileStream.Close();
            workbook.Close();
        }

        #region 未使用的代码
        /// <summary>
        /// NPOI导出Excel，不依赖本地是否装有Excel，导出速度快
        /// </summary>
        /// <param name="dataGridView1">要导出的dataGridView控件</param>
        /// <param name="sheetName">sheet表名</param>
        private void ExportToExcel(List<Lib.MemberInfoEntity> EntityList, string sheetName)
        {
            if (EntityList == null || EntityList.Count < 1)
            {
                return;
            }
            System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();
            fileDialog.Filter = "Excel|*.xls";
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            //不允许dataGridView显示添加行，负责导出时会报最后一行未实例化错误
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName);
            IRow rowHead = sheet.CreateRow(0);
            PropertyInfo[] EntityProps = EntityList[0].GetType().GetProperties();
            //填写表头
            for (int i = 0; i < EntityProps.Count(); i++)
            {
                rowHead.CreateCell(i, CellType.String).SetCellValue(EntityProps[i].Name);
            }
            //填写内容
            for (int i = 0; i < EntityList.Count; i++)
            {
                IRow row = sheet.CreateRow(i + 1);
                for (int j = 0; j < EntityProps.Count(); j++)
                {
                    object TempObj = EntityProps[j].GetValue(EntityList[i]);
                    if (TempObj != null)
                    {
                        row.CreateCell(j, CellType.String).SetCellValue(EntityProps[j].GetValue(EntityList[i]).ToString());
                    }
                    else
                    {
                        row.CreateCell(j, CellType.String).SetCellValue("");

                    }
                }
            }

            using (FileStream stream = File.OpenWrite(fileDialog.FileName))
            {
                workbook.Write(stream);
                stream.Close();
            }
            AppFuns.ShowMessage("导出数据成功!", "提示");
            GC.Collect();
        }
        #endregion
    }
}
