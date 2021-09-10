using System;
using System.Collections.Generic;
using System.Linq;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using BudgetManager.Services.Extensions;

namespace BudgetManager.Services
{
    internal class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository budgetRepository;

        public BudgetService(IBudgetRepository budgetRepository)
        {
            this.budgetRepository = budgetRepository;
        }

        public IEnumerable<BudgetModel> Get() => this.budgetRepository.FindAll().Select(b => b.MapToViewModel()).ToList();

        public BudgetModel Get(int id) => this.budgetRepository.FindByCondition(a => a.Id == id).Select(b => b.MapToViewModel()).Single();

        public IEnumerable<BudgetModel> GetByUserId(int userId) => this.budgetRepository.FindByCondition(a => a.UserIdentityId == userId).Select(b => b.MapToViewModel()).ToList();

        public IEnumerable<BudgetModel> GetActual(int userId)
        {
            return this.budgetRepository.FindByCondition(s => s.UserIdentityId == userId).ToList().Where(b => this.BudgetIsActual(b)).Select(b => b.MapToViewModel());
        }

        public IEnumerable<BudgetModel> Get(int userId, DateTime fromDate, DateTime? toDate)
        {
            toDate ??= DateTime.MaxValue;
            return this.budgetRepository.FindByCondition(b => b.UserIdentityId == userId && b.DateFrom >= fromDate && b.DateTo <= toDate).Select(b => b.MapToViewModel());
        }

        public void Add(BudgetModel budgetModel)
        {
            this.budgetRepository.Create(new Budget()
            {
                Amount = budgetModel.Amount,
                DateFrom = budgetModel.DateFrom,
                DateTo = budgetModel.DateTo,
                UserIdentityId = budgetModel.UserIdentityId,
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
            Budget budget = this.budgetRepository.FindByCondition(a => a.Id == id).Single();
            this.budgetRepository.Delete(budget);
            this.budgetRepository.Save();
        }

        private bool BudgetIsActual(Budget budget) => budget.DateFrom < DateTime.Now && budget.DateTo > DateTime.Now;
    }
}
