using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Widget;

namespace SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard.Entities
{
    public class WidgetOptionGroup : IWidgetOptionGroup
    {
        public string Description { get; set; }
        public IEnumerable<IWidgetOption> Options { get; set; }
    }
}
