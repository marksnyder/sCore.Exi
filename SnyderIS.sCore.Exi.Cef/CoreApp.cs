namespace SnyderIS.sCore.Exi.Cef
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Diagnostics;
    using Xilium.CefGlue.Wrapper;
    using Xilium.CefGlue;
    using System.Threading;
    using System.Configuration;


    public abstract class CoreApp : IDisposable
    {
        public static CefMessageRouterBrowserSide BrowserMessageRouter { get; private set; }

        private const string DumpRequestDomain = "dump-request.demoapp.cefglue.xilium.local";

        private IMainView _mainView;

        protected CoreApp()
        {
        }

        #region IDisposable

        ~CoreApp()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion

        public string Name { get { return "Kiosk"; } }
        public int DefaultWidth { get { return 800; } }
        public int DefaultHeight { get { return 600; } }


        protected IMainView MainView { get { return _mainView; } }

        public int Run(string[] args)
        {
            try
            {
                return RunInternal(args);
            }
            catch (Exception ex)
            {
                PlatformMessageBox(ex.ToString());
                return 1;
            }
        }

        protected bool MultiThreadedMessageLoop { get; private set; }

        private int RunInternal(string[] args)
        {
            System.Console.WriteLine("CEF Load");

            CefRuntime.Load();

            var settings = new CefSettings();
            settings.MultiThreadedMessageLoop = MultiThreadedMessageLoop = CefRuntime.Platform == CefRuntimePlatform.Windows;
            settings.SingleProcess = false;

            settings.RemoteDebuggingPort = 10224;
 
            settings.LogSeverity = Configuration.CefLogSeverity;
            settings.LogFile = Configuration.CefLogPath;
                 
            settings.ResourcesDirPath = System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetEntryAssembly().CodeBase).LocalPath);



            var argv = args;
            if (CefRuntime.Platform != CefRuntimePlatform.Windows)
            {
                argv = new string[args.Length + 1];
                Array.Copy(args, 0, argv, 1, args.Length);
                argv[0] = "-";
            }

            var mainArgs = new CefMainArgs(argv);
            var app = new CoreCefApp();

            System.Console.WriteLine("CEF Execute Process");

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app);
            Console.WriteLine("CefRuntime.ExecuteProcess() returns {0}", exitCode);
            if (exitCode != -1)
                return exitCode;

            // guard if something wrong
            foreach (var arg in args) { if (arg.StartsWith("--type=")) { return -2; } }

            System.Console.WriteLine("CEF Init");

            CefRuntime.Initialize(mainArgs, settings, app);

            RegisterSchemes();
            RegisterMessageRouter();

            PlatformInitialize();

            _mainView = CreateMainView();


            PlatformRunMessageLoop();

            _mainView.Dispose();
            _mainView = null;

            System.Console.WriteLine("CEF Shutdown");

            CefRuntime.Shutdown();

            PlatformShutdown();
            return 0;
        }

        public void Quit()
        {
            PlatformQuitMessageLoop();
        }

        protected abstract IMainView CreateMainView();

        protected abstract void PlatformInitialize();

        protected abstract void PlatformShutdown();

        protected abstract void PlatformRunMessageLoop();

        protected abstract void PlatformQuitMessageLoop();

        protected abstract void PlatformMessageBox(string message);


        private void RegisterSchemes()
        {
            // register custom scheme handler
            CefRuntime.RegisterSchemeHandlerFactory("http", DumpRequestDomain, new DemoAppSchemeHandlerFactory());
            // CefRuntime.AddCrossOriginWhitelistEntry("http://localhost", "http", "", true);

        }

        private void RegisterMessageRouter()
        {
            if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
            {
                PostTask(CefThreadId.UI, this.RegisterMessageRouter);
                return;
            }

            // window.cefQuery({ request: 'my_request', onSuccess: function(response) { console.log(response); }, onFailure: function(err,msg) { console.log(err, msg); } });
            CoreApp.BrowserMessageRouter = new CefMessageRouterBrowserSide(new CefMessageRouterConfig());
            CoreApp.BrowserMessageRouter.AddHandler(new DemoMessageRouterHandler());
            
        }

        private class DemoMessageRouterHandler : CefMessageRouterBrowserSide.Handler
        {
            public override bool OnQuery(CefBrowser browser, CefFrame frame, long queryId, string request, bool persistent, CefMessageRouterBrowserSide.Callback callback)
            {
                if (request == "wait5")
                {
                    new Thread(() =>
                    {
                        Thread.Sleep(5000);
                        callback.Success("success! responded after 5 sec timeout."); // TODO: at this place crash can occurs, if application closed
                    }).Start();
                    return true;
                }

                if (request == "wait5f")
                {
                    new Thread(() =>
                    {
                        Thread.Sleep(5000);
                        callback.Failure(12345, "success! responded after 5 sec timeout. responded as failure.");
                    }).Start();
                    return true;
                }

                if (request == "wait30")
                {
                    new Thread(() =>
                    {
                        Thread.Sleep(30000);
                        callback.Success("success! responded after 30 sec timeout.");
                    }).Start();
                    return true;
                }

                if (request == "noanswer")
                {
                    return true;
                }

                var chars = request.ToCharArray();
                Array.Reverse(chars);
                var response = new string(chars);
                callback.Success(response);
                return true;
            }

            public override void OnQueryCanceled(CefBrowser browser, CefFrame frame, long queryId)
            {
            }

            
        }

        public static void PostTask(CefThreadId threadId, Action action)
        {
            CefRuntime.PostTask(threadId, new ActionTask(action));
        }

        internal sealed class ActionTask : CefTask
        {
            public Action _action;

            public ActionTask(Action action)
            {
                _action = action;
            }

            protected override void Execute()
            {
                _action();
                _action = null;
            }
        }

        public delegate void Action();
    }
}
