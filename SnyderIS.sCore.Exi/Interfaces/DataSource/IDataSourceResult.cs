using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Interfaces.DataSource
{
    public interface IDataSourceResult
    {
    }

    public interface IDataSourceResult<T> : IDataSourceResult
    {
        IEnumerable<T> Metrics { get; }
        bool HasErrors { get; }
        IEnumerable<string> ErrorMessages { get; }
    }
}
