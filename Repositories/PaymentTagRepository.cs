using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace Repository
{
    public class PaymentTagRepository : Repository<PaymentTag>, IPaymentTagRepository
    {
        public PaymentTagRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
