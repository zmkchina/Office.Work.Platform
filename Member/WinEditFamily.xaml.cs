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
using System.Windows.Shapes;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// WinEditFamily.xaml 的交互逻辑
    /// </summary>
    public partial class WinEditFamily : Window
    {
        public WinEditFamily()
        {
            InitializeComponent();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
