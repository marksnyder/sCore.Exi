namespace SnyderIS.sCore.Exi.Cef.Browser
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue;
    using Xilium.CefGlue;

    internal sealed class WebLoadHandler : CefLoadHandler
    {
        private readonly WebBrowser _core;

        public WebLoadHandler(WebBrowser core)
        {
            _core = core;
        }

        protected override void OnLoadingStateChange(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward)
        {
            _core.OnLoadingStateChanged(isLoading, canGoBack, canGoForward);
        }


        protected override void OnLoadError(Xilium.CefGlue.CefBrowser browser, 
            Xilium.CefGlue.CefFrame frame, Xilium.CefGlue.CefErrorCode errorCode, string errorText, string failedUrl)
        {
            _core.OnLoadError(browser, frame, errorCode, errorText, failedUrl);
        }

        protected override void OnLoadEnd(Xilium.CefGlue.CefBrowser browser, Xilium.CefGlue.CefFrame frame, int httpStatusCode)
        {
            _core.OnLoadEnd(browser, frame, httpStatusCode);
        }

    }
}
