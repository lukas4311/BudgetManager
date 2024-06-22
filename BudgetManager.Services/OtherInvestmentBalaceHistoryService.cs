using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    /// <inheritdoc/>
    public class OtherInvestmentBalaceHistoryService : BaseService<OtherInvestmentBalaceHistoryModel, OtherInvestmentBalaceHistory, IOtherInvestmentBalaceHistoryRepository>, IOtherInvestmentBalaceHistoryService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="repository">The other investment balance repository.</param>
        /// <param name="mapper">The mapper for converting between models and entities.</param>
        public OtherInvestmentBalaceHistoryService(IOtherInvestmentBalaceHistoryRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
