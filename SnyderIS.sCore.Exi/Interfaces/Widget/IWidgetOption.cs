using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Interfaces.Widget
{
    public interface IWidgetOption
    {
        string Key { get; }
        string Label { get; }
        bool Required { get; }
        IEnumerable<KeyValuePair<string,string>> SelectList { get; }
        bool Hidden { get; }
        object DefaultValue { get; }
    }
}
