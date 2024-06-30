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
    /// <inheritdoc/>
    public class OtherInvestmentTagService : BaseService<OtherInvestmentTagModel, OtherInvestmentTag, IRepository<OtherInvestmentTag>>, IOtherInvestmentTagService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OtherInvestmentTagService"/> class.
        /// </summary>
        /// <param name="repository">The repository for other investment tags.</param>
        /// <param name="mapper">The mapper for mapping between models.</param>
        public OtherInvestmentTagService(IRepository<OtherInvestmentTag> repository, IMapper mapper) : base(repository, mapper)
        { }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public int ReplaceTagForOtherInvestment(int otherInvestmentId, int tagId)
        {
            // Delete existing tag for the other investment, if any
            OtherInvestmentTag otherInvestmentTag = this.repository.FindByCondition(o => o.OtherInvestmentId == otherInvestmentId).SingleOrDefault();

            if (otherInvestmentTag != null)
                this.repository.Delete(otherInvestmentTag);

            // Add the new tag association for the other investment
            return this.Add(new OtherInvestmentTagModel
            {
                OtherInvestmentId = otherInvestmentId,
                TagId = tagId
            });
        }
    }
}
