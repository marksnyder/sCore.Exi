using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Mvc.Json
{
    public class Slot
    {
        public string Title { get; set; }
        public Guid Identifier { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
