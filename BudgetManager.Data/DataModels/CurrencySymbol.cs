using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    public class CurrencySymbol : IDataModel
    {
        public int Id { get; set; }

        public string Symbol { get; set; }

        public List<CryptoTradeHistory> CryptoTradeHistory { get; set; }

        public List<ComodityTradeHistory> ComodityTradeHistory { get; set; }

        public IEnumerable<OtherInvestment> OtherInvestments { get; set; }
    }
}
