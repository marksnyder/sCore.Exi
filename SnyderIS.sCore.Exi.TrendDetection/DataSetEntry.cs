using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnyderIS.sCore.Exi.TrendDetection
{
    public class DataSetEntry<T>
    {
        public string UniqueIdentifier { get; set; }
        public IDictionary<string,string> Attributes { get; set;}
        public decimal QuantifiedValue { get; set; }
        public T Source { get; set; }
    }
}
