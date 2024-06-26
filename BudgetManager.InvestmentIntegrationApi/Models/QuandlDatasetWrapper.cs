using System.Text.Json.Serialization;

namespace BudgetManager.FinanceDataMining.Models
{
    internal class QuandlDatasetWrapper
    {
        [JsonPropertyName("dataset")]
        public QuandlDataset Dataset { get; set; }
    }
}
