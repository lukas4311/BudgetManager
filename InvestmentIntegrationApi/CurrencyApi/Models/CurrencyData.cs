using System;
using System.Collections.Generic;

namespace InvestmentIntegrationApi.CurrencyApi
{
    internal class CurrencyData
    {
        public DateTime Date { get; set; }
        public List<(string currency, decimal value)> PriceOfAnotherCurrencies { get; set; } = new List<(string currency, decimal value)>();
    }
}