using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberScore
{
    /// <summary>
    /// 录入积分考核信息
    /// </summary>
    public partial class PageScoreInput : Page
    {
        private PageViewModel _PageViewModel;
        public PageScoreInput()
        {
            InitializeComponent();
            _PageViewModel = new PageViewModel();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("添加或删除员工绩效考核信息。");
            this.DataContext = _PageViewModel;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AppFuns.SetStateBarText("就绪。");
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            await _PageViewModel.SearchRecords();
            if (_PageViewModel.CurMember != null)
            {
                await UcMemberScoreFile.InitFileDatasAsync(_PageViewModel.CurMember.Id, "绩效积分", true);
                _PageViewModel.CanOperation = true;
                AppFuns.SetStateBarText($"正在录入[{_PageViewModel.CurMember.Name}]的积分信息。");
            }
        }

        /// <summary>
        ///  新增记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddClickAsync(object sender, RoutedEventArgs e)
        {
            Lib.MemberScore NewRecord = new Lib.MemberScore()
            {
                OccurDate = DateTime.Now,
                UpDateTime = DateTime.Now,
                MemberId = _PageViewModel.CurMember.Id,
                MemberIndex = _PageViewModel.CurMember.OrderIndex,
                ScoreUnitName = AppSet.LoginUser.UnitName,
                UserId = AppSet.LoginUser.Id
            };

            PageScoreInputWin AddWin = new PageScoreInputWin(NewRecord);
            AddWin.Owner = AppSet.AppMainWindow;

            if (AddWin.ShowDialog().Value)
            {
                ExcuteResult excuteResult = await DataMemberScoreRepository.AddRecord(NewRecord);
                if (excuteResult != null)
                {
                    if (excuteResult.State == 0)
                    {
                        NewRecord.Id = excuteResult.Tag;
                        _PageViewModel.MemberScores.Add(NewRecord);
                    }
                    else
                    {
                        AppFuns.ShowMessage(excuteResult.Msg, Caption: "失败");
                    }
                }
                else
                {
                    AppFuns.ShowMessage("数据输入不正确！", Caption: "失败");
                }
            }
        }
        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnDelClickAsync(object sender, RoutedEventArgs e)
        {
            if (RecordDataGrid.SelectedItem is Lib.MemberScore SelectedRec)
            {

                if (AppFuns.ShowMessage($"确认要删除 {SelectedRec.Score}得分吗？", Caption: "确认", showYesNo: true))
                {
                    ExcuteResult excuteResult = await DataMemberScoreRepository.DeleteRecord(SelectedRec.Id);
                    if (excuteResult.State == 0)
                    {
                        _PageViewModel.MemberScores.Remove(SelectedRec);
                    }
                    else
                    {
                        AppFuns.ShowMessage(excuteResult.Msg, Caption: "失败");
                    }
                }
            }
        }

        //****************************************************************************************************************************************
        /// <summary>
        /// 该页面的视图模型类
        /// </summary>
        private class PageViewModel : NotificationObject
        {
            private bool _CanOperation = false;

            /// <summary>
            /// 是否可以操作
            /// </summary>
            public bool CanOperation
            {
                get { return _CanOperation; }
                set { _CanOperation = value; RaisePropertyChanged(); }
            }
            /// <summary>
            /// 查询得到的用户信息
            /// </summary>
            public Lib.Member CurMember { get; set; }

            /// <summary>
            /// 查询条件类对象
            /// </summary>
            public MemberScoreSearch SearchCondition { get; set; }

            /// <summary>
            /// 当前职工工资月度发放记录
            /// </summary>
            public ObservableCollection<Lib.MemberScore> MemberScores { get; set; }

            public PageViewModel()
            {
                SearchCondition = new MemberScoreSearch()
                {
                    UserId = AppSet.LoginUser.Id,
                    OccurYear = DateTime.Now.Year
                };
                CanOperation = false;
                CurMember = null;
                MemberScores = new ObservableCollection<Lib.MemberScore>();
            }
            /// <summary>
            /// 查询数据
            /// </summary>
            /// <returns></returns>
            public async Task SearchRecords()
            {
                //1.先查询用户信息
                List<Lib.Member> TempMembers = await DataMemberRepository.ReadMembers(new MemberSearch() { Id = SearchCondition.MemberId }).ConfigureAwait(false);

                if (TempMembers != null && TempMembers.Count > 0)
                {
                    CurMember = TempMembers[0];
                }
                if (CurMember == null)
                {
                    AppFuns.ShowMessage("未找到此用户信息！");
                    return;
                }
                if (SearchCondition != null)
                {
                    List<Lib.MemberScore> MemberScoreList = await DataMemberScoreRepository.GetRecords(SearchCondition);


                    App.Current.Dispatcher.Invoke(() =>
                    {
                        MemberScores.Clear();
                        MemberScoreList?.ToList().ForEach(e =>
                        {
                            MemberScores.Add(e);
                        });
                    });
                }
            }

        }
    }
}
