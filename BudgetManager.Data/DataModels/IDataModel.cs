namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Interface representing a basic data model contract.
    /// </summary>
    public interface IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the data model.
        /// </summary>
        int Id { get; set; }
    }
}
