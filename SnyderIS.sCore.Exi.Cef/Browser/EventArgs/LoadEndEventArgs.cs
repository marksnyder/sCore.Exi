using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Cef.Browser
{
    public class LoadEndEventArgs : System.EventArgs
    {
        public int HttpStatusCode { get; set; }
    }
}
