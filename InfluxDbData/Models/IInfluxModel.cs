using System;

namespace InfluxDbData
{
    public interface IInfluxModel
    {
        DateTime Time { get; set; }
    }
}