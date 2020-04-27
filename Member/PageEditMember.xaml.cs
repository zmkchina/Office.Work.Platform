using System.Windows.Controls;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// PageEditMember.xaml 的交互逻辑
    /// </summary>
    public partial class PageEditMember : Page
    {
        public PageEditMemberVM _PageEditMemberVM;

        public PageEditMember(Lib.Member PMember = null)
        {
            InitializeComponent();
            _PageEditMemberVM = new PageEditMemberVM(PMember);
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TabItem item in Person_TabControl.Items)
            {
                item.MouseLeftButtonUp += TabItem_MouseLeftButtonUp;
            }
            UCBasicInfo.initControl(this);
        }

        private void TabItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TabItem tb)
            {
                tb.IsSelected = true;
                switch (tb.Header)
                {
                    case "基本信息":
                        UCBasicInfo.initControl(this);
                        break;
                    case "工作信息":
                        UCWorkInfo.initControl(this);
                        break;
                }
            }
        }
    }
}
