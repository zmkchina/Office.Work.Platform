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
    public partial class PageMemberPaySheet : Page
    {

        public string MemberType { get; set; }
        public string PayTableName { get; set; }
        public MemberSettings MemberSet { get; set; }
        public string[] PayTableTypes { get; set; }

        public PageMemberPaySheet()
        {
            InitializeComponent();
            RichTBFlowDoc.Visibility = Visibility.Collapsed;
            MemberSet = new MemberSettings();
        }

        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            //获取已发放信息中所有工资表类型
            PayTableTypes = await DataMemberPaySheetRepository.GetPayTableTypes().ConfigureAwait(false);
            App.Current.Dispatcher.Invoke(() =>
            {
                this.DataContext = this;
            });
        }
        /// <summary>
        /// 查询正式人员待发放工资
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            string SearchResult = await DataMemberPaySheetRepository.GetMemberPaySheet(new MemberPaySheetSearch()
            {
                PayYear = PayMonthDate.Year,
                PayMonth = PayMonthDate.Month,
                MemberType = MemberType,
                PayTableType = PayTableName,
                PayUnitName = AppSettings.LoginUser.UnitName,
                UserId = AppSettings.LoginUser.Id
            }).ConfigureAwait(false);

            if (SearchResult != null)
            {
                JArray JArrayResult = (JArray)JsonConvert.DeserializeObject(SearchResult);
                RecordDataGrid.ItemsSource = JArrayResult;
            }

            AppSettings.SetStateBarText($"共查询到：{RecordDataGrid.Items.Count} 条数据。");
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
