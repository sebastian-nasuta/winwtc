using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using WinWTC.extensions;
using WinWTC.helpers;
using WinWTC.models;
using WinWTC.utils;

namespace WinWTC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DispatcherTimer _timer;
        private CornerTextWindow _countDownWindow;
        private CornerTextWindow _sleepWindow;
        private CornerTextWindow _timeOutWindow;
        private bool _isCountDownActive;
        private bool _isSleepActive;
        private bool _isTimeOutActive;

        private string _currentPeriodBreakTime;
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

        public string CurrentPeriodBreakTime
        {
            get { return _currentPeriodBreakTime; }
            set
            {
                if (value != _currentPeriodBreakTime)
                {
                    var oldValue = _currentPeriodBreakTime;
                    _currentPeriodBreakTime = value;
                    if (oldValue != value)
                    {
                        OnPropertyChanged("CurrentPeriodBreakTime");
                        OnPropertyChanged("FinishedBreaksView");
                    }
                }
            }
        }

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
            _timer.Interval = TimeSpan.FromMilliseconds(10); // in miliseconds
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
            CurrentPeriodBreakTime = GetCurrentPeriodBreaksSummaryDuration().ToString();

            if (idleTime.TotalSeconds <= WorkTimeConstants.shortestBreakSeconds && idleTime.TotalSeconds > WorkTimeConstants.shortestBreakSeconds - (App.DEBUG_MODE ? 3 : 10))
            {
                int secondToBreak = WorkTimeConstants.shortestBreakSeconds - (int)idleTime.TotalSeconds;
                DisplayCountDownWindow(secondToBreak);
            }
            else
            {
                CloseCountDownWindow();
            }

            if (idleTime.TotalSeconds < WorkTimeConstants.shortestBreakSeconds)
            {
                if (_currentBreak?.Duration.Ticks > 0)
                {
                    FinishedBreaks.Add(_currentBreak);
                    OnPropertyChanged("FinishedBreaksView");
                    _currentBreak = new Break();
                    CloseSleepWindow();
                }
            }
            else
            {
                _currentBreak.Duration = idleTime;
                DisplaySleepWindow();
            }
        }

        private void DisplayCountDownWindow(int number)
        {
            if (!_isCountDownActive)
            {
                _countDownWindow = new CornerTextWindow();
                if (_isTimeOutActive)
                    _countDownWindow.Height = 200;
                _countDownWindow.Closed += new EventHandler((s_tow, e_tow) => _isCountDownActive = false);
                _countDownWindow.Show();
                _isCountDownActive = true;
            }
            _countDownWindow.SetNumberToDisplay(number);
        }

        private void CloseCountDownWindow()
        {
            if (_isCountDownActive)
                _countDownWindow.Close();
        }

        private void DisplaySleepWindow()
        {
            if (!_isSleepActive)
            {
                _sleepWindow = new CornerTextWindow();
                _sleepWindow.Closed += new EventHandler((s_tow, e_tow) => _isSleepActive = false);
                _sleepWindow.Show();
                _isSleepActive = true;
                _sleepWindow.SetTextToDisplay("-_-");
            }
        }

        private void CloseSleepWindow()
        {
            if (_isSleepActive)
                _sleepWindow.Close();
        }

        private void SetTimeoutWindowVisibility()
        {
            if (GetProgramDuration().TotalSeconds > WorkTimeConstants.maxWorkSeconds)
            {
                if (GetCurrentPeriodBreaksSummaryDuration().TotalSeconds < WorkTimeConstants.requiredBreakSeconds)
                {
                    if (!_isTimeOutActive)
                    {
                        _timeOutWindow = new CornerTextWindow();
                        _timeOutWindow.Closed += new EventHandler((s_tow, e_tow) => _isTimeOutActive = false);
                        _timeOutWindow.Show();
                        _isTimeOutActive = true;
                        _timeOutWindow.SetTextToDisplay("!");
                        _timeOutWindow.FlashWindow();
                    }
                }
                else
                {
                    if (_isTimeOutActive)
                        _timeOutWindow.Close();
                }
            }
        }

        private TimeSpan GetCurrentPeriodBreaksSummaryDuration()
        {
            var periodBreaks = FinishedBreaks.Where(n => n.IsInCurrentPeriod);
            var ticks = periodBreaks.Select(n => n.Duration.Ticks).Sum();
            if (_currentBreak != null)
                ticks += _currentBreak.Duration.Ticks;
            return new TimeSpan(ticks);
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
