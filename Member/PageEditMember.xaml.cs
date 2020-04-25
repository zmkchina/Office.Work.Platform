using Office.Work.Platform.Lib;
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

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// PageEditMember.xaml 的交互逻辑
    /// </summary>
    public partial class PageEditMember : Page
    {
        private PageEditMemberVM _PageEditMemberVM;

        public PageEditMember(Lib.Member P_Member)
        {
            InitializeComponent();
            _PageEditMemberVM = new PageEditMemberVM(P_Member);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = _PageEditMemberVM;
        }

        private async void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            ExcuteResult runResult = await _PageEditMemberVM.AddOrUpdate();
            if (runResult.State == 0)
            {
                MessageBox.Show("保存成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btn_AddRelation_Click(object sender, RoutedEventArgs e)
        {
            WinEditFamily winEditFamily = new WinEditFamily();
            if (winEditFamily.ShowDialog() == false)
            {
                MessageBox.Show("cancel");
            }
        }
        /// <summary>
        /// 社会关系Tab标签被点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(_PageEditMemberVM.EntityMember == null || string.IsNullOrWhiteSpace(_PageEditMemberVM.EntityMember.Id))
            {
                MessageBox.Show("请先保存人员信息！");
            }
        }
    }
}
