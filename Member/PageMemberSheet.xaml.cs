using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// 职工基本信息表
    /// </summary>
    public partial class PageMemberSheet : Page
    {
        private Lib.MemberSearch mSearch;
        public PageMemberSheet()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mSearch = new MemberSearch();
            this.DataContext = mSearch;
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
            if (string.IsNullOrWhiteSpace(mSearch.Id)) { return; }
            List<Lib.Member> Members = await DataMemberRepository.ReadMembers(mSearch);
            if (Members.Count > 0)
            {
                string DateStr = $"打印日期：{DateTime.Now:yyy-MM-dd}";
                CreateFlowDoc("PrintMemberSheetDot.xaml", null, DateStr, Members[0], P_DocWidth: 793, P_DocHeight: 1122);
            }
        }
        private void CreateFlowDoc(string SheetTemplet, string Caption, string DateStr, Lib.Member data, double P_DocWidth, double P_DocHeight)
        {
            //1.导入流文件格式模板
            FlowDocument m_doc = (FlowDocument)Application.LoadComponent(new Uri($"/Office.Work.Platform;component/Member/{SheetTemplet}", UriKind.RelativeOrAbsolute));
            m_doc.PageWidth = P_DocWidth;
            m_doc.PageHeight = P_DocHeight;
            m_doc.ColumnWidth = P_DocWidth;
            m_doc.Background = System.Windows.Media.Brushes.Transparent;
            m_doc.PagePadding = new Thickness(85, 70, 85, 70);//设置页面与页面之间的边距宽度
            //2填充模板内容
            PrintMemberSheetRender renderer = new PrintMemberSheetRender();
            if (renderer != null)
            {
                renderer.Render(m_doc, Caption, DateStr, data);
            }
            //必须这样操作，否则m_doc中绑定的数据不显示，可能是未及时更新原因。
            //也就是说，等上面的Render完成后再，再构造内存中的Xps Document
            Dispatcher.BeginInvoke(() => { LoadXps(m_doc); }, DispatcherPriority.ApplicationIdle);
        }
        public void LoadXps(FlowDocument m_doc)
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
}
