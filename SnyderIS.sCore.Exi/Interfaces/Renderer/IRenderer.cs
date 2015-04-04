using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.DataSource;

namespace SnyderIS.sCore.Exi.Interfaces.Renderer
{
    public interface IRenderer
    {
        string RenderHtml(IDataSourceResult data);
    }

    public interface IRenderer<T> : IRenderer
    {
        string RenderHtml(IDataSourceResult<T> data);
    }
}
