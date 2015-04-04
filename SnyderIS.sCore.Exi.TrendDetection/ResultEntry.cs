using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnyderIS.sCore.Exi.TrendDetection
{
    public class ResultEntry<T>
    {
        public IDictionary<string, string> Attributes { get; set; }
        public decimal EvalResult { get; set; }
        public Enumerations.EvaluationMethod Method { get; set; }
        public IEnumerable<DataSetEntry<T>> Sample { get; set; }

        public int Depth
        {
            get
            {
                return this.Attributes.Keys.Count();
            }
        }
    }
}
