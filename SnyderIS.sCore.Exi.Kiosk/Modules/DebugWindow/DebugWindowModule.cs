

namespace SnyderIS.sCore.Exi.Kiosk.Modules.DebugWindow
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Gtk;
    using MenuItemImpl = Gtk.MenuItem;
    using SnyderIS.sCore.Exi.Cef;
    using Xilium.CefGlue;
    using System.Net;
    using System.IO;
    using System.Linq;

    public class DebugWindowModule : IKioskModule
    {
        private MainViewImpl _mainView;

        public void Initialize(MainViewImpl mainview)
        {
            _mainView = mainview;
            _mainView.BrowserKeyEvent += _mainView_BrowserKeyEvent;

        }

        void _mainView_BrowserKeyEvent(object sender, Cef.Browser.KeyEventArgs e)
        {
            if (e.Char == 'D')
            {
                var host = _mainView.CurrentBrowser.GetHost();
                var wi = CefWindowInfo.Create();
                wi.SetAsPopup(IntPtr.Zero, "DevTools");
                host.ShowDevTools(wi, new DevToolsWebClient(), new CefBrowserSettings());

            }
        }

        private class DevToolsWebClient : CefClient
        {
        }
    }
}
