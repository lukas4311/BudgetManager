using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Core.Flux.Domain;
using BudgetManager.InfluxDbData.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace BudgetManager.InfluxDbData
{
    public class Repository<TModel> : IRepository<TModel> where TModel : IInfluxModel, new()
    {
        private const string ParameterErrorMessage = "Parameter cannot be null!";
        private readonly IInfluxContext context;
        private readonly string measurementName;

        public Repository(IInfluxContext context)
        {
            this.context = context;

            Measurement measurementAttribute = typeof(TModel).GetCustomAttributes(typeof(Measurement)).Single() as Measurement;
            this.measurementName = measurementAttribute.Name;
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

        public async Task WriteAll(IEnumerable<TModel> model, DataSourceIdentification dataSourceIdentification)
        {
            if (model is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(model));

            if (dataSourceIdentification is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dataSourceIdentification));

            WriteApiAsync writeContext = this.context.Client.GetWriteApiAsync();
            await writeContext.WriteMeasurementsAsync(dataSourceIdentification.Bucket, dataSourceIdentification.Organization, WritePrecision.Ns, model).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TModel>> GetPastHoursData(DataSourceIdentification dataSourceIdentification, int hour)
        {
            FluxQueryBuilder queryBulder = this.GetHourDataQuery(dataSourceIdentification, hour);
            List<FluxTable> data = await this.context.Client.GetQueryApi().QueryAsync(queryBulder.CreateQuery(), dataSourceIdentification.Organization);

            return this.ParseData(data);
        }

        public async Task<IEnumerable<TModel>> GetPastHoursData(DataSourceIdentification dataSourceIdentification, int hour, string cryptoTicker)
        {
            FluxQueryBuilder queryBulder = this.GetHourDataQuery(dataSourceIdentification, hour).AddFilter("ticker", cryptoTicker);
            List<FluxTable> data = await this.context.Client.GetQueryApi().QueryAsync(queryBulder.CreateQuery(), dataSourceIdentification.Organization);

            return this.ParseData(data);
        }

        public async Task<IEnumerable<TModel>> GetPastDaysData(DataSourceIdentification dataSourceIdentification, int days)
        {
            if (dataSourceIdentification is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dataSourceIdentification));

            FluxQueryBuilder queryBuilder = new FluxQueryBuilder();
            string query = queryBuilder.From(dataSourceIdentification.Bucket)
                .RangePastDays(days)
                .AddMeasurementFilter(this.measurementName)
                .CreateQuery();

            List<FluxTable> data = await this.context.Client.GetQueryApi().QueryAsync(query, dataSourceIdentification.Organization);

            return this.ParseData(data);
        }

        public async Task<IEnumerable<TModel>> GetLastWrittenRecordsTime(DataSourceIdentification dataSourceIdentification)
        {
            if (dataSourceIdentification is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dataSourceIdentification));

            FluxQueryBuilder queryBuilder = new FluxQueryBuilder();
            string query = queryBuilder
                .From(dataSourceIdentification.Bucket)
                .RangePastDays(int.MaxValue)
                .AddMeasurementFilter(this.measurementName)
                .Sort(false)
                .Take(1)
                .CreateQuery();

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

            await this.context.Client.GetDeleteApi().Delete(deletePredicate, dataSourceIdentification.Bucket, dataSourceIdentification.Organization).ConfigureAwait(false);
        }

        public async Task DeleteAll(DataSourceIdentification dataSourceIdentification)
        {
            if (dataSourceIdentification is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dataSourceIdentification));

            await this.Delete(dataSourceIdentification, new DateTimeRange());
        }

        public Task<IEnumerable<TModel>> GetAllData(DataSourceIdentification dataSourceIdentification)
        {
            if (dataSourceIdentification is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dataSourceIdentification));

            return this.GetPastDaysData(dataSourceIdentification, int.MaxValue);
        }

        public Task<IEnumerable<TModel>> GetAllData(DataSourceIdentification dataSourceIdentification, Expression<Func<TModel, bool>> filterPredicate)
        {
            //var left = filterPredicate.Body.Left;
            var expresssionLambda = filterPredicate as LambdaExpression;
            //var expressionBinary = filterPredicate as System.Linq.Expressions.BinaryExpression;
            var bodyExpression = expresssionLambda.Body as System.Linq.Expressions.BinaryExpression;
            var memberExpr = bodyExpression.Right as System.Linq.Expressions.MemberExpression;
            var field = memberExpr.Member as FieldInfo;
            if (field != null)
            {
                var a = field.GetValue(bodyExpression.Right);
            }

            throw new NotImplementedException();
        }

        private FluxQueryBuilder GetHourDataQuery(DataSourceIdentification dataSourceIdentification, int hour)
        {
            if (dataSourceIdentification is null)
                throw new ArgumentException(ParameterErrorMessage, nameof(dataSourceIdentification));

            FluxQueryBuilder queryBuilder = new FluxQueryBuilder();
            return queryBuilder.From(dataSourceIdentification.Bucket)
                .RangePastDays(hour)
                .AddMeasurementFilter(this.measurementName);
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