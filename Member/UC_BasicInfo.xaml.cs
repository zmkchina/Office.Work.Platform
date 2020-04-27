using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// UC_BasicInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UC_BasicInfo : UserControl
    {
        private PageEditMember _PageEditMember;
        private string _OldCardId = string.Empty;
        public UC_BasicInfo()
        {
            InitializeComponent();
        }
        public void initControl(PageEditMember PPageEditMember)
        {
            _PageEditMember = PPageEditMember;
            _OldCardId = new string(_PageEditMember._PageEditMemberVM.EntityMember.Id);
            DataContext = _PageEditMember._PageEditMemberVM;
        }
        private async void btn_save_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_PageEditMember._PageEditMemberVM.EntityMember.Id))
            {
                MessageBox.Show("员工的身份证号必须输入！", "输入不正确", MessageBoxButton.OK, MessageBoxImage.Warning);
                Tb_UserId.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(_PageEditMember._PageEditMemberVM.EntityMember.Name))
            {
                MessageBox.Show("员工的姓名必须输入！", "输入不正确", MessageBoxButton.OK, MessageBoxImage.Warning);
                Tb_UserName.Focus();
                return;
            }
            ExcuteResult excuteResult;
            if (_OldCardId.Equals(_PageEditMember._PageEditMemberVM.EntityMember.Id, System.StringComparison.Ordinal))
            {
                excuteResult = await DataMemberRepository.UpdateMember(_PageEditMember._PageEditMemberVM.EntityMember);
            }
            else
            {
                excuteResult = await DataMemberRepository.AddMember(_PageEditMember._PageEditMemberVM.EntityMember);
            }
            if (excuteResult.State == 0)
            {
                _OldCardId = new string(_PageEditMember._PageEditMemberVM.EntityMember.Id);
                //保存成功表示可以进行编辑了，即其他控件可以保存了。
                _PageEditMember._PageEditMemberVM.isEditFlag = true;
            }
            MessageBox.Show(excuteResult.Msg);
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_delte_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 身份证号码框失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Tb_UserId_LostFocusAsync(object sender, RoutedEventArgs e)
        {
            MemberSearch msearch = new MemberSearch() { Id = _PageEditMember._PageEditMemberVM.EntityMember.Id };
            var members = await DataMemberRepository.ReadMembers(msearch);
            if (members.Count > 0)
            {
                _OldCardId = new string(_PageEditMember._PageEditMemberVM.EntityMember.Id);
                _PageEditMember._PageEditMemberVM.isEditFlag = true;
                _PageEditMember._PageEditMemberVM = new PageEditMemberVM(members[0]);
                DataContext = _PageEditMember._PageEditMemberVM;
            }
        }
    }
}
