using System;
using System.Collections.Generic;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IPaymentService
    {
        List<BankAccountModel> GetBankAccounts();

        PaymentModel Get(int id);

        List<PaymentCategoryModel> GetPaymentCategories();

        List<PaymentModel> GetPaymentsData(DateTime? fromDate, DateTime? toDate, int userId, int? bankAccountId);

        List<PaymentTypeModel> GetPaymentTypes();

        int Add(PaymentModel paymentViewModel);

        void Update(PaymentModel paymentViewModel);

        int ClonePayment(int id);

        bool UserHasRightToPayment(int paymentId, int userId);
    }
}