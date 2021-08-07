using System;
using System.Collections.Generic;
using System.Linq;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Services
{
    internal class BankAccountService : IBankAccountService
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly UserIdentification userIdentification;
        private readonly IBankAccountRepository bankAccountRepository;
        private readonly IPaymentTagRepository paymentTagRepository;
        private readonly IInterestRateRepository interestRateRepository;

        public BankAccountService(IPaymentRepository paymentRepository, IUserIdentityRepository userIdentityRepository,
            UserIdentification userIdentification, IBankAccountRepository bankAccountRepository, IPaymentTagRepository paymentTagRepository,
            IInterestRateRepository interestRateRepository)
        {
            this.paymentRepository = paymentRepository;
            this.userIdentityRepository = userIdentityRepository;
            this.userIdentification = userIdentification;
            this.bankAccountRepository = bankAccountRepository;
            this.paymentTagRepository = paymentTagRepository;
            this.interestRateRepository = interestRateRepository;
        }

        public IEnumerable<BankBalanceModel> GetBankAccountsBalanceToDate(DateTime? toDate)
        {
            toDate ??= DateTime.MinValue;

            List<BankBalanceModel> bankAccounts = this.userIdentityRepository.FindByCondition(a => a.Login == this.userIdentification.UserName)
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

        public IEnumerable<BankAccountModel> GetAllBankAccounts()
        {
            return bankAccountRepository.FindAll().Select(b => new BankAccountModel
            {
                Code = b.Code,
                Id = b.Id,
                OpeningBalance = b.OpeningBalance
            });
        }

        public int AddBankAccount(BankAccountModel bankAccountViewModel)
        {
            BankAccount bankAccount = new BankAccount
            {
                Code = bankAccountViewModel.Code,
                Id = bankAccountViewModel.Id,
                OpeningBalance = bankAccountViewModel.OpeningBalance,
                UserIdentityId = this.GetLoggedUserId()
            };

            this.bankAccountRepository.Create(bankAccount);
            this.bankAccountRepository.Save();
            return bankAccount.Id;
        }

        public void UpdateBankAccount(BankAccountModel bankAccountViewModel)
        {
            BankAccount bankAccount = this.bankAccountRepository.FindByCondition(p => p.Id == bankAccountViewModel.Id
                && p.UserIdentityId == this.GetLoggedUserId()).Single();
            bankAccount.Code = bankAccountViewModel.Code;
            bankAccount.OpeningBalance = bankAccountViewModel.OpeningBalance;

            this.bankAccountRepository.Update(bankAccount);
            this.bankAccountRepository.Save();
        }

        public void DeleteBankAccount(int id)
        {
            PaymentDeleteModel data = this.bankAccountRepository.FindAll().Where(b => b.Id == id && b.UserIdentityId == this.GetLoggedUserId())
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

        private int GetLoggedUserId() => this.userIdentification.UserId;
    }
}
