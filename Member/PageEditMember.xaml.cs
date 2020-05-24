using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
        private CurPageViewModel _CurPageViewModel;
        public PageEditMember(Lib.Member PMember = null)
        {
            InitializeComponent();
            _CurPageViewModel = new CurPageViewModel(PMember);
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TabItem item in Person_TabControl.Items)
            {
                item.MouseLeftButtonUp += TabItem_MouseLeftButtonUp;
            }
            // 初始化各属性值
            InitPropsAsync();
            //读取文件信息
            InitUcControlFilesAsync(true);
            DataContext = _CurPageViewModel;
        }

        /// <summary>
        /// 身份证号码框失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tb_UserId_LostFocus(object sender, RoutedEventArgs e)
        {
            // 初始化各属性值
            InitPropsAsync();
            //读取文件信息
            InitUcControlFilesAsync(true);
        }
        /// <summary>
        /// 初始化各属性值
        /// </summary>
        private async void InitPropsAsync()
        {
            if (string.IsNullOrEmpty(_CurPageViewModel.EntityMember.Id))
            {
                //  AppFuns.ShowMessage("若要开始，请选输入员工身份证号！", "输入不正确")).ShowDialog();
                return;
            }
            MemberSearch msearch = new MemberSearch() { Id = _CurPageViewModel.EntityMember.Id };
            var members = await DataMemberRepository.ReadMembers(msearch);
            Stream UserHeadStream = null;
            if (members != null && members.Count > 0)  //数据表中已存在该记录。
            {
                _CurPageViewModel.isEditFlag = true;
                _CurPageViewModel = new CurPageViewModel(members[0]);
                //读取用户头像信息
                var UserPhotos = await DataMemberFileRepository.ReadFiles(new MemberFileSearch()
                {
                    UserId = AppSet.LoginUser.Id,
                    MemberId = _CurPageViewModel.EntityMember.Id,
                    Name = _CurPageViewModel.EntityMember.Id
                }).ConfigureAwait(false);
                UserPhotos = UserPhotos.OrderByDescending(x => x.UpDateTime);
                if (UserPhotos != null && UserPhotos.Count() > 0)
                {
                    UserHeadStream = await DataMemberFileRepository.DownloadFileStream(UserPhotos.First().Id, null);
                }
            }
            else
            {
                _CurPageViewModel.isEditFlag = false;
            }
            App.Current.Dispatcher.Invoke(() =>
            {
                if (UserHeadStream != null)
                {
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(UserHeadStream);
                    MemoryStream PhotoStream = new MemoryStream();
                    bitmap.Save(PhotoStream, System.Drawing.Imaging.ImageFormat.Png);
                    _CurPageViewModel.UseHeadImage.BeginInit();
                    //如果希望关闭用于创建 BitmapImage的流，请将 CacheOption 设置为 BitmapCacheOption.OnLoad。 
                    //默认 OnDemand 缓存选项保留对流的访问权限，直到需要映像，并且清除由垃圾回收器处理
                    _CurPageViewModel.UseHeadImage.CacheOption = BitmapCacheOption.OnLoad;
                    _CurPageViewModel.UseHeadImage.StreamSource = PhotoStream;
                    _CurPageViewModel.UseHeadImage.EndInit();
                    PhotoStream.Dispose();
                }

                DataContext = _CurPageViewModel;
            });
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
            AppSet.AppMainWindow.lblCursorPosition.Text = $"正在编辑[{_CurPageViewModel.EntityMember.Name}]";


            if (_CurPageViewModel.isEditFlag && Person_TabControl.SelectedItem is TabItem tb && !string.IsNullOrWhiteSpace(_CurPageViewModel.EntityMember.Id))
            {
                switch (tb.Header)
                {
                    case "基本信息":
                        await UcBasicFile.InitFileDatasAsync(_CurPageViewModel.EntityMember.Id, "基本信息", isRead);
                        break;
                    case "工作信息":
                        await UcWorkFile.InitFileDatasAsync(_CurPageViewModel.EntityMember.Id, "工作信息", isRead);
                        break;
                    case "教育信息":
                        await UcEduFile.InitFileDatasAsync(_CurPageViewModel.EntityMember.Id, "教育信息", isRead);
                        break;
                    case "个人履历":
                        UcResume.initControlAsync(_CurPageViewModel.EntityMember);
                        await UcResumeFile.InitFileDatasAsync(_CurPageViewModel.EntityMember.Id, "个人履历", isRead);
                        break;
                    case "奖惩情况":
                        UcPrizePunish.initControlAsync(_CurPageViewModel.EntityMember);
                        await UcPrizePunishFile.InitFileDatasAsync(_CurPageViewModel.EntityMember.Id, "奖惩情况", isRead);
                        break;
                    case "考核情况":
                        UcAppraise.initControlAsync(_CurPageViewModel.EntityMember);
                        await UcAppraiseFile.InitFileDatasAsync(_CurPageViewModel.EntityMember.Id, "考核情况", isRead);
                        break;
                    case "社会关系":
                        UcRelations.initControlAsync(_CurPageViewModel.EntityMember);
                        await UcRelationsFile.InitFileDatasAsync(_CurPageViewModel.EntityMember.Id, "社会关系", isRead);
                        break;
                    case "其他说明":
                        await UcRemarkFile.InitFileDatasAsync(_CurPageViewModel.EntityMember.Id, "其他说明", isRead);
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
            if (string.IsNullOrWhiteSpace(_CurPageViewModel.EntityMember.Id))
            {
                AppFuns.ShowMessage("员工的身份证号必须输入！");
                Tb_UserId.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(_CurPageViewModel.EntityMember.Name))
            {
                AppFuns.ShowMessage("员工的姓名必须输入！");
                Tb_UserName.Focus();
                return;
            }
            ExcuteResult excuteResult;
            if (_CurPageViewModel.isEditFlag)
            {
                excuteResult = await DataMemberRepository.UpdateMember(_CurPageViewModel.EntityMember);
                AppFuns.ShowMessage(excuteResult.Msg);
            }
            else
            {
                excuteResult = await DataMemberRepository.AddMember(_CurPageViewModel.EntityMember);
                if (excuteResult.State == 0)
                {
                    //保存成功表示可以进行编辑了，即其他控件可以保存了。
                    _CurPageViewModel.isEditFlag = true;
                    //只传递两个字段信息，不实际读取（因为此时没有必要读取）
                    InitUcControlFilesAsync(false);
                    AppFuns.ShowMessage(excuteResult.Msg);
                }
            }
        }
        /// <summary>
        /// 更新员工的工作信息、受教育信息、更新备注信息等。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnUpdateClickAsync(object sender, RoutedEventArgs e)
        {
            ExcuteResult excuteResult = await DataMemberRepository.UpdateMember(_CurPageViewModel.EntityMember);
            AppFuns.ShowMessage(excuteResult.Msg);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AppSet.AppMainWindow.lblCursorPosition.Text = "就绪";
        }


        //该窗口所对象的ViewModel类
        private class CurPageViewModel : NotificationObject
        {
            private bool _isEditFlag;
            private string _AddOrEditStr;

            public CurPageViewModel(Lib.Member PMember)
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
                UseHeadImage = new BitmapImage();
                MSettings = AppSet.ServerSetting;
            }

            #region "属性"
            private BitmapImage _UseHeadImage;
            /// <summary>
            /// 显示的用户图片
            /// </summary>
            public BitmapImage UseHeadImage
            {
                get { return _UseHeadImage; }
                set
                {
                    _UseHeadImage = value;
                    RaisePropertyChanged();
                }
            }

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
            public Lib.SettingServer MSettings { get; set; }
            #endregion
        }
    }
}
