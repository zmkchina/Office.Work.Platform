using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;

namespace Office.Work.Platform.Remuneration
{
    /// <summary>
    /// 快速发放待遇，即拷贝上月待遇数据。
    /// </summary>
    public partial class PageMemberPayFast : Page
    {
        private List<string> PayItemNameList { get; set; }
        private JArray PaySetJArray { get; set; }
        private const string SelectedChar = "✔";
        private const string NoSelectedChar = "❌";
        public PageMemberPayFast()
        {
            InitializeComponent();
            PayItemNameList = new List<string>();
            Run_PayUnitName.Text = AppSettings.LoginUser.UnitName;
        }

        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {

            PaySetJArray = await InitDataJArray().ConfigureAwait(false);
            //4.显示结果
            App.Current.Dispatcher.Invoke(() =>
            {
                //DataGridResult.ItemsSource = PaySetJArray;
                // DataGridResult.Columns.Add(new DataGridTextColumn() { Header = "身份证号", Binding = new Binding("身份证号") });
                // DataGridResult.Columns.Add(new DataGridCheckBoxColumn() { Header = "个税", Binding = new Binding("个税") });
                DataGridResult.ItemsSource = PaySetJArray;
            });
        }
        private async System.Threading.Tasks.Task<JArray> InitDataJArray()
        {

            //1.查询所有可发放的待遇项目信息
            JArray PaySetJArray = new JArray();
            IEnumerable<MemberPayItem> PayItems = await DataMemberPayItemRepository.GetRecords(new MemberPayItemSearch()
            {
                UnitName = AppSettings.LoginUser.UnitName,
                UserId = AppSettings.LoginUser.Id
            }).ConfigureAwait(false);

            PayItemNameList = PayItems.Select(x => x.Name).ToList();

            PayItemNameList.Insert(0, "发放单位");
            PayItemNameList.Insert(1, "所属单位");
            PayItemNameList.Insert(2, "身份证号");
            PayItemNameList.Insert(3, "姓名");


            //2.查询已经配置的拷贝项目数据信息
            IEnumerable<MemberPaySet> OldPaySets = await DataMemberPaySetRepository.GetRecords(new MemberPaySetSearch()
            {
                UserId = AppSettings.LoginUser.UnitName,
                PayUnitName = AppSettings.LoginUser.UnitName
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
                    UnitName = AppSettings.LoginUser.UnitName
                }).ConfigureAwait(false);
                List<Lib.Member> MemberList = Members.ToList();
                MemberList.Sort((x, y) => x.OrderIndex - y.OrderIndex);

                for (int i = 0; i < MemberList.Count; i++)
                {
                    Lib.Member CurMember = MemberList[i];
                    JObject TempJobj = new JObject();
                    TempJobj[PayItemNameList[0]] = CurMember.UnitName;
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
                TempPaySet.UserId = AppSettings.LoginUser.Id;
                NewPaySetList.Add(TempPaySet);
            }
            //2.保存
            ExcuteResult excuteResult = await DataMemberPaySetRepository.AddOrUpdateRecord(NewPaySetList).ConfigureAwait(false);
            App.Current.Dispatcher.Invoke(() =>
            {
                if (excuteResult.State == 0)
                {
                    (new WinMsgDialog("人员待遇发放项目配置成功。")).ShowDialog();
                }
                else
                {
                    (new WinMsgDialog("保存失败，请稍候再试。", "错误", isErr: true)).ShowDialog();
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
            NewDic.Add(PayItemNameList[0], AppSettings.LoginUser.UnitName);
            NewDic.Add(PayItemNameList[1], NewMamber.UnitName);
            NewDic.Add(PayItemNameList[2], NewMamber.Id);
            NewDic.Add(PayItemNameList[3], NewMamber.Name);

            for (int i = 4; i < PayItemNameList.Count; i++)
            {
                NewDic.Add(PayItemNameList[i], SelectedChar);

            }
            if (PaySetJArray.Any(e => e["身份证号"].ToString().Equals(NewDic["身份证号"].ToString())))
            {
                (new WinMsgDialog($"身份证号为[{NewDic["身份证号"]}]的用户已经存在。", "错误", isErr: true)).ShowDialog();
                return;
            }
            PaySetJArray.Add(NewDic);
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
                PaySetJArray.Remove(curItem);
            }
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

        private void Btn_Pay_ClickAnsyc(object sender, RoutedEventArgs e)
        {

        }
    }
}
