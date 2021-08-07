using System.Collections.Generic;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Domain.DTOs
{
    public record PaymentDeleteModel(BankAccount bankAccount, IEnumerable<InterestRate> interests, IEnumerable<Payment> payments, IEnumerable<PaymentTag> paymentTags);
}
