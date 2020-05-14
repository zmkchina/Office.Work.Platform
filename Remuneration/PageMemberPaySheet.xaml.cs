using System;
using System.Windows;
using System.Windows.Controls;
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

        public DateTime PayMonthDate { get; set; } = DateTime.Now;
        public string MemberType { get; set; }
        public string PayTableName { get; set; }
        public MemberSettings MemberSet { get; set; }
        public string[] PayTableTypes { get; set; }
        public string[] PayTableMemberTypes { get; set; }
        public JArray JArrayResult { get; set; }
        public PageMemberPaySheet()
        {
            InitializeComponent();
            MemberSet = new MemberSettings();
            JArrayResult = new JArray();
        }

        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            //获取已发放信息中所有工资表类型
            PayTableTypes = await DataMemberPaySheetRepository.GetPayTableTypes().ConfigureAwait(false);
            //获取所有已生成的待遇表中人员的类型
            PayTableMemberTypes = await DataMemberPaySheetRepository.GetPayTableMemberTypes().ConfigureAwait(false);
            App.Current.Dispatcher.Invoke(() =>
            {
                this.DataContext = this;
            });
        }
        /// <summary>
        /// 查询待发放信息
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
                PayUnitName = AppSet.LoginUser.UnitName,
                UserId = AppSet.LoginUser.Id
            }).ConfigureAwait(false);
            App.Current.Dispatcher.Invoke(() =>
            {
                if (SearchResult != null)
                {
                    try
                    {
                        JArray TempJArrayResult = (JArray)JsonConvert.DeserializeObject(SearchResult);
                        JArrayResult.Clear();
                        foreach (JObject item in TempJArrayResult)
                        {
                            JArrayResult.Add(item);
                        }
                        RecordDataGrid.ItemsSource = null;
                        RecordDataGrid.Items.Refresh();

                        RecordDataGrid.ItemsSource = JArrayResult;
                        AppFuns.SetStateBarText($"共查询到：{JArrayResult.Count} 条数据。");
                    }
                    catch (Exception ex)
                    {

                        AppFuns.SetStateBarText(ex.Message);
                    }


                }
                else
                {
                    AppFuns.SetStateBarText($"共查询到： 0 条数据。");

                }
            });
        }
        /// <summary>
        /// 打印工资表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPrintClickAsync(object sender, RoutedEventArgs e)
        {
            PrintPreviewWin previewWnd = new PrintPreviewWin("PrintMemberPaySheetDot.xaml", P_DocWidth: 1122, P_DocHeight: 793, JArrayResult, PayMonthDate, new PrintMemberPaySheetRender());
            previewWnd.Owner = AppSet.AppMainWindow;
            previewWnd.ShowInTaskbar = false;
            previewWnd.ShowDialog();


            //第一种打印方式
            //PrintDialog dialog = new PrintDialog();
            //if (dialog.ShowDialog() == true)
            //{
            //    dialog.PrintVisual(printArea, "Print Test");
            //}
            //第二种方式
            //A4：793,1122
            //PrintPreviewWindow previewWnd = new PrintPreviewWindow(this.FlowDoc, P_DocWidth: 1122, P_DocHeight: 793);//在这里我们将FlowDocument.xaml这个页面传进去，之后通过打印预览窗口的构造函数填充打印内容,如果有数据要插入应该在此传数据结构进去
            //previewWnd.ShowDialog();//显示打印预览窗体

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

    }
}
