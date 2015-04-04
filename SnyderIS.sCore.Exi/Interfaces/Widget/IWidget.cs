using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Interfaces.DataSource;

namespace SnyderIS.sCore.Exi.Interfaces.Widget
{
    public interface IWidget
    {
        string Name { get; }
        string Description { get; }
        IEnumerable<IWidgetOption> Options { get; set; }
        IEnumerable<IWidgetRenderer> RendererOptions { get;}
        string DataSourceTypeName { get; }
    }

}
