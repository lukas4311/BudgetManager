using Data;
using Data.DataModels;

namespace Repository
{
    public class PaymentTagRepository : Repository<PaymentTag>, IPaymentTagRepository
    {
        public PaymentTagRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
