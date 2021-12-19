using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Extensions
{
    internal static class ComodityMapperExtension
    {
        public static ComodityTradeHistoryModel MapToComodityTradeHistoryModel(this ComodityTradeHistory cryptoTradeHistory)
        {
            return new ComodityTradeHistoryModel
            {
                ComodityType = cryptoTradeHistory.ComodityType.Name,
                ComodityTypeId = cryptoTradeHistory.ComodityTypeId,
                ComodityUnit = cryptoTradeHistory.ComodityType.ComodityUnit.Name,
                ComodityUnitId = cryptoTradeHistory.ComodityType.ComodityUnitId,
                CurrencySymbol = cryptoTradeHistory.CurrencySymbol.Symbol,
                CurrencySymbolId = cryptoTradeHistory.CurrencySymbolId,
                Id = cryptoTradeHistory.Id,
                TradeSize = cryptoTradeHistory.TradeSize,
                TradeTimeStamp = cryptoTradeHistory.TradeTimeStamp,
                TradeValue = cryptoTradeHistory.TradeValue,
                UserIdentityId = cryptoTradeHistory.UserIdentityId,
                Company = cryptoTradeHistory.Company
            };
        }
    }
}
