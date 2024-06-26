﻿using BudgetManager.ManagerWeb.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BudgetManager.ManagerWeb.Models.ViewModels
{
    public class PaymentViewModel
    {
        public int? Id { get; set; }

        public decimal Amount { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime Date { get; set; }

        public int? BankAccountId { get; set; }

        public int? PaymentTypeId { get; set; }

        public int? PaymentCategoryId { get; set; }

        public string PaymentTypeCode { get; set; }

        public string PaymentCategoryIcon { get; set; }

        public string PaymentCategoryCode { get; set; }

        public List<string> Tags { get; set; }
    }
}
