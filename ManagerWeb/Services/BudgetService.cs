using Data.DataModels;
using ManagerWeb.Extensions;
using ManagerWeb.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ManagerWeb.Services
{
    internal class BudgetService : IBudgetService
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
            string loggedUserLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            return this.budgetRepository.FindByCondition(a => a.UserIdentity.Login == loggedUserLogin).Select(b => b.MapToViewModel()).ToList();
        }

        public BudgetModel Get(int id)
        {
            return this.budgetRepository.FindByCondition(a => a.Id == id).Select(b => b.MapToViewModel()).Single();
        }

        public IEnumerable<BudgetModel> GetActual()
        {
            int userId = this.GetUserId();
            return this.budgetRepository.FindAll().ToList().Where(b => this.BudgetIsActual(b)).Select(b => b.MapToViewModel());
        }

        public IEnumerable<BudgetModel> Get(DateTime fromDate, DateTime? toDate)
        {
            toDate ??= DateTime.MaxValue;
            return this.budgetRepository.FindByCondition(b => b.DateFrom >= fromDate && b.DateTo <= toDate).Select(b => b.MapToViewModel());
        }

        public void Add(BudgetModel budgetModel)
        {
            int userId = this.GetUserId();

            this.budgetRepository.Create(new Budget()
            {
                Amount = budgetModel.Amount,
                DateFrom = budgetModel.DateFrom,
                DateTo = budgetModel.DateTo,
                UserIdentityId = userId,
                Name = budgetModel.Name
            });

            this.budgetRepository.Save();
        }

        public void Update(BudgetModel budgetModel)
        {
            Budget budget = this.budgetRepository.FindByCondition(a => a.Id == budgetModel.Id).Single();
            budgetModel.MapToDataModel(budget);

            this.budgetRepository.Update(budget);
            this.budgetRepository.Save();
        }

        public void Delete(int id)
        {
            int userId = this.GetUserId();
            Budget budget = this.budgetRepository.FindByCondition(a => a.Id == id).Single();
            this.budgetRepository.Delete(budget);
            this.budgetRepository.Save();
        }

        private int GetUserId()
        {
            string loggedUserLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            return this.userIdentityRepository.FindByCondition(a => a.Login == loggedUserLogin).Select(u => u.Id).Single();
        }

        private bool BudgetIsActual(Budget budget) => budget.DateFrom < DateTime.Now && budget.DateTo > DateTime.Now;
    }
}
