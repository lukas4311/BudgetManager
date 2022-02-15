using BudgetManager.Data.DataModels;

namespace BudgetManager.Domain.DTOs
{
    public class TagModel : IDtoModel
    {
        public int? Id { get; set; }

        public string Code {get; set;}
    }
}

