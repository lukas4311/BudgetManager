using System;

namespace BudgetManager.InfluxDbData
{
    public interface IInfluxModel
    {
        DateTime Time { get; set; }
    }
}