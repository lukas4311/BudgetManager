using InfluxDB.Client.Api.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.InfluxDbData
{
    public interface IRepository<TModel> where TModel : IInfluxModel, new()
    {
        Task Delete(DataSourceIdentification dataSourceIdentification, DateTimeRange dateTimeRange);

        Task Delete(DataSourceIdentification dataSourceIdentification, DeletePredicateRequest deletePredicate);

        Task DeleteAll(DataSourceIdentification dataSourceIdentification);

        Task<List<TModel>> GetPastDaysData(DataSourceIdentification dataSourceIdentification, int days);

        Task<List<TModel>> GetPastHoursData(DataSourceIdentification dataSourceIdentification, int hour);

        Task<List<TModel>> GetLastWrittenRecordsTime(DataSourceIdentification dataSourceIdentification);

        Task<IEnumerable<TModel>> GetAllData(DataSourceIdentification dataSourceIdentification);

        Task Write(TModel model, DataSourceIdentification dataSourceIdentification);

        Task WriteAll(IEnumerable<TModel> model, DataSourceIdentification dataSourceIdentification);
    }
}