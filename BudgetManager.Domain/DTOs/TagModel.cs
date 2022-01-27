using BudgetManager.Data.DataModels;

namespace BudgetManager.Domain.DTOs
{
    public class TagModel : IDtoModel<Tag>
    {
        public int Id { get; set; }

        public string Code {get; set;}

        public Tag ToEntity()
        {
            throw new System.NotImplementedException();
        }
    }
}

