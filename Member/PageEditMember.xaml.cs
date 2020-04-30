using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// PageEditMember.xaml 的交互逻辑
    /// </summary>
    public partial class PageEditMember : Page
    {
        public PageEditMemberVM _PageEditMemberVM;
        public PageEditMember(Lib.Member PMember = null)
        {
            InitializeComponent();
            _PageEditMemberVM = new PageEditMemberVM(PMember);
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TabItem item in Person_TabControl.Items)
            {
                item.MouseLeftButtonUp += TabItem_MouseLeftButtonUp;
            }
            if (_PageEditMemberVM.EntityMember != null)
            {
                //读取文件信息
                InitUcControlFiles(true);
            }
            DataContext = _PageEditMemberVM;
        }

        /// <summary>
        /// 身份证号码框失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Tb_UserId_LostFocusAsync(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_PageEditMemberVM.EntityMember.Id))
            {
                //MessageBox.Show("若要开始，请选输入员工身份证号！", "输入不正确", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MemberSearch msearch = new MemberSearch() { Id = _PageEditMemberVM.EntityMember.Id };
            var members = await DataMemberRepository.ReadMembers(msearch);
            if (members.Count > 0)  //数据表中已存在该记录。
            {
                //读取文件信息
                InitUcControlFiles(true);

                _PageEditMemberVM.isEditFlag = true;
                _PageEditMemberVM = new PageEditMemberVM(members[0]);
                DataContext = _PageEditMemberVM;
            }
            else
            {
                _PageEditMemberVM.isEditFlag = false;
            }

        }
        private void TabItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (e.Source is TabItem)
            {
                InitUcControlFiles(true);
            }
        }

        private void InitUcControlFiles(bool isRead = true)
        {
            if (_PageEditMemberVM.isEditFlag && Person_TabControl.SelectedItem is TabItem tb && !string.IsNullOrWhiteSpace(_PageEditMemberVM.EntityMember.Id))
            {
                switch (tb.Header)
                {
                    case "基本信息":
                        UcBasicFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "基本信息", null, isRead);
                        break;
                    case "工作信息":
                        UcWorkFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "工作信息", null, isRead);
                        break;
                    case "教育信息":
                        UcEduFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "教育信息", null, isRead);
                        break;
                    case "个人简历":
                        break;
                    case "奖惩情况":
                        break;
                    case "社会关系":
                        break;
                    case "在编月待遇":
                        UcPayMonth.initControlAsync(_PageEditMemberVM.EntityMember);
                        UcPayMonthFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "在编月待遇", null, isRead);
                        break;
                    case "编外月待遇":
                        UcPayMonthUnofficial.initControlAsync(_PageEditMemberVM.EntityMember);
                        UcPayMonthUnofficialFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "编外月待遇", null, isRead);
                        break;
                    case "月度社保":
                        UcPayMonthInsurance.initControlAsync(_PageEditMemberVM.EntityMember);
                        UcPayMonthInsuranceFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "月度社保", null, isRead);
                        break;
                    case "补充待遇":
                        UcPayTemp.initControlAsync(_PageEditMemberVM.EntityMember);
                        UcPayTempFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "补充待遇", null, isRead);
                        break;
                    case "考勤信息":
                        break;
                    case "其他说明":
                        break;
                }
            }
        }
        /// <summary>
        /// 保存职工的基本信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSaveBasicClickAsync(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_PageEditMemberVM.EntityMember.Id))
            {
                MessageBox.Show("员工的身份证号必须输入！", "输入不正确", MessageBoxButton.OK, MessageBoxImage.Warning);
                Tb_UserId.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(_PageEditMemberVM.EntityMember.Name))
            {
                MessageBox.Show("员工的姓名必须输入！", "输入不正确", MessageBoxButton.OK, MessageBoxImage.Warning);
                Tb_UserName.Focus();
                return;
            }
            ExcuteResult excuteResult;
            if (_PageEditMemberVM.isEditFlag)
            {
                excuteResult = await DataMemberRepository.UpdateMember(_PageEditMemberVM.EntityMember);
            }
            else
            {
                excuteResult = await DataMemberRepository.AddMember(_PageEditMemberVM.EntityMember);
                if (excuteResult.State == 0)
                {
                    //保存成功表示可以进行编辑了，即其他控件可以保存了。
                    _PageEditMemberVM.isEditFlag = true;
                    //只传递两个字段信息，不实际读取（因为此时没有必要读取）
                    InitUcControlFiles(false);
                }
            }
            MessageBox.Show(excuteResult.Msg);
        }
        /// <summary>
        /// 保存员工的工作信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSaveWorkClickAsync(object sender, RoutedEventArgs e)
        {
            ExcuteResult excuteResult = await DataMemberRepository.UpdateMember(_PageEditMemberVM.EntityMember);
            MessageBox.Show(excuteResult.Msg);
        }
        /// <summary>
        /// 保存员工受教育信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSaveEduClickAsync(object sender, RoutedEventArgs e)
        {
            ExcuteResult excuteResult = await DataMemberRepository.UpdateMember(_PageEditMemberVM.EntityMember);
            MessageBox.Show(excuteResult.Msg);
        }
        /// <summary>
        /// 保存备注信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSaveRemarkClickAsync(object sender, RoutedEventArgs e)
        {
            ExcuteResult excuteResult = await DataMemberRepository.UpdateMember(_PageEditMemberVM.EntityMember);
            MessageBox.Show(excuteResult.Msg);
        }
    }
}
