using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

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
