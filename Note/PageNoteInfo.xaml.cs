using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Note
{
    /// <summary>
    /// PageEditMember.xaml 的交互逻辑
    /// </summary>
    public partial class PageNoteInfo : Page
    {
        private CurPageViewModel _CurPageViewModel;

        public PageNoteInfo(bool IsMySelf)
        {
            InitializeComponent();
            _CurPageViewModel = new CurPageViewModel(IsMySelf);
        }
        public void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("查询平台备忘内容。");
            Col_NoteInfo.Width = new System.Windows.GridLength(0, System.Windows.GridUnitType.Pixel);
            btn_Search_ClickAsync(null, null);
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("就绪");
        }

        /// <summary>
        /// 查询备忘录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_Search_ClickAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            Col_NoteInfo.Width = new System.Windows.GridLength(0, System.Windows.GridUnitType.Pixel);
            _CurPageViewModel.SearchCondition.PageIndex = 1;

            await _CurPageViewModel.GetNotesAsync();
            DataContext = _CurPageViewModel;
            AppFuns.SetStateBarText($"共查询到 :{_CurPageViewModel.SearchCondition.RecordCount}条记录,每页{_CurPageViewModel.SearchCondition.PageSize}条，共{_CurPageViewModel.SearchCondition.PageCount}页！");
        }

        /// <summary>
        /// 添加一个备忘录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Add_ClickAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            RichTBox.Document.Blocks.Clear();
            _CurPageViewModel.UserGrantSelectList.Clear();
            foreach (User item in AppSet.SysUsers.Where(e => !e.Id.Equals("admin", StringComparison.Ordinal)).OrderBy(x => x.OrderIndex))
            {
                _CurPageViewModel.UserGrantSelectList.Add(new SelectObj<User>(true, item));
            }
            _CurPageViewModel.CurNote = new Lib.Note()
            {
                UserId = AppSet.LoginUser.Id,
                CanReadUserIds = AppSet.LoginUser.Id
            };
            TB_NoteCaption.DataContext = _CurPageViewModel.CurNote;
            Col_NoteInfo.Width = new System.Windows.GridLength(2, System.Windows.GridUnitType.Star);
        }

        /// <summary>
        /// 保存备忘录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_CurPageViewModel.CurNote.Caption))
            {
                AppFuns.ShowMessage("必须输入备忘标题！", Caption: "警告");
                TB_NoteCaption.Focus();
                return;
            }
            _CurPageViewModel.CurNote.CanReadUserIds = _CurPageViewModel.GetSelectUserIds(_CurPageViewModel.UserGrantSelectList);

            if (!_CurPageViewModel.CurNote.CanReadUserIds.Contains(AppSet.LoginUser.Id))
            {
                AppFuns.ShowMessage("你本人必须有读取该计划的权限！", Caption: "警告");
                return;
            }
            TextRange documentTextRange = new TextRange(RichTBox.Document.ContentStart, RichTBox.Document.ContentEnd);
            _CurPageViewModel.CurNote.TextContent = documentTextRange.Text;
            if (string.IsNullOrWhiteSpace(_CurPageViewModel.CurNote.TextContent))
            {
                AppFuns.ShowMessage("必须输入备忘内容！", Caption: "警告");
                return;
            }
            this.Btn_Save.IsEnabled = false;
            string dataFormat = DataFormats.Rtf;
            MemoryStream mStream = new MemoryStream();
            documentTextRange.Save(mStream, dataFormat);
            _CurPageViewModel.CurNote.Content = System.Convert.ToBase64String(mStream.ToArray());
            App.Current.Dispatcher.Invoke(async () =>
            {
                ExcuteResult result = await _CurPageViewModel.SaveNode();
                AppFuns.ShowMessage(result.Msg);
                this.Btn_Save.IsEnabled = true;
            });
        }

        /// <summary>
        /// 选中一个备忘录标题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            _CurPageViewModel.CurNote = ListBox_Notes.SelectedItem as Lib.Note;
            if (_CurPageViewModel.CurNote != null)
            {
                _CurPageViewModel.UserGrantSelectList.Clear();
                foreach (User item in AppSet.SysUsers.Where(e => !e.Id.Equals("admin", StringComparison.Ordinal)).OrderBy(x => x.OrderIndex))
                {
                    _CurPageViewModel.UserGrantSelectList.Add(new SelectObj<User>(_CurPageViewModel.CurNote.CanReadUserIds != null && (_CurPageViewModel.CurNote.CanReadUserIds.Contains(item.Id) || _CurPageViewModel.CurNote.CanReadUserIds.Equals("all", StringComparison.Ordinal)), item));
                }
                TB_NoteCaption.DataContext = _CurPageViewModel.CurNote;
                TextRange TR = new TextRange(this.RichTBox.Document.ContentStart, this.RichTBox.Document.ContentEnd);
                MemoryStream s = new MemoryStream((System.Convert.FromBase64String(_CurPageViewModel.CurNote.Content)));
                TR.Load(s, DataFormats.Rtf);
                if (Col_NoteInfo.Width.Value == 0)
                {
                    Col_NoteList.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
                    Col_NoteInfo.Width = new System.Windows.GridLength(2, System.Windows.GridUnitType.Star);
                }
                if (!_CurPageViewModel.CurNote.UserId.Equals(AppSet.LoginUser.Id))
                {
                    this.Btn_Save.IsEnabled = false;
                }
                else
                {
                    this.Btn_Save.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// 删除备忘录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Delete_ClickAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_CurPageViewModel.CurNote != null)
            {
                if (!AppFuns.ShowMessage($"确定要删除[{_CurPageViewModel.CurNote.Caption}]备忘信息吗？", "确认", false, true, false))
                {
                    return;
                }
                App.Current.Dispatcher.Invoke(async () =>
                {
                    ExcuteResult result = await _CurPageViewModel.DelNode();
                    if (result.State == 0)
                    {
                        Col_NoteInfo.Width = new System.Windows.GridLength(0, System.Windows.GridUnitType.Pixel);
                    }
                    AppFuns.ShowMessage(result.Msg);
                });
            }
        }


        private async void btn_PrevPage_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_CurPageViewModel.SearchCondition.PageIndex <= 1)
            {
                _CurPageViewModel.SearchCondition.PageIndex = _CurPageViewModel.SearchCondition.PageCount;
            }
            else
            {
                _CurPageViewModel.SearchCondition.PageIndex--;
            }
            await _CurPageViewModel.GetNotesAsync();
        }

        private async void btn_NextPage_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_CurPageViewModel.SearchCondition.PageIndex >= _CurPageViewModel.SearchCondition.PageCount)
            {
                _CurPageViewModel.SearchCondition.PageIndex = 1;
            }
            else
            {
                _CurPageViewModel.SearchCondition.PageIndex++;
            }
            await _CurPageViewModel.GetNotesAsync();
        }

        /// <summary>
        /// 该页面的视图模型
        /// </summary>
        private class CurPageViewModel : NotificationObject
        {
            private string _CanVisible;

            public CurPageViewModel(bool IsMySelf)
            {
                UserGrantSelectList = new ObservableCollection<SelectObj<User>>();
                CollectNotes = new ObservableCollection<Lib.Note>();
                if (IsMySelf)
                {
                    SearchCondition = new NoteSearch()
                    {
                        PageIndex = 1,
                        PageSize = 15,
                        IsMySelft = "Yes",
                        UserId = AppSet.LoginUser.Id
                    };
                }
                else
                {
                    SearchCondition = new NoteSearch()
                    {
                        PageIndex = 1,
                        PageSize = 15,
                        IsMySelft = "No",
                        UserId = AppSet.LoginUser.Id
                    };
                }
                CanVisible = "Collapsed";
            }


            /// <summary>
            /// 获取所有选中的用记信息
            /// </summary>
            /// <param name="UserSelectList"></param>
            /// <returns></returns>
            public string GetSelectUserIds(ObservableCollection<SelectObj<User>> UserSelectList)
            {
                List<string> SelectIds = UserSelectList.Where(x => x.IsSelect).Select(y => y.Obj.Id).ToList();
                return string.Join(",", SelectIds.ToArray());
            }

            public async Task GetNotesAsync()
            {
                CollectNotes.Clear();
                NoteSearchResult NoteSearchResult = await DataNoteRepository.GetRecords(SearchCondition);
                if (NoteSearchResult == null) { return; }

                SearchCondition.RecordCount = NoteSearchResult.SearchCondition.RecordCount;

                NoteSearchResult.RecordList?.ToList().ForEach(e =>
                {
                    CollectNotes.Add(e);
                });
                if (SearchCondition.PageCount > 1)
                {
                    CanVisible = "Visible";
                }
                else
                {
                    CanVisible = "Collapsed";
                }
            }
            public async Task<ExcuteResult> DelNode()
            {
                ExcuteResult excuteResult = new ExcuteResult();
                if (CurNote != null)
                {
                    excuteResult = await DataNoteRepository.DeleteRecord(CurNote.Id);
                    if (excuteResult.State == 0)
                    {
                        CollectNotes.Remove(CurNote);
                    }
                }
                return excuteResult;
            }
            public async Task<ExcuteResult> SaveNode()
            {
                ExcuteResult excuteResult;
                if (CurNote.Id == null)
                {
                    excuteResult = await DataNoteRepository.AddRecord(CurNote);
                    if (excuteResult.State == 0)
                    {
                        CurNote.Id = excuteResult.Tag;
                        CollectNotes.Add(CurNote);
                    }
                }
                else
                {
                    excuteResult = await DataNoteRepository.UpdateRecord(CurNote);
                }
                return excuteResult;
            }
            /// <summary>
            /// 有权读取该信息的用户
            /// </summary>
            public ObservableCollection<SelectObj<User>> UserGrantSelectList { get; set; }
            public ObservableCollection<Lib.Note> CollectNotes { get; set; }
            public Lib.Note CurNote { get; set; }
            public NoteSearch SearchCondition { get; set; }
            /// <summary>
            /// 上下页按钮是否可见
            /// </summary>
            public string CanVisible
            {
                get { return _CanVisible; }
                set { _CanVisible = value; RaisePropertyChanged(); }
            }
        }
    }
}
