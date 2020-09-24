using Data.DataModels;
using ManagerWeb.Models.DTOs;
using ManagerWeb.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerWeb.Services
{
    public class PaymentService : IPaymentService
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

        public List<PaymentViewModel> GetPaymentsData(DateTime? fromDate, int? bankAccountId)
        {
            return this.paymentRepository.FindAll().Where(a => a.Date > (fromDate ?? DateTime.MinValue)
                && (!bankAccountId.HasValue || a.BankAccountId == bankAccountId))
            .Include(a => a.PaymentType)
            .Include(a => a.PaymentCategory)
            .Select(a => new PaymentViewModel
            {
                Amount = a.Amount,
                Date = a.Date,
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                PaymentTypeCode = a.PaymentType.Code,
                PaymentCategoryIcon = a.PaymentCategory.Icon,
                PaymentCategoryCode = a.PaymentCategory.Code
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
                Name = p.Name
            }).ToList();
        }

        public List<InterestRate> GetBankAccounts()
        {
            return this.bankAccountRepository.FindAll().ToList();
        }

        public int AddPayment(PaymentViewModel paymentViewModel)
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

        public void UpdatePayment(PaymentViewModel paymentViewModel)
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

        public PaymentViewModel GetPayment(int id)
        {
            return this.paymentRepository.FindAll().Where(p => p.Id == id).Select(a => new PaymentViewModel
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
    }
}
