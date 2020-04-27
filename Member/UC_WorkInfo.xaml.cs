using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// UC_BasicInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UC_WorkInfo : UserControl
    {
        private PageEditMember _PageEditMember;

        public UC_WorkInfo()
        {
            InitializeComponent();
        }
        public void initControl(PageEditMember PPageEditMember)
        {
            _PageEditMember = PPageEditMember;
            DataContext = _PageEditMember._PageEditMemberVM;
        }
        private async void btn_save_ClickAsync(object sender, RoutedEventArgs e)
        {            
            ExcuteResult excuteResult = await DataMemberRepository.UpdateMember(_PageEditMember._PageEditMemberVM.EntityMember);
            if (excuteResult.State == 0)
            {
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
    }
}
