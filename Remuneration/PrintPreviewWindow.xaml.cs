using System;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace Office.Work.Platform.Remuneration
{
    /// <summary>
    /// PrintPreviewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PrintPreviewWindow : Window
    {
        private delegate void LoadXpsMethod();//委托事件，相当于函数指针
        private readonly FlowDocument flowDoc;//流文档
        public PrintPreviewWindow(FlowDocument P_FlowDoc,double P_DocWidth,double P_DocHeight)//从上面得到待打印的文档
        {
            this.Owner = App.Current.MainWindow;
            InitializeComponent();
            flowDoc = P_FlowDoc;// (FlowDocument)Application.LoadComponent(new Uri(strTmplName, UriKind.RelativeOrAbsolute));//从xaml文件中加载流文档对象
            flowDoc.PageWidth = P_DocWidth;
            flowDoc.PageHeight = P_DocHeight;
            flowDoc.ColumnWidth = P_DocWidth;
            flowDoc.Background = System.Windows.Media.Brushes.Transparent;
            flowDoc.PagePadding = new Thickness(85,70,85,70);//设置页面与页面之间的边距宽度
            Dispatcher.BeginInvoke(new LoadXpsMethod(LoadXps), DispatcherPriority.ApplicationIdle);//“延后”调用，不然刚刚更改的数据不会马上更新，也就是说打印或者预览不到更新后的数据
        }
        public void LoadXps()
        {
            //构造一个基于内存的xps document
            MemoryStream ms = new MemoryStream();
            Package package = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);
            Uri DocumentUri = new Uri("pack://InMemoryDocument.xps");
            PackageStore.RemovePackage(DocumentUri);
            PackageStore.AddPackage(DocumentUri, package);
            XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Fast, DocumentUri.AbsoluteUri);
            //将flow document写入基于内存的xps document中去
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);//在这里需要添加对.NET 4.0 的一些应用，比较蛋疼
            writer.Write(((IDocumentPaginatorSource)flowDoc).DocumentPaginator);
            //获取这个基于内存的xps document的fixed documen
            docViewer.Document = xpsDocument.GetFixedDocumentSequence();
            //关闭基于内存的xps document
            xpsDocument.Close();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            flowDoc.PageWidth = Double.NaN;
            flowDoc.PageHeight = Double.NaN;
            flowDoc.ColumnWidth = Double.NaN;
        }
        //private void SaveToXps(Stream fileStream)
        //{
        //    Package package = Package.Open(fileStream, FileMode.Create, FileAccess.ReadWrite);
        //    XpsDocument doc = new XpsDocument(package);
        //    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
        //    IDocumentPaginatorSource document = CreateDocViewer().Document;
        //    writer.Write(document.DocumentPaginator);
        //    doc.Close();
        //    package.Close();
        //}

        //public void SaveToPdf(string pdfFileName)
        //{
        //    MemoryStream memStream = new MemoryStream();
        //    SaveToXps(memStream);
        //    WebSupergoo.ABCpdf11.Doc pdfDoc = new WebSupergoo.ABCpdf11.Doc();
        //    pdfDoc.Read(memStream, null, "xps");
        //    pdfDoc.Save(pdfFileName);
        //    pdfDoc.Clear();
        //    memStream.Close();
        //}
    }
}
