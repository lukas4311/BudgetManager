using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    public class ComodityUnit : IDataModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public List<ComodityType> ComodityTypes { get; set; }
    }
}
