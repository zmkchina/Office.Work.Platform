using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
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
        private CurPageViewModel _CurViewModel;

        public PageMemberPaySheet()
        {
            _CurViewModel = new CurPageViewModel();
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = _CurViewModel;
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
        private async void Btn_Search_ClickAsync(object sender, RoutedEventArgs e)
        {
            //获取所有可发放的待遇项目

            IEnumerable<MemberPayItem> CollectPayItem = await DataMemberPayItemRepository.GetRecords(new MemberPayItemSearch() { PayUnitName = AppSet.LoginUser.UnitName, UserId = AppSet.LoginUser.Id }).ConfigureAwait(false);
            _CurViewModel.PayItems = CollectPayItem?.ToList();
            //1.查询所有可发放的待遇项目信息
            _CurViewModel.SearchCondition.PayYear = _CurViewModel.PayYearMonth.Year;
            _CurViewModel.SearchCondition.PayMonth = _CurViewModel.PayYearMonth.Month;
            _CurViewModel.SalaryJArray.Clear();
            IEnumerable<MemberSalarySearchResult> SalaryList = await DataMemberSalaryRepository.GetRecords(_CurViewModel.SearchCondition).ConfigureAwait(false);
            if (SalaryList != null && SalaryList.Count() > 0)
            {
                foreach (MemberSalarySearchResult item in SalaryList)
                {
                    float YingFaDaiYu = 0f;
                    float GeRenJiaoNa = 0f;
                    float DanWeiJiaoNa = 0f;

                    JObject TempJobj = new JObject();
                    PropertyInfo[] Props = item.GetType().GetProperties();
                    for (int i = 0; i < Props.Length; i++)
                    {
                        if (_CurViewModel.NoPrintItemNames.Contains(Props[i].Name)) { continue; }
                        var CurValue = Props[i].GetValue(item);
                        if (CurValue != null)
                        {
                            if (Props[i].Name == "SalaryItems")
                            {
                                _CurViewModel.SalaryItems = CurValue as List<SalaryItem>;

                                //在 _CurViewModel.SalaryItems 中添加相应的汇总项
                                switch (item.TableType)
                                {
                                    case "月度工资表":
                                        int k = 0;
                                        for (k = 0; k < _CurViewModel.SalaryItems.Count; k++)
                                        {
                                            MemberPayItem CurPayItem = _CurViewModel.PayItems.Where(x => x.Name.Equals(_CurViewModel.SalaryItems[k].Name)).FirstOrDefault();
                                            if (CurPayItem != null && CurPayItem.PayType.Equals("个人交纳")) { break; }
                                        }
                                        _CurViewModel.SalaryItems.Insert(k, new SalaryItem() { Name = "应发合计", Amount = 0 });
                                        _CurViewModel.SalaryItems.Add(new SalaryItem() { Name = "实发合计", Amount = 0 });
                                        break;
                                    case "月度补贴表":
                                        _CurViewModel.SalaryItems.Add(new SalaryItem() { Name = "补贴合计", Amount = 0 });
                                        break;
                                    case "其他待遇表":
                                        _CurViewModel.SalaryItems.Add(new SalaryItem() { Name = "发放合计", Amount = 0 });
                                        break;
                                }
                                for (int ik = 0; ik < _CurViewModel.SalaryItems.Count; ik++)
                                {
                                    TempJobj[_CurViewModel.SalaryItems[ik].Name] = _CurViewModel.SalaryItems[ik].Amount;
                                    MemberPayItem CurPayItem = _CurViewModel.PayItems.Where(x => x.Name.Equals(_CurViewModel.SalaryItems[ik].Name)).FirstOrDefault();

                                    if (CurPayItem != null)
                                    {
                                        if (CurPayItem.PayType.Equals("应发待遇"))
                                        {
                                            YingFaDaiYu += _CurViewModel.SalaryItems[ik].Amount;
                                        }
                                        else if (CurPayItem.PayType.Equals("个人交纳"))
                                        {
                                            GeRenJiaoNa += _CurViewModel.SalaryItems[ik].Amount;
                                        }
                                        else if (CurPayItem.PayType.Equals("单位交纳"))
                                        {
                                            DanWeiJiaoNa += _CurViewModel.SalaryItems[ik].Amount;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Props[i].Name.Equals("UpDateTime"))
                                {
                                    DateTime upDate = DateTime.MinValue;
                                    DateTime.TryParse(CurValue.ToString(), out upDate);

                                    if (upDate == DateTime.MinValue) { upDate = DateTime.Now; }

                                    TempJobj[_CurViewModel.NamesEnCn[Props[i].Name]] = upDate.ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    TempJobj[_CurViewModel.NamesEnCn[Props[i].Name]] = CurValue.ToString();
                                }
                            }
                        }
                        else
                        {

                            TempJobj[_CurViewModel.NamesEnCn[Props[i].Name]] = "";
                            continue;
                        }
                    }
                    switch (item.TableType)
                    {
                        case "月度工资表":
                            TempJobj["应发合计"] = YingFaDaiYu.ToString("0.00");
                            TempJobj["实发合计"] = (YingFaDaiYu - GeRenJiaoNa).ToString("0.00");
                            break;
                        case "月度补贴表":
                            TempJobj["补贴合计"] = YingFaDaiYu.ToString("0.00");
                            break;
                        case "其他待遇表":
                            TempJobj["发放合计"] = YingFaDaiYu.ToString("0.00");
                            break;
                    }
                    _CurViewModel.SalaryJArray.Add(TempJobj);
                }
            }
            else
            {
                _CurViewModel.SalaryJArray.Clear();
                AppFuns.ShowMessage("未发现指定的待遇发放记录！");
                return;
            }
            App.Current.Dispatcher.Invoke(() =>
            {
                AppFuns.SetStateBarText($"共查询到：{_CurViewModel.SalaryJArray.Count} 条数据。");
                string Caption = $"{AppSet.LoginUser.UnitShortName}{_CurViewModel.SearchCondition.MemberType}人员{_CurViewModel.SearchCondition.TableType}";
                string DateStr = $"发放月份：（{_CurViewModel.SearchCondition.PayYear}年{_CurViewModel.SearchCondition.PayMonth}月）";
                if (!_CurViewModel.SearchCondition.TableType.Contains("月"))
                {
                    DateStr = $"发放时间：{_CurViewModel.PayYearMonth.Year}年{_CurViewModel.PayYearMonth.Month}月{_CurViewModel.PayYearMonth.Day}日";
                }
                CreateFlowDoc("PrintMemberPaySheetDot.xaml", Caption, DateStr, _CurViewModel.SalaryJArray, P_DocWidth: 1122, P_DocHeight: 793);
            });
        }

        private DocumentPaginator GetPaginator(FlowDocument doc)
        {
            bool? bPrintHeaderAndFooter = doc.Resources["PrintHeaderAndFooter"] as bool?;
            if (bPrintHeaderAndFooter == true)
            {
                return new PaginatorHeaderFooter(((IDocumentPaginatorSource)doc).DocumentPaginator);
            }
            else
            {
                return ((IDocumentPaginatorSource)doc).DocumentPaginator;
            }
        }

        private void CreateFlowDoc(string SheetTemplet, string Caption, string DateStr, JArray data, double P_DocWidth, double P_DocHeight)
        {
            //1.导入流文件格式模板
            FlowDocument m_doc = (FlowDocument)Application.LoadComponent(new Uri($"/Office.Work.Platform;component/MemberPay/{SheetTemplet}", UriKind.RelativeOrAbsolute));
            m_doc.PageWidth = P_DocWidth;
            m_doc.PageHeight = P_DocHeight;
            m_doc.ColumnWidth = P_DocWidth;
            m_doc.Background = System.Windows.Media.Brushes.Transparent;
            m_doc.PagePadding = new Thickness(85, 70, 85, 100);//设置页面与页面之间的边距宽度
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
            writer.Write(GetPaginator(m_doc));
            //使用自定义的 DocumentPaginator 取待之，以便可以设置页眉页脚。
            //writer.Write(((IDocumentPaginatorSource)m_doc).DocumentPaginator);

            //获取这个基于内存的xps document的fixed document
            docViewer.Document = xpsDocument.GetFixedDocumentSequence();

            //关闭基于内存的xps document
            xpsDocument.Close();
        }



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


        /// <summary>
        /// 当前页面的视图模型
        /// </summary>
        private class CurPageViewModel : NotificationObject
        {
            private bool _CanOperation = false;


            public JArray SalaryJArray { get; set; }
            public SettingServer ServerSettings { get; set; }
            public MemberSalarySearch SearchCondition { get; set; }
            public DateTime PayYearMonth { get; set; }
            public Dictionary<string, string> NamesEnCn = new Dictionary<string, string>();
            public List<SalaryItem> SalaryItems { get; set; }
            public List<MemberPayItem> PayItems { get; set; }
            public string[] NoPrintItemNames { get; set; }
            public bool CanOperation
            {
                get { return _CanOperation; }
                set
                {
                    _CanOperation = value; RaisePropertyChanged();
                }
            }

            public CurPageViewModel()
            {
                ServerSettings = AppSet.ServerSetting;
                SearchCondition = new MemberSalarySearch()
                {
                    UserId = AppSet.LoginUser.Id,
                    PayUnitName = AppSet.LoginUser.UnitName,
                    FillEmpty = false
                };
                PayYearMonth = DateTime.Now;
                SalaryJArray = new JArray();

                NamesEnCn.Add("Id", "编号");
                NamesEnCn.Add("PayUnitName", "发放单位");
                NamesEnCn.Add("PayYear", "年度");
                NamesEnCn.Add("PayMonth", "月份");
                NamesEnCn.Add("TableType", "发放类型");
                NamesEnCn.Add("MemberId", "身份证号");
                NamesEnCn.Add("MemberName", "姓名");
                NamesEnCn.Add("Remark", "备注");
                NamesEnCn.Add("UpDateTime", "更新时间");
                NamesEnCn.Add("UserId", "操作人员");

                NoPrintItemNames = new string[] { "Id", "PayUnitName", "PayYear", "PayMonth", "TableType", "MemberId", "UpDateTime", "UserId" };
            }
        }


    }


}
