using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Services
{
    /// <inheritdoc/>
    public class BankAccountService : BaseService<BankAccountModel, BankAccount, IRepository<BankAccount>>, IBankAccountService
    {
        private readonly IRepository<Payment> paymentRepository;
        private readonly IRepository<BankAccount> bankAccountRepository;
        private readonly IRepository<PaymentTag> paymentTagRepository;
        private readonly IRepository<InterestRate> interestRateRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BankAccountService"/> class.
        /// </summary>
        /// <param name="paymentRepository">The payment repository.</param>
        /// <param name="userIdentityRepository">The user identity repository.</param>
        /// <param name="bankAccountRepository">The bank account repository.</param>
        /// <param name="paymentTagRepository">The payment tag repository.</param>
        /// <param name="interestRateRepository">The interest rate repository.</param>
        /// <param name="autoMapper">The auto mapper instance.</param>
        public BankAccountService(IRepository<Payment> paymentRepository, IRepository<BankAccount> bankAccountRepository, IRepository<PaymentTag> paymentTagRepository,
            IRepository<InterestRate> interestRateRepository, IMapper autoMapper) : base(bankAccountRepository, autoMapper)
        {
            this.paymentRepository = paymentRepository;
            this.bankAccountRepository = bankAccountRepository;
            this.paymentTagRepository = paymentTagRepository;
            this.interestRateRepository = interestRateRepository;
        }

        /// <inheritdoc/>
        public IEnumerable<BankBalanceModel> GetBankAccountsBalanceToDate(string userLogin, DateTime? toDate)
            => FilterBankAccountsForUserLogin(userLogin, toDate);

        /// <inheritdoc/>
        public IEnumerable<BankBalanceModel> GetBankAccountsBalanceToDate(int userId, DateTime? toDate)
            => FilterBankAccountsForUser(userId, toDate);

        /// <inheritdoc/>
        public BankBalanceModel GetBankAccountBalanceToDate(int bankAccountId, DateTime? toDate)
        {
            toDate ??= DateTime.MinValue;

            BankBalanceModel bankAccount = bankAccountRepository.FindByCondition(b => b.Id == bankAccountId)
                .AsNoTracking()
                .Select(b => new BankBalanceModel { Id = b.Id, OpeningBalance = b.OpeningBalance })
                .Single();

            decimal bankAccountsBalance = paymentRepository
                .FindByCondition(p => bankAccount.Id == p.BankAccountId && p.Date > toDate)
                .AsNoTracking()
                .Sum(a => a.Amount);

            return new BankBalanceModel
            {
                Balance = bankAccountsBalance,
                Id = bankAccount.Id,
                OpeningBalance = bankAccount.OpeningBalance
            };
        }

        /// <inheritdoc/>
        public bool UserHasRightToBankAccount(int bankAccountId, int userId)
            => bankAccountRepository.FindByCondition(a => a.Id == bankAccountId && a.UserIdentityId == userId).Count() == 1;

        /// <inheritdoc/>
        public IEnumerable<BankAccountModel> GetAllBankAccounts(int userId)
        {
            return bankAccountRepository.FindByCondition(b => b.UserIdentityId == userId).Select(b => new BankAccountModel
            {
                Code = b.Code,
                Id = b.Id,
                OpeningBalance = b.OpeningBalance,
                UserIdentityId = b.UserIdentityId
            });
        }

        /// <inheritdoc/>
        public override void Delete(int id)
        {
            PaymentDeleteModel data = bankAccountRepository.FindByCondition(b => b.Id == id)
                    .Include(a => a.InterestRates)
                    .Include(a => a.Payments)
                    .ThenInclude(pt => pt.PaymentTags)
                    .Select(b => new PaymentDeleteModel(b, b.InterestRates, b.Payments, b.Payments.SelectMany(a => a.PaymentTags)))
                    .Single();

            foreach (PaymentTag paymentTag in data.paymentTags)
                paymentTagRepository.Delete(paymentTag);

            foreach (Payment payment in data.payments)
                paymentRepository.Delete(payment);

            foreach (InterestRate interest in data.interests)
                interestRateRepository.Delete(interest);

            bankAccountRepository.Delete(data.bankAccount);
            bankAccountRepository.Save();
        }

        /// <summary>
        /// Filters bank accounts for a user based on a predicate and date.
        /// </summary>
        /// <param name="userPredicate">The user predicate for filtering.</param>
        /// <param name="toDate">The date to filter balances to.</param>
        /// <returns>A collection of bank balance models.</returns>
        private IEnumerable<BankBalanceModel> FilterBankAccountsForUser(int userId, DateTime? toDate)
        {
            toDate ??= DateTime.MinValue;

            return bankAccountRepository
                .FindAll()
                .Include(b => b.Payments)
                .ThenInclude(p => p.PaymentType)
                .Where(b => b.UserIdentityId == userId)
                .Select(ToBalanceModel);
        }

        private IEnumerable<BankBalanceModel> FilterBankAccountsForUserLogin(string userLogin, DateTime? toDate)
        {
            toDate ??= DateTime.MinValue;

            return bankAccountRepository
                .FindAll()
                .Include(b => b.Payments)
                .ThenInclude(p => p.PaymentType)
                .Include(b => b.UserIdentity)
                .Where(b => b.UserIdentity.Login == userLogin)
                .Select(ToBalanceModel);
        }

        private BankBalanceModel ToBalanceModel(BankAccount b)
        {
            return new BankBalanceModel
            {
                Id = b.Id,
                OpeningBalance = b.OpeningBalance,
                Balance = b.OpeningBalance + b.Payments.Where(p => p.BankAccountId == b.Id && p.PaymentType.Code == "Revenue").Sum(a => a.Amount) - b.Payments.Where(p => p.BankAccountId == b.Id && p.PaymentType.Code == "Expense").Sum(a => a.Amount)
            };
        }
    }
}
