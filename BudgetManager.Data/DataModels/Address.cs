namespace BudgetManager.Data.DataModels
{
    public class Address : IDataModel
    {
        public int Id { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public decimal? Lat { get; set; }

        public decimal? Lng { get; set; }
    }
}
