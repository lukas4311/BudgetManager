using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataModels
{
    public class Payment
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int MyProperty { get; set; }

        public DateTime Date { get; set; }

        public int BankAccountId { get; set; }

        public BankAccount BankAccount { get; set; }
    }
}
