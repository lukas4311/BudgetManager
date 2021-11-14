using System.Collections.Generic;
using System;

namespace BudgetManager.FinanceDataMining.Models
{
    public interface IQuandlData
    {
        Dictionary<string, Action<object>> PropertySetters { get; init; }
    }
}
