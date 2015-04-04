using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Interfaces.Widget
{
    public interface IWidgetOptionGroup
    {
        string Description { get; }
        IEnumerable<IWidgetOption> Options { get; }
    }
}
