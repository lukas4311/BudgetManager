using Data;
using Data.DataModels;

namespace Repository
{
    public class PaymentTypeRepository : Repository<PaymentType>, IPaymentTypeRepository
    {
        public PaymentTypeRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
