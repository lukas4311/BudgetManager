using InfluxDB.Client.Api.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.InfluxDbData
{
    /// <summary>
    /// Represents a generic repository interface for CRUD operations on a specific model type.
    /// </summary>
    /// <typeparam name="TModel">The type of the model that this repository operates on.</typeparam>
    public interface IRepository<TModel> where TModel : IInfluxModel, new()
    {
        /// <summary>
        /// Deletes data from the specified data source within the given date time range.
        /// </summary>
        Task Delete(DataSourceIdentification dataSourceIdentification, DateTimeRange dateTimeRange);

        /// <summary>
        /// Deletes data from the specified data source based on the provided delete predicate request.
        /// </summary>
        Task Delete(DataSourceIdentification dataSourceIdentification, DeletePredicateRequest deletePredicate);

        /// <summary>
        /// Deletes all data from the specified data source.
        /// </summary>
        Task DeleteAll(DataSourceIdentification dataSourceIdentification);

        /// <summary>
        /// Retrieves data for the past specified number of days from the data source.
        /// </summary>
        Task<IEnumerable<TModel>> GetPastDaysData(DataSourceIdentification dataSourceIdentification, int days);

        /// <summary>
        /// Retrieves data for the past specified number of hours from the data source.
        /// </summary>
        Task<IEnumerable<TModel>> GetPastHoursData(DataSourceIdentification dataSourceIdentification, int hours);

        /// <summary>
        /// Retrieves the last written records' time from the data source.
        /// </summary>
        Task<IEnumerable<TModel>> GetLastWrittenRecordsTime(DataSourceIdentification dataSourceIdentification);

        /// <summary>
        /// Retrieves all data from the specified data source.
        /// </summary>
        Task<IEnumerable<TModel>> GetAllData(DataSourceIdentification dataSourceIdentification);

        /// <summary>
        /// Retrieves all data from the specified data source with applied filters.
        /// </summary>
        Task<IEnumerable<TModel>> GetAllData(DataSourceIdentification dataSourceIdentification, Dictionary<string, object> filters);

        /// <summary>
        /// Retrieves all data from the specified data source from a specified date/time with applied filters.
        /// </summary>
        Task<IEnumerable<TModel>> GetAllData(DataSourceIdentification dataSourceIdentification, DateTime from, Dictionary<string, object> filters);

        /// <summary>
        /// Retrieves all data from the specified data source within the given date time range with applied filters.
        /// </summary>
        Task<IEnumerable<TModel>> GetAllData(DataSourceIdentification dataSourceIdentification, DateTimeRange dateTimeRange, Dictionary<string, object> filters);

        /// <summary>
        /// Writes a single model instance to the specified data source.
        /// </summary>
        Task Write(TModel model, DataSourceIdentification dataSourceIdentification);

        /// <summary>
        /// Writes a collection of model instances to the specified data source.
        /// </summary>
        Task WriteAll(IEnumerable<TModel> model, DataSourceIdentification dataSourceIdentification);
    }
}