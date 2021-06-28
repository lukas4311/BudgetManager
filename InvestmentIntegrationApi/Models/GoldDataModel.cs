using System;
using System.Collections.Generic;
using System.Globalization;
using BudgetManager.FinanceDataMining.Attributes;

namespace BudgetManager.FinanceDataMining.Models
{
    public class GoldDataModel : IQuandlData
    {
        private const string DateColumnName = "Date";
        private const string UsdColumnName = "USD (AM)";

        public GoldDataModel()
        {
            this.PropertySetters = new Dictionary<string, Action<object>>
            {
                {DateColumnName, o => this.Date = DateTime.Parse(o.ToString())},
                {UsdColumnName, o => {
                        double.TryParse(o?.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double usdPrice);

                        if(usdPrice != default)
                            this.GoldPrice = usdPrice;
                    }
                }
            };
        }

        [DataColumnDescription(DateColumnName)]
        public DateTime Date { get; set; }

        [DataColumnDescription(UsdColumnName)]
        public double GoldPrice { get; set; }

        public Dictionary<string, Action<object>> PropertySetters { get; init; }
    }
}
