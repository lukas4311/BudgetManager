using System;
using System.Collections.Generic;

namespace BudgetManager.FinanceDataMining.CurrencyApi
{
    public class CurrencyData
    {
        public string BaseCurrency { get; set; }

        public DateTime Date { get; set; }

        public List<(string currency, decimal value)> PriceOfAnotherCurrencies { get; set; } = new List<(string currency, decimal value)>();
    }
}