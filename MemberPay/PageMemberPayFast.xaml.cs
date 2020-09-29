using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

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
        }

        /// <summary>
        /// 获取上次数据，以便供编辑使用。
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
                                    _CurViewModel.SalaryItems = CurValue as List<SalaryItem>;
                                    for (int ik = 0; ik < _CurViewModel.SalaryItems.Count; ik++)
                                    {
                                        TempJobj[_CurViewModel.SalaryItems[ik].Name] = _CurViewModel.SalaryItems[ik].Amount;
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

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn_Save_ClickAnsyc(object sender, RoutedEventArgs e)
        {
            int AddedCount = 0;
            List<Lib.MemberSalary> MemberSalaries = new List<MemberSalary>();
            for (int i = 0; i < _CurViewModel.SalaryJArray.Count; i++)
            {
                JToken TempJtoken = _CurViewModel.SalaryJArray[i];
                Lib.MemberSalary TempSalary = new MemberSalary();
                PropertyInfo[] propertyInfos = TempSalary.GetType().GetProperties();
                foreach (PropertyInfo item in propertyInfos)
                {

                    if (_CurViewModel.NamesEnCn.Keys.Contains(item.Name))
                    {
                        if (item.Name.Equals("PayYear") || item.Name.Equals("PayMonth"))
                        {
                            if (int.TryParse(TempJtoken[_CurViewModel.NamesEnCn[item.Name]].ToString(), out int CurNumValue))
                            {
                                item.SetValue(TempSalary, CurNumValue);
                            }
                            continue;
                        }
                        if (item.Name.Equals("UpDateTime"))
                        {
                            if (DateTime.TryParse(TempJtoken[_CurViewModel.NamesEnCn[item.Name]].ToString(), out DateTime CurDateValue))
                            {
                                item.SetValue(TempSalary, CurDateValue);
                            }
                            continue;
                        }
                        item.SetValue(TempSalary, TempJtoken[_CurViewModel.NamesEnCn[item.Name]].ToString());
                        continue;
                    }
                    if (item.Name.Equals("NameAndAmount"))
                    {
                        for (int ik = 0; ik < _CurViewModel.SalaryItems.Count; ik++)
                        {
                            if (float.TryParse(TempJtoken[_CurViewModel.SalaryItems[ik].Name].ToString(), out float CurFloatValue))
                            {
                                _CurViewModel.SalaryItems[ik].Amount = CurFloatValue;
                            }
                            else
                            {
                                AppFuns.ShowMessage($"“{TempJtoken[_CurViewModel.SalaryItems[ik].Name].ToString()}”应为金额！");
                                return;
                            }
                        }
                        string SalaryJsonStr = JsonConvert.SerializeObject(_CurViewModel.SalaryItems);
                        item.SetValue(TempSalary, SalaryJsonStr);
                    }
                }
                if (string.IsNullOrWhiteSpace(TempSalary.UserId)) { TempSalary.UserId = AppSet.LoginUser.Id; }

                MemberSalaries.Add(TempSalary);
            }
            foreach (Lib.MemberSalary TempSalary in MemberSalaries)
            {
                ExcuteResult excuteResult = await DataMemberSalaryRepository.AddOrUpdate(TempSalary).ConfigureAwait(false);
                if (excuteResult.State == 0)
                {
                    JToken TempJtoken = _CurViewModel.SalaryJArray.Where(x => x["身份证号"].ToString().Equals(TempSalary.MemberId)).FirstOrDefault();
                    if (TempJtoken != null)
                    {
                        TempJtoken["编号"] = excuteResult.Tag;
                    }
                    AddedCount++;
                }
            }

            if (AddedCount > 0)
            {
                if (AddedCount == _CurViewModel.SalaryJArray.Count)
                {
                    AppFuns.ShowMessage("数据保存成功");
                }
                else
                {
                    AppFuns.ShowMessage($"数据部分保存成功{AddedCount}/{_CurViewModel.SalaryJArray.Count}");
                }
            }
            else
            {
                AppFuns.ShowMessage("数据保存失败！");
            }
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_AddPerson_ClickAnsyc(object sender, RoutedEventArgs e)
        {
            Lib.MemberInfoEntity NewMamber = new Lib.MemberInfoEntity();
            PageMemberPayFastWin WinAddMember = new PageMemberPayFastWin(NewMamber);

            if (WinAddMember.ShowDialog().Value == false)
            {
                return;
            }
            JToken TempJtoken = _CurViewModel.SalaryJArray.Where(x => x["身份证号"].ToString().Equals(NewMamber.Id)).FirstOrDefault();
            if (TempJtoken != null)
            {
                AppFuns.ShowMessage($"[{NewMamber.Name}]的已经存在。", "重复");
                return;
            }
            JToken NewJtoken = _CurViewModel.SalaryJArray.First.DeepClone();
            NewJtoken["身份证号"] = NewMamber.Id;
            NewJtoken["姓名"] = NewMamber.Name;
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
            public List<SalaryItem> SalaryItems { get; set; }
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
                    FillEmpty = true
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
            }
        }
    }
}
