using System.Windows.Controls;
using System.Windows.Input;
using Office.Work.Platform.AppCodes;

namespace Office.Work.Platform.MemberScore
{
    /// <summary>
    /// 考勤考核菜单
    /// </summary>
    public partial class PageScoreMenu : Page
    {
        public PageScoreMenu()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Member_ScoreCount_MouseLeftButtonUp(null, null);
        }
        private void Member_ScoreInput_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PageScoreInput CurPage = new PageScoreInput();
            AppSet.AppMainWindow.FrameContentPage.Content = CurPage;
        }

        private void Member_ScoreCount_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Page_HolidayInput_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PageHolidayInput CurPage = new PageHolidayInput();
            AppSet.AppMainWindow.FrameContentPage.Content = CurPage;
        }

        private void Page_HolidayCount_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
