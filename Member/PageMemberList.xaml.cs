using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// 职工信息列表页面
    /// </summary>
    public partial class PageMemberList : Page
    {
        private PageViewModel _PageViewModel;

        public PageMemberList()
        {
            InitializeComponent();
        }
        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            if (_PageViewModel == null)
            {
                _PageViewModel = new PageViewModel();
                await SearchMember();
                DataContext = _PageViewModel;
                CB_FieldName.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_Refrash_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_PageViewModel.FieldValue))
            {
                AppFuns.SetStateBarText("必须输入查询内容!");
                return;
            }
            //是否在结果中查询
            if (!_PageViewModel.SearchInResult)
            {
                _PageViewModel.mSearch = new MemberSearch();
            }

            //设置查询条件
            PropertyInfo[] SourceAttris = _PageViewModel.mSearch.GetType().GetProperties();
            var tempObj = SourceAttris.Where(x => x.Name.Equals(_PageViewModel.FieldEnName, StringComparison.Ordinal)).FirstOrDefault();
            if (tempObj != null)
            {
                tempObj.SetValue(_PageViewModel.mSearch, _PageViewModel.FieldValue);
            }
            await SearchMember();
        }
        /// <summary>
        /// 查询指定条件的记录
        /// </summary>
        /// <returns></returns>
        private async System.Threading.Tasks.Task SearchMember()
        {
            List<Lib.MemberInfoEntity> MemberList = await DataMemberRepository.ReadMembers(_PageViewModel.mSearch);

            if (MemberList == null)
            {
                AppFuns.SetStateBarText("查询过程出现错误，请重试!");
                return;
            }
            if (MemberList.Count < 1)
            {
                _PageViewModel.EntityList.Clear();
            }
            else
            {
                MemberList.Sort((x, y) => x.OrderIndex - y.OrderIndex);

                _PageViewModel.EntityList.Clear();
                MemberList.ForEach(e => { _PageViewModel.EntityList.Add(e); });
            }
            AppFuns.SetStateBarText($"共查询到记录：{_PageViewModel.EntityList.Count}条");
        }
        /// <summary>
        /// 双击开始编辑职工信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (((System.Windows.FrameworkElement)sender).DataContext is Lib.MemberInfoEntity CurMember)
            {
                PageEditMember pageEditMember = new PageEditMember(CurMember);
                AppSet.AppMainWindow.FrameContentPage.Content = pageEditMember;
            }
        }

        /// <summary>
        /// 删除选定记录。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_Delete_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (RecordDataGrid.SelectedItem is Lib.MemberInfoEntity SelectMember)
            {
                if (SelectMember != null && AppFuns.ShowMessage($"确定要删除[{SelectMember.Name}]信息吗？", "确认", showYesNo: true))
                {
                    ExcuteResult excuteResult = await DataMemberRepository.DeleteMember(SelectMember).ConfigureAwait(false);
                    if (excuteResult.State == 0)
                    {
                        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { _PageViewModel.EntityList.Remove(SelectMember); })); ;
                    }
                    else
                    {
                        AppFuns.ShowMessage(excuteResult.Msg, Caption: "失败");
                    }
                }
            }
        }
        /// <summary>
        /// 本页面退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("就绪");
        }

        /// <summary>
        /// 导出数据。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Export_ClickAsync(object sender, RoutedEventArgs e)
        {
            List<Lib.MemberInfoEntity> EntityList = _PageViewModel.EntityList.ToList();
            if (EntityList == null || EntityList.Count < 1)
            {
                return;
            }
            System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();
            fileDialog.Filter = "Excel|*.xls";
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            try
            {
                NpoiExcel.ExportExcels(fileDialog.FileName, "员工信息", EntityList);
                AppFuns.ShowMessage("数据导出成功！", Caption: "完成");
            }
            catch (Exception Ex)
            {
                AppFuns.ShowMessage(Ex.Message, Caption: "失败");
            }
        }



        /// <summary>
        /// 本页面的视图模型类
        /// </summary>
        public class PageViewModel : NotificationObject
        {

            public ObservableCollection<Lib.MemberInfoEntity> EntityList { get; set; }
            public Dictionary<string, string> FieldCn2En { get; set; }
            public MemberSearch mSearch;
            public string FieldEnName { get; set; }
            public string FieldValue { get; set; }
            public bool SearchInResult { get; set; }
            public bool _CanExportAll { get; set; }

            #region "方法"
            /// <summary>
            /// 构造函数
            /// </summary>
            public PageViewModel()
            {
                mSearch = new MemberSearch()
                {
                    UnitName = AppSet.LoginUser.UnitName
                };
                EntityList = new ObservableCollection<Lib.MemberInfoEntity>();
                FieldCn2En = new Dictionary<string, string>() {
                    { "Name", "姓名" }, { "UnitName", "单位" },{ "Job", "岗位性质" },{ "JobGrade", "岗位级别" },{ "Post", "行政职务" }, { "TechnicalTitle", "技术职称" },
                    { "EducationTop", "最高学历" }, { "Age", "年龄" },{ "PoliticalStatus", "政治面貌"  },{ "Remarks", "备注" }
                };
                _CanExportAll = AppSet.LoginUser.Grants.Contains("MemberExportAll");
            }
            #endregion
        }


    }
}
