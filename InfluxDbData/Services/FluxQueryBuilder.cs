using System.Collections.Generic;
using System.Linq;

namespace InfluxDbData.Services
{
    public class FluxQueryBuilder
    {
        private const string rangeStartStopAbsoluteClause = "range(start: {0}, stop: {1})";
        private const string rangeStartRelativeClause = "range(start: {0})";
        private const string pivot = @"pivot(rowKey:[""_time""],columnKey: [""_field""],valueColumn: ""_value"")";

        private string fromClause = "from(bucket:\"{0}\")";
        private string range = string.Empty;

        private List<(string, object)> filters = new List<(string, object)>();

        public FluxQueryBuilder From(string bucket)
        {
            this.fromClause = string.Format(fromClause, bucket);
            return this;
        }

        public FluxQueryBuilder Range(DateTimeRange dateTimeRange)
        {
            this.range = string.Format(rangeStartStopAbsoluteClause, dateTimeRange.From, dateTimeRange.To);
            return this;
        }

        public FluxQueryBuilder RangePastDays(int days)
        {
            this.range = string.Format(rangeStartRelativeClause, $"-{days}d");
            return this;
        }

        public FluxQueryBuilder RangePastHours(int hours)
        {
            this.range = string.Format(rangeStartRelativeClause, $"-{hours}h");
            return this;
        }

        public FluxQueryBuilder RangePastMinutes(int minutes)
        {
            this.range = string.Format(rangeStartRelativeClause, $"-{minutes}m");
            return this;
        }

        public FluxQueryBuilder RangePastSeconds(int seconds)
        {
            this.range = string.Format(rangeStartRelativeClause, $"-{seconds}s");
            return this;
        }

        public FluxQueryBuilder AddFilter(string filterName, object filterValue)
        {
            this.filters.Add((filterName, filterValue));
            return this;
        }

        public string CreateQuery()
        {
            string query = this.fromClause;
            query += " |> ";
            query += this.range;
            query += " |> ";
            query += this.GetFilterQeuryPartToQuery();
            query += " |> ";
            query += pivot;

            return query;
        }

        private string GetFilterQeuryPartToQuery()
        {
            if (this.filters.Count == 0)
                return string.Empty;

            string filters = string.Join(" and ", this.filters.Select(s => $"r[\"{s.Item1}\"] == \"{s.Item2}\""));

            return "filter(fn: (r) =>" + filters + ")";
        }
    }
}
