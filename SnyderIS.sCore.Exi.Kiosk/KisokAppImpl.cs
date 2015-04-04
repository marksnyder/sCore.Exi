namespace SnyderIS.sCore.Exi.Kiosk
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Gtk;
    using SnyderIS.sCore.Exi.Cef;
    using Xilium.CefGlue;

    public sealed class KioskAppImpl : CoreApp
    {
        protected override void PlatformInitialize()
        {
            Application.Init();
        }

        protected override void PlatformShutdown()
        {
        }

        protected override void PlatformRunMessageLoop()
        {
            if (CefRuntime.Platform == CefRuntimePlatform.Windows) Application.Run();
            else CefRuntime.RunMessageLoop();
        }

        protected override void PlatformQuitMessageLoop()
        {
            if (CefRuntime.Platform == CefRuntimePlatform.Windows) Application.Quit();
            else CefRuntime.QuitMessageLoop();
        }

        protected override IMainView CreateMainView()
        {
            return new MainViewImpl(this);
        }      

        protected override void PlatformMessageBox(string message)
        {
            System.Console.WriteLine(message);

            // TODO: this will fail without message loop?
            //MessageDialog md = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, message);
            //md.Run();
            //md.Destroy();
        }
    }
}
