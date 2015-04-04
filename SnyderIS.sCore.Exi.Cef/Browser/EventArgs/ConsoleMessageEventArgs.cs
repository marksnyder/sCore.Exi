namespace SnyderIS.sCore.Exi.Cef.Browser
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class ConsoleMessageEventArgs : EventArgs
    {

        private readonly string _message;
        private readonly string _source;
        private readonly int _line;

        public ConsoleMessageEventArgs(string message,
            string source,
            int line)
        {
            _message = message;
            _source = source;
            line = _line;
        }

        public string Message
        {
            get { return _message; }
        }

        public string Source
        {
            get { return _source; }
        }

        public int Line
        {
            get { return _line; }
        }
    }
}
