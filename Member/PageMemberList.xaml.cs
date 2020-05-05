﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// PageFilesList.xaml 的交互逻辑
    /// </summary>
    public partial class PageMemberList : Page
    {
        private PageMemberListVM _PageMemberListVM;
        private MemberSearch mSearch;

        public PageMemberList()
        {
            InitializeComponent();
        }
        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            if (_PageMemberListVM == null)
            {
                _PageMemberListVM = new PageMemberListVM();
                mSearch = new MemberSearch()
                {
                    UnitName = AppSettings.LoginUser.UnitName
                };
                await SearchMember(mSearch);
                DataContext = _PageMemberListVM;
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
            if (string.IsNullOrWhiteSpace(_PageMemberListVM.FieldValue))
            {
                return;
            }
            if (!_PageMemberListVM.SearchInResult)
            {
                mSearch = new MemberSearch();
            }
            //设置查询条件
            PropertyInfo[] SourceAttris = mSearch.GetType().GetProperties();
            var tempObj = SourceAttris.Where(x => x.Name.Equals(_PageMemberListVM.FieldEnName, StringComparison.Ordinal)).FirstOrDefault();
            if (tempObj != null)
            {
                tempObj.SetValue(mSearch, _PageMemberListVM.FieldValue);
            }
            await SearchMember(mSearch);
        }
        /// <summary>
        /// 查询指定条件的记录
        /// </summary>
        /// <param name="mSearch"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task SearchMember(MemberSearch mSearch)
        {
            List<Lib.Member> MemberList = await DataMemberRepository.ReadMembers(mSearch);
            MemberList.Sort((x, y) => x.OrderIndex - y.OrderIndex);
            if (MemberList != null && MemberList.Count > 0)
            {
                _PageMemberListVM.EntityList.Clear();
                MemberList.ForEach(e => { _PageMemberListVM.EntityList.Add(e); });
            }
            else
            {
                _PageMemberListVM.EntityList.Clear();
            }
            AppSettings.AppMainWindow.lblCursorPosition.Text = $"共查询到记录：{_PageMemberListVM.EntityList.Count}条";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (((System.Windows.FrameworkElement)sender).DataContext is Lib.Member CurMember)
            {
                PageEditMember pageEditMember = new PageEditMember(CurMember);
                AppSettings.AppMainWindow.FrameContentPage.Content = pageEditMember;
            }
        }

        /// <summary>
        /// 删除选定记录。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_Delete_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (RecordDataGrid.SelectedItem is Lib.Member SelectMember)
            {
                if (SelectMember != null && (new WinMsgDialog($"确定要删除[{SelectMember.Name}]信息吗？", "确认", showYesNo: true)).ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberRepository.DeleteMember(SelectMember).ConfigureAwait(false);
                    if (excuteResult.State == 0)
                    {
                        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { _PageMemberListVM.EntityList.Remove(SelectMember); })); ;
                    }
                    else
                    {
                        (new WinMsgDialog(excuteResult.Msg)).ShowDialog();
                    }
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AppSettings.AppMainWindow.lblCursorPosition.Text = "就绪";
        }
    }



    public class PageMemberListVM : NotificationObject
    {

        public ObservableCollection<Lib.Member> EntityList { get; set; }
        public Dictionary<string, string> FieldCn2En { get; set; }

        public string FieldEnName { get; set; }
        public string FieldValue { get; set; }
        public bool SearchInResult { get; set; }

        #region "方法"
        /// <summary>
        /// 构造函数
        /// </summary>
        public PageMemberListVM()
        {
            EntityList = new ObservableCollection<Lib.Member>();
            FieldCn2En = new Dictionary<string, string>() { { "Name", "姓名" }, { "UnitName", "单位" },
                { "Job", "岗位性质" }, { "JobGrade", "岗位级别" }, { "EducationTop", "最高学历" }, { "Age", "年龄" },{ "Remarks", "备注" }
        };
        }

        #endregion
    }
}
