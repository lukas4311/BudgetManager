using Newtonsoft.Json;
using System.Collections.Generic;

namespace InvestmentIntegrationApi.Comodity.JsonModelDto
{
    internal class Dataset
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("dataset_code")]
        public string Dataset_code { get; set; }

        [JsonProperty("database_code")]
        public string Database_code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("refreshed_at")]
        public string Refreshed_at { get; set; }

        [JsonProperty("newest_available_date")]
        public string Newest_available_date { get; set; }

        [JsonProperty("oldest_available_date")]
        public string Oldest_available_date { get; set; }

        [JsonProperty("column_names")]
        public List<string> Column_names { get; set; }

        [JsonProperty("frequency")]
        public string Frequency { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("premium")]
        public bool Premium { get; set; }

        [JsonProperty("limit")]
        public object Limit { get; set; }

        [JsonProperty("transform")]
        public object Transform { get; set; }
        [JsonProperty("column_index")]
        public object Column_index { get; set; }

        [JsonProperty("start_date")]
        public string Start_date { get; set; }

        [JsonProperty("end_date")]
        public string End_date { get; set; }

        [JsonProperty("data")]
        public List<List<object>> Data { get; set; }

        [JsonProperty("collapse")]
        public object Collapse { get; set; }

        [JsonProperty("order")]
        public object Order { get; set; }

        [JsonProperty("database_id")]
        public int Database_id { get; set; }
    }
}
