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
        }

        public DateTime StartTime { get; set; }
        public TimeSpan? Duration { get; set; }
    }
}
