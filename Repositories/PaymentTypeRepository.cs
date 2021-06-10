using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class PaymentTypeRepository : Repository<PaymentType>, IPaymentTypeRepository
    {
        public PaymentTypeRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
