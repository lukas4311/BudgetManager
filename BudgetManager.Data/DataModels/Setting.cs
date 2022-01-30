namespace BudgetManager.Data.DataModels
{
    public class Setting : IDataModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string JsonSetting { get; set; }
    }
}
