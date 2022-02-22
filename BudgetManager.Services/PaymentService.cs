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
    internal class PaymentService : BaseService<PaymentModel, Payment, IPaymentRepository>, IPaymentService
    {
        private readonly IPaymentTypeRepository paymentTypeRepository;
        private readonly IPaymentCategoryRepository paymentCategoryRepository;
        private readonly IPaymentRepository paymentRepository;

        public PaymentService(IPaymentTypeRepository paymentTypeRepository, IPaymentCategoryRepository paymentCategoryRepository, 
            IPaymentRepository paymentRepository, IMapper autoMapper) : base(paymentRepository, autoMapper)
        {
            this.paymentTypeRepository = paymentTypeRepository;
            this.paymentCategoryRepository = paymentCategoryRepository;
            this.paymentRepository = paymentRepository;
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

        public override PaymentModel Get(int id)
        {
            PaymentModel model = this.repository.FindByCondition(p => p.Id == id)
                .Include(a => a.PaymentTags)
                .ThenInclude(a => a.Tag)
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
                    PaymentTypeId = a.PaymentTypeId,
                    Tags = a.PaymentTags.Select(a => a.Tag.Code).ToList()
                })
                .Single();
            return model;
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
