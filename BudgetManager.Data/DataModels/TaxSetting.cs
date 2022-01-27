namespace BudgetManager.Data.DataModels
{
    public class TaxSetting : IDataModel
    {
        public int Id { get; set; }

        public string TaxType { get; set; }

        public decimal Value { get; set; }
    }
}
