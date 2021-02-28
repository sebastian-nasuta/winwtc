using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public TimeSpan Duration { get; set; }
        public DateTime EndTime => StartTime.Add(Duration);
    }
}
