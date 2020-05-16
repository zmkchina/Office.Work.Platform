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
        private CurViewModel _CurViewModel;

        public PageNoteInfo(bool IsMySelf)
        {
            InitializeComponent();
            _CurViewModel = new CurViewModel(IsMySelf);
        }
        public void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Col_NoteInfo.Width = new System.Windows.GridLength(0, System.Windows.GridUnitType.Pixel);
            btn_Search_ClickAsync(null, null);
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("就绪");
        }
        private async void btn_Search_ClickAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            Col_NoteInfo.Width = new System.Windows.GridLength(0, System.Windows.GridUnitType.Pixel);
            await _CurViewModel.SearchNodes();
            DataContext = _CurViewModel;
        }
        private void btn_Add_ClickAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            RichTBox.Document.Blocks.Clear();
            _CurViewModel.UserGrantSelectList.Clear();
            foreach (User item in AppSet.SysUsers.Where(e => !e.Id.Equals("admin", StringComparison.Ordinal)).OrderBy(x => x.OrderIndex))
            {
                _CurViewModel.UserGrantSelectList.Add(new SelectObj<User>(true, item));
            }
            _CurViewModel.CurNote = new Lib.Note()
            {
                UserId = AppSet.LoginUser.Id,
                CanReadUserIds = AppSet.LoginUser.Id
            };
            TB_NoteCaption.DataContext = _CurViewModel.CurNote;
            Col_NoteInfo.Width = new System.Windows.GridLength(2, System.Windows.GridUnitType.Star);
        }
        private void Btn_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_CurViewModel.CurNote.Caption))
            {
                AppFuns.ShowMessage("必须输入备忘标题！", Caption: "警告");
                TB_NoteCaption.Focus();
                return;
            }
            _CurViewModel.CurNote.CanReadUserIds = _CurViewModel.GetSelectUserIds(_CurViewModel.UserGrantSelectList);

            if (!_CurViewModel.CurNote.CanReadUserIds.Contains(AppSet.LoginUser.Id))
            {
                AppFuns.ShowMessage("你本人必须有读取该计划的权限！", Caption: "警告");
                return;
            }
            TextRange documentTextRange = new TextRange(RichTBox.Document.ContentStart, RichTBox.Document.ContentEnd);
            _CurViewModel.CurNote.TextContent = documentTextRange.Text;
            if (string.IsNullOrWhiteSpace(_CurViewModel.CurNote.TextContent))
            {
                AppFuns.ShowMessage("必须输入备忘内容！", Caption: "警告");
                return;
            }
            this.Btn_Save.IsEnabled = false;
            string dataFormat = DataFormats.Rtf;
            MemoryStream mStream = new MemoryStream();
            documentTextRange.Save(mStream, dataFormat);
            _CurViewModel.CurNote.Content = System.Convert.ToBase64String(mStream.ToArray());
            App.Current.Dispatcher.Invoke(async () =>
            {
                ExcuteResult result = await _CurViewModel.SaveNode();
                AppFuns.ShowMessage(result.Msg);
                this.Btn_Save.IsEnabled = true;
            });
        }
        private void ListBox_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            _CurViewModel.CurNote = ListBox_Notes.SelectedItem as Lib.Note;
            if (_CurViewModel.CurNote != null)
            {
                _CurViewModel.UserGrantSelectList.Clear();
                foreach (User item in AppSet.SysUsers.Where(e => !e.Id.Equals("admin", StringComparison.Ordinal)).OrderBy(x => x.OrderIndex))
                {
                    _CurViewModel.UserGrantSelectList.Add(new SelectObj<User>(_CurViewModel.CurNote.CanReadUserIds != null && (_CurViewModel.CurNote.CanReadUserIds.Contains(item.Id) || _CurViewModel.CurNote.CanReadUserIds.Equals("all", StringComparison.Ordinal)), item));
                }
                TB_NoteCaption.DataContext = _CurViewModel.CurNote;
                TextRange TR = new TextRange(this.RichTBox.Document.ContentStart, this.RichTBox.Document.ContentEnd);
                MemoryStream s = new MemoryStream((System.Convert.FromBase64String(_CurViewModel.CurNote.Content)));
                TR.Load(s, DataFormats.Rtf);
                if (Col_NoteInfo.Width.Value == 0)
                {
                    Col_NoteList.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
                    Col_NoteInfo.Width = new System.Windows.GridLength(2, System.Windows.GridUnitType.Star);
                }
                if (!_CurViewModel.CurNote.UserId.Equals(AppSet.LoginUser.Id))
                {
                    this.Btn_Save.IsEnabled = false;
                }
                else
                {
                    this.Btn_Save.IsEnabled = true;
                }
            }
        }
        private void btn_Delete_ClickAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(async () =>
            {
                ExcuteResult result = await _CurViewModel.DelNode();
                if (result.State == 0)
                {
                    Col_NoteInfo.Width = new System.Windows.GridLength(0, System.Windows.GridUnitType.Pixel);
                }
                AppFuns.ShowMessage(result.Msg);
            });
        }

        /// <summary>
        /// 该页面的视图模型
        /// </summary>
        private class CurViewModel : NotificationObject
        {
            public CurViewModel(bool IsMySelf)
            {
                UserGrantSelectList = new ObservableCollection<SelectObj<User>>();
                CollectNotes = new ObservableCollection<Lib.Note>();
                if (IsMySelf)
                {
                    NoteSearch = new NoteSearch()
                    {
                        IsMySelft = "Yes",
                        UserId = AppSet.LoginUser.Id
                    };
                }
                else
                {
                    NoteSearch = new NoteSearch()
                    {
                        IsMySelft = "No",
                        UserId = AppSet.LoginUser.Id
                    };
                }
            }
            /// <summary>
            /// 有权读取该信息的用户
            /// </summary>
            public ObservableCollection<SelectObj<User>> UserGrantSelectList { get; set; }
            public ObservableCollection<Lib.Note> CollectNotes { get; set; }
            public Lib.Note CurNote { get; set; }
            public NoteSearch NoteSearch { get; set; }

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

            public async Task SearchNodes()
            {
                CollectNotes.Clear();
                IEnumerable<Lib.Note> EnuNodes = await DataNoteRepository.GetRecords(NoteSearch);
                EnuNodes?.ToList().ForEach(e =>
                {
                    CollectNotes.Add(e);
                });
                AppFuns.SetStateBarText($"共查询到：{CollectNotes.Count} 条信息！");
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
        }

    }
}
