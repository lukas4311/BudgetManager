using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    internal class CompanyProfileService : BaseService<CompanyProfileModel, CompanyProfile, ICompanyProfileRepository>, ICompanyProfileService
    {
        public CompanyProfileService(ICompanyProfileRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
