using System;

namespace BudgetManager.Data.DataModels
{
    public class ComodityTradeHistory
    {
        public int Id { get; set; }

        public DateTime TradeTimeStamp { get; set; }

        public int ComodityTypeId { get; set; }

        public ComodityType ComodityType { get; set; }

        public double TradeSize { get; set; }

        public double TradeValue { get; set; }

        public int CurrencySymbolId { get; set; }

        public CurrencySymbol CurrencySymbol { get; set; }

        public int UserIdentityId { get; set; }

        public UserIdentity UserIdentity { get; set; }

        public string Company { get; set; }
    }
}
