using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace Repository
{
    public class PaymentTypeRepository : Repository<PaymentType>, IPaymentTypeRepository
    {
        public PaymentTypeRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
