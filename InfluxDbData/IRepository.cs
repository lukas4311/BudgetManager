using InfluxDB.Client.Api.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfluxDbData
{
    public interface IRepository<TModel> where TModel : IInfluxModel, new()
    {
        Task Delete(DataSourceIdentification dataSourceIdentification, DateTimeRange dateTimeRange);

        Task Delete(DataSourceIdentification dataSourceIdentification, DeletePredicateRequest deletePredicate);

        Task DeleteAll(DataSourceIdentification dataSourceIdentification);

        Task<List<TModel>> GetPastDaysData(DataSourceIdentification dataSourceIdentification, int days);

        Task<List<TModel>> GetPastHoursData(DataSourceIdentification dataSourceIdentification, int hour);

        Task<DateTime> GetLastWrittenRecordTime(DataSourceIdentification dataSourceIdentification);

        Task Write(TModel model, DataSourceIdentification dataSourceIdentification);

        Task WriteAll(List<TModel> model, DataSourceIdentification dataSourceIdentification);
    }
}