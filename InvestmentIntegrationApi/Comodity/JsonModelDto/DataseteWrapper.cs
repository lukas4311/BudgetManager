using Newtonsoft.Json;

namespace FinanceDataMining.Comodity.JsonModelDto
{
    internal class DataseteWrapper
    {
        [JsonProperty("dataset")]
        public Dataset Dataset { get; set; }
    }
}
