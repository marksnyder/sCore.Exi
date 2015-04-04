using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Interfaces.Widget
{
    public interface IWidgetGroup
    {
        string Name { get;  }
        string Description { get;  }
        IEnumerable<IWidget> Widgets { get; }
    }
}
