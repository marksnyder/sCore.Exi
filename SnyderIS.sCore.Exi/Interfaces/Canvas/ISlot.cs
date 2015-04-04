using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Widget;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Interfaces.DataSource;

namespace SnyderIS.sCore.Exi.Interfaces.Canvas
{
    public interface ISlot
    {
        Guid Identifier { get; }
        string Title { get; }
        System.Type DataSource { get; }
        System.Type Renderer { get; }
        IDictionary<string, string> Options { get; }
        int X { get; }
        int Y { get; }
    }
}
