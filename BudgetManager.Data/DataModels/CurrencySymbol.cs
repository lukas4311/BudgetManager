using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    public class CurrencySymbol
    {
        public int Id { get; set; }

        public string Symbol { get; set; }

        public List<CryptoTradeHistory> CryptoTradeHistory { get; set; }
    }
}
