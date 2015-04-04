using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace SnyderIS.sCore.Exi.Cef.Browser
{
    public class LoadErrorEventArgs : EventArgs
    {
        public CefErrorCode ErrorCode { get; set; }
        public string ErrorText { get; set; }
        public string FailedUrl { get; set; }
    }
}
