using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Widget;

namespace SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard.Entities
{
    public class WidgetGroup : IWidgetGroup
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<IWidget> Widgets { get; set;}
    }
}
