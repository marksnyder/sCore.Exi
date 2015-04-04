using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Mvc.Json
{
    public class Canvas
    {
        public List<Section> Sections { get; set; }
        public Guid DefaultSection { get; set; }
    }
}
