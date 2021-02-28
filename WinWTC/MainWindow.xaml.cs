using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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
        private const int shortestBreak = 5;
        private const int firstMiddleBreak = 10;
        private const int secondMiddleBreak = 15;
        private const int longestBreak = 20;
        private const int maxWorkTime = 90;

        private DispatcherTimer _timer;
        private TimeOutWindow _timeOutWindow;
        private bool _isTimeOutActive;

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

        public int ShortestBreaksCount => GetCount(shortestBreak);
        public int FirstMiddleBreaksCount => GetCount(firstMiddleBreak);
        public int SecondMiddleBreaksCount => GetCount(secondMiddleBreak);
        public int LongestBreaksCount => GetCount(longestBreak);

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
            _timer.Tick += new EventHandler(OnEveryTick);
            _timer.Interval = TimeSpan.FromMilliseconds(1); // in miliseconds
            _timer.Start();
        }

        private void OnEveryTick(object sender, EventArgs e)
        {
            SetProperties();
            SetTimeoutWindowVisibility();
        }

        private void SetProperties()
        {
            if (_currentBreak == null)
                _currentBreak = new Break();

            var idleTime = IdleTimeDetector.GetIdleTimeInfo().IdleTime;

            IdleTime = idleTime.ToString();

            if (idleTime.TotalMinutes < shortestBreak && _currentBreak?.Duration != null)
            {
                FinishedBreaks.Add(_currentBreak);
                _currentBreak = new Break();
                RaiseCounts();
            }

            switch ((int)idleTime.TotalMinutes)
            {
                case shortestBreak:
                case firstMiddleBreak:
                case secondMiddleBreak:
                case longestBreak:
                    _currentBreak.Duration = idleTime;
                    RaiseCounts();
                    break;
            }
        }

        private void SetTimeoutWindowVisibility()
        {
            if (GetProgramDuration().TotalMinutes > maxWorkTime)
            {
                var periodBreaks = FinishedBreaks.Where(n => new TimeSpan(DateTime.Now.Ticks - n.EndTime.Ticks).TotalMinutes <= maxWorkTime);
                if (periodBreaks.Select(n => n.Duration.TotalMinutes).Sum() + _currentBreak.Duration.TotalMinutes < longestBreak)
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
        }

        private void RaiseCounts()
        {
            OnPropertyChanged("ShortestBreaksCount");
            OnPropertyChanged("FirstMiddleBreaksCount");
            OnPropertyChanged("SecondMiddleBreaksCount");
            OnPropertyChanged("LongestBreaksCount");
        }

        private int GetCount(int minutes)
        {
            int result = FinishedBreaks.Where(n => (int)n.Duration.TotalMinutes == minutes).Count();
            if (_currentBreak != null && (int)_currentBreak.Duration.TotalMinutes == minutes)
                result++;
            return result;
        }

        private DateTime GetProgramStartTime()
        {
            if (FinishedBreaks.Count() > 0)
                return FinishedBreaks.Select(n => n.StartTime).Min();
            else
                return _currentBreak.StartTime;
        }

        private TimeSpan GetProgramDuration() => new TimeSpan(DateTime.Now.Ticks - GetProgramStartTime().Ticks);
    }
}
