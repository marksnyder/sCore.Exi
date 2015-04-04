using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.DataSource;

namespace SnyderIS.sCore.Exi.Implementation.DataSource.Caching
{
    public abstract class BasicProvider<T> : IDataSource<T>
    {
        private static Dictionary<string,CacheEntry> _Cache = new Dictionary<string,CacheEntry>();
         
        public IDataSourceResult<T> GetResult(IDictionary<string, string> criteria)
        {
            var signature = CreateOptionsSignature(criteria);

            CacheEntry match = null;
            
            if(_Cache.ContainsKey(signature))
            {
                match = _Cache[signature];
            }

            if(match == null)
            {
                lock(_Cache)
                {
                _Cache[signature] = new CacheEntry() { Result = LoadCache(criteria), CachedOn = DateTime.Now };
                }
            }
            else if(IsItemStale(match.CachedOn))
            {
                lock(_Cache)
                {
                _Cache[signature] = new CacheEntry() { Result = LoadCache(criteria), CachedOn = DateTime.Now };
                }
            }

            return _Cache[signature].Result;
            
        }

        public abstract IDataSourceResult<T> LoadCache(IDictionary<string, string> criteria);
        public abstract bool IsItemStale(DateTime time);

        IDataSourceResult IDataSource.GetResult(IDictionary<string, string> criteria)
        {
            return this.GetResult(criteria);
        }

        public abstract string DefaultTitle(IDictionary<string, string> criteria);

        private string CreateOptionsSignature(IDictionary<string,string> criteria)
        {
            var sigBuilder = new System.Text.StringBuilder();

            foreach (var key in criteria.Keys)
            {
                sigBuilder.Append(key);
                sigBuilder.Append(":");
                sigBuilder.Append(criteria[key]);
            }

            return sigBuilder.ToString();
        }

        private class CacheEntry
        {
            public IDataSourceResult<T> Result { get; set;}
            public DateTime CachedOn { get; set;}
        }
    }

}
