using System.Text.Json.Serialization;
using BudgetManager.FinanceDataMining.Comodity.JsonModelDto;

namespace BudgetManager.FinanceDataMining.Models
{
    internal class QuandlDataseteWrapper
    {
        [JsonPropertyName("dataset")]
        public Dataset Dataset { get; set; }
    }
}
