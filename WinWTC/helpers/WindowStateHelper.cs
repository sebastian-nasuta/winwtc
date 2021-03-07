using System;
using System.Windows;

namespace WinWTC.helpers
{
    public class WindowStateHelper
    {
        private System.Windows.Forms.NotifyIcon _myNotifyIcon;
        private Window _window;

        public WindowStateHelper(Window window)
        {
            _window = window;
            _window.StateChanged += new EventHandler(Window_StateChanged);

            _myNotifyIcon = new System.Windows.Forms.NotifyIcon();
            _myNotifyIcon.Icon = new System.Drawing.Icon(@"assets\espresso-cup-clock-100.ico");
            _myNotifyIcon.MouseDoubleClick += (s, e) => _window.WindowState = WindowState.Normal;
        }

        public void Window_StateChanged(object sender, EventArgs e)
        {
            if (_window.WindowState == WindowState.Minimized)
            {
                _window.ShowInTaskbar = false;
                _myNotifyIcon.Visible = true;
            }
            else if (_window.WindowState == WindowState.Normal)
            {
                _window.ShowInTaskbar = true;
                _myNotifyIcon.Visible = true;
            }
        }
    }
}
