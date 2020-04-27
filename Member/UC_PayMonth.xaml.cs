using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// UC_PayTemp.xaml 的交互逻辑
    /// </summary>
    public partial class UC_PayMonth : UserControl
    {
        private PageEditMemberVM _CurMemberVM;
        public UC_PayMonth()
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //await _UC_PayTempVM.Init_PayTempVMAsync(null)
        }
    }
}
