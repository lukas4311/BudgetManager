using System;
using System.Collections.Generic;

namespace BudgetManager.FinanceDataMining.CurrencyApi
{
    public class CurrencyData
    {
        public string BaseCurrency { get; set; }

        public IEnumerable<(string currency, decimal value, DateTime Date)> PriceOfAnotherCurrencies { get; set; } = new List<(string currency, decimal value, DateTime Date)>();
    }
}