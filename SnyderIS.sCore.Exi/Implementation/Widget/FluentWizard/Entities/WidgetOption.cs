using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Widget;

namespace SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard.Entities
{
    public class WidgetOption : IWidgetOption
    {

        public string Key { get; set; }
        public string Label { get; set; }
        public bool Required { get; set; }
        public IEnumerable<KeyValuePair<string, string>> SelectList { get; set; }
        public bool Hidden { get; set; }
        public object DefaultValue { get; set; }

    }
}
