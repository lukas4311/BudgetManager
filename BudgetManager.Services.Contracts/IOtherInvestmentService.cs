using BudgetManager.Domain.DTOs;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    public interface IOtherInvestmentService : IBaseService<OtherInvestmentModel>
    {
        IEnumerable<OtherInvestmentModel> GetAll(int userId);
        bool UserHasRightToPayment(int otherInvestmentId, int userId);
    }
}
