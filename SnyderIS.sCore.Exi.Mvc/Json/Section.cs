using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Mvc.Json
{
    public class Section
    {
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
        public Guid Identifier { get; set; }
        public bool IsShared { get; set; }
    }
}
