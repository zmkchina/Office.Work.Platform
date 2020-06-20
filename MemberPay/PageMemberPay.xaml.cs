using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
    /// PageMemberPay.xaml 的交互逻辑
    /// </summary>
    public partial class PageMemberPay : Page
    {
        private CurPageViewModel _CurViewModel;
        public PageMemberPay()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _CurViewModel = new CurPageViewModel();
            DataContext = _CurViewModel;
        }

        /// <summary>
        ///  获取上次数据，以便供编辑使用。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn_GetData_ClickAsync(object sender, RoutedEventArgs e)
        {
            string MemberId = null;
            if (string.IsNullOrWhiteSpace(_CurViewModel.SearchCondition.TableType) || string.IsNullOrWhiteSpace(_CurViewModel.SearchCondition.MemberId))
            {
                return;
            }
            //1.查询所有可发放的待遇项目信息
            _CurViewModel.SalaryJArray.Clear();
            IEnumerable<MemberSalarySearchResult> SalaryList = await DataMemberSalaryRepository.GetRecords(_CurViewModel.SearchCondition).ConfigureAwait(false);
            if (SalaryList != null && SalaryList.Count() > 0)
            {
                await App.Current.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    MemberId = _CurViewModel.SearchCondition.MemberId.Clone().ToString();
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

                    DataGridResult.ItemsSource = null;
                    DataGridResult.ItemsSource = _CurViewModel.SalaryJArray;

                    foreach (DataGridColumn item in DataGridResult.Columns)
                    {
                        if (_CurViewModel.NamesEnCn.Values.Contains(item.Header.ToString()))
                        {
                            if (item.Header.ToString().Equals("备注")) { continue; }
                            item.IsReadOnly = true;
                        }
                    }
                    _CurViewModel.CanOperation = true;
                    await UcMemberPayFile.InitFileDatasAsync(MemberId, "个人待遇", true);
                }), null);
            }
            else
            {
                _CurViewModel.CanOperation = false;
                AppFuns.ShowMessage("未查询到相关记录，请先使用快速发放！");
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn_SaveData_ClickAsync(object sender, RoutedEventArgs e)
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
        /// 当前页面的视图模型
        /// </summary>
        private class CurPageViewModel : NotificationObject
        {
            private bool _CanOperation = false;

            public JArray SalaryJArray { get; set; }
            public SettingServer ServerSettings { get; set; }
            public MemberSalarySearch SearchCondition { get; set; }
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
                    PayYear = DateTime.Now.Year,
                    FillEmpty = false
                };

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
