using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Newtonsoft.Json.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Remuneration
{
    /// <summary>
    /// 快速发放待遇，即拷贝上月待遇数据。
    /// </summary>
    public partial class PageMemberPayFast : Page
    {
        private List<string> PayItemNameList;
        private JArray PaySetJArray;

        public PageMemberPayFast()
        {
            InitializeComponent();
            PayItemNameList = new List<string>();
        }

        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {

            PaySetJArray = await InitDataJArray().ConfigureAwait(false);
            //4.显示结果
            App.Current.Dispatcher.Invoke(() =>
            {
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
            PayItemNameList.Insert(0, "单位名称");
            PayItemNameList.Insert(1, "身份证号");
            PayItemNameList.Insert(2, "姓名");
            //2.查询已经配置的拷贝项目数据信息
            IEnumerable<MemberPaySet> OldPaySets = await DataMemberPaySetRepository.GetRecords(new MemberPaySetSearch()
            {
                UserId = AppSettings.LoginUser.UnitName,
                UnitName = AppSettings.LoginUser.UnitName
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
                    TempJobj[PayItemNameList[0]] = AppSettings.LoginUser.UnitName;
                    TempJobj[PayItemNameList[1]] = CurPaySet.MemberId;
                    TempJobj[PayItemNameList[2]] = CurPaySet.MemberName;


                    for (int j = 3; j < PayItemNameList.Count(); j++)
                    {
                        if (CurPaySet.PayItemNames == null)
                        {
                            TempJobj[PayItemNameList[j]] = "否";
                        }
                        else
                        {
                            TempJobj[PayItemNameList[j]] = CurPaySet.PayItemNames.Contains(PayItemNameList[j], StringComparison.Ordinal) ? "是" : "否";
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
                    TempJobj[PayItemNameList[0]] = AppSettings.LoginUser.UnitName;
                    TempJobj[PayItemNameList[1]] = CurMember.Id;
                    TempJobj[PayItemNameList[2]] = CurMember.Name;

                    for (int j = 3; j < PayItemNameList.Count(); j++)
                    {
                        TempJobj[PayItemNameList[j]] = "是";
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
                        case "单位名称":
                            TempPaySet.UnitName = TempJObject[PayItemNameList[j]].ToString();
                            break;
                        case "身份证号":
                            TempPaySet.MemberId = TempJObject[PayItemNameList[j]].ToString();
                            break;
                        case "姓名":
                            TempPaySet.MemberName = TempJObject[PayItemNameList[j]].ToString();
                            break;
                        default:
                            if (TempJObject[PayItemNameList[j]].ToString().Equals("是", StringComparison.Ordinal))
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
        /// 生成数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Pay_ClickAnsyc(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 新增员工
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Add_ClickAnsyc(object sender, RoutedEventArgs e)
        {
            JObject NewDic = new JObject();
            NewDic.Add(PayItemNameList[0], "市大柳巷船闸管理处");
            NewDic.Add(PayItemNameList[1], "321302111111111");
            NewDic.Add(PayItemNameList[2], "无名氏");

            for (int i = 3; i < PayItemNameList.Count; i++)
            {
                NewDic.Add(PayItemNameList[i], false);

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

        private void DataGridCell_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DataGridCell curCell)
            {
                if (DataGridResult.SelectedItem is JObject CurJobject)
                {
                    if (CurJobject[curCell.Column.Header].ToString().Equals("是"))
                    { CurJobject[curCell.Column.Header] = "否"; }
                    else if (CurJobject[curCell.Column.Header].ToString().Equals("否"))
                    { CurJobject[curCell.Column.Header] = "是"; }
                }
            }
        }




        #region 备份，未使用以下代码。

        private void ShowGridResult(JArray PaySetList, List<string> PayItemNameList)
        {
            Grid GridResult = new Grid();
            if (PaySetList == null) return;
            GridResult.Children.Clear();
            GridResult.ColumnDefinitions.Clear();
            GridResult.RowDefinitions.Clear();

            //生成GridResult表头
            GridResult.RowDefinitions.Add(new RowDefinition());
            for (int j = 0; j < PayItemNameList.Count(); j++)
            {
                ColumnDefinition tempCol = new ColumnDefinition();
                if (PayItemNameList[j].Equals("身份证号", StringComparison.Ordinal))
                {
                    tempCol.Width = new GridLength(0);
                }
                else
                {
                    tempCol.Width = new GridLength(1, GridUnitType.Auto);
                }
                GridResult.ColumnDefinitions.Add(tempCol);
                TextBlock tempTB = new TextBlock()
                {
                    Text = PayItemNameList[j],
                    Margin = new Thickness(4, 6, 4, 6),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(tempTB, 0); Grid.SetColumn(tempTB, j);
                GridResult.Children.Add(tempTB);
            }
            ColumnDefinition tempLastCol = new ColumnDefinition();
            tempLastCol.Width = new GridLength(1, GridUnitType.Auto);
            GridResult.ColumnDefinitions.Add(tempLastCol);

            //生成GridResult数据行
            for (int i = 0; i < PaySetList.Count; i++)
            {
                RowDefinition CurRow = new RowDefinition();
                GridResult.RowDefinitions.Add(CurRow);
                //添加行矩形，以便能提供鼠标移上去变色。
                //System.Windows.Shapes.Rectangle GRT = new System.Windows.Shapes.Rectangle();
                //GRT.PreviewMouseMove += GRT_MouseMove;
                //GRT.MouseLeave += GRT_MouseLeave;
                //GRT.Fill = System.Windows.Media.Brushes.Transparent;
                //Grid.SetRow(GRT, i + 1);
                //Grid.SetColumnSpan(GRT, PayItemNameList.Count() + 1);
                //GridResult.Children.Add(GRT);

                for (int j = 0; j < PayItemNameList.Count(); j++)
                {
                    if (j > 2)
                    {
                        //序号2之后应为checkbox
                        CheckBox tempCB = new CheckBox()
                        {
                            Margin = new Thickness(4, 6, 4, 6),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            ToolTip = $"{ PaySetList[i]["姓名"]} ：{ PayItemNameList[j]}",
                            FontSize = 18

                        };

                        //if (i % 2 == 0)
                        //{
                        //    tempCB.Background = Brushes.LightSkyBlue;
                        //}
                        //else
                        //{
                        //    tempCB.Background = Brushes.LightYellow;
                        //}
                        tempCB.IsChecked = (bool)PaySetList[i][PayItemNameList[j]];
                        Grid.SetRow(tempCB, i + 1); Grid.SetColumn(tempCB, j);
                        GridResult.Children.Add(tempCB);
                    }
                    else
                    {
                        TextBlock tempTB = new TextBlock()
                        {
                            Text = PaySetList[i][PayItemNameList[j]].ToString(),
                            Margin = new Thickness(6),
                            HorizontalAlignment = HorizontalAlignment.Center
                        };
                        //if (i % 2 == 0)
                        //{
                        //    tempTB.Background = Brushes.LightSkyBlue;
                        //}
                        //else
                        //{
                        //    tempTB.Background = Brushes.LightYellow;
                        //}
                        Grid.SetRow(tempTB, i + 1); Grid.SetColumn(tempTB, j);
                        GridResult.Children.Add(tempTB);
                    }
                }
            }
        }
        private async void SaveDatas(Grid GridResult)
        {
            //1.读取结果
            List<MemberPaySet> NewPaySetList = new List<MemberPaySet>();
            int k = 0;
            MemberPaySet TempPaySet = new MemberPaySet();
            for (int i = GridResult.ColumnDefinitions.Count - 1; i < GridResult.Children.Count; i++)
            {
                switch (k)
                {
                    case 0: //单位名称
                        if (GridResult.Children[i] is TextBlock tb1)
                        {
                            TempPaySet.UnitName = tb1.Text;
                        }
                        break;
                    case 1: //身份证号
                        if (GridResult.Children[i] is TextBlock tb2)
                        {
                            TempPaySet.MemberId = tb2.Text;
                        }
                        break;
                    case 2: //姓名
                        if (GridResult.Children[i] is TextBlock tb3)
                        {
                            TempPaySet.MemberName = tb3.Text;
                        }
                        break;
                    default:
                        if (GridResult.Children[i] is CheckBox cb1)
                        {
                            if (cb1.IsChecked is true)
                            {
                                TempPaySet.PayItemNames += $"{PayItemNameList[k]},";
                            }
                        }
                        break;
                }
                if (k == PayItemNameList.Count - 1)
                {
                    TempPaySet.UserId = AppSettings.LoginUser.Id;
                    NewPaySetList.Add(TempPaySet);
                    TempPaySet = new MemberPaySet();
                    k = 0;
                }
                else
                {
                    k++;
                }
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
            });
        }
        #endregion


    }
}
