using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberPay
{
    /// <summary>
    /// 正式人员月度工资查询（发放）
    /// </summary>
    public partial class PageMemberPaySheet : Page
    {
        public PageMemberPaySheet()
        {
            InitializeComponent();
            MemberSet = AppSet.ServerSetting;
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
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("就绪");
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
                        AppFuns.SetStateBarText($"共查询到：{JArrayResult.Count} 条数据。");
                    }
                    catch (Exception ex)
                    {
                        AppFuns.ShowMessage(ex.Message, "错误", true);
                        return;
                    }
                }
                else
                {
                    AppFuns.SetStateBarText($"共查询到： 0 条数据。");
                }
                string Caption = $"{AppSet.LoginUser.UnitShortName}{MemberType}人员{PayTableName}";
                string DateStr = $"发放月份：（{PayMonthDate.Year}年{PayMonthDate.Month}月）";
                if (!PayTableTypes.Contains("月"))
                {
                    DateStr = $"发放时间：{PayMonthDate.Year}年{PayMonthDate.Month}月{PayMonthDate.Day}日";
                }
                CreateFlowDoc("PrintMemberPaySheetDot.xaml", Caption, DateStr, JArrayResult, P_DocWidth: 1122, P_DocHeight: 793);
            });
        }
        private void CreateFlowDoc(string SheetTemplet, string Caption, string DateStr, JArray data, double P_DocWidth, double P_DocHeight)
        {
            //1.导入流文件格式模板
            FlowDocument m_doc = (FlowDocument)Application.LoadComponent(new Uri($"/Office.Work.Platform;component/MemberPay/{SheetTemplet}", UriKind.RelativeOrAbsolute));
            m_doc.PageWidth = P_DocWidth;
            m_doc.PageHeight = P_DocHeight;
            m_doc.ColumnWidth = P_DocWidth;
            m_doc.Background = System.Windows.Media.Brushes.Transparent;
            m_doc.PagePadding = new Thickness(85, 70, 85, 70);//设置页面与页面之间的边距宽度

            //2.填充模板内容
            PrintMemberPaySheetRender renderer = new PrintMemberPaySheetRender();
            if (renderer != null)
            {
                renderer.Render(m_doc, Caption, DateStr, data);
            }
            //3.生成Xps
            Dispatcher.BeginInvoke(() => { LoadXps(m_doc); }, DispatcherPriority.ApplicationIdle);
        }
        public void LoadXps(FlowDocument m_doc)
        {
            //构造一个基于内存的xps document
            MemoryStream ms = new MemoryStream();
            Package package = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);
            Uri DocumentUri = new Uri("pack://InMemoryPayDoc.xps");
            PackageStore.RemovePackage(DocumentUri);
            PackageStore.AddPackage(DocumentUri, package);
            XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Fast, DocumentUri.AbsoluteUri);

            //将flow document写入基于内存的xps document中去
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            writer.Write(((IDocumentPaginatorSource)m_doc).DocumentPaginator);

            //获取这个基于内存的xps document的fixed document
            docViewer.Document = xpsDocument.GetFixedDocumentSequence();

            //关闭基于内存的xps document
            xpsDocument.Close();
        }

        public DateTime PayMonthDate { get; set; } = DateTime.Now;
        public string MemberType { get; set; }
        public string PayTableName { get; set; }
        public Lib.SettingServer MemberSet { get; set; }
        public string[] PayTableTypes { get; set; }
        public string[] PayTableMemberTypes { get; set; }
        public JArray JArrayResult { get; set; }

        #region 备注
        ///// <summary>
        ///// 打印工资表
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void BtnPrintClickAsync(object sender, RoutedEventArgs e)
        //{
        //    PrintPreviewWin previewWnd = new PrintPreviewWin("PrintMemberPaySheetDot.xaml", P_DocWidth: 1122, P_DocHeight: 793, JArrayResult, PayMonthDate, new PrintMemberPaySheetRender());
        //    previewWnd.Owner = AppSet.AppMainWindow;
        //    previewWnd.ShowInTaskbar = false;
        //    previewWnd.ShowDialog();


        //    //第一种打印方式
        //    //PrintDialog dialog = new PrintDialog();
        //    //if (dialog.ShowDialog() == true)
        //    //{
        //    //    dialog.PrintVisual(printArea, "Print Test");
        //    //}
        //    //第二种方式
        //    //A4：793,1122
        //    //PrintPreviewWindow previewWnd = new PrintPreviewWindow(this.FlowDoc, P_DocWidth: 1122, P_DocHeight: 793);//在这里我们将FlowDocument.xaml这个页面传进去，之后通过打印预览窗口的构造函数填充打印内容,如果有数据要插入应该在此传数据结构进去
        //    //previewWnd.ShowDialog();//显示打印预览窗体

        //    ////第三种方式，直接打印
        //    //PrintDialog printDlg = new PrintDialog();
        //    //LocalPrintServer ps = new LocalPrintServer();
        //    //PrintQueue pq = ps.DefaultPrintQueue;
        //    //PrintTicket pt = pq.UserPrintTicket;
        //    ////pt.PageOrientation = PageOrientation.Landscape;
        //    //PageMediaSize pageMediaSize = new PageMediaSize(FlowDoc.PageWidth, FlowDoc.PageHeight);
        //    //pt.PageMediaSize = pageMediaSize;
        //    //IDocumentPaginatorSource source = FlowDoc as IDocumentPaginatorSource;
        //    //printDlg.PrintDocument(source.DocumentPaginator, "sum");
        //}
        #endregion
    }
}
