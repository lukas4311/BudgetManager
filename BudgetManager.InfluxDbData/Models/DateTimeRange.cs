using System;

namespace BudgetManager.InfluxDbData
{
    public class DateTimeRange
    {
        public DateTime From { get; set; } = DateTime.MinValue;

        public DateTime To { get; set; } = DateTime.MaxValue;
    }
}
