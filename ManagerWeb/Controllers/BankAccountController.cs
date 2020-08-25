using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.DataModels;
using ManagerWeb.Models;
using ManagerWeb.Models.DTOs;
using ManagerWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace ManagerWeb.Controllers
{
    public partial class BankAccountController : Controller
    {
        private readonly IPaymentTypeRepository paymentTypeRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IBankAccountRepository bankAccountRepository;
        private readonly IUserIdentityRepository userIdentityRepository;

        public BankAccountController(IPaymentTypeRepository paymentTypeRepository, IPaymentRepository paymentRepository, IBankAccountRepository bankAccountRepository, IUserIdentityRepository userIdentityRepository)
        {
            this.paymentTypeRepository = paymentTypeRepository;
            this.paymentRepository = paymentRepository;
            this.bankAccountRepository = bankAccountRepository;
            this.userIdentityRepository = userIdentityRepository;
        }

        [HttpGet]
        public JsonResult GetBankAccountsBalanceToDate(DateTime? toDate)
        {
            toDate ??= DateTime.MinValue;

            List<BankBalanceModel> bankAccounts = this.userIdentityRepository.FindByCondition(a => a.Login == this.HttpContext.User.Identity.Name)
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

            IEnumerable<BankBalanceModel> bankInfo = bankAccounts
                .GroupJoin(bankAccountsBalance, bank => bank.Id, balance => balance.BankAccountId, (x, y) => new { Bank = x, BankBalance = y })
                .SelectMany(x => x.BankBalance.DefaultIfEmpty(),(x, y) => {
                    x.Bank.Balance = y?.Sum ?? 0;
                    return x.Bank;
                });

            return Json(new { success = true, bankAccountsBalance = bankInfo });
        }
    }
}