
namespace SnyderIS.sCore.Exi.Kiosk.Modules.HeartBeat
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

    public class HeartBeatModule : IKioskModule
    {
        private MainViewImpl _mainView;
        private DateTime _lastHeartBeat;

        public void Initialize(MainViewImpl mainView)
        {
            _mainView = mainView;
            _lastHeartBeat = DateTime.Now;
            _mainView.MainPump += _mainView_MainPump;

            mainView.RegisterCommand("HeartBeat", new HeartBeatCommand(this, _mainView));
            mainView.RegisterCommand("HeartBeatStatus", new HeartBeatStatusCommand(this, _mainView));

        }


        public void SetHeartBeat()
        {
            _lastHeartBeat = DateTime.Now;
        }

        public DateTime GetHeartBeat()
        {
            return _lastHeartBeat;
        }

        void _mainView_MainPump(object sender, EventArgs e)
        {
            if (_lastHeartBeat < DateTime.Now.AddMinutes(-3))
            {
                _mainView.ShellLog("Heartbeat exceeded 3 minutes. Attempting restart");
                _mainView.Restart();
            }
        }
    }
}
