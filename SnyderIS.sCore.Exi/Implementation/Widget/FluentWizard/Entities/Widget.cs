using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Widget;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Interfaces.DataSource;

namespace SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard.Entities
{
    public class Widget : IWidget
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<IWidgetOption> Options { get; set; }
        public IEnumerable<IWidgetRenderer> RendererOptions { get; set; }
        public string DataSourceTypeName { get; set; }
    }
}
