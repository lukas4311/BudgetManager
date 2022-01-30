using System;

namespace BudgetManager.Data.DataModels
{
    public class CryptoTradeHistory : IDataModel
    {
        public int Id { get; set; }

        public DateTime TradeTimeStamp { get; set; }

        public int CryptoTickerId { get; set; }

        public CryptoTicker CryptoTicker { get; set; }

        public double TradeSize { get; set; }

        public double TradeValue { get; set; }

        public int CurrencySymbolId { get; set; }

        public CurrencySymbol CurrencySymbol { get; set; }

        public int UserIdentityId { get; set; }

        public UserIdentity UserIdentity { get; set; }
    }
}
