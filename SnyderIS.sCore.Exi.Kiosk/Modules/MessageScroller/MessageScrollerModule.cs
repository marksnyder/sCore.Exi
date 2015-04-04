

namespace SnyderIS.sCore.Exi.Kiosk.Modules.MessageScroller
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
    using System.Runtime.CompilerServices;

    public class MessageScrollerModule : IKioskModule
    {
        private MainViewImpl _mainView;
        private string _messageText = string.Empty;
        private GraphicsEngine.GdiEngine _engine;

        public void Initialize(MainViewImpl mainview)
        {
            _mainView = mainview;
            _engine = new GraphicsEngine.GdiEngine(mainview);

            _mainView.RegisterCommand("MessageChange", new MessageChangeCommand(this, _mainView));
            _mainView.RegisterCommand("SetLogo", new LoadLogoCommand(this, _mainView));

            GLib.Timeout.Add(15, new GLib.TimeoutHandler(DrawMessage));

            _mainView.ShellLog("Loaded message scroller module");

            LoadLogo("http://facility1.qa.iqisystems.com/Content/Images/logo.png");
        }

        public void SetMessage(string val)
        {
            _messageText = val;
        }

        public void LoadLogo(string url)
        {
            _engine.LoadLogo(url);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        bool DrawMessage()
        {
            _engine.DrawMessage(_messageText);
            return true;
        }
    }
}
