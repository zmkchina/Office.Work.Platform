using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Office.Work.Platform.AppCodes;

namespace Office.Work.Platform.MemberPay
{
    /// <summary>
    /// 此类用于新增或编辑月度待遇发放记录。
    /// 此窗体作为对话框使用，当DialogResult不被设为true时，将始终为false
    /// </summary>
    public partial class PageMemberPayItemWin : Window
    {

        public Lib.User CurLoginUser { get; set; }
        public Lib.MemberPayItem CurPayItem { get; set; }
        public Lib.SettingServer MemberSets { get; set; } = AppSet.ServerSetting;
        /// <summary>
        /// 有权读取该计划的用户选择
        /// </summary>
        public ObservableCollection<SelectObj<string>> MemberTypeList { get; set; }

        public PageMemberPayItemWin(Lib.MemberPayItem PPayItem)
        {
            this.Owner = AppSet.AppMainWindow;
            InitializeComponent();
            CurPayItem = PPayItem;
            MemberTypeList = new ObservableCollection<SelectObj<string>>();
            CurLoginUser = AppSet.LoginUser;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CurPayItem.Name))
            {
                //说明是编辑项目。
                Tb_ItemName.IsReadOnly = true;
            }
            foreach (string item in MemberSets.MemberTypeArr)
            {
                if (!string.IsNullOrEmpty(CurPayItem.MemberTypes))
                {
                    MemberTypeList.Add(new SelectObj<string>(CurPayItem.MemberTypes.Contains(item), item));
                }
                else
                {
                    MemberTypeList.Add(new SelectObj<string>(false, item));
                }
            }

            DataContext = this;
        }

        private void BtnSaveClickAsync(object sender, RoutedEventArgs e)
        {
            CurPayItem.MemberTypes = GetSelectMemberTypes(MemberTypeList);
            CurPayItem.UnitName = AppSet.LoginUser.UnitName;
            CurPayItem.UserId = AppSet.LoginUser.Id;
            DialogResult = true;
            this.Close();
        }

        private void BtnCancelClickAsync(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// 拖动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Dispatcher.BeginInvoke(new Action(() => this.DragMove()));
            }
        }

        /// <summary>
        /// 获取所有选中的员工类型
        /// </summary>
        /// <param name="UserSelectList"></param>
        /// <returns></returns>
        public string GetSelectMemberTypes(ObservableCollection<SelectObj<string>> MemberTypeList)
        {
            List<string> SelectIds = MemberTypeList.Where(x => x.IsSelect).Select(y => y.Obj).ToList();
            return string.Join(",", SelectIds.ToArray());
        }
    }
}
