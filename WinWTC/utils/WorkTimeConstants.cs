namespace WinWTC.utils
{
    public class WorkTimeConstants
    {
        public static int shortestBreakSeconds = App.DEBUG_MODE ? 5 : (3 * 60);
        public static int longestBreakSeconds = App.DEBUG_MODE ? 20 : (20 * 60);
        public static int maxWorkSeconds = App.DEBUG_MODE ? 60 : (90 * 60);
    }
}
