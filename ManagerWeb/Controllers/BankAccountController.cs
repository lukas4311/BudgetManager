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
    public class BankAccountController : Controller
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
        public JsonResult GetBankAccountBalanceToDate(DateTime? toDate)
        {
            IEnumerable<BankBalanceModel> bankAccounts = this.userIdentityRepository.FindByCondition(a => a.Login == this.HttpContext.User.Identity.Name)
                .AsNoTracking()
                .Include(b => b.BankAccounts)
                .SelectMany(a => a.BankAccounts)
                .AsEnumerable()
                .Select(b => new BankBalanceModel { Id = b.Id, Balance = b.OpeningBalance });

            var payments = this.paymentRepository
                .FindByCondition(p => bankAccounts.Select(b => b.Id).Contains(p.BankAccountId))
                .AsNoTracking()
                .GroupBy(a => a.BankAccountId)
                .Select(g => new
                {
                    BankAccountId = g.Key,
                    Sum = g.Sum(a => a.Amount)
                })
                .ToList()
                .Join(bankAccounts, payment => payment.BankAccountId, bankAccount => bankAccount.Id, (payment, bankAccount) => {
                    bankAccount.Balance = payment.Sum;
                    return bankAccount;
                });

            return Json(new { success = true, payment = payments });
        }
    }
}