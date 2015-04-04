using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.DataSource;

namespace SnyderIS.sCore.Exi.Implementation.DataSource
{
    public class DataSourceResult<T> : IDataSourceResult<T>
    {

        public DataSourceResult()
        {
            Metrics = new List<T>();
            HasErrors = false;
            ErrorMessages = new List<string>();
        }

        public IEnumerable<T> Metrics { get; set; }
        public bool HasErrors { get; set; }
        public IEnumerable<String> ErrorMessages { get; set; }


    }
}
