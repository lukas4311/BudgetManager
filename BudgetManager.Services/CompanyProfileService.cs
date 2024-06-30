using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    internal class CompanyProfileService : BaseService<CompanyProfileModel, CompanyProfile, IRepository<CompanyProfile>>, ICompanyProfileService
    {
        public CompanyProfileService(IRepository<CompanyProfile> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
