using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
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
        private readonly bool _isDebugMode = false;
        private int _shortestBreakSeconds => 5 * (_isDebugMode ? 1 : 60);
        private int _longestBreakSeconds => 4 * _shortestBreakSeconds * (_isDebugMode ? 1 : 60);
        private int _maxWorkSeconds => _isDebugMode ? 30 : (90 * 60);
        
        private DispatcherTimer _timer;
        private TimeOutWindow _timeOutWindow;
        private bool _isTimeOutActive;

        private string _idleTime;
        private Break _currentBreak;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public List<Break> FinishedBreaks = new List<Break>();
        public ListCollectionView FinishedBreaksView => new ListCollectionView(FinishedBreaks);

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

        public int ShortestBreaksCount => GetCount(_shortestBreakSeconds, 2 * _shortestBreakSeconds);
        public int FirstMiddleBreaksCount => GetCount(2 * _shortestBreakSeconds, 3 * _shortestBreakSeconds);
        public int SecondMiddleBreaksCount => GetCount(3 * _shortestBreakSeconds, _longestBreakSeconds);
        public int LongestBreaksCount => GetCount(_longestBreakSeconds, null);

        public MainWindow()
        {
#if DEBUG
            _isDebugMode = true;
#endif
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

            if (idleTime.TotalSeconds <= _shortestBreakSeconds && idleTime.TotalSeconds > _shortestBreakSeconds - (_isDebugMode ? 3 : 10))
            {
                int secondToBreak = _shortestBreakSeconds - (int)idleTime.TotalSeconds;
                SplashScreen splashScreen = new SplashScreen($"assets/icons8-circled-{secondToBreak}-64.png");
                splashScreen.Show(true, true);
            }

            if (idleTime.TotalSeconds < _shortestBreakSeconds)
            {
                if (_currentBreak?.Duration.Ticks > 0)
                {
                    FinishedBreaks.Add(_currentBreak);
                    _currentBreak = new Break();
                    RaiseCounts();
                }
            }
            else
            {
                _currentBreak.Duration = idleTime;
                RaiseCounts();
            }
        }

        private void SetTimeoutWindowVisibility()
        {
            if (GetProgramDuration().TotalSeconds > _maxWorkSeconds)
            {
                var periodBreaks = FinishedBreaks.Where(n => new TimeSpan(DateTime.Now.Ticks - n.EndTime.Ticks).TotalSeconds <= _maxWorkSeconds);
                if (periodBreaks.Select(n => n.Duration.TotalSeconds).Sum() + _currentBreak.Duration.TotalSeconds < _longestBreakSeconds)
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
            OnPropertyChanged("FinishedBreaksView");
        }

        private int GetCount(int? minSeconds, int? maxSeconds)
        {
            int result = FinishedBreaks.Where(n => (!minSeconds.HasValue || (int)n.Duration.TotalSeconds >= minSeconds)
                                                && (!maxSeconds.HasValue || (int)n.Duration.TotalSeconds < maxSeconds)).Count();

            if (_currentBreak != null
                && (!minSeconds.HasValue || (int)_currentBreak.Duration.TotalSeconds >= minSeconds)
                && (!maxSeconds.HasValue || (int)_currentBreak.Duration.TotalSeconds < maxSeconds))
            {
                result++;
            }

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
