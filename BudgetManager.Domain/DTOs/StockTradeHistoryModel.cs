using System;

namespace BudgetManager.Domain.DTOs
{
    public class StockTradeHistoryModel : IDtoModel
    {
        public int? Id { get; set; }

        public DateTime TradeTimeStamp { get; set; }

        public int StockTickerId { get; set; }

        public double TradeSize { get; set; }

        public double TradeValue { get; set; }

        public int CurrencySymbolId { get; set; }

        public int UserIdentityId { get; set; }
    }
}
