using Newtonsoft.Json;
using System.Collections.Generic;

namespace BudgetManager.FinanceDataMining.Models
{
    internal class CandleStickData
    {
        [JsonProperty("14400")]
        public List<List<double>> FourHour { get; set; }
    }
}
