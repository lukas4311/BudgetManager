using Data.DataModels;
using ManagerWeb.Models.DTOs;

namespace ManagerWeb.Extensions
{
    public static class CryptoMapperExtension
    {
        public static TradeHistory MapToOverViewViewModel(this CryptoTradeHistory cryptoTradeHistory)
        {
            return new TradeHistory
            {
                CryptoTicker = cryptoTradeHistory.CryptoTicker.Ticker,
                CryptoTickerId = cryptoTradeHistory.CryptoTickerId,
                CurrencySymbol = cryptoTradeHistory.CurrencySymbol.Symbol,
                CurrencySymbolId = cryptoTradeHistory.CurrencySymbolId,
                Id = cryptoTradeHistory.Id,
                TradeSize = cryptoTradeHistory.TradeSize,
                TradeTimeStamp = cryptoTradeHistory.TradeTimeStamp,
                TradeValue = cryptoTradeHistory.TradeValue
            };
        }
    }
}
