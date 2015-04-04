namespace SnyderIS.sCore.Exi.Kiosk
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


    public class MainViewImpl : Window, IMainView
    {

        public IList<Modules.IKioskModule> _modules;
        public IDictionary<string,Modules.IKioskCommand> _commands;

        private readonly CoreApp _application;
        private CefWebBrowser _browserControl;
        private Window _statusWindow;
        private Label _statusLabel;
        private string _browserUrl;
        private bool _reloadRequested;

        private VBox _content;

        public MainViewImpl(CoreApp application)
            : base(WindowType.Toplevel)
        {

            _browserUrl = Configuration.BrowseUrl;
            _modules = new List<Modules.IKioskModule>();
            _commands = new Dictionary<string,Modules.IKioskCommand>();

            _modules.Add(new Modules.DebugWindow.DebugWindowModule());
            _modules.Add(new Modules.MessageScroller.MessageScrollerModule());
            _modules.Add(new Modules.HeartBeat.HeartBeatModule());

            InitializeStatusWindow();
            _application = application;

            Resize(_application.DefaultWidth, _application.DefaultHeight);

            this.Fullscreen();
            Destroyed += onDestroyed;

            var vbox = new VBox(false, 0);
            Add(vbox);

            _content = new VBox(true, 0);
            _content.Homogeneous = false;
            vbox.PackStart(_content, true, true, 2);
            _content.ShowAll();

            CreateBrowserInstance();
            ShowAll();

            /* Setup timer */
            GLib.Timeout.Add(3000, new GLib.TimeoutHandler(OnMainPump));
            ShellLog("Core Initialized");

            foreach (var m in _modules)
            {
                m.Initialize(this);
            }
        }


        private void CreateBrowserInstance()
        {
            Console.WriteLine("Creating browser instance");


            if (_browserControl != null)
            {
                _content.Remove(_browserControl);
                _browserControl.Dispose();
                _browserControl = null;
            }

            _browserControl = new CefWebBrowser();
            _content.Add(_browserControl);
            ((Box.BoxChild)_content[_browserControl]).Expand = true;
            ((Box.BoxChild)_content[_browserControl]).Fill = true;

            var browser = _browserControl.WebBrowser;

            browser.StartUrl = _browserUrl;


            browser.LoadingStateChanged += (s, e) =>
            {
                onLoadingStateChanged(s, e);
            };

            browser.LoadError += (s, e) =>
            {
                onLoadError(s, e);
            };

            browser.LoadEnd += (s, e) =>
            {
                onLoadEnd(s, e);
            };

            browser.ConsoleMessage += (s, e) =>
            {
                onConsoleMessage(s, e);
            };
            
            browser.RenderProcessTerminated += (s, e) =>
            {
                onRenderProcessTerminated(s, e);
            };

            browser.KeyEvent += (s, e) =>
            {
                onBrowserKeyEvent(s, e);
            };
        }

        void onRenderProcessTerminated(object sender, Cef.Browser.RenderProcessTerminatedEventArgs e)
        {

            if (e.Status == CefTerminationStatus.ProcessCrashed)
            {
                ShellLog("CEF Crash Detected");
                ShowStatus("Restarting");
                this.Restart();
            }
        }

        public event EventHandler<Cef.Browser.KeyEventArgs> BrowserKeyEvent;

        void onBrowserKeyEvent(object sender, Cef.Browser.KeyEventArgs e)
        {
            if (e.Char == 'E')
            {
                ShellLog("Close requested");
                this.Close();
            }

            var handler = BrowserKeyEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<Cef.Browser.ConsoleMessageEventArgs> ConsoleMessage;

        private void onConsoleMessage(object sender, Cef.Browser.ConsoleMessageEventArgs e)
        {
            if (e.Message.StartsWith("sCore.Exi.Kiosk.Command."))
            {
                var command = e.Message.Replace("sCore.Exi.Kiosk.Command.", "").ToLower();

                if (command.IndexOf(' ') > 0)
                {
                    command = command.Substring(0, command.IndexOf(' '));
                }

                string argsString = string.Empty;
                int argLength = e.Message.Length - e.Message.IndexOf(' ');

                if (argLength > 0 && e.Message.IndexOf(' ') > 0)
                {
                    argsString = e.Message.Substring(
                        e.Message.IndexOf(' '), argLength);
                }

                var args = argsString.Split(' ');

                if (_commands.ContainsKey(command))
                {
                    var instance = _commands[command];
                    instance.Run(args);
                }
                else
                {
                    ShellLog(string.Format("Unknown shell command:", command));
                }
            }

            var handler = ConsoleMessage;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler MainPump;

        bool OnMainPump()
        {
            if (_reloadRequested)
            {

                if (_browserControl.WebBrowser.CefBrowser != null)
                {
                    _reloadRequested = false;
                    _browserControl.WebBrowser.CefBrowser.Reload();
                }

            }


            var handler = MainPump;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            return true;
        }

        void onLoadError(object sender, Cef.Browser.LoadErrorEventArgs e)
        {
            ShowStatus(string.Concat("Error:", e.ErrorCode));
            ShellLog(string.Concat("Error loading page ",Configuration.BrowseUrl));
            _reloadRequested = true;

        }

        void onLoadEnd(object sender, Cef.Browser.LoadEndEventArgs e)
        {
            if (e.HttpStatusCode != 200)
            {
                ShowStatus(string.Concat("Bad Request:"));
                ShellLog(string.Concat("Bad request ", Configuration.BrowseUrl));
                _reloadRequested = true;
            }
            else
            {
                HideStatus();
            }

        }

        void onLoadingStateChanged(object sender, Cef.Browser.LoadingStateChangedEventArgs e)
        {
            if (e.Loading)
            {
                ShowStatus("Loading");
            }
        }

        void onDestroyed(object sender, EventArgs e)
        {
            _application.Quit();
        }

        void InitializeStatusWindow()
        {
            /* Setup popup label */
            _statusLabel = new Label();
            _statusLabel.Show();
            

            Gdk.Color labelColor = new Gdk.Color();
            Gdk.Color.Parse("yellow", ref labelColor);
            _statusLabel.ModifyFg(StateType.Normal, labelColor);


            /* Setup popop window */
            _statusWindow = new Window(WindowType.Popup);
            _statusWindow.Resize(500, 100);
            _statusWindow.Add(_statusLabel);
            _statusWindow.WindowPosition = Gtk.WindowPosition.CenterAlways;
            _statusWindow.Opacity = Configuration.MessageOpacity;

            Gdk.Color windowBGColor = new Gdk.Color();
            Gdk.Color.Parse("black", ref windowBGColor);
            _statusWindow.ModifyBg(StateType.Normal, windowBGColor);

        }

        void ShowStatus(string message)
        {
            _statusLabel.Text = message;
            _statusWindow.Show();
        }

        void HideStatus()
        {
            _statusLabel.Text = string.Empty;
            _statusWindow.Hide();
        }

        public CefBrowser CurrentBrowser
        {
            get
            {
                if (_browserControl != null && _browserControl.WebBrowser != null)
                {
                    return _browserControl.WebBrowser.CefBrowser;
                }
                return null;
            }
        }

        public void Close()
        {
            ShellLog("Closing");
            Destroy();
        }

        public void Restart()
        {
            Program.Recycle = true;
            ShellLog("Closing with restart");
            Destroy();
        }

        public event EventHandler<ShellLogEventArgs> ShellMessage;

        public void ShellLog(string message)
        {
            var m = string.Concat(DateTime.Now.ToString("MM/dd/yy mm:HH:ss"), " ", message, System.Environment.NewLine);
            Console.WriteLine(m);
            File.AppendAllText(Configuration.ShellLogPath, m);

            var handler = ShellMessage;
            if (handler != null)
            {
                handler(this, new ShellLogEventArgs() { Message = message });
            }

            if (this.CurrentBrowser != null && this.CurrentBrowser.GetFrame(1) != null)
            {
                this.CurrentBrowser.GetFrame(1).ExecuteJavaScript(string.Format("console.log('Shell:{0}')", message), null, 0);
            }
        }

        public class ShellLogEventArgs : EventArgs
        {
            public string Message { get; set; }
        }

        public void RegisterCommand(string name, Modules.IKioskCommand command)
        {
            _commands.Add(name.ToLower(), command);
        }
    }
}
