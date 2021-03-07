using System;
using WinWTC.utils;

namespace WinWTC.models
{
    public class Break
    {
        public Break()
        {
            StartTime = DateTime.Now;
            Duration = new TimeSpan();
        }

        public DateTime StartTime { get; }
        public DateTime EndTime => StartTime.Add(Duration);
        public TimeSpan Duration { get; set; }
        public bool IsInCurrentPeriod => new TimeSpan(DateTime.Now.Ticks - EndTime.Ticks).TotalSeconds <= WorkTimeConstants.maxWorkSeconds;
    }
}
