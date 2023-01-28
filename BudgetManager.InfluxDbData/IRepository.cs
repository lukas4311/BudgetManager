using InfluxDB.Client.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BudgetManager.InfluxDbData
{
    public interface IRepository<TModel> where TModel : IInfluxModel, new()
    {
        Task Delete(DataSourceIdentification dataSourceIdentification, DateTimeRange dateTimeRange);

        Task Delete(DataSourceIdentification dataSourceIdentification, DeletePredicateRequest deletePredicate);

        Task DeleteAll(DataSourceIdentification dataSourceIdentification);

        Task<IEnumerable<TModel>> GetPastDaysData(DataSourceIdentification dataSourceIdentification, int days);

        Task<IEnumerable<TModel>> GetPastHoursData(DataSourceIdentification dataSourceIdentification, int hour);

        Task<IEnumerable<TModel>> GetLastWrittenRecordsTime(DataSourceIdentification dataSourceIdentification);

        Task<IEnumerable<TModel>> GetAllData(DataSourceIdentification dataSourceIdentification);

        Task<IEnumerable<TModel>> GetAllData(DataSourceIdentification dataSourceIdentification, Expression<Func<TModel, bool>> filterPredicate);

        Task Write(TModel model, DataSourceIdentification dataSourceIdentification);

        Task WriteAll(IEnumerable<TModel> model, DataSourceIdentification dataSourceIdentification);
    }
}