using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    public class ComodityType : IDataModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public ComodityUnit ComodityUnit { get; set; }

        public int ComodityUnitId { get; set; }

        public List<ComodityTradeHistory> ComodityTradeHistory { get; set; }
    }
}
