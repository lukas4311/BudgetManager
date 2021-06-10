using Data.DataModels;
using System.Collections.Generic;

namespace BudgetManager.ManagerWeb.Models
{
    internal record PaymentDeleteModel(BankAccount bankAccount, IEnumerable<InterestRate> interests, IEnumerable<Payment> payments, IEnumerable<PaymentTag> paymentTags);
}
