using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// 正式人员月度工资查询（发放）
    /// </summary>
    public partial class PagePayMonthOfficialSheet : Page
    {
        public PagePayMonthOfficialSheet()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PaySheet = new ObservableCollection<MemberPayMonthOfficialSheet>();
            this.DataContext = this;
        }
        /// <summary>
        /// 查询正式人员待发放工资
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearchClickAsync(object sender, RoutedEventArgs e)
        {
            IEnumerable<MemberPayMonthOfficialSheet> TempRec = await DataMemberCombiningRepository.GetMemberPayMonthOfficialSheet(PayMonthDate.Year, PayMonthDate.Month);
            PaySheet.Clear();
            TempRec.ToList().ForEach(e =>
            {
                e.ShouldGetMoney = e.LivingAllowance + e.IncentivePerformancePay + e.PostAllowance + e.ScalePay + e.PostPay;
                e.FactGetMoney = e.HousingFund + e.MedicalInsurance + e.OccupationalPension + e.PensionInsurance + e.Tax + e.UnionFees;
                e.FactGetMoney = e.ShouldGetMoney - e.FactGetMoney;
                PaySheet.Add(e);
            });
        }
        /// <summary>
        /// 打印工资表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPrintClickAsync(object sender, RoutedEventArgs e)
        {

        }
        public ObservableCollection<MemberPayMonthOfficialSheet> PaySheet { get; set; }
        public DateTime PayMonthDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 生成正式人员指定月份的工资。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnCreateClickAsync(object sender, RoutedEventArgs e)
        {
            ExcuteResult excuteResult = await DataMemberCombiningRepository.PostMemberPayMonthOfficialSheet(PayMonthDate.Year, PayMonthDate.Month);
            (new WinMsgDialog(excuteResult.Msg)).ShowDialog();
        }
    }

}
