using System;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetManager.InfluxDbData;

namespace BudgetManager.Services.Contracts
{
    public interface ICryptoService : IBaseService<TradeHistory, CryptoTradeHistory>
    {
        IEnumerable<TradeHistory> GetByUser(string userLogin);

        IEnumerable<TradeHistory> GetByUser(int userId);

        TradeHistory Get(int id, int userId);

        Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol);

        bool UserHasRightToCryptoTrade(int cryptoTradeId, int userId);

        Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol, DateTime atDate);

        Task<IEnumerable<CryptoDataV2>> GetCryptoPriceHistory(string ticker);

        Task<CryptoDataV2> GetCryptoPriceAtDate(string ticker, DateTime atDate);

        Task<IEnumerable<CryptoDataV2>> GetCryptoPriceHistory(string ticker, DateTime from);

        void StoreReportToProcess(byte[] brokerFileData, int userId);
    }
}