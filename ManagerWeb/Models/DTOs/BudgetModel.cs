using BudgetManager.ManagerWeb.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BudgetManager.ManagerWeb.Models.DTOs
{
    public class BudgetModel
    {
        public int? Id { get; set; }

        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime DateFrom { get; set; }

        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime DateTo { get; set; }

        public int Amount { get; set; }

        public string Name { get; set; }
    }
}
