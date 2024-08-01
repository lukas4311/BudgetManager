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
    internal class PaymentService : BaseService<PaymentModel, Payment, IRepository<Payment>>, IPaymentService
    {
        private readonly IRepository<PaymentType> paymentTypeRepository;
        private readonly IRepository<PaymentCategory> paymentCategoryRepository;
        private readonly IRepository<Payment> paymentRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        /// <param name="paymentTypeRepository">The repository for payment types.</param>
        /// <param name="paymentCategoryRepository">The repository for payment categories.</param>
        /// <param name="paymentRepository">The repository for payments.</param>
        /// <param name="autoMapper">The mapper for mapping between models.</param>
        public PaymentService(IRepository<PaymentType> paymentTypeRepository, IRepository<PaymentCategory> paymentCategoryRepository,
                              IRepository<Payment> paymentRepository, IMapper autoMapper) : base(paymentRepository, autoMapper)
        {
            this.paymentTypeRepository = paymentTypeRepository;
            this.paymentCategoryRepository = paymentCategoryRepository;
            this.paymentRepository = paymentRepository;
        }

        /// <inheritdoc/>
        public List<PaymentModel> GetPaymentsData(DateTime? fromDate, DateTime? toDate, int userId, int? bankAccountId)
        {
            return paymentRepository.FindAll()
                .Where(a => a.BankAccount.UserIdentityId == userId &&
                            a.Date > (fromDate ?? DateTime.MinValue) &&
                            a.Date < (toDate ?? DateTime.MaxValue) &&
                            (!bankAccountId.HasValue || a.BankAccountId == bankAccountId))
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
                })
                .ToList();
        }

        /// <summary>
        /// Retrieves a payment model by its ID, including associated tags.
        /// </summary>
        /// <param name="id">The ID of the payment to retrieve.</param>
        /// <returns>The payment model with associated tags.</returns>
        public override PaymentModel Get(int id)
        {
            PaymentModel model = repository.FindByCondition(p => p.Id == id)
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

        /// <inheritdoc/>
        public List<PaymentTypeModel> GetPaymentTypes()
        {
            return paymentTypeRepository.FindAll().Select(p => new PaymentTypeModel
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
        }

        /// <inheritdoc/>
        public List<PaymentCategoryModel> GetPaymentCategories()
        {
            return paymentCategoryRepository.FindAll().Select(p => new PaymentCategoryModel
            {
                Id = p.Id,
                Name = p.Name,
                Icon = p.Icon
            }).ToList();
        }

        /// <inheritdoc/>
        public int ClonePayment(int id)
        {
            Payment paymentToClone = paymentRepository.FindByCondition(p => p.Id == id).Single();
            paymentToClone.Id = default;
            paymentRepository.Create(paymentToClone);
            paymentRepository.Save();
            return paymentToClone.Id;
        }

        /// <inheritdoc/>
        public bool UserHasRightToPayment(int paymentId, int userId) =>
            paymentRepository.FindByCondition(a => a.Id == paymentId && a.BankAccount.UserIdentityId == userId).Count() == 1;
    }

}
