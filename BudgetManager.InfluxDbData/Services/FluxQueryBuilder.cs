﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.InfluxDbData.Services
{
    public class FluxQueryBuilder
    {
        private const string rangeStartStopAbsoluteClause = "range(start: {0}, stop: {1})";
        private const string rangeStartRelativeClause = "range(start: {0})";
        private const string pivotClause = @"pivot(rowKey:[""_time""],columnKey: [""_field""],valueColumn: ""_value"")";
        private const string sortClause = @"sort(columns: [""_time""], desc: {0})";
        private const string tailClause = "tail(n: {0})";
        private string fromClause = "from(bucket:\"{0}\")";

        private string range = string.Empty;
        private string tail = string.Empty;
        private string sort = string.Empty;

        private List<(string, object)> filters = new List<(string, object)>();

        public FluxQueryBuilder From(string bucket)
        {
            this.fromClause = string.Format(fromClause, bucket);
            return this;
        }

        public FluxQueryBuilder Range(DateTimeRange dateTimeRange)
        {
            this.range = dateTimeRange.To != DateTime.MaxValue
                ? string.Format(rangeStartStopAbsoluteClause, dateTimeRange.From.ToString("yyyy-MM-dd"), dateTimeRange.To.ToString("yyyy-MM-dd"))
                : string.Format(rangeStartRelativeClause, dateTimeRange.From.ToString("yyyy-MM-dd"));

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

        public FluxQueryBuilder AddMeasurementFilter(string measurementName)
        {
            return this.AddFilter("_measurement", measurementName);
        }

        public FluxQueryBuilder Sort(bool ascending = true)
        {
            this.sort = string.Format(sortClause, ascending.ToString().ToLower());
            return this;
        }

        public FluxQueryBuilder Take(int count)
        {
            this.tail = string.Format(tailClause, count);
            return this;
        }

        public string CreateQuery()
        {
            string query = this.fromClause;
            query += $" |> {this.range}";
            query += $" |> {this.GetFilterQeuryPartToQuery()}";
            query += $" |> {pivotClause}";

            if (!string.IsNullOrEmpty(this.sort))
            {
                query += $" |> {this.sort}";
            }

            if (!string.IsNullOrEmpty(this.tail))
            {
                query += $" |> {this.tail}";
            }

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
