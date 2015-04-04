using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace SnyderIS.sCore.Exi.Cef.Browser
{
    public sealed class RenderProcessTerminatedEventArgs : EventArgs
    {
        private readonly CefBrowser _Browser;
        private readonly CefTerminationStatus _Status;

        public RenderProcessTerminatedEventArgs(CefBrowser browser, CefTerminationStatus status)
        {
            _Browser = browser;
            _Status = status;
        }

        public CefBrowser Browser
        {
            get { return _Browser; }
        }

        public CefTerminationStatus Status
        {
            get { return _Status; }
        }
    }
}
