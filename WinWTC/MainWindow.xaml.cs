using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WinWTC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DispatcherTimer _timer;
        private string _idleTime;

        public string IdleTime
        {
            get { return _idleTime; }
            set
            {
                if (value != _idleTime)
                {
                    _idleTime = value;
                    OnPropertyChanged("IdleTime");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
            InitTimer();
        }
        
        public void InitTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(SetIdleTime);
            _timer.Interval = TimeSpan.FromMilliseconds(1); // in miliseconds
            _timer.Start();
        }

        private void SetIdleTime(object sender, EventArgs e)
        {
            IdleTime = IdleTimeDetector.GetIdleTimeInfo().IdleTime.ToString();
            SetSignalField();
        }

        private void SetSignalField()
        {
            if(IdleTimeDetector.GetIdleTimeInfo().IdleTime >= TimeSpan.FromSeconds(10))
                signalField.Background = Brushes.IndianRed;
            else
                signalField.Background = Brushes.Green;
        }
    }
}
