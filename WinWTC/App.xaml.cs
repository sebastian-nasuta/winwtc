using System.Windows;

namespace WinWTC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool DEBUG_MODE =
#if DEBUG
            true;
#else
            false;
#endif
    }
}
