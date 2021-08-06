using System;
using System.Collections.Generic;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IPaymentService
    {
        int AddPayment(PaymentModel paymentViewModel);

        List<BankAccountModel> GetBankAccounts();

        PaymentModel GetPayment(int id);

        List<PaymentCategoryModel> GetPaymentCategories();

        List<PaymentModel> GetPaymentsData(DateTime? fromDate, DateTime? toDate, int? bankAccountId);

        List<PaymentTypeModel> GetPaymentTypes();

        void UpdatePayment(PaymentModel paymentViewModel);

        int ClonePayment(int id);
    }
}