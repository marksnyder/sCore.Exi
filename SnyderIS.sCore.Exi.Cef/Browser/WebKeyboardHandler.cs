namespace SnyderIS.sCore.Exi.Cef.Browser
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue;

    internal sealed class WebKeyboardHandler : CefKeyboardHandler
    {
        private readonly WebBrowser _core;

        public WebKeyboardHandler(WebBrowser core)
        {
            _core = core;
        }

        protected override bool OnKeyEvent(CefBrowser browser, CefKeyEvent keyEvent, IntPtr osEvent)
        {
            _core.OnKeyEvent(browser, keyEvent, osEvent);
            return false;
        }

    }
}
