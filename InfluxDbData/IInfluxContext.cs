using InfluxDB.Client;

namespace InfluxDbData
{
    public interface IInfluxContext
    {
        InfluxDBClient Client { get; }
    }
}