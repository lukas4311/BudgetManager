using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using BudgetManager.Services.Extensions;

namespace BudgetManager.Services
{
    public class BudgetService : BaseService<BudgetModel, Budget, IBudgetRepository>, IBudgetService
    {
        private readonly IBudgetRepository budgetRepository;

        public BudgetService(IBudgetRepository budgetRepository, IMapper autoMapper) : base(budgetRepository, autoMapper)
            => this.budgetRepository = budgetRepository;

        public IEnumerable<BudgetModel> Get()
            => this.budgetRepository.FindAll().Select(b => b.MapToViewModel()).ToList();

        public bool UserHasRightToBudget(int budgetId, int userId)
            => this.budgetRepository.FindByCondition(a => a.Id == budgetId && a.UserIdentityId == userId).Count() == 1;

        public IEnumerable<BudgetModel> GetByUserId(int userId)
            => this.budgetRepository.FindByCondition(a => a.UserIdentityId == userId).Select(b => b.MapToViewModel()).ToList();

        public IEnumerable<BudgetModel> GetActual(int userId)
            => this.budgetRepository.FindByCondition(s => s.UserIdentityId == userId).ToList().Where(b => this.BudgetIsActual(b)).Select(b => b.MapToViewModel());

        public IEnumerable<BudgetModel> Get(int userId, DateTime fromDate, DateTime? toDate)
        {
            toDate ??= DateTime.MaxValue;
            return this.budgetRepository.FindByCondition(b => b.UserIdentityId == userId && b.DateFrom >= fromDate && b.DateTo <= toDate).Select(b => b.MapToViewModel());
        }

        private bool BudgetIsActual(Budget budget) => budget.DateFrom < DateTime.Now && budget.DateTo > DateTime.Now;
    }
}
