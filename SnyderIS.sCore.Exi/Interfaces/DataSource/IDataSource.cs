using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Interfaces.DataSource
{
    public interface IDataSource
    {
        IDataSourceResult GetResult(IDictionary<string, string> criteria);
        string DefaultTitle(IDictionary<string, string> criteria);
    }

    public interface IDataSource<T> : IDataSource
    {
        IDataSourceResult<T> GetResult(IDictionary<string, string> criteria); 
    }
}
