using System.ComponentModel;
using System.Windows;
using WinWTC.extensions;

namespace WinWTC
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CornerTextWindow : Window, INotifyPropertyChanged
    {
        private string _textToDisplay = "";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public string TextToDisplay
        {
            get => _textToDisplay;
            private set
            {
                if (value != _textToDisplay)
                {
                    _textToDisplay = value;
                    OnPropertyChanged("TextToDisplay");
                }
            }
        }

        public CornerTextWindow()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Left;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        public void SetTextToDisplay(string text)
        {
            this.Title = text;
            TextToDisplay = text;
        }

        public void SetNumberToDisplay(int number)
        {
            SetTextToDisplay(number.ToString());
        }
    }
}
