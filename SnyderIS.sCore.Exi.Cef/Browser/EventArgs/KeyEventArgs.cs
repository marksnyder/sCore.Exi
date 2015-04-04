namespace SnyderIS.sCore.Exi.Cef.Browser
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class KeyEventArgs : EventArgs
    {
        private readonly char _char;

        public KeyEventArgs(char c)
        {
            _char = c;
        }

        public char Char
        {
            get { return _char; }
        }
    }
}
