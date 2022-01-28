using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Services
{
    public class BankAccountService : BaseService<BankAccountModel, BankAccount, IBankAccountRepository>, IBankAccountService
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly IBankAccountRepository bankAccountRepository;
        private readonly IPaymentTagRepository paymentTagRepository;
        private readonly IInterestRateRepository interestRateRepository;

        public BankAccountService(IPaymentRepository paymentRepository, IUserIdentityRepository userIdentityRepository,
            IBankAccountRepository bankAccountRepository, IPaymentTagRepository paymentTagRepository, 
            IInterestRateRepository interestRateRepository, IMapper autoMapper):base(bankAccountRepository, autoMapper)
        {
            this.paymentRepository = paymentRepository;
            this.userIdentityRepository = userIdentityRepository;
            this.bankAccountRepository = bankAccountRepository;
            this.paymentTagRepository = paymentTagRepository;
            this.interestRateRepository = interestRateRepository;
        }

        public IEnumerable<BankBalanceModel> GetBankAccountsBalanceToDate(string userLogin, DateTime? toDate)
            => this.FilterBankAccountsForUser(a => a.Login == userLogin, toDate);

        public IEnumerable<BankBalanceModel> GetBankAccountsBalanceToDate(int userId, DateTime? toDate)
            => this.FilterBankAccountsForUser(a => a.Id == userId, toDate);

        public BankBalanceModel GetBankAccountBalanceToDate(int bankAccountId, DateTime? toDate)
        {
            toDate ??= DateTime.MinValue;

            BankBalanceModel bankAccount = this.bankAccountRepository.FindByCondition(b => b.Id == bankAccountId)
                .AsNoTracking()
                .Select(b => new BankBalanceModel { Id = b.Id, OpeningBalance = b.OpeningBalance })
                .Single();

            decimal bankAccountsBalance = this.paymentRepository
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

        public bool UserHasRightToBankAccount(int bankAccountId, int userId)
            => this.bankAccountRepository.FindByCondition(a => a.Id == bankAccountId && a.UserIdentityId == userId).Count() == 1;

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

        public override void Delete(int id)
        {
            PaymentDeleteModel data = this.bankAccountRepository.FindByCondition(b => b.Id == id)
                    .Include(a => a.InterestRates)
                    .Include(a => a.Payments)
                    .ThenInclude(pt => pt.PaymentTags)
                    .Select(b => new PaymentDeleteModel(b, b.InterestRates, b.Payments, b.Payments.SelectMany(a => a.PaymentTags)))
                    .Single();

            foreach (PaymentTag paymentTag in data.paymentTags)
                this.paymentTagRepository.Delete(paymentTag);

            foreach (Payment payment in data.payments)
                this.paymentRepository.Delete(payment);

            foreach (InterestRate interest in data.interests)
                this.interestRateRepository.Delete(interest);

            this.bankAccountRepository.Delete(data.bankAccount);
            this.bankAccountRepository.Save();
        }

        private IEnumerable<BankBalanceModel> FilterBankAccountsForUser(Expression<Func<UserIdentity, bool>> userPredicate, DateTime? toDate)
        {
            toDate ??= DateTime.MinValue;

            List<BankBalanceModel> bankAccounts = this.userIdentityRepository.FindByCondition(userPredicate)
                .AsNoTracking()
                .Include(b => b.BankAccounts)
                .SelectMany(a => a.BankAccounts)
                .AsEnumerable()
                .Select(b => new BankBalanceModel { Id = b.Id, OpeningBalance = b.OpeningBalance })
                .ToList();

            List<BankPaymentSumModel> bankAccountsBalance = this.paymentRepository
                .FindByCondition(p => bankAccounts.Select(b => b.Id).Contains(p.BankAccountId) && p.Date > toDate)
                .AsNoTracking()
                .GroupBy(a => a.BankAccountId)
                .Select(g => new BankPaymentSumModel
                {
                    BankAccountId = g.Key,
                    Sum = g.Sum(a => a.Amount)
                })
                .ToList();

            return bankAccounts
                .GroupJoin(bankAccountsBalance, bank => bank.Id, balance => balance.BankAccountId, (x, y) => new { Bank = x, BankBalance = y })
                .SelectMany(x => x.BankBalance.DefaultIfEmpty(), (x, y) =>
                {
                    x.Bank.Balance = y?.Sum ?? 0;
                    return x.Bank;
                });
        }
    }
}
