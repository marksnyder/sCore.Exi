using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Interfaces.Widget
{
    public interface IWidgetRenderer
    {
        string TypeName { get; }
        string Name { get; set; }
        string Description { get; set; }
    }
}
