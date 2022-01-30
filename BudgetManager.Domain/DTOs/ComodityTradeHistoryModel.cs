using BudgetManager.Data.DataModels;
using System;

namespace BudgetManager.Domain.DTOs
{
    public class ComodityTradeHistoryModel : IUserDtoModel
    {
        public int Id { get; set; }

        public DateTime TradeTimeStamp { get; set; }

        public int ComodityTypeId { get; set; }

        public string ComodityType { get; set; }

        public int ComodityUnitId { get; set; }

        public string ComodityUnit { get; set; }

        public double TradeSize { get; set; }

        public double TradeValue { get; set; }

        public int CurrencySymbolId { get; set; }

        public string CurrencySymbol { get; set; }

        public int UserIdentityId { get; set; }

        public string Company { get; set; }
    }
}
