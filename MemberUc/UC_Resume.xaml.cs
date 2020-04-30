using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberUc
{
    /// <summary>
    /// UC_BasicInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UC_Resume : UserControl
    {
        private PageEditMemberVM _CurMemberVM;
        public UC_Resume()
        {
            InitializeComponent();
        }
        public void initControl(PageEditMemberVM PMember)
        {
            _CurMemberVM = PMember;
            this.DataContext = _CurMemberVM;
        }
        private async void btn_save_ClickAsync(object sender, RoutedEventArgs e)
        {
            ExcuteResult excuteResult = await DataMemberRepository.UpdateMember(_CurMemberVM.EntityMember);
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
