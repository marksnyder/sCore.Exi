using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Widget;


namespace SnyderIS.sCore.Exi.Interfaces.Canvas
{
    public interface IColumn
    {
        IEnumerable<ISlot> Slots { get; }
    }
}
