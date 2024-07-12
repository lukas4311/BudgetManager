using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BudgetManager.Services
{
    

    public class EnumService : BaseService<EnumItemModelAdjusted, EnumItem, IRepository<EnumItem>>, IEnumService
    {
        public EnumService(IRepository<EnumItem> repository, IMapper mapper) : base(repository, mapper)
        {
        }

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
    }
}
