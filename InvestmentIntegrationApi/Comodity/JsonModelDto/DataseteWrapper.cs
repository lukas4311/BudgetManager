using Newtonsoft.Json;

namespace InvestmentIntegrationApi.Comodity.JsonModelDto
{
    internal class DataseteWrapper
    {
        [JsonProperty("dataset")]
        public Dataset Dataset { get; set; }
    }
}
