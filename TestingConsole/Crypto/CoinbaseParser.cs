using CsvHelper;
using BudgetManager.Data.DataModels;
using BudgetManager.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BudgetManager.TestingConsole.Crypto
{
    internal class CoinbaseParser
    {
        private readonly IRepository<Data.DataModels.CryptoTicker> cryptoTickerRepository;
        private readonly IRepository<CurrencySymbol> currencySymbolRepository;
        private readonly IRepository<CryptoTradeHistory> cryptoTradeHistoryRepository;
        private List<CurrencySymbol> currencySymbols;
        private List<Data.DataModels.CryptoTicker> cryptoTickers;

        public CoinbaseParser(IRepository<Data.DataModels.CryptoTicker> cryptoTickerRepository, IRepository<CurrencySymbol> currencySymbolRepository, IRepository<CryptoTradeHistory> cryptoTradeHistoryRepository)
        {
            this.cryptoTickerRepository = cryptoTickerRepository;
            this.currencySymbolRepository = currencySymbolRepository;
            this.cryptoTradeHistoryRepository = cryptoTradeHistoryRepository;
        }

        public void ParseCoinbaseReport()
        {
            Console.WriteLine("Set path to coinbase report:");
            string pathToFile = Console.ReadLine();
            this.LoadFile(pathToFile);
        }

        private void LoadFile(string path)
        {
            TextReader reader = new StreamReader(path);
            using CsvReader csvReader = new CsvReader(reader, culture: CultureInfo.InvariantCulture);
            List<CoinbaseRecord> records = csvReader.GetRecords<CoinbaseRecord>().ToList();
            this.CacheCurrencySymbols();
            this.CacheCryptoTickers();

            foreach (CoinbaseRecord record in records)
                this.MapCoinbaseRecordToDbRecord(record);

            this.cryptoTradeHistoryRepository.Save();
        }

        private void MapCoinbaseRecordToDbRecord(CoinbaseRecord coinbaseRecord)
        {
            int crypto = this.cryptoTickers.Single(t => string.Equals(t.Ticker, coinbaseRecord.SizeUnit, StringComparison.OrdinalIgnoreCase)).Id;
            int currency = currencySymbols.Single(t => string.Equals(t.Symbol, coinbaseRecord.PriceFeeTotalUnit, StringComparison.OrdinalIgnoreCase)).Id;

            CryptoTradeHistory cryptoTradeHistory = new CryptoTradeHistory
            {
                CryptoTickerId = crypto,
                CurrencySymbolId = currency,
                TradeValue = coinbaseRecord.Total * -1,
                TradeSize = coinbaseRecord.Size,
                TradeTimeStamp = coinbaseRecord.CreatedAt,
                UserIdentityId = 1 // for testing purpose no user contedxt in this app
            };

            this.cryptoTradeHistoryRepository.Create(cryptoTradeHistory);
        }

        private void CacheCurrencySymbols() => this.currencySymbols = this.currencySymbolRepository.FindAll().ToList();

        private void CacheCryptoTickers() => this.cryptoTickers = this.cryptoTickerRepository.FindAll().ToList();
    }
}
