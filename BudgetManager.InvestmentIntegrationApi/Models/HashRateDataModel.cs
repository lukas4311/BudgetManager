using System;
using System.Collections.Generic;
using System.Globalization;
using BudgetManager.FinanceDataMining.Attributes;

namespace BudgetManager.FinanceDataMining.Models
{
    public class HashRateDataModel : IQuandlData
    {
        private const string DateColumnName = "Date";
        private const string HashRateColumnName = "Value";

        public HashRateDataModel()
        {
            this.PropertySetters = new Dictionary<string, Action<object>>
            {
                {DateColumnName, o => this.Date = DateTime.Parse(o.ToString())},
                {HashRateColumnName, o => {
                    double.TryParse(o?.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double usdPrice);

                    if(usdPrice != default)
                        this.HashRate = usdPrice;
                    }
                }
            };
        }

        [DataColumnDescription(DateColumnName)]
        public DateTime Date { get; set; }

        [DataColumnDescription(HashRateColumnName)]
        public double HashRate { get; set; }

        public Dictionary<string, Action<object>> PropertySetters { get; init; }
    }
}
