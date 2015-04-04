using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnyderIS.sCore.Exi.TrendDetection.MethodImp
{
    public class SimpleAttributeMethod<T>
    {

        private Engine<T> _Engine;


        public IEnumerable<ResultEntry<T>> Evaluate(IEnumerable<DataSetEntry<T>> data,
            Engine<T> engine,
            Func<IEnumerable<DataSetEntry<T>>, decimal> eval)
        {
            _Engine = engine;

            var keys = data.First().Attributes.Keys.ToList();
            var permutations = GetPowerSet<string>(keys);

            var allResults = new List<ResultEntry<T>>();

            int count = 0;

            foreach(var permutation in permutations)
            {
                count++;

               
                var results = (AnalyzeAttributePermutation(data, permutation,eval));

                allResults.AddRange(results);

                var dBuilder = new StringBuilder();

                foreach (var key in permutation)
                {
                    dBuilder.Append(key);
                    dBuilder.Append(",");
                }

                System.Console.Clear();

                System.Console.WriteLine("{0} of {1}", count, permutations.Count());

                System.Console.WriteLine("Evaluating attribute permutation {0}"
                    , dBuilder.ToString());

                _Engine.NewResults(Enumerations.EvaluationMethod.SimpleAttribute, results);
            }

            return allResults;
        }

        private IEnumerable<ResultEntry<T>> AnalyzeAttributePermutation(IEnumerable<DataSetEntry<T>> data,
            IEnumerable<string> evalKeys,
            Func<IEnumerable<DataSetEntry<T>>,decimal> eval)
        {


            var results = new List<ResultEntry<T>>();

            var distinctValues = new List<DistinctValueCombo<T>>();

            /* Build distinct value sets */
            foreach (var item in data)
            {
                var match = distinctValues.Where(x => x.IsMatch(evalKeys, item)).FirstOrDefault();

                if (match == null)
                {
                    match = new DistinctValueCombo<T>(evalKeys, item);
                    distinctValues.Add(match);
                }
            }


            /* If we only have 1 unique value set for this permetation then it doesn't make much sense
             * to analyze it given that this information has been capture at a more shallow iteration.
             * THis however doesn't apply to level 1 depth 
             */

            if (distinctValues.Count() > 1 || evalKeys.Count() < 2)
            {

                /* Iterate through unique value sets and analyze data */

                foreach (var dVal in distinctValues)
                {
                    var dResults = data.ToList();

                    foreach (var key in dVal.Values.Keys)
                    {
                        dResults = dResults.Where(x => x.Attributes[key] == dVal.Values[key]).ToList();
                    }

                    results.Add(new ResultEntry<T>()
                    {
                        Attributes = dVal.Values,
                        Method = Enumerations.EvaluationMethod.SimpleAttribute,
                        EvalResult = eval.Invoke(dResults),
                        Sample = dResults
                    });
                }
            }

            return results;

        }

        public IEnumerable<IEnumerable<T>> GetPowerSet<T>(List<T> list)
        {
            return from m in Enumerable.Range(0, 1 << list.Count)
                   select
                       from i in Enumerable.Range(0, list.Count)
                       where (m & (1 << i)) != 0
                       select list[i];
        }


        public class DistinctValueCombo<T>
        {
            public Dictionary<string, string> Values;

            public DistinctValueCombo(IEnumerable<string> evalKeys,
                DataSetEntry<T> entry)
            {
                Values = new Dictionary<string, string>();

                foreach (var key in evalKeys)
                {
                    Values[key] = entry.Attributes[key];
                }
            }

            public bool IsMatch(IEnumerable<string> evalKeys,
                DataSetEntry<T> entry)
            {
                foreach (var key in evalKeys)
                {
                    if (entry.Attributes[key] != Values[key])
                    {
                        return false;
                    }
                }

                return true;
            }

        }

    }
}
