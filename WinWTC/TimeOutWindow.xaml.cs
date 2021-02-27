using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
