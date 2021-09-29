using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class PaymentTagRepository : Repository<PaymentTag>, IPaymentTagRepository
    {
        public PaymentTagRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
