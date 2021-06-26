using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BudgetManager.FinanceDataMining.Models
{
    internal class QuandlDataset
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("dataset_code")]
        public string DatasetCode { get; set; }

        [JsonPropertyName("database_code")]
        public string DatabaseCode { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("refreshed_at")]
        public DateTime RefreshedAt { get; set; }

        [JsonPropertyName("newest_available_date")]
        public string NewestAvailableDate { get; set; }

        [JsonPropertyName("oldest_available_date")]
        public string OldestAvailableDate { get; set; }

        [JsonPropertyName("column_names")]
        public List<string> ColumnNames { get; set; }

        [JsonPropertyName("frequency")]
        public string Frequency { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("premium")]
        public bool Premium { get; set; }

        [JsonPropertyName("limit")]
        public object Limit { get; set; }

        [JsonPropertyName("transform")]
        public object Transform { get; set; }

        [JsonPropertyName("column_index")]
        public object ColumnIndex { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("data")]
        public List<List<object>> Data { get; set; }

        [JsonPropertyName("collapse")]
        public object Collapse { get; set; }

        [JsonPropertyName("order")]
        public object Order { get; set; }

        [JsonPropertyName("database_id")]
        public int DatabaseId { get; set; }
    }
}
