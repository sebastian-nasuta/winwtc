using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using WinWTC.extensions;
using WinWTC.helpers;
using WinWTC.models;

namespace WinWTC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const int firstBreak = 5;
        private const int secondBreak = 10;

        private DispatcherTimer _timer;
        //private TimeOutWindow _timeOutWindow;
        //private bool _isTimeOutActive;

        private string _idleTime;
        private Break _currentBreak;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public List<Break> FinishedBreaks = new List<Break>();

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

        public int TwoCount => GetCount(firstBreak);
        public int FiveCount => GetCount(secondBreak);

        public MainWindow()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
            new WindowStateHelper(this);
            InitTimer();
        }

        public void InitTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(SetProperties);
            _timer.Interval = TimeSpan.FromMilliseconds(1); // in miliseconds
            _timer.Start();
        }

        private void SetProperties(object sender, EventArgs e)
        {
            if (_currentBreak == null)
                _currentBreak = new Break();

            var idleTime = IdleTimeDetector.GetIdleTimeInfo().IdleTime;
            SetIdleTime(idleTime);
            if (idleTime.TotalSeconds < firstBreak && _currentBreak?.Duration != null)
            {
                FinishedBreaks.Add(_currentBreak);
                _currentBreak = new Break();
                RaiseCounts();
            }

            switch (idleTime.TotalSeconds)
            {
                case firstBreak:
                case secondBreak:
                    _currentBreak.Duration = idleTime;
                    RaiseCounts();
                    break;
            }
        }

        private void SetIdleTime(TimeSpan idleTime)
        {
            IdleTime = idleTime.ToString();


            //if (IdleTimeDetector.GetIdleTimeInfo().IdleTime >= TimeSpan.FromSeconds(2))
            //{
            //    if (!_isTimeOutActive)
            //    {
            //        _timeOutWindow = new TimeOutWindow();
            //        _timeOutWindow.Closed += new EventHandler((s_tow, e_tow) => _isTimeOutActive = false);
            //        _timeOutWindow.Show();
            //        _isTimeOutActive = true;
            //    }
            //}
            //else
            //{
            //    if (_isTimeOutActive)
            //        _timeOutWindow.Close();
            //}
        }

        private void RaiseCounts()
        {
            OnPropertyChanged("TwoCount");
            OnPropertyChanged("FiveCount");
        }

        private int GetCount(int seconds)
        {
            int result = FinishedBreaks.Where(n => n.Duration?.Seconds == seconds).Count();
            if (_currentBreak != null && _currentBreak.Duration?.Seconds == seconds)
                result++;
            return result;
        }
    }
}
