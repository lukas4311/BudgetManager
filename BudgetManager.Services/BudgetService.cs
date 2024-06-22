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
    /// <inheritdoc/>
    public class BudgetService : BaseService<BudgetModel, Budget, IBudgetRepository>, IBudgetService
    {
        private readonly IBudgetRepository budgetRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetService"/> class.
        /// </summary>
        /// <param name="budgetRepository">The budget repository.</param>
        /// <param name="autoMapper">The AutoMapper instance.</param>
        public BudgetService(IBudgetRepository budgetRepository, IMapper autoMapper) : base(budgetRepository, autoMapper)
            => this.budgetRepository = budgetRepository;

        /// <inheritdoc/>
        public IEnumerable<BudgetModel> Get()
            => this.budgetRepository.FindAll().Select(b => b.MapToViewModel()).ToList();

        /// <inheritdoc/>
        public bool UserHasRightToBudget(int budgetId, int userId)
            => this.budgetRepository.FindByCondition(a => a.Id == budgetId && a.UserIdentityId == userId).Count() == 1;

        /// <inheritdoc/>
        public IEnumerable<BudgetModel> GetByUserId(int userId)
            => this.budgetRepository.FindByCondition(a => a.UserIdentityId == userId).Select(b => b.MapToViewModel()).ToList();

        /// <inheritdoc/>
        public IEnumerable<BudgetModel> GetActual(int userId)
            => this.budgetRepository.FindByCondition(s => s.UserIdentityId == userId).ToList().Where(b => this.BudgetIsActual(b)).Select(b => b.MapToViewModel());

        /// <inheritdoc/>
        public IEnumerable<BudgetModel> Get(int userId, DateTime fromDate, DateTime? toDate)
        {
            toDate ??= DateTime.MaxValue;
            return this.budgetRepository.FindByCondition(b => b.UserIdentityId == userId && b.DateFrom >= fromDate && b.DateTo <= toDate).Select(b => b.MapToViewModel());
        }

        /// <summary>
        /// Determines if a budget is actual (currently valid).
        /// </summary>
        /// <param name="budget">The budget to check.</param>
        /// <returns><c>true</c> if the budget is currently valid; otherwise, <c>false</c>.</returns>
        private bool BudgetIsActual(Budget budget) => budget.DateFrom < DateTime.Now && budget.DateTo > DateTime.Now;
    }
}