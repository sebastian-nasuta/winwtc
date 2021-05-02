using System.Configuration;

namespace WinWTC.utils
{
    public class WorkTimeConstants
    {
        public static int shortestBreakSeconds = App.DEBUG_MODE ? 5 : (int.Parse(ConfigurationManager.AppSettings["shortestBreakMinutes"]) * 60);
        public static int requiredBreakSeconds = App.DEBUG_MODE ? 10 : (int.Parse(ConfigurationManager.AppSettings["requiredBreakMinutes"]) * 60);
        public static int maxWorkSeconds = App.DEBUG_MODE ? 30 : (int.Parse(ConfigurationManager.AppSettings["maxWorkMinutes"]) * 60);
    }
}
