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
            AppSet.AppMainWindow.lblCursorPosition.Text = $"正在编辑[{_PageEditMemberVM.EntityMember.Name}]";
            if (_PageEditMemberVM.isEditFlag && Person_TabControl.SelectedItem is TabItem tb && !string.IsNullOrWhiteSpace(_PageEditMemberVM.EntityMember.Id))
            {
                switch (tb.Header)
                {
                    case "基本信息":
                        await UcBasicFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "基本信息", isRead);
                        break;
                    case "工作信息":
                        await UcWorkFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "工作信息", isRead);
                        break;
                    case "教育信息":
                        await UcEduFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "教育信息", isRead);
                        break;
                    case "个人履历":
                        UcResume.initControlAsync(_PageEditMemberVM.EntityMember);
                        await UcResumeFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "个人履历", isRead);
                        break;
                    case "奖惩情况":
                        UcPrizePunish.initControlAsync(_PageEditMemberVM.EntityMember);
                        await UcPrizePunishFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "奖惩情况", isRead);
                        break;
                    case "社会关系":
                        UcRelations.initControlAsync(_PageEditMemberVM.EntityMember);
                        await UcRelationsFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "社会关系", isRead);
                        break;
                    case "休假信息":
                        UcHoliday.initControlAsync(_PageEditMemberVM.EntityMember);
                        await UcHolidayFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "休假信息", isRead);
                        break;
                    case "其他说明":
                        await UcRemarkFile.InitFileDatasAsync(_PageEditMemberVM.EntityMember.Id, "其他说明", isRead);
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
            AppSet.AppMainWindow.lblCursorPosition.Text = "就绪";
        }
    }

    //该窗口所对象的ViewModel类
    public class PageEditMemberVM : NotificationObject
    {
        private bool _isEditFlag;
        private string _AddOrEditStr;

        public PageEditMemberVM(Lib.Member PMember)
        {
            if (PMember == null)
            {
                isEditFlag = false;
                EntityMember = new Lib.Member();
            }
            else
            {
                isEditFlag = true;
                EntityMember = PMember;
            }
            JobGrades = new string[] { "管理4级", "管理5级", "管理6级", "管理7级", "管理8级", "管理9级", "管理10级",
                "专技3级", "专技4级", "专技5级", "专技6级", "专技7级", "专技8级", "专技9级", "专技10级", "专技11级", "专技12级", "专技13级(技术员)",
                "高级技师","技师","高级工","中级工","初级工" ,"普通工" };
            DepartmentNames = new string[] { "领导班子","综合科", "政工科", "发展计划科", "科技信息科", "航道养护科", "船闸事务科","安全运行科", "工程建设科","港口业务科",
                "财务审计科", "工会","政秘股", "养护股", "运调股", "财务股", "其他部门", };
            PostNames = new string[] { "办事员", "副科长", "科长", "副主任", "主任", "工会主席", "工会副主席", "股长", "副股长" };
            UnitNames = new string[] { "市港航事业发展中心", "市大柳巷船闸管理处", "市成子河船闸管理处", "市古泊河船闸管理处" };
        }

        #region "属性"
        public bool isEditFlag
        {
            get { return _isEditFlag; }
            set
            {
                _isEditFlag = value;
                AddOrEditStr = value ? "编辑" : "新增";
                RaisePropertyChanged();
            }
        }
        public string AddOrEditStr
        {
            get
            {
                return _AddOrEditStr;
            }
            set
            {
                _AddOrEditStr = value; RaisePropertyChanged();
            }
        }
        public Lib.Member EntityMember { get; set; }
        public string[] JobGrades { get; private set; }
        public string[] DepartmentNames { get; private set; }
        public string[] PostNames { get; private set; }
        public string[] UnitNames { get; private set; }
        #endregion
    }
}
