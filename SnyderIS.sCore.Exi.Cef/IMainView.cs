namespace SnyderIS.sCore.Exi.Cef
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue;

    public interface IMainView : IDisposable
    {

        void Close();
        CefBrowser CurrentBrowser { get; }


    }
}
