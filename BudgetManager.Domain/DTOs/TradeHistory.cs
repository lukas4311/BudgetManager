using System;

namespace BudgetManager.Domain.DTOs
{
    public class TradeHistory : IUserDtoModel
    {
        public int? Id { get; set; }

        public DateTime TradeTimeStamp { get; set; }

        public int CryptoTickerId { get; set; }

        public string CryptoTicker { get; set; }

        public double TradeSize { get; set; }

        public double TradeValue { get; set; }

        public int CurrencySymbolId { get; set; }

        public string CurrencySymbol { get; set; }

        public int UserIdentityId { get; set; }
    }
}
