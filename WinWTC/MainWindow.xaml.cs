using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace WinWTC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;
        private DispatcherTimer _timer;
        private string _idleTime;
        private bool _isTimeOutActive;
        private TimeOutWindow _timeOutWindow;

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

            this.StateChanged += new EventHandler(Window_StateChanged);
            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();
            MyNotifyIcon.Icon = new System.Drawing.Icon(@"assets\espresso-cup-clock-100.ico");
            MyNotifyIcon.MouseDoubleClick += (s,e) => this.WindowState = WindowState.Normal;
            
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
            if (IdleTimeDetector.GetIdleTimeInfo().IdleTime >= TimeSpan.FromSeconds(2))
            {
                if (!_isTimeOutActive)
                {
                    _timeOutWindow = new TimeOutWindow();
                    _timeOutWindow.Closed += new EventHandler((s_tow, e_tow) => _isTimeOutActive = false);
                    _timeOutWindow.Show();
                    _isTimeOutActive = true;
                }
            }
            else
            {
                if (_isTimeOutActive)
                    _timeOutWindow.Close();
            }
        }

        private void SetSignalField()
        {
            if (IdleTimeDetector.GetIdleTimeInfo().IdleTime >= TimeSpan.FromSeconds(10))
                signalField.Background = Brushes.IndianRed;
            else
                signalField.Background = Brushes.Green;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                MyNotifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                MyNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }
    }
}
