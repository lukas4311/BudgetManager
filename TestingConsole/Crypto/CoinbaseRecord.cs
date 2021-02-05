using CsvHelper.Configuration.Attributes;

namespace TestingConsole.Crypto
{

    public class CoinbaseRecord
    {
        [Name("portfolio")]
        public string Portfolio { get; set; }

        [Name("trade id")]
        public string TradeId { get; set; }

        [Name("product")]
        public string Product { get; set; }

        [Name("side")]
        public string Side { get; set; }

        [Name("created at")]
        public string CreatedAt { get; set; }

        [Name("size")]
        public string Size { get; set; }

        [Name("size unit")]
        public string SizeUnit { get; set; }

        [Name("price")]
        public string Price { get; set; }

        [Name("fee")]
        public string Fee { get; set; }

        [Name("total")]
        public string Total { get; set; }

        [Name("price/fee/total unit")]
        public string PriceFeeTotalUnit { get; set; }
    }
}
