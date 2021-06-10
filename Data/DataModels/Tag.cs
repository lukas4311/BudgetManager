using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    public class Tag
    {
        public int Id { get; set; }

        public string Code {get; set;}

        public IList<PaymentTag> PaymentTags { get; set; }
    }
}
