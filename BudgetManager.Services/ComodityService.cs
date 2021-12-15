using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Services
{
    public class ComodityService
    {
        private readonly IComodityTradeHistoryRepository comodityTradeHistoryRepository;
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly IComodityTypeRepository comodityTypeRepository;
        private readonly IComodityUnitRepository comodityUnitRepository;

        public ComodityService(IComodityTradeHistoryRepository comodityTradeHistoryRepository, IUserIdentityRepository userIdentityRepository, 
            IComodityTypeRepository comodityTypeRepository, IComodityUnitRepository comodityUnitRepository)
        {
            this.comodityTradeHistoryRepository = comodityTradeHistoryRepository;
            this.userIdentityRepository = userIdentityRepository;
            this.comodityTypeRepository = comodityTypeRepository;
            this.comodityUnitRepository = comodityUnitRepository;
        }

        public IEnumerable<ComodityTypeModel> GetComodityTypes()
        {

            return this.comodityTypeRepository.FindAll().Select(c => new ComodityTypeModel
            {
                Code = c.Code,
                Name = c.Name,
                ComodityUnitId = c.ComodityUnitId,
                Id = c.Id,
                ComodityUnit = c.ComodityUnit.Name
            });
        }
    }
}
