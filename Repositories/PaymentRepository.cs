using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace Repository
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
