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
    /// 考勤记录
    /// </summary>
    public partial class UC_Attendance : UserControl
    {
        //private UC_PayTempVM _UC_PayTempVM;

        public UC_Attendance()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //await _UC_PayTempVM.Init_PayTempVMAsync(null)
        }
    }
}
