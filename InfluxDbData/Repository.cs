﻿using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDbData.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace InfluxDbData
{
    public class Repository<TModel> : IRepository<TModel> where TModel : IInfluxModel, new()
    {
        private const string ParameterErrorMessage = "Parameter cannot be null!";
        private readonly IInfluxContext context;

        public Repository(IInfluxContext context)
        {
            this.context = context;
        }

        public async Task Write(TModel model, DataSourceIdentification dataSourceIdentification)
        {
            if (model is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(model));

            if (dataSourceIdentification is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dataSourceIdentification));

            WriteApiAsync writeContext = this.context.Client.GetWriteApiAsync();
            await writeContext.WriteMeasurementAsync(dataSourceIdentification.Bucket, dataSourceIdentification.Organization, WritePrecision.Ns, model).ConfigureAwait(false);
        }

        public async Task WriteAll(List<TModel> model, DataSourceIdentification dataSourceIdentification)
        {
            if (model is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(model));

            if (dataSourceIdentification is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dataSourceIdentification));

            WriteApiAsync writeContext = this.context.Client.GetWriteApiAsync();
            await writeContext.WriteMeasurementsAsync(dataSourceIdentification.Bucket, dataSourceIdentification.Organization, WritePrecision.Ns, model).ConfigureAwait(false);
        }

        public async Task<List<TModel>> GetPastHoursData(DataSourceIdentification dataSourceIdentification, int hour)
        {
            if (dataSourceIdentification is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dataSourceIdentification));

            FluxQueryBuilder queryBuilder = new FluxQueryBuilder();
            string query = queryBuilder.From(dataSourceIdentification.Bucket).RangePastDays(hour).AddFilter("_measurement", "stockData").CreateQuery();
            List<FluxTable> data = await this.context.Client.GetQueryApi().QueryAsync(query, dataSourceIdentification.Organization);

            return this.ParseData(data);
        }

        public async Task<List<TModel>> GetPastDaysData(DataSourceIdentification dataSourceIdentification, int days)
        {
            if (dataSourceIdentification is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dataSourceIdentification));

            FluxQueryBuilder queryBuilder = new FluxQueryBuilder();
            string query = queryBuilder.From(dataSourceIdentification.Bucket).RangePastDays(days).AddFilter("_measurement", "stockData").CreateQuery();
            List<FluxTable> data = await this.context.Client.GetQueryApi().QueryAsync(query, dataSourceIdentification.Organization);

            return this.ParseData(data);
        }

        public async Task Delete(DataSourceIdentification dataSourceIdentification, DateTimeRange dateTimeRange)
        {
            if (dateTimeRange is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dateTimeRange));

            if (dataSourceIdentification is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dataSourceIdentification));

            await this.context.Client.GetDeleteApi().Delete(dateTimeRange.From, dateTimeRange.To, null, dataSourceIdentification.Bucket, dataSourceIdentification.Organization);
        }

        public async Task Delete(DataSourceIdentification dataSourceIdentification, DeletePredicateRequest deletePredicate)
        {
            if (dataSourceIdentification is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dataSourceIdentification));

            if (deletePredicate is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(deletePredicate));

            await this.context.Client.GetDeleteApi().Delete(deletePredicate, dataSourceIdentification.Bucket, dataSourceIdentification.Organization);
        }

        public async Task DeleteAll(DataSourceIdentification dataSourceIdentification)
        {
            if (dataSourceIdentification is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dataSourceIdentification));

            await this.Delete(dataSourceIdentification, new DateTimeRange());
        }

        private List<TModel> ParseData(List<FluxTable> fluxTables)
        {
            List<TModel> models = new List<TModel>();

            PropertyInfo[] listofProperties = typeof(TModel).GetProperties();

            fluxTables.ForEach(fluxTable =>
            {
                List<FluxRecord> fluxRecords = fluxTable.Records;

                for (int i = 0; i < fluxTable.Records.Count; i++)
                {
                    FluxRecord record = fluxTable.Records[i];
                    TModel model = this.ParseRowToModel(record);
                    models.Add(model);
                }
            });

            return models;
        }

        private TModel ParseRowToModel(FluxRecord record)
        {
            TModel model = new TModel
            {
                Time = record.GetTimeInDateTime().Value
            };

            foreach (KeyValuePair<string, object> pair in record.Values)
                this.SetPropertyOfModel(model, pair);

            return model;
        }

        private void SetPropertyOfModel(TModel model, KeyValuePair<string, object> pair)
        {
            PropertyInfo property = typeof(TModel).GetProperties().SingleOrDefault(predicate: prop => Attribute.IsDefined(prop, typeof(Column)) && prop.GetCustomAttribute<Column>().Name == pair.Key);

            property?.SetValue(model, pair.Value);
        }
    }
}