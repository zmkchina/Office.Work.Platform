using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Settings
{
    /// <summary>
    /// WinUpLoadFile.xaml 的交互逻辑
    /// </summary>
    public partial class PageSettingsGrant : Page
    {
        private PageViewModel _PageViewModel = null;
        public PageSettingsGrant()
        {
            InitializeComponent();

            _PageViewModel = new PageViewModel();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSet.LoginUser.Post.Equals("管理员"))
            {
                _PageViewModel.CanOperation = true;
            }
            this.DataContext = _PageViewModel;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((System.Windows.Controls.Primitives.Selector)sender).SelectedItem is Lib.User CurUser)
            {
                _PageViewModel.CurSelectUser = CurUser;
                _PageViewModel.UserGrantSelectList.Clear();

                foreach (UserGrant item in _PageViewModel.AllGrants)
                {
                    if (_PageViewModel.CurSelectUser != null && _PageViewModel.CurSelectUser.Grants != null && _PageViewModel.CurSelectUser.Grants.Contains(item.EnName))
                    {
                        _PageViewModel.UserGrantSelectList.Add(new SelectObj<UserGrant>(true, item));
                    }
                    else
                    {
                        _PageViewModel.UserGrantSelectList.Add(new SelectObj<UserGrant>(false, item));
                    }
                }
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn_UpdateSettings_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_PageViewModel.CurSelectUser != null)
            {
                string SelectGrants = _PageViewModel.GetSelectUserIds();
                _PageViewModel.CurSelectUser.Grants = SelectGrants;
                ExcuteResult excute = await DataUserRepository.UpdateRecord(_PageViewModel.CurSelectUser);
                if (excute.State == 0)
                {
                    AppFuns.ShowMessage("更新成功！");
                }
                else
                {
                    AppFuns.ShowMessage($"更新失败功！{excute.Msg}", "失败");
                }
            }
        }

        private class PageViewModel : NotificationObject
        {
            private bool _CanOperation;

            public bool CanOperation
            {
                get
                {
                    return _CanOperation;
                }
                set
                {
                    _CanOperation = value; RaisePropertyChanged();
                }
            }
            public UserGrant[] AllGrants { get; set; }
            public Lib.User CurSelectUser { get; set; }
            public List<Lib.User> SysUsers { get; set; }
            public ObservableCollection<SelectObj<UserGrant>> UserGrantSelectList { get; set; }
            /// <summary>
            /// 构造函数
            /// </summary>
            public PageViewModel()
            {
                CanOperation = false;
                SysUsers = AppSet.SysUsers;
                AllGrants = new UserGrant[] {
                    new UserGrant("计划附件操作", "PlanFileDele","计划附件删除"),  new UserGrant("计划附件","PlanFileAdd","计划附件添加"),
                    new UserGrant("计划操作","PlanDele","计划删除"), new UserGrant("计划","PlanResetState","计划状态重置"),
                    new UserGrant("员工信息操作","MemberExportAll","导出全部用户"), new UserGrant("员工信息操作","MemberImportAll","导入全部用户") };
                CurSelectUser = null;
                UserGrantSelectList = new ObservableCollection<SelectObj<UserGrant>>();
            }
            /// <summary>
            /// 获取所有选中的用记信息
            /// </summary>
            /// <param name="UserSelectList"></param>
            /// <returns></returns>
            public string GetSelectUserIds()
            {
                List<string> SelectIds = UserGrantSelectList.Where(x => x.IsSelect).Select(y => y.Obj.EnName).ToList();
                return string.Join(",", SelectIds.ToArray());
            }
        }
        private class UserGrant : NotificationObject
        {
            private string _EnName;
            private string _CnName;

            public string GrantType { get; private set; }
            public string EnName { get { return _EnName; } set { _EnName = value; RaisePropertyChanged(); } }
            public string CnName { get { return _CnName; } set { _CnName = value; RaisePropertyChanged(); } }
            public UserGrant(string PGrantType, string PEnName, string PCnName)
            {
                this.GrantType = PGrantType;
                this.EnName = PEnName;
                this.CnName = PCnName;
            }
        }
    }
}
