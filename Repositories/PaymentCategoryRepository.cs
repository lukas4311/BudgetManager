using Data;
using Data.DataModels;

namespace Repository
{
    public class PaymentCategoryRepository : Repository<PaymentCategory>, IPaymentCategoryRepository
    {
        public PaymentCategoryRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
