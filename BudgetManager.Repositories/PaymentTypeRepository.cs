using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="IPaymentTypeRepository" />
    public class PaymentTypeRepository : Repository<PaymentType>, IPaymentTypeRepository
    {
        public PaymentTypeRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
