using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
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
                InitUcControlFilesAsync(true);
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
                // (new WinMsgDialog("若要开始，请选输入员工身份证号！", "输入不正确")).ShowDialog();
                return;
            }
            MemberSearch msearch = new MemberSearch() { Id = _PageEditMemberVM.EntityMember.Id };
            var members = await DataMemberRepository.ReadMembers(msearch);
            if (members.Count > 0)  //数据表中已存在该记录。
            {

                _PageEditMemberVM.isEditFlag = true;
                _PageEditMemberVM = new PageEditMemberVM(members[0]);
                DataContext = _PageEditMemberVM;
                //读取文件信息
                InitUcControlFilesAsync(true);
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
                InitUcControlFilesAsync(true);
            }
        }

        private async void InitUcControlFilesAsync(bool isRead = true)
        {
            AppSettings.AppMainWindow.lblCursorPosition.Text = $"正在编辑[{_PageEditMemberVM.EntityMember.Name}]";
            if (_PageEditMemberVM.isEditFlag && Person_TabControl.SelectedItem is TabItem tb && !string.IsNullOrWhiteSpace(_PageEditMemberVM.EntityMember.Id))
            {
                switch (tb.Header)
                {
                    case "基本信息":
                        await UcBasicFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember, "基本信息", isRead);
                        break;
                    case "工作信息":
                        await UcWorkFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember, "工作信息",  isRead);
                        break;
                    case "教育信息":
                        await UcEduFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember, "教育信息", isRead);
                        break;
                    case "个人履历":
                        UcResume.initControlAsync(_PageEditMemberVM.EntityMember);
                        await UcResumeFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember, "个人履历",  isRead);
                        break;
                    case "奖惩情况":
                        UcPrizePunish.initControlAsync(_PageEditMemberVM.EntityMember);
                        await UcPrizePunishFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember, "奖惩情况", isRead);
                        break;
                    case "社会关系":
                        UcRelations.initControlAsync(_PageEditMemberVM.EntityMember);
                        await UcRelationsFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember, "社会关系",  isRead);
                        break;
                    case "编内月待遇":
                        UcPayMonthOfficial.initControlAsync(_PageEditMemberVM.EntityMember);
                        await UcPayMonthOfficialFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember, "编内月待遇",  isRead);
                        break;
                    case "编外月待遇":
                        UcPayMonthUnofficial.initControlAsync(_PageEditMemberVM.EntityMember);
                        await UcPayMonthUnofficialFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember, "编外月待遇", isRead);
                        break;
                    case "月度社保":
                        UcPayMonthInsurance.initControlAsync(_PageEditMemberVM.EntityMember);
                        await UcPayMonthInsuranceFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember, "月度社保",  isRead);
                        break;
                    case "补充待遇":
                        UcPayTemp.initControlAsync(_PageEditMemberVM.EntityMember);
                        await UcPayTempFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember, "补充待遇", isRead);
                        break;
                    case "休假信息":
                        UcHoliday.initControlAsync(_PageEditMemberVM.EntityMember);
                        await UcHolidayFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember, "休假信息",  isRead);
                        break;
                    case "其他说明":
                        await UcRemarkFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember, "其他说明", isRead);
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
                (new WinMsgDialog("员工的身份证号必须输入！")).ShowDialog();
                Tb_UserId.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(_PageEditMemberVM.EntityMember.Name))
            {
                (new WinMsgDialog("员工的姓名必须输入！")).ShowDialog();
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
                    InitUcControlFilesAsync(false);
                }
            }
              (new WinMsgDialog(excuteResult.Msg)).ShowDialog();
        }
        /// <summary>
        /// 更新员工的工作信息、受教育信息、更新备注信息等。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnUpdateClickAsync(object sender, RoutedEventArgs e)
        {
            ExcuteResult excuteResult = await DataMemberRepository.UpdateMember(_PageEditMemberVM.EntityMember);
            (new WinMsgDialog(excuteResult.Msg)).ShowDialog();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AppSettings.AppMainWindow.lblCursorPosition.Text = "就绪";
        }
    }
}
