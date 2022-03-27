namespace BudgetManager.Data.DataModels
{
    public class CompanyProfile : IDataModel
    {
        public int Id { get; set; }

        public string Symbol { get; set; }

        public string CompanyName { get; set; }

        public string Currency { get; set; }

        public string Isin { get; set; }

        public string ExchangeShortName { get; set; }

        public string Industry { get; set; }

        public string Website { get; set; }

        public string Description { get; set; }

        public string Sector { get; set; }

        public string Image { get; set; }

        public int? AddressId { get; set; }

        public Address Address { get; set; }
    }
}
