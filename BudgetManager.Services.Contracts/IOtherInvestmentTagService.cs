using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.Services.Contracts
{
    public interface IOtherInvestmentTagService : IBaseService<OtherInvestmentTagModel, OtherInvestmentTag>
    {
        Task<IEnumerable<PaymentModel>> GetPaymentsForTag(int otherInvestmentId, int tagId);
    }
}
