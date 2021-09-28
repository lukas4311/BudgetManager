using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    public class CryptoTicker
    {
        public int Id { get; set; }

        public string Ticker { get; set; }

        public string Name { get; set; }

        public IList<CryptoTradeHistory> CryptoTradeHistories { get; set; }
    }
}
