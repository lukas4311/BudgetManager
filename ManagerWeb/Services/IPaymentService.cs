using BudgetManager.Data.DataModels;
using BudgetManager.ManagerWeb.Models.DTOs;
using BudgetManager.ManagerWeb.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace BudgetManager.ManagerWeb.Services
{
    public interface IPaymentService
    {
        int AddPayment(PaymentViewModel paymentViewModel);

        List<BankAccount> GetBankAccounts();

        PaymentViewModel GetPayment(int id);

        List<PaymentCategoryModel> GetPaymentCategories();

        List<PaymentViewModel> GetPaymentsData(DateTime? fromDate, DateTime? toDate, int? bankAccountId);

        List<PaymentTypeModel> GetPaymentTypes();

        void UpdatePayment(PaymentViewModel paymentViewModel);
    }
}