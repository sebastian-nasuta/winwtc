using System.Windows;
using WinWTC.extensions;

namespace WinWTC
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TimeOutWindow : Window
    {
        public TimeOutWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
            this.FlashWindow();
        }
    }
}
