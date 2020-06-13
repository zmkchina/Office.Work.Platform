using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using Button = System.Windows.Controls.Button;

namespace Office.Work.Platform.MemberPay
{
    /// <summary>
    /// 快速发放待遇，即拷贝上月待遇数据。
    /// </summary>
    public partial class PageMemberPayFast : Page
    {
        private CurPageViewModel _CurViewModel;
        public PageMemberPayFast()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _CurViewModel = new CurPageViewModel();
            DataContext = _CurViewModel;

            //DataGridResult.UpdateLayout();
            //PayItemNameList = new List<string>();
            //Run_PayUnitName.Text = AppSet.LoginUser.UnitName;
            //PaySetJArray = await InitDataJArray().ConfigureAwait(false);
            ////4.显示结果
            //App.Current.Dispatcher.Invoke(() =>
            //{
            //    //DataGridResult.ItemsSource = PaySetJArray;
            //    //DataGridResult.Columns.Add(new DataGridTextColumn() { Header = "身份证号", Binding = new Binding("身份证号") });
            //    //DataGridResult.Columns.Add(new DataGridCheckBoxColumn() { Header = "个税", Binding = new Binding("个税") });
            //    this.DataContext = this;
            //});
        }

        /*
      private async System.Threading.Tasks.Task<JArray> InitDataJArray()
      {

          //1.查询所有可发放的待遇项目信息
          JArray PaySetJArray = new JArray();
          IEnumerable<MemberPayItem> PayItems = await DataMemberPayItemRepository.GetRecords(new Lib.MemberPayItemSearch()
          {
              PayUnitName = AppSet.LoginUser.UnitName,
              UserId = AppSet.LoginUser.Id
          }).ConfigureAwait(false);

          if (PayItems == null || PayItems.Count() < 1) { return PaySetJArray; }

          PayItemNameList = PayItems.Select(x => x.Name).ToList();

          PayItemNameList.Insert(0, "发放单位");
          PayItemNameList.Insert(1, "所属单位");
          PayItemNameList.Insert(2, "身份证号");
          PayItemNameList.Insert(3, "姓名");


          //2.查询已经配置的拷贝项目数据信息
          IEnumerable<MemberPaySet> OldPaySets = await DataMemberPaySetRepository.GetRecords(new MemberPaySetSearch()
          {
              PayUnitName = AppSet.LoginUser.UnitName,
              UserId = AppSet.LoginUser.Id
          }).ConfigureAwait(false);

          //3.定义绑定到界面的数据对象。
          if (OldPaySets != null && OldPaySets.Count() > 0)
          {
              //(1)如果该单位有旧的设置数据，则显示之。
              List<MemberPaySet> OldPaySetList = OldPaySets.ToList();
              for (int i = 0; i < OldPaySetList.Count; i++)
              {
                  MemberPaySet CurPaySet = OldPaySetList[i];
                  JObject TempJobj = new JObject();
                  TempJobj[PayItemNameList[0]] = CurPaySet.PayUnitName;
                  TempJobj[PayItemNameList[1]] = CurPaySet.MemberUnitName;
                  TempJobj[PayItemNameList[2]] = CurPaySet.MemberId;
                  TempJobj[PayItemNameList[3]] = CurPaySet.MemberName;


                  for (int j = 4; j < PayItemNameList.Count(); j++)
                  {
                      if (CurPaySet.PayItemNames == null)
                      {
                          TempJobj[PayItemNameList[j]] = NoSelectedChar;// false;// "否";
                      }
                      else
                      {
                          TempJobj[PayItemNameList[j]] = CurPaySet.PayItemNames.Contains(PayItemNameList[j], StringComparison.Ordinal) ? SelectedChar : NoSelectedChar;// true : false;// "是" : "否";
                      }
                  }
                  PaySetJArray.Add(TempJobj);
              }
          }
          else
          {
              //(1)如果没有则“查询该单位所有人员信息列表”
              IEnumerable<Lib.Member> Members = await DataMemberRepository.ReadMembers(new MemberSearch()
              {
                  UnitName = AppSet.LoginUser.UnitName
              }).ConfigureAwait(false);

              if (Members == null || Members.Count() < 1) { return PaySetJArray; }

              List<Lib.Member> MemberList = Members.ToList();
              MemberList.Sort((x, y) => x.OrderIndex - y.OrderIndex);

              for (int i = 0; i < MemberList.Count; i++)
              {
                  Lib.Member CurMember = MemberList[i];
                  JObject TempJobj = new JObject();
                  TempJobj[PayItemNameList[0]] = AppSet.LoginUser.UnitName;
                  TempJobj[PayItemNameList[1]] = CurMember.UnitName;
                  TempJobj[PayItemNameList[2]] = CurMember.Id;
                  TempJobj[PayItemNameList[3]] = CurMember.Name;

                  for (int j = 4; j < PayItemNameList.Count(); j++)
                  {
                      TempJobj[PayItemNameList[j]] = SelectedChar;// true;// "是";
                  }
                  PaySetJArray.Add(TempJobj);
              }
          }
          return PaySetJArray;
         
    }
      


        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn_Save_ClickAnsyc(object sender, RoutedEventArgs e)
        {
            if (sender is Button curBtn) { curBtn.IsEnabled = false; } else { curBtn = null; }
            //1.读取结果
            List<MemberPaySet> NewPaySetList = new List<MemberPaySet>();
            for (int i = 0; i < PaySetJArray.Count; i++)
            {
                MemberPaySet TempPaySet = new MemberPaySet();
                JObject TempJObject = PaySetJArray[i] as JObject;
                for (int j = 0; j < PayItemNameList.Count(); j++)
                {
                    switch (PayItemNameList[j])
                    {
                        case "发放单位":
                            TempPaySet.PayUnitName = TempJObject[PayItemNameList[j]].ToString();
                            break;
                        case "所属单位":
                            TempPaySet.MemberUnitName = TempJObject[PayItemNameList[j]].ToString();
                            break;
                        case "身份证号":
                            TempPaySet.MemberId = TempJObject[PayItemNameList[j]].ToString();
                            break;
                        case "姓名":
                            TempPaySet.MemberName = TempJObject[PayItemNameList[j]].ToString();
                            break;
                        default:
                            if (TempJObject[PayItemNameList[j]].ToString().Equals(SelectedChar, StringComparison.Ordinal))
                            {
                                TempPaySet.PayItemNames += $"{PayItemNameList[j]},";
                            }
                            break;
                    }
                }
                TempPaySet.Id = TempPaySet.MemberId;
                TempPaySet.UserId = AppSet.LoginUser.Id;
                NewPaySetList.Add(TempPaySet);
            }
            //2.保存
            ExcuteResult excuteResult = await DataMemberPaySetRepository.AddOrUpdateRecord(NewPaySetList).ConfigureAwait(false);
            App.Current.Dispatcher.Invoke(() =>
            {
                if (excuteResult.State == 0)
                {
                    AppFuns.ShowMessage("人员待遇发放项目配置成功。");
                }
                else
                {
                    AppFuns.ShowMessage("保存失败，请稍候再试。", "错误", isErr: true);
                }
                if (curBtn != null) { curBtn.IsEnabled = true; }
            });
        }

        /// <summary>
        /// 新增员工
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Add_Click(object sender, RoutedEventArgs e)
        {
            Lib.Member NewMamber = new Lib.Member();
            PageMemberPayFastWin WinAddMember = new PageMemberPayFastWin(NewMamber);

            if (WinAddMember.ShowDialog().Value == false)
            {
                return;
            }
            JObject NewDic = new JObject();
            NewDic.Add(PayItemNameList[0], AppSet.LoginUser.UnitName);
            NewDic.Add(PayItemNameList[1], NewMamber.UnitName);
            NewDic.Add(PayItemNameList[2], NewMamber.Id);
            NewDic.Add(PayItemNameList[3], NewMamber.Name);

            for (int i = 4; i < PayItemNameList.Count; i++)
            {
                NewDic.Add(PayItemNameList[i], SelectedChar);

            }
            if (PaySetJArray.Any(e => e["身份证号"].ToString().Equals(NewDic["身份证号"].ToString())))
            {
                AppFuns.ShowMessage($"身份证号为[{NewDic["身份证号"]}]的用户已经存在。", "错误", isErr: true);
                return;
            }
            PaySetJArray.Add(NewDic);
        }

        /// <summary>
        /// 双击变换选中状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridCell_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DataGridCell curCell)
            {
                if (DataGridResult.SelectedItem is JObject CurJobject)
                {
                    if (CurJobject[curCell.Column.Header].ToString().Equals(SelectedChar))
                    {
                        CurJobject[curCell.Column.Header] = NoSelectedChar;
                        curCell.Foreground = Brushes.Red;
                    }
                    else if (CurJobject[curCell.Column.Header].ToString().Equals(NoSelectedChar))
                    {
                        CurJobject[curCell.Column.Header] = SelectedChar;
                        curCell.Foreground = Brushes.Green;
                    }
                }
            }
        }
        /// <summary>
        /// 快速生成数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn_Pay_ClickAnsyc(object sender, RoutedEventArgs e)
        {
            if (sender is Button curBtn) { curBtn.IsEnabled = false; } else { curBtn = null; }

            MemberPayFastByPaySet PayFastInfo = new MemberPayFastByPaySet()
            {
                PayYear = PayYearMonth.Year,
                PayMonth = PayYearMonth.Month,
                PayUnitName = AppSet.LoginUser.UnitName,
                UserId = AppSet.LoginUser.Id
            };
            ExcuteResult excuteResult = await DataMemberPaySheetRepository.PostMemberPaySheet(PayFastInfo).ConfigureAwait(false);
            App.Current.Dispatcher.Invoke(() =>
            {
                AppCodes.AppFuns.ShowMessage(excuteResult.Msg);
                if (curBtn != null) { curBtn.IsEnabled = true; }
            });
        }
        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Del_ClickAnsyc(object sender, RoutedEventArgs e)
        {
            if (DataGridResult.SelectedItem is JObject curItem)
            {
                if (AppFuns.ShowMessage($"确定要删除所选人员？", "确认"))
                {
                    PaySetJArray.Remove(curItem);
                }
            }
        }
        */
        /// <summary>
        /// 设定发放条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn_Last_ClickAnsyc(object sender, RoutedEventArgs e)
        {
            //1.查询所有可发放的待遇项目信息
            _CurViewModel.SearchCondition.PayYear = _CurViewModel.PayYearMonth.Year;
            _CurViewModel.SearchCondition.PayMonth = _CurViewModel.PayYearMonth.Month;
            _CurViewModel.SalaryJArray.Clear();
            IEnumerable<MemberSalarySearchResult> SalaryList = await DataMemberSalaryRepository.GetRecords(_CurViewModel.SearchCondition).ConfigureAwait(false);
            if (SalaryList != null && SalaryList.Count() > 0)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    DataGridResult.ItemsSource = null;
                    foreach (MemberSalarySearchResult item in SalaryList)
                    {
                        JObject TempJobj = new JObject();
                        PropertyInfo[] Props = item.GetType().GetProperties();
                        for (int i = 0; i < Props.Length; i++)
                        {
                            var CurValue = Props[i].GetValue(item);
                            if (CurValue != null)
                            {
                                if (Props[i].Name == "SalaryItems")
                                {
                                    List<SalaryItem> SalaryItems = CurValue as List<SalaryItem>;
                                    for (int ik = 0; ik < SalaryItems.Count; ik++)
                                    {
                                        TempJobj[SalaryItems[ik].Name] = SalaryItems[ik].Amount;
                                    }
                                }
                                else
                                {
                                    if (Props[i].Name.Equals("UpDateTime"))
                                    {
                                        if (DateTime.TryParse(CurValue.ToString(), out DateTime upDate))
                                        {
                                            TempJobj[_CurViewModel.NamesEnCn[Props[i].Name]] = upDate.ToString("yyyy-MM-dd");
                                        }
                                        else
                                        {
                                            TempJobj[_CurViewModel.NamesEnCn[Props[i].Name]] = DateTime.Now.ToString("yyyy-MM-dd");
                                        }
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
                        _CurViewModel.SalaryJArray.Add(TempJobj);
                    }
                    DataGridResult.ItemsSource = _CurViewModel.SalaryJArray;
                    foreach (DataGridColumn item in DataGridResult.Columns)
                    {
                        if (_CurViewModel.NamesEnCn.Values.Contains(item.Header.ToString()))
                        {
                            if (item.Header.ToString().Equals("备注")) { continue; }
                            item.IsReadOnly = true;
                        }
                    }
                    //DataGridResult.Columns.Add(new DataGridCheckBoxColumn() { Header = "姓名", Binding = new Binding("MemberName") });
                });
                _CurViewModel.CanOperation = true;
            }
            else
            {
                _CurViewModel.CanOperation = false;
            }
        }

        private void Btn_Save_ClickAnsyc(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_AddPerson_ClickAnsyc(object sender, RoutedEventArgs e)
        {
            JToken NewJtoken = _CurViewModel.SalaryJArray.First.DeepClone();
            NewJtoken["身份证号"] = "333333";
            NewJtoken["姓名"] = "张三";
            _CurViewModel.SalaryJArray.Add(NewJtoken);
            DataGridResult.SelectedIndex = DataGridResult.Items.Count - 1;
            DataGridResult.ScrollIntoView(DataGridResult.SelectedItem);
        }

        private void Btn_DelPerson_ClickAnsyc(object sender, RoutedEventArgs e)
        {
            if (DataGridResult.SelectedItem is JObject CurJobject)
            {
                if (AppFuns.ShowMessage("确认要删除选定记录吗？", "确认", false, true))
                {
                    _CurViewModel.SalaryJArray.Remove(CurJobject);
                }
            }
        }

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
                    PayUnitName = AppSet.LoginUser.UnitName
                };
                PayYearMonth = DateTime.Now;
                SalaryJArray = new JArray();

                NamesEnCn.Add("PayUnitName", "发放单位");
                NamesEnCn.Add("PayYear", "年度");
                NamesEnCn.Add("PayMonth", "月份");
                NamesEnCn.Add("TableType", "发放类型");
                NamesEnCn.Add("MemberId", "身份证号");
                NamesEnCn.Add("MemberName", "姓名");
                NamesEnCn.Add("Remark", "备注");
                NamesEnCn.Add("UpDateTime", "更新时间");
                NamesEnCn.Add("UserId", "操作人员");
            }


            //public DateTime PayYearMonth { get; set; } = DateTime.Now;
            //public JArray PaySetJArray { get; set; }
            //private List<string> PayItemNameList { get; set; }
            //private const string SelectedChar = "✔";
            //private const string NoSelectedChar = "❌";


        }

    }
}
