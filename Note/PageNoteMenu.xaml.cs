using System.Windows.Controls;
using System.Windows.Input;
using Office.Work.Platform.AppCodes;

namespace Office.Work.Platform.Note
{
    /// <summary>
    /// PageNodeMenu.xaml 的交互逻辑
    /// </summary>
    public partial class PageNoteMenu : Page
    {
        public PageNoteMenu()
        {
            InitializeComponent();
        }

        private void ListBoxItem_MySelf_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PageNoteInfo(true);
        }

        private void ListBoxItem_AllNotes_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AppSet.AppMainWindow.FrameContentPage.Content = new PageNoteInfo(false);
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ListBoxItem_MySelf_MouseLeftButtonUp(null, null);
        }
    }
}
