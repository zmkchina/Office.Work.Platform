﻿using System;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Newtonsoft.Json.Linq;
using Office.Work.Platform.AppCodes;

namespace Office.Work.Platform.MemberPay
{
    /// <summary>
    /// 打印预览窗口，此类目前未使用。
    /// </summary>
    partial class PrintPreviewWin : Window
    {
        private delegate void LoadXpsMethod();
        private readonly FlowDocument m_doc;

        public PrintPreviewWin(string strTmplName, double P_DocWidth, double P_DocHeight, JArray data, DateTime DataDateTime, IDocumentRenderer renderer = null)
        {
            InitializeComponent();
            m_doc = LoadDocumentAndRender(strTmplName, data, DataDateTime, renderer);
            m_doc.PageWidth = P_DocWidth;
            m_doc.PageHeight = P_DocHeight;
            m_doc.ColumnWidth = P_DocWidth;
            m_doc.Background = System.Windows.Media.Brushes.Transparent;
            m_doc.PagePadding = new Thickness(85, 70, 85, 70);//设置页面与页面之间的边距宽度
            Dispatcher.BeginInvoke(new LoadXpsMethod(LoadXps), DispatcherPriority.ApplicationIdle);
        }
        public static FlowDocument LoadDocumentAndRender(string strTmplName, JArray data, DateTime DataDateTime, IDocumentRenderer renderer = null)
        {
            FlowDocument doc = (FlowDocument)Application.LoadComponent(new Uri($"/Office.Work.Platform;component/Remuneration/{strTmplName}", UriKind.RelativeOrAbsolute));
            //doc.PagePadding = new Thickness(50);
            //doc.DataContext = data;
            if (renderer != null)
            {
                renderer.Render(doc, data, DataDateTime);
            }
            return doc;
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
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            writer.Write(((IDocumentPaginatorSource)m_doc).DocumentPaginator);

            //获取这个基于内存的xps document的fixed document
            docViewer.Document = xpsDocument.GetFixedDocumentSequence();

            //关闭基于内存的xps document
            xpsDocument.Close();
        }
    }
    ///// <summary>
    ///// PrintPreviewWindow.xaml 的交互逻辑
    ///// </summary>
    //public partial class PrintPreviewWindow : Window
    //{
    //    private delegate void LoadXpsMethod();//委托事件，相当于函数指针
    //    private readonly FlowDocument flowDoc;//流文档
    //    public PrintPreviewWindow(FlowDocument P_FlowDoc, double P_DocWidth, double P_DocHeight)//从上面得到待打印的文档
    //    {
    //        InitializeComponent();
    //        this.Owner = App.Current.MainWindow;
    //        flowDoc = new FlowDocument();
    //        flowDoc.Blocks.Add(P_FlowDoc.Blocks.FirstBlock);
    //        flowDoc.Blocks.Add(P_FlowDoc.Blocks.LastBlock);
    //        //flowDoc = P_FlowDoc;// (FlowDocument)Application.LoadComponent(new Uri(strTmplName, UriKind.RelativeOrAbsolute));//从xaml文件中加载流文档对象
    //        flowDoc.PageWidth = P_DocWidth;
    //        flowDoc.PageHeight = P_DocHeight;
    //        flowDoc.ColumnWidth = P_DocWidth;
    //        flowDoc.Background = System.Windows.Media.Brushes.Transparent;
    //        flowDoc.PagePadding = new Thickness(85, 70, 85, 70);//设置页面与页面之间的边距宽度
    //        Dispatcher.BeginInvoke(new LoadXpsMethod(LoadXps), DispatcherPriority.ApplicationIdle);//“延后”调用，不然刚刚更改的数据不会马上更新，也就是说打印或者预览不到更新后的数据
    //    }

    //    public void LoadXps()
    //    {

    //        //构造一个基于内存的xps document
    //        MemoryStream ms = new MemoryStream();
    //        Package package = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);
    //        Uri DocumentUri = new Uri("pack://InMemoryDocument.xps");
    //        PackageStore.RemovePackage(DocumentUri);
    //        PackageStore.AddPackage(DocumentUri, package);
    //        XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Fast, DocumentUri.AbsoluteUri);
    //        //将flow document写入基于内存的xps document中去
    //        XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);//在这里需要添加对.NET 4.0 的一些应用，比较蛋疼
    //        writer.Write(((IDocumentPaginatorSource)flowDoc).DocumentPaginator);
    //        //获取这个基于内存的xps document的fixed documen
    //        docViewer.Document = xpsDocument.GetFixedDocumentSequence();
    //        //关闭基于内存的xps document
    //        xpsDocument.Close();
    //    }

    //    private void Window_Unloaded(object sender, RoutedEventArgs e)
    //    {
    //        //flowDoc.PageWidth = Double.NaN;
    //        //flowDoc.PageHeight = Double.NaN;
    //        //flowDoc.ColumnWidth = Double.NaN;
    //    }
    //    //private void SaveToXps(Stream fileStream)
    //    //{
    //    //    Package package = Package.Open(fileStream, FileMode.Create, FileAccess.ReadWrite);
    //    //    XpsDocument doc = new XpsDocument(package);
    //    //    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
    //    //    IDocumentPaginatorSource document = CreateDocViewer().Document;
    //    //    writer.Write(document.DocumentPaginator);
    //    //    doc.Close();
    //    //    package.Close();
    //    //}

    //    //public void SaveToPdf(string pdfFileName)
    //    //{
    //    //    MemoryStream memStream = new MemoryStream();
    //    //    SaveToXps(memStream);
    //    //    WebSupergoo.ABCpdf11.Doc pdfDoc = new WebSupergoo.ABCpdf11.Doc();
    //    //    pdfDoc.Read(memStream, null, "xps");
    //    //    pdfDoc.Save(pdfFileName);
    //    //    pdfDoc.Clear();
    //    //    memStream.Close();
    //    //}
    //}
}
