using System;

namespace InfluxDbData
{
    public class DateTimeRange
    {
        public DateTime From { get; set; } = DateTime.MinValue;

        public DateTime To { get; set; } = DateTime.MaxValue;
    }
}
