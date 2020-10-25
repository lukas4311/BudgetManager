using Data.DataModels;
using ManagerWeb.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ManagerWeb.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserIdentityRepository userIdentityRepository;

        public BudgetService(IBudgetRepository budgetRepository, IUserIdentityRepository userIdentityRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.userIdentityRepository = userIdentityRepository;
        }

        public IEnumerable<BudgetModel> Get()
        {
            string loggedUserLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return this.budgetRepository.FindByCondition(a => a.UserIdentity.Login == loggedUserLogin).Select(b => new BudgetModel
            {
                Amount = b.Amount,
                DateFrom = b.DateFrom,
                DateTo = b.DateTo,
                Id = b.Id,
                Name = b.Name
            }).ToList();
        }

        public void Add(BudgetModel budgetModel)
        {
            string loggedUserLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = this.userIdentityRepository.FindByCondition(a => a.Login == loggedUserLogin).Select(u => u.Id).Single();

            this.budgetRepository.Create(new Budget()
            {
                Amount = budgetModel.Amount,
                DateFrom = budgetModel.DateFrom,
                DateTo = budgetModel.DateTo,
                UserIdentityId = userId,
                Name = budgetModel.Name
            });
        }

        public void Update(BudgetModel budgetModel)
        {
            Budget budget = this.budgetRepository.FindByCondition(a => a.Id == budgetModel.Id).Single();
            budget.Amount = budgetModel.Amount;
            budget.DateFrom = budgetModel.DateFrom;
            budget.DateTo = budgetModel.DateTo;
            budget.Name = budgetModel.Name;

            this.budgetRepository.Update(budget);
        }
    }
}
