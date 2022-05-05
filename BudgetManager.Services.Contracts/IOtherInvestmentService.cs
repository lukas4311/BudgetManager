using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.Services.Contracts
{
    public interface IOtherInvestmentService : IBaseService<OtherInvestmentModel, OtherInvestment>
    {
        IEnumerable<OtherInvestmentModel> GetAll(int userId);

        bool UserHasRightToPayment(int otherInvestmentId, int userId);

        Task<decimal> GetProgressForYears(int id, int? years = null);

        IEnumerable<OtherInvestmentBalaceHistoryModel> GetAllInvestmentLastBalance();
    }
}
