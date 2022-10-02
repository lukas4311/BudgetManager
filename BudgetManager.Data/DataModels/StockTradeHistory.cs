using System;

namespace BudgetManager.Data.DataModels
{
    public class StockTradeHistory : IDataModel
    {
        public int Id { get; set; }

        public DateTime TradeTimeStamp { get; set; }

        public int StockTickerId { get; set; }

        public StockTicker StockTicker { get; set; }

        public double TradeSize { get; set; }

        public double TradeValue { get; set; }

        public int CurrencySymbolId { get; set; }

        public CurrencySymbol CurrencySymbol { get; set; }

        public int UserIdentityId { get; set; }

        public UserIdentity UserIdentity { get; set; }
    }
}
