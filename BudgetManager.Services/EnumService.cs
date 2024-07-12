using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Services
{
    /// <inheritdoc/>
    public class EnumService : BaseService<EnumItemModelAdjusted, EnumItem, IRepository<EnumItem>>, IEnumService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumService"/> class.
        /// </summary>
        /// <param name="repository">Enum item repository</param>
        /// <param name="mapper">Automapper instance</param>
        public EnumService(IRepository<EnumItem> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        /// <inheritdoc/>
        public override EnumItemModelAdjusted Get(int id)
        {
            return repository.FindByCondition(p => p.Id == id)
                .Include(d => d.EnumItemType)
                .Select(e => new EnumItemModelAdjusted
                {
                    Code = e.Code,
                    Name = e.Name,
                    EnumItemTypeId = e.EnumItemTypeId,
                    EnumItemTypeCode = e.EnumItemType.Code,
                    EnumItemTypeName = e.EnumItemType.Name,
                    Metadata = e.Metadata,
                    Id = e.Id
                })
                .SingleOrDefault();
        }

        /// <inheritdoc/>
        public EnumItemModelAdjusted GetByCode(string enumItemCode)
        {
            return repository.FindAll()
                .Where(p => p.Code == enumItemCode)
                .Include(d => d.EnumItemType)
                .Select(e => new EnumItemModelAdjusted
                {
                    Code = e.Code,
                    Name = e.Name,
                    EnumItemTypeId = e.EnumItemTypeId,
                    EnumItemTypeCode = e.EnumItemType.Code,
                    EnumItemTypeName = e.EnumItemType.Name,
                    Metadata = e.Metadata,
                    Id = e.Id
                })
                .SingleOrDefault();
        }

        /// <inheritdoc/>
        public IEnumerable<EnumItemModelAdjusted> GetAllByTypeCode(string enumItemTypeCode)
        {
            return repository.FindAll()
                .Include(d => d.EnumItemType)
                .Where(p => p.EnumItemType.Code == enumItemTypeCode)
                .Select(e => new EnumItemModelAdjusted
                {
                    Code = e.Code,
                    Name = e.Name,
                    EnumItemTypeId = e.EnumItemTypeId,
                    EnumItemTypeCode = e.EnumItemType.Code,
                    EnumItemTypeName = e.EnumItemType.Name,
                    Metadata = e.Metadata,
                    Id = e.Id
                });
        }
    }
}
