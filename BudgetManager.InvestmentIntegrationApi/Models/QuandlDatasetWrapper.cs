using System.Text.Json.Serialization;
using BudgetManager.FinanceDataMining.Comodity.JsonModelDto;

namespace BudgetManager.FinanceDataMining.Models
{
    internal class QuandlDatasetWrapper
    {
        [JsonPropertyName("dataset")]
        public QuandlDataset Dataset { get; set; }
    }
}
