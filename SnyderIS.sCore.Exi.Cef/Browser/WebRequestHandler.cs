namespace SnyderIS.sCore.Exi.Cef.Browser
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue;

    class WebRequestHandler : CefRequestHandler
    {
         private readonly WebBrowser _core;

         public WebRequestHandler(WebBrowser core)
        {
            _core = core;
        }

        protected override bool OnBeforeBrowse(CefBrowser browser, CefFrame frame, CefRequest request, bool isRedirect)
        {
            return base.OnBeforeBrowse(browser, frame, request, isRedirect);
        }

        protected override void OnRenderProcessTerminated(CefBrowser browser, CefTerminationStatus status)
        {
            _core.OnRenderProcessTerminated(browser, status);
            base.OnRenderProcessTerminated(browser, status);
        }
    }
}
