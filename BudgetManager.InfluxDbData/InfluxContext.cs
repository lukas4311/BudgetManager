using InfluxDB.Client;

namespace BudgetManager.InfluxDbData
{
    public class InfluxContext : IInfluxContext
    {
        public InfluxContext(string influxUrl, string influxToken)
        {
            this.Client = InfluxDBClientFactory.Create(influxUrl, influxToken.ToCharArray());
        }

        public InfluxDBClient Client { get; }
    }
}
