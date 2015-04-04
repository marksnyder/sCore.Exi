using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnyderIS.sCore.Exi.TrendDetection
{

    public delegate void NewResultsProducedHandler<T>(
    Enumerations.EvaluationMethod method,
    IEnumerable<ResultEntry<T>> results);

    public class Engine<T>
    {

        public event NewResultsProducedHandler<T> NewResultsProduced;

        public IEnumerable<ResultEntry<T>> Evaluate(IEnumerable<DataSetEntry<T>> data,
            Func<IEnumerable<DataSetEntry<T>>, decimal> eval)
        {
            var m =  new MethodImp.SimpleAttributeMethod<T>();
            return m.Evaluate(data, this, eval);
        }

        internal void NewResults(Enumerations.EvaluationMethod method,
            IEnumerable<ResultEntry<T>> results)
        {
            if (NewResultsProduced != null)
            {
                NewResultsProduced(method, results);
            }
        }
    }
}
