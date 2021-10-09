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
    internal class PaymentService : IPaymentService
    {
        private readonly IPaymentTypeRepository paymentTypeRepository;
        private readonly IPaymentCategoryRepository paymentCategoryRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IBankAccountRepository bankAccountRepository;

        public PaymentService(IPaymentTypeRepository paymentTypeRepository, IPaymentCategoryRepository paymentCategoryRepository, IPaymentRepository paymentRepository, IBankAccountRepository bankAccountRepository)
        {
            this.paymentTypeRepository = paymentTypeRepository;
            this.paymentCategoryRepository = paymentCategoryRepository;
            this.paymentRepository = paymentRepository;
            this.bankAccountRepository = bankAccountRepository;
        }

        public List<PaymentModel> GetPaymentsData(DateTime? fromDate, DateTime? toDate, int userId, int? bankAccountId)
        {
            return this.paymentRepository.FindAll().Where(a => a.BankAccount.UserIdentityId == userId && a.Date > (fromDate ?? DateTime.MinValue) && a.Date < (toDate ?? DateTime.MaxValue)
                && (!bankAccountId.HasValue || a.BankAccountId == bankAccountId))
            .Include(a => a.PaymentType)
            .Include(a => a.PaymentCategory)
            .Select(a => new PaymentModel
            {
                Amount = a.Amount,
                Date = a.Date,
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                PaymentTypeCode = a.PaymentType.Code,
                PaymentCategoryIcon = a.PaymentCategory.Icon,
                PaymentCategoryCode = a.PaymentCategory.Code,
                BankAccountId = a.BankAccountId,
                PaymentCategoryId = a.PaymentCategoryId,
                PaymentTypeId = a.PaymentTypeId
            }).ToList();
        }

        public List<PaymentTypeModel> GetPaymentTypes()
        {
            return this.paymentTypeRepository.FindAll().Select(p => new PaymentTypeModel
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
        }

        public List<PaymentCategoryModel> GetPaymentCategories()
        {
            return this.paymentCategoryRepository.FindAll().Select(p => new PaymentCategoryModel
            {
                Id = p.Id,
                Name = p.Name,
                Icon = p.Icon
            }).ToList();
        }

        public int Add(PaymentModel paymentViewModel)
        {
            Payment payment = new Payment
            {
                Amount = paymentViewModel.Amount,
                Date = paymentViewModel.Date,
                Description = paymentViewModel.Description,
                Name = paymentViewModel.Name,
                PaymentCategoryId = paymentViewModel.PaymentCategoryId.Value,
                PaymentTypeId = paymentViewModel.PaymentTypeId.Value,
                BankAccountId = paymentViewModel.BankAccountId.Value
            };

            this.paymentRepository.Create(payment);
            this.paymentRepository.Save();
            return payment.Id;
        }

        public void Update(PaymentModel paymentViewModel)
        {
            Payment payment = this.paymentRepository.FindByCondition(p => p.Id == paymentViewModel.Id).Single();
            payment.Amount = paymentViewModel.Amount;
            payment.Date = paymentViewModel.Date;
            payment.Description = paymentViewModel.Description;
            payment.Name = paymentViewModel.Name;
            payment.PaymentCategoryId = paymentViewModel.PaymentCategoryId.Value;
            payment.PaymentTypeId = paymentViewModel.PaymentTypeId.Value;
            payment.BankAccountId = paymentViewModel.BankAccountId.Value;

            this.paymentRepository.Update(payment);
            this.paymentRepository.Save();
        }

        public void Delete(int paymentId)
        {
            Payment payment = this.paymentRepository.FindByCondition(a => a.Id == paymentId).Single();
            this.paymentRepository.Delete(payment);
            this.paymentRepository.Save();
        }

        public PaymentModel Get(int id)
        {
            return this.paymentRepository.FindAll().Where(p => p.Id == id).Select(a => new PaymentModel
            {
                Amount = a.Amount,
                Date = a.Date,
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                BankAccountId = a.BankAccountId,
                PaymentCategoryId = a.PaymentCategoryId,
                PaymentTypeId = a.PaymentTypeId
            })
            .Single();
        }

        public int ClonePayment(int id)
        {
            Payment paymentToClone = this.paymentRepository.FindByCondition(p => p.Id == id).Single();
            paymentToClone.Id = default;
            this.paymentRepository.Create(paymentToClone);
            this.paymentRepository.Save();
            return paymentToClone.Id;
        }

        public bool UserHasRightToPayment(int paymentId, int userId) => this.paymentRepository.FindByCondition(a => a.Id == paymentId && a.BankAccount.UserIdentityId == userId).Count() == 1;
    }
}
