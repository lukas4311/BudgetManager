﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BudgetManager.FinanceDataMining.Models
{
    public class FearAndGreedReponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("data")]
        public List<FearAndGreedResponseData> Data { get; set; }

        [JsonPropertyName("metadata")]
        public FearAndGreedResponseMetadata Metadata { get; set; }
    }
}
