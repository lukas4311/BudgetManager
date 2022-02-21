using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetManager.Services
{
    public class OtherInvestmentTagService : BaseService<OtherInvestmentTagModel, OtherInvestmentTag, IOtherInvestmentTagRepository>, IOtherInvestmentTagService
    {
        public OtherInvestmentTagService(IOtherInvestmentTagRepository repository, IMapper mapper) : base(repository, mapper)
        { }

        public async Task<IEnumerable<PaymentModel>> GetPaymentsForTag(int otherInvestmentId, int tagId)
        {
            IEnumerable<PaymentModel> payments = await this.repository.FindByCondition(t => t.TagId == tagId && t.OtherInvestmentId == otherInvestmentId)
                .Include(t => t.Tag)
                .ThenInclude(t => t.PaymentTags)
                .ThenInclude(t => t.Payment)
                .SelectMany(t => t.Tag.PaymentTags)
                .Select(t => this.mapper.Map<PaymentModel>(t.Payment))
                .ToListAsync();

            return payments;
        }
    }
}
