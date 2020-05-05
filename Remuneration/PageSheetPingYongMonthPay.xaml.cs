using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Remuneration
{
    /// <summary>
    /// 正式人员月度工资查询（发放）
    /// </summary>
    public partial class PageSheetPingYongMonthPay : Page
    {
        public PageSheetPingYongMonthPay()
        {
            InitializeComponent();
            RichTBFlowDoc.Visibility = Visibility.Collapsed;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;
        }
        /// <summary>
        /// 查询正式人员待发放工资
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            string ResultString = await DataMemberCombiningRepository.GetPingYongMemberMonthPaySheet(PayMonthDate.Year, PayMonthDate.Month);
            if (!string.IsNullOrWhiteSpace(ResultString))
            {
                var searchObjs = JsonConvert.DeserializeObject(ResultString);
                if (searchObjs != null)
                {
                    JContainer jc = (Newtonsoft.Json.Linq.JContainer)searchObjs;
                    foreach (var item in (JObject)jc[0])
                    {
                        RecordDataGrid.Columns.Add(new DataGridTextColumn() { Header = item.Key, Binding = new Binding(item.Key) });
                    }
                    RecordDataGrid.ItemsSource = jc;
                    TableContentRows.Rows.Clear();
                    foreach (JObject items in jc)
                    {
                        TableRow tr = new TableRow();
                        foreach (var item in items)
                        {
                            //item.Key + ":" + item.Value + ",";
                            //设置待打印的FlowDocument内部的内容。
                            Paragraph TempParagraph = new Paragraph(new Run(item.Value.ToString()));
                            TempParagraph.TextAlignment = TextAlignment.Center;
                            TempParagraph.Margin = new Thickness(0, 5, 0, 5);
                            TableCell tempCell = new TableCell(TempParagraph);
                            tempCell.TextAlignment = TextAlignment.Center;
                            tr.Cells.Add(tempCell);
                        }
                        TableContentRows.Rows.Add(tr);
                    }
                }
            }

            //e.ShouldGetMoney = e.LivingAllowance + e.IncentivePerformancePay + e.PostAllowance + e.ScalePay + e.PostPay;
            //e.FactGetMoney = e.HousingFund + e.MedicalInsurance + e.OccupationalPension + e.PensionInsurance + e.Tax + e.UnionFees;
            //e.FactGetMoney = e.ShouldGetMoney - e.FactGetMoney;
            //e.ShouldGetMoney = (float)Math.Round(e.ShouldGetMoney, 2);
            //e.FactGetMoney = (float)Math.Round(e.FactGetMoney, 2);
            AppSettings.SetStateBarText($"共查询到：{TableContentRows.Rows.Count()} 条数据。");
        }
        /// <summary>
        /// 打印工资表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPrintClickAsync(object sender, RoutedEventArgs e)
        {
            //PrintDialog dialog = new PrintDialog();
            //if (dialog.ShowDialog() == true)
            //{
            //    dialog.PrintVisual(printArea, "Print Test");
            //}
            //第二种方式
            //A4：793,1122
            PrintPreviewWindow previewWnd = new PrintPreviewWindow(this.FlowDoc, P_DocWidth: 1122, P_DocHeight: 793);//在这里我们将FlowDocument.xaml这个页面传进去，之后通过打印预览窗口的构造函数填充打印内容,如果有数据要插入应该在此传数据结构进去
            previewWnd.Owner = App.Current.MainWindow;
            previewWnd.ShowInTaskbar = false;//设置预览窗体在最小化时不要出现在任务栏中 
            previewWnd.ShowDialog();//显示打印预览窗体

            ////第三种方式，直接打印
            //PrintDialog printDlg = new PrintDialog();
            //LocalPrintServer ps = new LocalPrintServer();
            //PrintQueue pq = ps.DefaultPrintQueue;
            //PrintTicket pt = pq.UserPrintTicket;
            ////pt.PageOrientation = PageOrientation.Landscape;
            //PageMediaSize pageMediaSize = new PageMediaSize(FlowDoc.PageWidth, FlowDoc.PageHeight);
            //pt.PageMediaSize = pageMediaSize;
            //IDocumentPaginatorSource source = FlowDoc as IDocumentPaginatorSource;
            //printDlg.PrintDocument(source.DocumentPaginator, "sum");
        }
        public DateTime PayMonthDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 生成正式人员指定月份的工资。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnCreateClickAsync(object sender, RoutedEventArgs e)
        {
            ExcuteResult excuteResult = await DataMemberCombiningRepository.PostMemberPayMonthOfficialSheet(PayMonthDate.Year, PayMonthDate.Month);
            (new WinMsgDialog(excuteResult.Msg)).ShowDialog();
        }
        /// <summary>
        /// 克隆一个对象
        /// </summary>
        /// <param name="sampleObject"></param>
        /// <returns></returns>
        private T Clone<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制
                System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

    }
}
