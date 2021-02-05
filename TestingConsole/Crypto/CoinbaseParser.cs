using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace TestingConsole.Crypto
{
    internal class CoinbaseParser
    {
        private readonly ICryptoTickerRepository cryptoTickerRepository;
        private readonly ICurrencySymbolRepository currencySymbolRepository;
        private readonly ICryptoTradeHistoryRepository cryptoTradeHistoryRepository;

        public CoinbaseParser(ICryptoTickerRepository cryptoTickerRepository, ICurrencySymbolRepository currencySymbolRepository, ICryptoTradeHistoryRepository cryptoTradeHistoryRepository)
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

        public void LoadFile(string path)
        {
            TextReader reader = new StreamReader(path);
            CsvReader csvReader = new CsvReader(reader, culture: CultureInfo.InvariantCulture);
            IEnumerable<CoinbaseRecord> records = csvReader.GetRecords<CoinbaseRecord>();

            foreach (CoinbaseRecord record in records)
            {
                Console.Write(record.Price);
            }
        }

        public void MapCoinbaseRecordToDbRecord(CoinbaseRecord coinbaseRecord)
        {

        }
    }
}
