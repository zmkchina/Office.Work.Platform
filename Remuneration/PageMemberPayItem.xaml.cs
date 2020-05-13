using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Remuneration
{
    /// <summary>
    /// PageMakeNewPay.xaml 的交互逻辑
    /// </summary>
    public partial class PageMemberPayItem : Page
    {
        CurViewModel cvm;
        public PageMemberPayItem()
        {
            InitializeComponent();
            cvm = new CurViewModel();
        }

        private async void Page_LoadedAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            IEnumerable<Lib.MemberPayItem> MemberPayItems = await DataMemberPayItemRepository.GetRecords(new Lib.MemberPayItemSearch()
            {
                PayUnitName = AppSet.LoginUser.UnitName,
                UserId = AppSet.LoginUser.Id
            }).ConfigureAwait(false);

            if (MemberPayItems != null)
            {
                MemberPayItems = MemberPayItems.OrderBy(x => x.OrderIndex);
                MemberPayItems.ToList().ForEach(e =>
                {
                    cvm.PayItems.Add(e);
                });
            }
            App.Current.Dispatcher.Invoke(() => { this.DataContext = cvm; });
        }


        private async void Btn_Add_ClickAnsyc(object sender, System.Windows.RoutedEventArgs e)
        {
            Lib.MemberPayItem NewRecord = new Lib.MemberPayItem();

            PageMemberPayItemWin AddWin = new PageMemberPayItemWin(NewRecord);

            if (AddWin.ShowDialog().Value)
            {
                ExcuteResult excuteResult = await DataMemberPayItemRepository.AddRecord(NewRecord);
                if (excuteResult.State == 0)
                {
                    cvm.PayItems.Add(NewRecord);
                }
                else
                {
                    (new WinMsgDialog(excuteResult.Msg, Caption: "失败")).ShowDialog();
                }
            }
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn_Del_ClickAnsyc(object sender, System.Windows.RoutedEventArgs e)
        {
            if (RecordDataGrid.SelectedItem is Lib.MemberPayItem SelectedRec)
            {
                if ((new WinMsgDialog($"确认要删除该待遇项目吗？", Caption: "确认", showYesNo: true)).ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberPayItemRepository.DeleteRecord(SelectedRec);
                    if (excuteResult.State == 0)
                    {
                        cvm.PayItems.Remove(SelectedRec);
                    }
                    else
                    {
                        (new WinMsgDialog(excuteResult.Msg, Caption: "失败")).ShowDialog();
                    }
                }
            }
        }
        /// <summary>
        /// 编辑项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn_Edit_ClickAnsyc(object sender, System.Windows.RoutedEventArgs e)
        {

            if (RecordDataGrid.SelectedItem is Lib.MemberPayItem SelectedRec)
            {
                PageMemberPayItemWin AddWin = new PageMemberPayItemWin(SelectedRec);

                if (AddWin.ShowDialog().Value)
                {
                    ExcuteResult excuteResult = await DataMemberPayItemRepository.UpdateRecord(SelectedRec);
                    if (excuteResult == null || excuteResult.State == 0)
                    {
                        cvm.PayItems.Add(SelectedRec);
                    }
                    else
                    {
                        (new WinMsgDialog(excuteResult.Msg, Caption: "失败")).ShowDialog();
                    }
                }
            }
        }


        /// <summary>
        /// 该页面的视图模型
        /// </summary>
        private class CurViewModel
        {
            public CurViewModel()
            {
                PayItems = new ObservableCollection<MemberPayItem>();
            }
            public ObservableCollection<Lib.MemberPayItem> PayItems { get; set; }
            public Lib.MemberPayItem CurPayItem { get; set; }
        }
    }
}
