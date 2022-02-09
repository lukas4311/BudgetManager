using InfluxDB.Client;

namespace BudgetManager.InfluxDbData
{
    public interface IInfluxContext
    {
        InfluxDBClient Client { get; }
    }
}